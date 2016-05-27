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
            ResultBuffer rb = new ResultBuffer(new TypedValue(GetExtendetDataType(value.GetType()), value));
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
        /// <typeparam name="T">Хранимый тип - может быть string, int, double</typeparam>        
        public T Load<T> (string rec)
        {
            int type = GetExtendetDataType(typeof(T));
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
            if (dbo.ExtensionDictionary.IsNull)
            {
                if (!create)
                {
                    return null;
                }
                dbo.CreateExtensionDictionary();
            }
            var extDict = dbo.ExtensionDictionary.GetObject(OpenMode.ForRead) as DBDictionary;

            DBDictionary pluginDict;
            if (!extDict.Contains(pluginName))
            {
                extDict.UpgradeOpen();
                pluginDict = new DBDictionary();
                extDict.SetAt(pluginName, pluginDict);
                dbo.Database.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(pluginDict, true);
            }
            else
            {
                var idInnerDict = extDict.GetAt(pluginName);
                pluginDict = idInnerDict.GetObject(OpenMode.ForWrite) as DBDictionary;
            }
            return pluginDict;
        }

        public void Dispose()
        {            
        }
    }
}