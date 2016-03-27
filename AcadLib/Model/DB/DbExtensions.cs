using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCAD_PIK_Manager.Settings;
using Autodesk.AutoCAD.DatabaseServices;

namespace Autodesk.AutoCAD.DatabaseServices
{
    public static class DbExtensions
    {
        public static ObjectId GetLineTypeIdByName(this Database db, string name)
        {
            ObjectId resVal = ObjectId.Null;

            using (var ltTable = db.LinetypeTableId.Open(OpenMode.ForRead) as LinetypeTable)
            {
                if (ltTable.Has(name))
                {
                    resVal = ltTable[name];
                }
                else if (!string.Equals(name, SymbolUtilityServices.LinetypeContinuousName, StringComparison.OrdinalIgnoreCase))
                {
                    resVal = db.GetLineTypeIdContinuous();
                }
            }
            return resVal;
        }

        public static ObjectId GetLineTypeIdContinuous(this Database db)
        {
            return db.GetLineTypeIdByName(SymbolUtilityServices.LinetypeContinuousName);
        }


        /// <summary>
        /// Получение табличного стиля ПИК
        /// </summary>      
        public static ObjectId GetTableStylePIK(this Database db)
        {
            ObjectId idStyle = getTableStylePik(db);
            if (idStyle.IsNull)
            {
                // Копирование стиля таблиц из шаблона
                try
                {
                    idStyle = copyObjectFromTemplate(db, getTableStylePik, db.TableStyleDictionaryId);
                }
                catch
                { }
                if (idStyle.IsNull)
                {
                    idStyle = db.Tablestyle;
                }
            }
            return idStyle;
        }

        /// <summary>
        /// Получение табличного стиля ПИК с обновлением (DuplicateRecordCloning.Replace)
        /// Не обновляется существующий стиль ПИК!!!
        /// </summary>      
        public static ObjectId GetTableStylePIK(this Database db, bool update)
        {
            ObjectId idStyle = ObjectId.Null;
            if (!update)
            {
                idStyle = getTableStylePik(db);
            }
            if (update || idStyle.IsNull)
            {
                // Копирование стиля таблиц из шаблона
                try
                {
                    idStyle = copyObjectFromTemplate(db, getTableStylePik, db.TableStyleDictionaryId);
                }
                catch
                { }
                if (idStyle.IsNull)
                {
                    idStyle = db.Tablestyle;
                }
            }
            return idStyle;
        }

        /// <summary>
        /// Получение табличного стиля ПИК
        /// </summary>      
        public static ObjectId GetTableStylePIK(this Database db, string styleName)
        {
            ObjectId idStyle = getTableStylePik(db, styleName);
            if (idStyle.IsNull)
            {
                // Копирование стиля таблиц из шаблона
                try
                {
                    idStyle = copyObjectFromTemplate(db, getTableStylePik, styleName, db.TableStyleDictionaryId);
                }
                catch
                { }
                if (idStyle.IsNull)
                {
                    idStyle = db.Tablestyle;
                }
            }
            return idStyle;
        }

        /// <summary>
        /// Получение текстового стиля ПИК
        /// </summary>  
        public static ObjectId GetTextStylePIK(this Database db)
        {
            ObjectId idStyle = getTextStylePik(db);

            if (idStyle.IsNull)
            {
                // Копирование стиля таблиц из шаблона
                try
                {
                    idStyle = copyObjectFromTemplate(db, getTextStylePik, db.TextStyleTableId);
                }
                catch
                { }
                if (idStyle.IsNull)
                {
                    idStyle = db.Textstyle;
                }
            }
            return idStyle;
        }

        /// <summary>
        /// Получение размерного стиля ПИК
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static ObjectId GetDimStylePIK(this Database db)
        {
            ObjectId idStyle = getDimStylePik(db);

            if (idStyle.IsNull)
            {
                // Копирование размерного стиля из шаблона
                try
                {
                    idStyle = copyObjectFromTemplate(db, getDimStylePik, db.DimStyleTableId);
                }
                catch
                { }
                if (idStyle.IsNull)
                {
                    idStyle = db.Dimstyle;
                }
            }
            return idStyle;
        }

        private static ObjectId getDimStylePik(Database db)
        {
            ObjectId idStyle = ObjectId.Null;
            using (var dimStylesTable = db.DimStyleTableId.Open(OpenMode.ForRead) as DimStyleTable)
            {
                if (dimStylesTable.Has("PIK"))
                {
                    idStyle = dimStylesTable["PIK"];
                }
            }
            return idStyle;
        }

        private static ObjectId getTableStylePik(Database db)
        {
            ObjectId idStyle = ObjectId.Null;
            using (var dictTableStyles = db.TableStyleDictionaryId.Open(OpenMode.ForRead) as DBDictionary)
            {
                if (dictTableStyles.Contains("ПИК"))
                {
                    idStyle = dictTableStyles.GetAt("ПИК");
                }
            }
            return idStyle;
        }

        private static ObjectId getTableStylePik(Database db, string styleName)
        {
            ObjectId idStyle = ObjectId.Null;
            using (var dictTableStyles = db.TableStyleDictionaryId.Open(OpenMode.ForRead) as DBDictionary)
            {
                if (dictTableStyles.Contains(styleName))
                {
                    idStyle = dictTableStyles.GetAt(styleName);
                }
            }
            return idStyle;
        }

        private static ObjectId getTextStylePik(Database db)
        {
            ObjectId idStyle = ObjectId.Null;
            using (var textStyles = db.TextStyleTableId.Open(OpenMode.ForRead) as TextStyleTable)
            {
                if (textStyles.Has("PIK"))
                {
                    idStyle = textStyles["PIK"];
                }
            }
            return idStyle;
        }

        // Копирование стиля таблиц ПИК из файла шаблона
        private static ObjectId copyObjectFromTemplate(Database db, Func<Database, ObjectId> getObjectId, ObjectId ownerIdTable)
        {
            ObjectId idStyleDest = ObjectId.Null;
            // файл шаблона
            string fileTemplate = Path.Combine(PikSettings.LocalSettingsFolder, "Template", PikSettings.UserGroup, PikSettings.UserGroup + ".dwt");
            if (File.Exists(fileTemplate))
            {
                using (Database dbTemplate = new Database(false, true))
                {
                    dbTemplate.ReadDwgFile(fileTemplate, FileOpenMode.OpenForReadAndAllShare, false, "");
                    dbTemplate.CloseInput(true);
                    ObjectId idStyleInTemplate = getObjectId(dbTemplate);
                    if (!idStyleInTemplate.IsNull)
                    {
                        using (IdMapping map = new IdMapping())
                        {
                            using (var ids = (new ObjectIdCollection(new ObjectId[] { idStyleInTemplate })))
                            {
                                db.WblockCloneObjects(ids, ownerIdTable, map, DuplicateRecordCloning.Replace, false);
                                idStyleDest = map[idStyleInTemplate].Value;
                            }
                        }
                    }
                }
            }
            return idStyleDest;
        }

        // Копирование стиля таблиц ПИК из файла шаблона
        private static ObjectId copyObjectFromTemplate(Database db, Func<Database, string, ObjectId> getObjectId, string styleName, ObjectId ownerIdTable)
        {
            ObjectId idStyleDest = ObjectId.Null;
            // файл шаблона
            string fileTemplate = Path.Combine(PikSettings.LocalSettingsFolder, "Template", PikSettings.UserGroup, PikSettings.UserGroup + ".dwt");
            if (File.Exists(fileTemplate))
            {
                using (Database dbTemplate = new Database(false, true))
                {
                    dbTemplate.ReadDwgFile(fileTemplate, FileOpenMode.OpenForReadAndAllShare, false, "");
                    dbTemplate.CloseInput(true);
                    ObjectId idStyleInTemplate = getObjectId(dbTemplate, styleName);
                    if (!idStyleInTemplate.IsNull)
                    {
                        using (IdMapping map = new IdMapping())
                        {
                            using (var ids = (new ObjectIdCollection(new ObjectId[] { idStyleInTemplate })))
                            {
                                db.WblockCloneObjects(ids, ownerIdTable, map, DuplicateRecordCloning.Ignore, false);
                                idStyleDest = map[idStyleInTemplate].Value;
                            }
                        }
                    }
                }
            }
            return idStyleDest;
        }
    }
}
