using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AcadLib;
using AutoCAD_PIK_Manager.Settings;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Autodesk.AutoCAD.DatabaseServices
{
    public static class DbExtensions
    {
        public const string PIK = General.Company;

        public static IEnumerable<T> IterateDB<T>(this Database db) where T : DBObject
        {
            for (var i = db.BlockTableId.Handle.Value; i < db.Handseed.Value; i++)
            {
                if (!db.TryGetObjectId(new Handle(i), out ObjectId id)) continue;
                var objT = id.GetObject<T>();
                if (objT != null)
                {
                    yield return objT;
                }
            }
        }

        public static ObjectId GetLineTypeIdByName(this Database db, string name)
        {
            var resVal = ObjectId.Null;

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
            var idStyle = getTableStylePik(db,PIK);
            if (idStyle.IsNull)
            {
                // Копирование стиля таблиц из шаблона
                try
                {
                    idStyle = copyObjectFromTemplate(db, getTableStylePik, PIK, db.TableStyleDictionaryId);
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
            var idStyle = ObjectId.Null;
            if (!update)
            {
                idStyle = getTableStylePik(db, PIK);
            }
            if (update || idStyle.IsNull)
            {
                // Копирование стиля таблиц из шаблона
                try
                {
                    idStyle = copyObjectFromTemplate(db, getTableStylePik, PIK, db.TableStyleDictionaryId);
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
            var idStyle = getTableStylePik(db, styleName);
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
        /// Получение табличного стиля ПИК
        /// </summary>      
        public static ObjectId GetTableStylePIK(this Database db, string styleName, bool update)
        {
            var idStyle = ObjectId.Null;
            if (!update)
            {
                idStyle = getTableStylePik(db, styleName);
            }
            if (update || idStyle.IsNull)
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
            var idStyle = getTextStylePik(db, PIK);

            if (idStyle.IsNull)
            {
                // Копирование стиля таблиц из шаблона
                try
                {
                    idStyle = copyObjectFromTemplate(db, getTextStylePik, PIK, db.TextStyleTableId);
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
        /// Получение табличного стиля ПИК
        /// </summary>      
        public static ObjectId GetTextStylePIK (this Database db, string styleName)
        {
            var idStyle = getTextStylePik(db, styleName);
            if (idStyle.IsNull)
            {
                // Копирование стиля таблиц из шаблона
                try
                {
                    idStyle = copyObjectFromTemplate(db, getTextStylePik, styleName, db.TableStyleDictionaryId);
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
        /// Получение размерного стиля ПИК
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static ObjectId GetDimStylePIK(this Database db)
        {
            var idStyle = getDimStylePik(db, PIK);

            if (idStyle.IsNull)
            {
                // Копирование размерного стиля из шаблона
                try
                {
                    idStyle = copyObjectFromTemplate(db, getDimStylePik, PIK, db.DimStyleTableId);
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

        /// <summary>
        /// Получение углового размерного стиля ПИК
        /// </summary>        
        public static ObjectId GetDimAngularStylePIK(this Database db)
        {
            // Загрузка простого стиля ПИК
            db.GetDimStylePIK();
            // Загрузка углового стиля ПИК
            var angStyle = PIK + "$2";
            var idStyle = getDimStylePik(db, angStyle);

            if (idStyle.IsNull)
            {
                // Копирование размерного стиля из шаблона
                try
                {
                    idStyle = copyObjectFromTemplate(db, getDimStylePik, angStyle, db.DimStyleTableId);
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

        private static ObjectId getDimStylePik(Database db, string styleName)
        {
            var idStyle = ObjectId.Null;
            using (var dimStylesTable = db.DimStyleTableId.Open(OpenMode.ForRead) as DimStyleTable)
            {
                if (dimStylesTable.Has(styleName))
                {
                    idStyle = dimStylesTable[styleName];
                }
            }
            return idStyle;
        }        

        private static ObjectId getTableStylePik(Database db, string styleName)
        {
            var idStyle = ObjectId.Null;
            using (var dictTableStyles = db.TableStyleDictionaryId.Open(OpenMode.ForRead) as DBDictionary)
            {
                if (dictTableStyles.Contains(styleName))
                {
                    idStyle = dictTableStyles.GetAt(styleName);
                }
            }
            return idStyle;
        }

        private static ObjectId getTextStylePik(Database db, string styleName)
        {
            var idStyle = ObjectId.Null;
            using (var textStyles = db.TextStyleTableId.Open(OpenMode.ForRead) as TextStyleTable)
            {
                if (textStyles.Has(styleName))
                {
                    idStyle = textStyles[styleName];
                }
            }
            return idStyle;
        }

        // Копирование стиля таблиц ПИК из файла шаблона
        private static ObjectId copyObjectFromTemplate(Database db, Func<Database, string, ObjectId> getObjectId, string styleName, ObjectId ownerIdTable)
        {
            var idStyleDest = ObjectId.Null;
            // файл шаблона
            var userGroup = PikSettings.UserGroupsCombined.First();
            var fileTemplate = Path.Combine(PikSettings.LocalSettingsFolder, "Template", userGroup,
                userGroup + ".dwt");
            if (File.Exists(fileTemplate))
            {
                using (var dbTemplate = new Database(false, true))
                {
                    dbTemplate.ReadDwgFile(fileTemplate, FileOpenMode.OpenForReadAndAllShare, false, "");
                    dbTemplate.CloseInput(true);
                    var idStyleInTemplate = getObjectId(dbTemplate, styleName);
                    if (!idStyleInTemplate.IsNull)
                    {
                        using (var map = new IdMapping())
                        {
                            using (var ids = (new ObjectIdCollection(new ObjectId[] { idStyleInTemplate })))
                            {
                                using (Application.DocumentManager.MdiActiveDocument?.LockDocument())
                                {
                                    db.WblockCloneObjects(ids, ownerIdTable, map, DuplicateRecordCloning.Replace, false);
                                }
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
