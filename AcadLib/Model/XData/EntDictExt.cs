using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.XData
{
    /// <summary>
    /// Расширенные данные примитива.
    /// Должна быть запущена Транзакция! И объект открыт для записи.
    /// </summary>
    public class EntDictExt : IDisposable
    {
        const string PikApp = "PIK";
        readonly DBObject dbo;
        readonly string pluginName;

        public EntDictExt(DBObject dbo, string pluginName)
        {
            if (dbo == null) throw new ArgumentNullException();
            this.dbo = dbo;
            this.pluginName = pluginName;            
        }        

        /// <summary>
        /// Сохранение данных в объекте. 
        /// Можно передать только string, int, double
        /// </summary>        
        public void Save (string rec, object value)
        {
            Save(new List<TypedValue> { new TypedValue(GetExtendetDataType(value.GetType()), value) }, rec);            
        }

        public void Save (List<TypedValue> values, string rec)
        {
            ResultBuffer rb = new ResultBuffer(values.ToArray());
            Xrecord xRec = GetXRecord(rec, true);
            if (xRec == null)
            {
                return;
            }
            xRec.Data = rb;
        }

        /// <summary>
        /// Чтение Xrecord из объекта
        /// </summary>
        /// <typeparam name="T">Хранимый тип - может быть string, int, double, List/<TypedValue/></typeparam>        
        public T Load<T> (string rec)
        {
            var typeT = typeof(T);
            int type = GetExtendetDataType(typeT);
            var xRec = GetXRecord(rec, false);
            if (xRec == null)
            {
                return default(T);
            }            

            foreach (var item in xRec.Data)
            {
                if (type == item.TypeCode)
                {
                    return (T)item.Value;
                }
            }
            return default(T);
        }

        /// <summary>
        /// Чтение всех Xrecord из словаря плагина
        /// </summary>        
        public Dictionary<string, List<TypedValue>> LoadAllXRecords ()
        {
            var dict = GetDict(false);
            if (dict == null) return null;

            var res = new Dictionary<string, List<TypedValue>>();
            foreach (var item in dict)
            {
                var rec = item.Value.GetObject(OpenMode.ForRead) as Xrecord;
                if (rec == null) continue;
                res.Add(item.Key, rec.Data.AsArray().ToList());
            }
            return res;
        }

        private int GetExtendetDataType(Type value)
        {
            if(value == typeof(string))
            {
                return (int)DxfCode.ExtendedDataAsciiString;
            }
            else if (value == typeof(int))
            {
                return (int)DxfCode.ExtendedDataInteger32;
            }
            else if (value == typeof(double))
            {
                return (int)DxfCode.ExtendedDataReal;                
            }
            else
            {
                throw new ArgumentException($"В расшир.данные можно сохранять только string, int, double, а тип '{value.GetType()}' нет.");
            }
        }

        private Xrecord GetXRecord(string rec, bool create)
        {
            var dict = GetDict(create);
            if (dict == null) return null;

            if (dict.Contains(rec))
            {
                OpenMode mode = create ? OpenMode.ForWrite : OpenMode.ForRead;
                return dict.GetAt(rec).GetObject(mode) as Xrecord;
            }
            else
            {
                if (create)
                {
                    Xrecord xRec = new Xrecord();
                    dict.SetAt(rec, xRec);
                    dict.Database.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(xRec, true);
                    return xRec;
                }
            }
            return null;
        }       

        private DBDictionary GetDict(bool create)
        {
            DBDictionary pluginDict = null;
            if (dbo.ExtensionDictionary.IsNull)
            {
                if (create)
                {
                    dbo.CreateExtensionDictionary();
                }
                else
                {
                    return pluginDict;
                }
            }
            var extDict = dbo.ExtensionDictionary.GetObject(OpenMode.ForRead) as DBDictionary;
            
            var pikDict = GetDict(extDict, PikApp, create);
            if (pikDict == null)
            {
                return pluginDict;
            }
            return GetDict(pikDict, pluginName, create);            
        }

        private DBDictionary GetDict(DBDictionary dict, string name, bool create)
        {
            DBDictionary innerDict = null;
            if (!dict.Contains(name))
            {
                if (create)
                {
                    dict.UpgradeOpen();
                    innerDict = new DBDictionary();
                    dict.SetAt(name, innerDict);
                    dbo.Database.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(innerDict, true);
                }
            }
            else
            {
                var idInnerDict = dict.GetAt(name);
                innerDict = idInnerDict.GetObject(OpenMode.ForWrite) as DBDictionary;
            }
            return innerDict;
        }

        public void Dispose()
        {            
        }
    }
}