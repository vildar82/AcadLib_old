using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AcadLib.Blocks
{
    public class BlockAttribute
    {
        private string _name;
        private Dictionary<string, string> _atts;

        // Public read only properties
        public string Name
        {
            get { return _name; }
        }

        public Dictionary<string, string> Attributes
        {
            get { return _atts; }
        }

        public string this[string key]
        {
            get { return _atts[key.ToUpper()]; }
        }

        // Constructors
        public BlockAttribute(BlockReference br)
        {
            SetProperties(br);
        }

        public BlockAttribute(ObjectId idBlRef)
        {
            var db = idBlRef.Database;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                SetProperties(tr.GetObject(idBlRef, OpenMode.ForRead) as BlockReference);
            }
        }

        // Public method
        new public string ToString()
        {
            if (_atts != null && _atts.Count > 0)
                return $"{_name}: {_atts.Select(a => $"{a.Key}={a.Value}").Aggregate((a, b) => $"{a}; {b}")}";
            return _name;
        }

        // Private method
        private void SetProperties(BlockReference br)
        {
            if (br == null) return;
            _name = br.GetEffectiveName();
            _atts = new Dictionary<string, string>();
            br.AttributeCollection
                .GetObjects<AttributeReference>()
                .Iterate(att => _atts.Add(att.Tag.ToUpper(), att.TextString));
        }
    }

    public class BlockAttributeEqualityComparer : IEqualityComparer<BlockAttribute>
    {
        public bool Equals(BlockAttribute x, BlockAttribute y)
        {
            return
                x.Name.Equals(y.Name, StringComparison.CurrentCultureIgnoreCase) &&
                x.Attributes.SequenceEqual(y.Attributes);
        }

        public int GetHashCode(BlockAttribute obj)
        {
            return base.GetHashCode();
        }
    }
}