using System;
using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib.Dim
{
    [PublicAPI]
    public static class DimensionExt
    {
        public static ObjectId GetDimBlkObjectId([NotNull] this Database db, DimBlkEnum dimBlk)
        {
            var blkName = GetDimBlkName(dimBlk);
            // Get the current value of DIMBLK
            var oldArrName = Application.GetSystemVariable("DIMBLK") as string;
            // Set DIMBLK to the new style (this action may create a new block) 
            Application.SetSystemVariable("DIMBLK", blkName);
            // Reset the previous value of DIMBLK
            if (oldArrName?.Length != 0) Application.SetSystemVariable("DIMBLK", oldArrName);
#pragma warning disable 618
            using (var bt = (BlockTable)db.BlockTableId.Open(OpenMode.ForRead))
#pragma warning restore 618
            {
                return bt[blkName];
            }
        }

        [NotNull]
        private static string GetDimBlkName(DimBlkEnum dimBlk)
        {
            switch (dimBlk)
            {
                case DimBlkEnum.FilledClosed: return "";
                case DimBlkEnum.DOT: return "_DOT";
                case DimBlkEnum.DOTSMALL: return "_DOTSMALL";
                case DimBlkEnum.DOTBLANK:return "_DOTBLANK";
                case DimBlkEnum.ORIGIN:return "_ORIGIN";
                case DimBlkEnum.ORIGIN2:return "_ORIGIN2";
                case DimBlkEnum.OPEN:return "_OPEN";
                case DimBlkEnum.OPEN90:return "_OPEN90";
                case DimBlkEnum.OPEN30:return "_OPEN30";
                case DimBlkEnum.CLOSED:return "_CLOSED";
                case DimBlkEnum.SMALL:return "_SMALL";
                case DimBlkEnum.NONE:return "_NONE";
                case DimBlkEnum.OBLIQUE:return "_OBLIQUE";
                case DimBlkEnum.BOXFILLED:return "_BOXFILLED";
                case DimBlkEnum.BOXBLANK:return "_BOXBLANK";
                case DimBlkEnum.CLOSEDBLANK:return "_CLOSEDBLANK";
                case DimBlkEnum.DATUMFILLED:return "_DATUMFILLED";
                case DimBlkEnum.DATUMBLANK:return "_DATUMBLANK";
                case DimBlkEnum.INTEGRAL:return "_INTEGRAL";
                case DimBlkEnum.ARCHTICK:return "_ARCHTICK";
                default: throw new ArgumentOutOfRangeException(nameof(dimBlk), dimBlk, null);
            }
        }
    }
}
