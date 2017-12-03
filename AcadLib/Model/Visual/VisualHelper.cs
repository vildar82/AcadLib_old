using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib.Visual
{
    public static class VisualHelper
    {
        private const string textStyleName = "PIK-Visual";
        private const string textStyleFontFile = "Arial.ttf";

        public static ObjectId GetTextStyleId(Document doc)
        {
            var db = doc.Database;
            return db.GetTextStylePIK();
        }

        public static Polyline CreatePolyline(List<Point2d> points, VisualOption opt)
        {
            var pts = DistincPoints(points);
            var pl = new Polyline();
            for (var i = 0; i < pts.Length; i++)
            {
                pl.AddVertexAt(i, pts[i], 0, 0, 0);
            }
            pl.Closed = true;
            SetEntityOpt(pl, opt);
            return pl;
        }

        public static Hatch CreateHatch(List<Point2d> points, VisualOption opt)
        {
            var pts = DistincPoints(points);
            // Штриховка            
            var ptCol = new Point2dCollection(pts) { points[0] };
            var dCol = new DoubleCollection(new double[points.Count]);
            var h = new Hatch();
            h.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");
            SetEntityOpt(h, opt);
            h.AppendLoop(HatchLoopTypes.Default, ptCol, dCol);
            return h;
        }

        private static Point2d[] DistincPoints(List<Point2d> points)
        {
            //  Отсеивание одинаковых точек
            return points.Distinct(new Comparers.Point2dEqualityComparer()).ToArray();
        }

        public static DBText CreateText(string text, VisualOption opt, double height, AttachmentPoint justify)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var dbText = new DBText();
            SetEntityOpt(dbText, opt);
            dbText.TextStyleId = GetTextStyleId(doc);
            dbText.Position = opt.Position;
            dbText.TextString = text;
            dbText.Height = height;
            dbText.Justify = justify;
            dbText.AlignmentPoint = opt.Position;
            dbText.AdjustAlignment(db);
            return dbText;
        }

        public static MText CreateMText(string text, VisualOption opt, double height, AttachmentPoint justify)
        {
            var mtext = new MText
            {
                Location = opt.Position,
                TextStyleId = GetTextStyleId(Application.DocumentManager.MdiActiveDocument),
                Attachment = justify,
                TextHeight = height,
                Contents = text,
                Color = opt.Color
            };
            return mtext;
        }

        public static Circle CreateCircle(double radius, VisualOption opt)
        {
            var c = new Circle(opt.Position, Vector3d.ZAxis, radius);
            SetEntityOpt(c, opt);
            return c;
        }

        public static void SetEntityOpt(Entity ent, VisualOption opt)
        {
            if (ent == null) return;
            if (opt.Color != null) ent.Color = opt.Color;
            if (opt.Transparency.Alpha != 0) ent.Transparency = opt.Transparency;
            if (opt.LineWeight.HasValue) ent.LineWeight = opt.LineWeight.Value;
            if (opt.TextHeight.HasValue)
            {
                if (ent is DBText t) t.Height = opt.TextHeight.Value;
                if (ent is MText mt) mt.TextHeight = opt.TextHeight.Value;
            }
        }

        /// <summary>
        /// Проверка свойств текстового стиля для визуализации
        /// </summary>
        /// <param name="textStyleId">Тестовый стиль инсоляции</param>
        private static void CheckTextStyle(ObjectId textStyleId)
        {
            var textStyle = textStyleId.GetObject(OpenMode.ForRead) as TextStyleTableRecord;
            // Шрифт
            if (!textStyle.FileName.Equals(textStyleFontFile, StringComparison.OrdinalIgnoreCase))
            {
                textStyle.UpgradeOpen();
                textStyle.FileName = textStyleFontFile;
            }
        }
    }
}
