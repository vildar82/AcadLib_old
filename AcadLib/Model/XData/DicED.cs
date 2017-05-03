using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;

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
            if (recXd == null || recXd.IsEmpty()) return;     
            if (!IsCorrectName(recXd.Name))            
                throw new Exception("Invalid Name - " + recXd.Name);
                        
            if (Recs == null) Recs = new List<RecXD>();
            Recs.Add(recXd);
        }
        public void AddRec (string name, List<TypedValue> values)
        {
            AddRec(new RecXD(name, values));
        }

        public void AddInner(DicED dic)
        {
            if (dic == null || dic.IsEmpty()) return;
            if (!IsCorrectName(dic.Name))
                throw new Exception("Invalid Name - " + dic.Name);

            if (Inners == null) Inners = new List<DicED>();
            Inners.Add(dic);
        }

        /// <summary>
        /// Проверка, пустой ли словарь - нет записей и нет вложенных словарей или они пустые
        /// </summary>        
        public bool IsEmpty()
        {
            // Если нет записей или они все пустые, и если нет вложенных словарей или они все пустые
            return (Recs == null || Recs.All(r => r.IsEmpty())) &&
                   (Inners == null || Inners.All(i => i.IsEmpty()));            
        }

        public void AddInner (string name, DicED dic)
        {
            if (dic != null)
            {
                dic.Name = name;
                AddInner(dic);
            }
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
