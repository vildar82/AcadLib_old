using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;

namespace AcadLib.Blocks
{
    [PublicAPI]
    public class DictBlockName : IDisposable
    {
        private Dictionary<ObjectId, string> dict = new Dictionary<ObjectId, string>();

        [NotNull]
        public string GetName([NotNull] BlockReference blRef)
        {
            if (!dict.TryGetValue(blRef.DynamicBlockTableRecord, out var blName))
            {
#pragma warning disable 618
                blName = blRef.GetEffectiveName();
#pragma warning restore 618
                dict[blRef.DynamicBlockTableRecord] = blName;
            }
            return blName;
        }

        public void Dispose()
        {
            dict = new Dictionary<ObjectId, string>();
        }
    }
}