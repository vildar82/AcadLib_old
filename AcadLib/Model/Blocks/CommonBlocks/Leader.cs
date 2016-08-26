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
        public const string ParamName = "ОБОЗНАЧЕНИЕ";

        public string Name { get; set; }
        public Leader (BlockReference blRef, string blName) : base(blRef, blName)
        {
            Name = GetPropValue<string>(ParamName);
        }

        public void SetName (string value)
        {
            FillPropValue(ParamName, value);
        }
    }
}
