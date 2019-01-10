namespace AcadLib.Jigs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.EditorInput;
    using Autodesk.AutoCAD.Geometry;
    using Autodesk.AutoCAD.GraphicsInterface;
    using JetBrains.Annotations;

    [PublicAPI]
    public class PolylineJig : DrawJig
    {
        [NotNull]
        private readonly Point3dCollection ptsCol = new Point3dCollection();
        private Point2d basePt;
        private Point2d newPt;
        private Editor ed;

        public List<Point2d> Pts => ptsCol.Cast<Point3d>().Select(s => s.Convert2d()).ToList();

        public PromptStatus DrawPolyline(Editor ed)
        {
            this.ed = ed;
            var ptOpt = new PromptPointOptions("Первая точка: [Полилиния]");
            var ptRes = ed.GetPoint(ptOpt);
            if (ptRes.Status == PromptStatus.Keyword)
            {
                return SelectPolyline();
            }

            if (ptRes.Status == PromptStatus.OK)
            {
                basePt = ptRes.Value.TransformBy(ed.CurrentUserCoordinateSystem).Convert2d();
                ptsCol.Add(basePt.Convert3d());
                newPt = basePt;
                PromptResult res;
                while (true)
                {
                    res = ed.Drag(this);
                    if (res.Status != PromptStatus.OK && res.Status != PromptStatus.Other)
                        break;
                    ptsCol.Add(newPt.Convert3d());
                    basePt = newPt;
                }

                if (res.Status == PromptStatus.Keyword)
                {
                    return PromptStatus.Keyword;
                }

                return res.Status == PromptStatus.None ? PromptStatus.OK : res.Status;
            }

            throw new OperationCanceledException();
        }

        private PromptStatus SelectPolyline()
        {
            var plId = ed.SelectEntity<Autodesk.AutoCAD.DatabaseServices.Polyline>("Выбор полилинии");
            using (var pl = plId.Open(OpenMode.ForRead, false, true) as Autodesk.AutoCAD.DatabaseServices.Polyline)
            {
            }

            return PromptStatus.Cancel;
        }

        /// <inheritdoc />
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            var promptPt = new JigPromptPointOptions("След. точка");
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
                    break;
                case PromptStatus.Error:
                    break;
                case PromptStatus.Keyword:
                    return SamplerStatus.Cancel;
                case PromptStatus.Modeless:
                    break;
                case PromptStatus.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return SamplerStatus.NoChange;
        }

        /// <inheritdoc />
        protected override bool WorldDraw(WorldDraw draw)
        {
            draw.Geometry.Polyline(ptsCol, Vector3d.ZAxis, IntPtr.Zero);
            return draw.Geometry.WorldLine(basePt.Convert3d(), newPt.Convert3d());
        }
    }
}
