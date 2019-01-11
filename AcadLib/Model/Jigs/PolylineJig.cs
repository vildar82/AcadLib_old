using AcadLib.Hatches;
using Autodesk.AutoCAD.Colors;

namespace AcadLib.Jigs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.EditorInput;
    using Autodesk.AutoCAD.Geometry;
    using Autodesk.AutoCAD.GraphicsInterface;
    using Geometry;
    using JetBrains.Annotations;

    [PublicAPI]
    public class PolylineJig : DrawJig
    {
        private Point2d basePt;
        private Point2d newPt;
        private Editor ed;
        private ObjectId crossLineTypeId;
        private Color hatchColor;
        private Transparency hatchTransp;

        [NotNull]
        public List<Point2d> Pts => PtsCol.Cast<Point3d>().Select(s => s.Convert2d()).ToList();

        [NotNull]
        public Point3dCollection PtsCol { get; set; } = new Point3dCollection();

        public PromptStatus DrawPolyline(Editor ed)
        {
            this.ed = ed;
            var ptOpt = new PromptPointOptions("\nПервая точка:");
            ptOpt.Keywords.Add("Полилиния");
            ptOpt.AppendKeywordsToMessage = true;
            var ptRes = ed.GetPoint(ptOpt);
            if (ptRes.Status == PromptStatus.Keyword)
                return SelectPolyline();

            if (ptRes.Status == PromptStatus.OK)
            {
                DefineCrossDecor();
                basePt = ptRes.Value.TransformBy(ed.CurrentUserCoordinateSystem).Convert2d();
                PtsCol.Add(basePt.Convert3d());
                newPt = basePt;
                while (true)
                {
                    var res = ed.Drag(this);
                    if (res.Status == PromptStatus.None)
                        continue;
                    if (res.Status == PromptStatus.Cancel || res.Status == PromptStatus.Error)
                        throw new OperationCanceledException();
                    if (res.Status == PromptStatus.Keyword)
                        return SelectPolyline();
                    if (res.Status != PromptStatus.OK)
                        break;
                    PtsCol.Add(newPt.Convert3d());
                    basePt = newPt;
                }

                return PromptStatus.OK;
            }

            throw new OperationCanceledException();
        }

        private void DefineCrossDecor()
        {
            crossLineTypeId = HostApplicationServices.WorkingDatabase.LoadLineTypeDotPIK();
            try
            {
                var colorIndex = "CROSSINGAREACOLOR".GetSystemVariable<short>();
                hatchColor = Color.FromColorIndex(ColorMethod.ByAci, colorIndex);
                var trans = "SELECTIONAREAOPACITY".GetSystemVariable<int>();
                var transByte = (byte) (255 - trans / 100d * 255);
                hatchTransp = new Transparency(transByte);
            }
            catch
            {
                hatchColor = Color.FromColor(System.Drawing.Color.Green);
            }
        }

        private PromptStatus SelectPolyline()
        {
            var plId = ed.SelectEntity<Autodesk.AutoCAD.DatabaseServices.Polyline>("Выбор полилинии");
            using (var pl = plId.Open(OpenMode.ForRead, false, true) as Autodesk.AutoCAD.DatabaseServices.Polyline)
            {
                PtsCol = new Point3dCollection(pl.GetPoints().Select(s=>s.Convert3d()).ToArray());
            }

            return PromptStatus.Cancel;
        }

        /// <inheritdoc />
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            var promptPt = new JigPromptPointOptions("\nСлед. точка:");
            promptPt.Keywords.Add("Полилиния");
            promptPt.AppendKeywordsToMessage = true;
            promptPt.BasePoint = basePt.Convert3d();
            promptPt.UseBasePoint = true;
            promptPt.UserInputControls =
                UserInputControls.UseBasePointElevation | UserInputControls.AcceptOtherInputString
                                                        | UserInputControls.GovernedByOrthoMode;
            var res = prompts.AcquirePoint(promptPt);
            switch (res.Status)
            {
                case PromptStatus.OK:
                    var pt = res.Value.Convert2d();
                    if (pt.GetDistanceTo(basePt) < 0.01)
                        return SamplerStatus.NoChange;
                    newPt = pt;
                    return SamplerStatus.OK;
                case PromptStatus.Cancel:
                    return SamplerStatus.Cancel;
                case PromptStatus.None:
                    return SamplerStatus.Cancel;
                case PromptStatus.Error:
                    return SamplerStatus.Cancel;
                case PromptStatus.Keyword:
                    return SamplerStatus.Cancel;
                case PromptStatus.Modeless:
                    break;
                case PromptStatus.Other:
                    return SamplerStatus.Cancel;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return SamplerStatus.NoChange;
        }

        /// <inheritdoc />
        protected override bool WorldDraw(WorldDraw draw)
        {
            bool res;
            using (var pl = Pts.CreatePolyline())
            {
                pl.ColorIndex = 7;
                pl.LinetypeId = crossLineTypeId;
                pl.LineWeight = LineWeight.LineWeight018;
                pl.LinetypeScale = 1;
                res = draw.Geometry.Polyline(PtsCol, Vector3d.ZAxis, IntPtr.Zero);
                if (res) return true;
            }

            res = draw.Geometry.WorldLine(basePt.Convert3d(), newPt.Convert3d());
            if (res) return true;
            try
            {
                var pts = Pts;
                pts.Add(newPt);
                using (var hatch = pts.CreateHatch())
                {
                    hatch.SetHatchPattern(HatchPatternType.UserDefined, "SOLID");
                    hatch.Color = hatchColor;
                    hatch.Transparency = hatchTransp;
                    return draw.Geometry.Draw(hatch);
                }
            }
            catch
            {
                return true;
            }
        }
    }
}
