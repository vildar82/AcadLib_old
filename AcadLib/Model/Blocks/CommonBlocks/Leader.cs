using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Blocks.CommonBlocks
{
    public class Leader : BlockBase
    {
        public const string BlockName = "Обозначение_Выноска_ПИК";
        public Leader (BlockReference blRef, string blName) : base(blRef, blName)
        {
        }
    }
}
