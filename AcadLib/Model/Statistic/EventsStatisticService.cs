using System;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using EventStatistic;
using JetBrains.Annotations;
using NetLib;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib.Statistic
{
    public static class EventsStatisticService
    {
        [NotNull]
        private static Eventer Eventer =>
            eventer ?? (eventer = new Eventer(GetApp(), HostApplicationServices.Current.releaseMarketVersion));

        [NotNull]
        private static string GetApp()
        {
            // ReSharper disable once EmptyGeneralCatchClause
            try {if (CivilTest.IsCivil()) return "Civil"; } catch {  }
            return "AutoCAD";
        }

        private static Database db;
        private static string sn;
        private static Eventer eventer;

        public static void Start()
        {
            Application.DocumentManager.DocumentCreateStarted += DocumentManager_DocumentCreateStarted;
            Application.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
            Application.DocumentManager.DocumentActivated += DocumentManager_DocumentActivated;
            try
            {
                SubscribeDoc(AcadHelper.Doc);
            }
            catch
            {
                //
            }
        }

        private static void DocumentManager_DocumentCreateStarted(object sender, DocumentCollectionEventArgs e)
        {
            Eventer.Start();
        }

        private static void DocumentManager_DocumentCreated(object sender, [NotNull] DocumentCollectionEventArgs e)
        {
            DocumentCreated(e.Document);
        }

        private static void DocumentCreated(Document doc)
        {
            try
            {
                var res = Eventer.Finish("Открытие", doc.Name, sn);
                if (!string.IsNullOrEmpty(res))
                {
                    Logger.Log.Error($"Ошибка EventsStatistic Открытие Finish Result - {res}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "EventsStatisticService - DocumentManager_DocumentCreated");
            }
        }

        private static void DocumentManager_DocumentActivated(object sender, [NotNull] DocumentCollectionEventArgs e)
        {
            SubscribeDoc(e.Document);
        }

        private static void SubscribeDoc([CanBeNull] Document doc)
        {
            if (doc == null) return;
            if (db != null)
            {
                db.SaveComplete -= Db_SaveComplete;
                db.BeginSave -= Db_BeginSave;
            }
            if (sn == null || sn.StartsWith("000"))
            {
                try
                {
                    sn = Application.GetSystemVariable("_pkser") as string;
                    Logger.Log.Info($"EventsStatisticService SerialNumber = {sn}");
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, "EventsStatisticService - GetSystemVariable(\"_pkser\")");
                }
            }
            db = doc.Database;
            db.SaveComplete += Db_SaveComplete;
            db.BeginSave += Db_BeginSave;

            // Если запустили автокад открытием файла dwg из проводника.
            Eventer.Start();
            DocumentCreated(doc);
        }

        private static void Db_BeginSave(object sender, [NotNull] DatabaseIOEventArgs e)
        {
            try
            {
                if (!IsDwg(e.FileName)) return;
                Eventer.Start();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"Ошибка EventsStatistic Start '{e.FileName}'");
            }
        }

        private static bool IsDwg(string fileName)
        {
            return Path.GetExtension(fileName).EqualsIgnoreCase(".dwg");
        }

        private static void Db_SaveComplete(object sender, [NotNull] DatabaseIOEventArgs e)
        {
            try
            {
                if (!IsDwg(e.FileName)) return;
                var res = Eventer.Finish("Сохранить", e.FileName, sn);
                if (!res.IsNullOrEmpty())
                {
                    Logger.Log.Error($"Ошибка EventsStatistic Сохранить Finish Result - {res}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex ,$"Ошибка EventsStatistic Finish '{e.FileName}'");
            }
        }
    }
}
