using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Filer
{
    public class ReferenceFilerResult
    {
        public List<ObjectId> SoftPointerIds { get; set; } 
        public List<ObjectId> HardPointerIds { get; set;} 
        public List<ObjectId> SoftOwnershipIds { get;set; } 
        public List<ObjectId> HardOwnershipIds { get;set; } 
    }
}
