﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.XData
{
    /// <summary>
    /// Значение для сохранения в словарь Extension Dictionary. 
    /// Имена Recs и Inners должны быть уникальными
    /// </summary>
    public class DicED
    {
        /// <summary>
        /// Имя словаря
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Вложенные словари
        /// </summary>
        public List<DicED> Inners { get; set; }
        /// <summary>
        /// Записи этого словаря
        /// </summary>
        public List<RecXD> Recs { get; set; }

        public DicED () { }
        public DicED (string name)
        {
            Name = name;
        }

        public void AddRec (RecXD recXd)
        {
            if (recXd == null) return;     
            if (!IsCorrectName(recXd.Name))            
                throw new Exception("Invalid Name - " + recXd.Name);
            
            if (Recs == null) Recs = new List<RecXD>();

            Recs.Add(recXd);
        }

        public void AddInner(DicED recEd)
        {
            if (recEd == null) return;
            if (!IsCorrectName(recEd.Name))
                throw new Exception("Invalid Name - " + recEd.Name);

            if (Inners == null) Inners = new List<DicED>();

            Inners.Add(recEd);
        }

        public RecXD GetRec(string name)
        {            
            return Recs?.Find(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));            
        }

        public DicED GetInner (string name)
        {
            return Inners?.Find(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsCorrectName (string name)
        {
            if (!name.IsValidDbSymbolName())            
                return false;

            if (string.IsNullOrEmpty(name))
                return false;
                        
            if (Inners != null)            
                if (Inners.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    return false;
            if (Recs != null)
                if (Recs.Any(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    return false;

            return true;            
        }
    }
}
