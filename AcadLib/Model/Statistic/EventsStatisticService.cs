namespace AcadLib.Statistic
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using JetBrains.Annotations;
    using Naming.Dto;
    using NetLib;
    using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
    using General = AcadLib.General;

    public static class EventsStatisticService
    {
        private static bool veto;
        private static string sn;
        private static Eventer eventer;

        public static void Start()
        {
            try
            {
                Application.DocumentManager.DocumentLockModeChanged += DocumentManager_DocumentLockModeChanged;
                eventer = new Eventer(GetApp(), HostApplicationServices.Current.releaseMarketVersion);
                Application.DocumentManager.DocumentCreateStarted += DocumentManager_DocumentCreateStarted;
                Application.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
                Application.DocumentManager.DocumentToBeDestroyed += DocumentManager_DocumentToBeDestroyed;
                Application.DocumentManager.DocumentDestroyed += DocumentManager_DocumentDestroyed;

                foreach (Document doc in Application.DocumentManager)
                {
                    SubscribeDoc(doc);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "EventsStatisticService.Start");
            }
        }

        [NotNull]
        private static string GetApp()
        {
            try
            {
                if (CivilTest.IsCivil())
                    return "Civil";
            }
            catch
            {
                // Это не Civil
            }

            return "AutoCAD";
        }

        private static void DocumentManager_DocumentToBeDestroyed(object sender, DocumentCollectionEventArgs e)
        {
            eventer.Start("close", null);
        }

        private static void DocumentManager_DocumentLockModeChanged(object sender, DocumentLockModeChangedEventArgs e)
        {
            Debug.WriteLine($"DocumentManager_DocumentLockModeChanged {e.GlobalCommandName}");
            if (veto)
            {
                veto = false;
                e.Veto();
            }
        }

        private static void DocumentManager_DocumentDestroyed(object sender, [NotNull] DocumentDestroyedEventArgs e)
        {
            eventer.Finish("Закрытие", e.FileName, sn);
        }

        private static void DocumentManager_DocumentCreateStarted(object sender, DocumentCollectionEventArgs e)
        {
            eventer.Start("open", null);
        }

        private static void DocumentManager_DocumentCreated(object sender, [NotNull] DocumentCollectionEventArgs e)
        {
            SubscribeDoc(e.Document);
        }

        private static void SubscribeDoc([CanBeNull] Document doc)
        {
            if (doc == null)
                return;

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

            var db = doc.Database;
            db.SaveComplete -= Db_SaveComplete;
            db.SaveComplete += Db_SaveComplete;
            db.BeginSave -= Db_BeginSave;
            db.BeginSave += Db_BeginSave;

            // Если запустили автокад открытием файла dwg из проводника.
            eventer.Start("open", null);
            eventer.Finish("Открытие", doc.Name, sn);
        }

        private static void Db_BeginSave(object sender, [NotNull] DatabaseIOEventArgs e)
        {
            Debug.WriteLine($"Db_BeginSave {e.FileName}");
            if (!IsDwg(e.FileName))
                return;
            if (IsCheckError(eventer.Start("save", e.FileName)))
            {
                // Отменить сохранение файла
                veto = true;
                Debug.WriteLine($"Отменить сохранение {e.FileName}");
            }
        }

        private static void Db_SaveComplete(object sender, [NotNull] DatabaseIOEventArgs e)
        {
            if (!IsDwg(e.FileName))
                return;
            eventer.Finish("Сохранить", e.FileName, sn);
        }

        private static bool IsCheckError(CheckResultDto checkRes)
        {
#if DEBUG
            return checkRes?.Status == "Warning";
#endif
            return checkRes?.Status == "Error";
        }

        private static bool IsDwg(string fileName)
        {
            return Path.GetExtension(fileName).EqualsIgnoreCase(".dwg");
        }
    }
}