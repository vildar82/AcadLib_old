namespace AcadLib.Statistic
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.Windows.Themes;
    using JetBrains.Annotations;
    using Naming.Dto;
    using NetLib;
    using PathChecker;
    using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

    public static class EventsStatisticService
    {
        private static bool veto;
        private static string sn;
        private static Eventer eventer;
        private static string overrideName;

        public static void Start()
        {
            try
            {
                CheckProductUser();
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

        private static void CheckProductUser()
        {
            var isProductUser = false;
            try
            {
                isProductUser = UserInfo.IsProductUser;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "UserInfo.IsProductUser");
            }

            if (isProductUser)
                throw new Exception("Пользователь из Деп.Продукта - Статистика и нейминг пропущен.");
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
            if (e.GlobalCommandName == "QSAVE")
            {
                BeginSave(e.Document.Name);
                if (veto)
                {
                    e.Veto();
                    Debug.WriteLine($"DocumentManager_DocumentLockModeChanged Veto {e.GlobalCommandName}");
                }
                else
                {
                    Debug.WriteLine($"DocumentManager_DocumentLockModeChanged {e.GlobalCommandName}");
                }
            }
            else
            {
                Debug.WriteLine($"DocumentManager_DocumentLockModeChanged {e.GlobalCommandName}");
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

            // Если запустили автокад открытием файла dwg из проводника.
            eventer.Start("open", null);
            eventer.Finish("Открытие", doc.Name, sn);
        }

        private static void BeginSave(string file)
        {
            veto = false;
            Debug.WriteLine($"Db_BeginSave {file}");
            if (!IsDwg(file))
                return;
            if (IsCheckError(eventer.Start("save", file)))
            {
                // Отменить сохранение файла
                veto = true;
                Debug.WriteLine($"Отменить сохранение {file}");
            }
        }

        private static void Db_SaveComplete(object sender, [NotNull] DatabaseIOEventArgs e)
        {
            Debug.WriteLine($"Db_SaveComplete {e.FileName}");
            if (!IsDwg(e.FileName))
                return;
            eventer.Finish("Сохранить", e.FileName, sn);
        }

        private static bool IsCheckError(PathCheckerResult checkRes)
        {
            switch (checkRes.NexAction)
            {
                case NexAction.Proceed:
                    return false;
                case NexAction.SaveOverride:
                    SaveOverride(checkRes.FilePathOverride);
                    return true;
                case NexAction.Cancel:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SaveOverride(string overrideName)
        {
            EventsStatisticService.overrideName = overrideName;
            Application.Idle += Application_Idle;
        }

        private static void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= Application_Idle;
            if (overrideName == null)
                return;
            var doc = AcadHelper.Doc;
            var oldFile = doc.Name;
            try
            {
                doc.Database.SaveAs(overrideName, DwgVersion.Current);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла как '{overrideName}' - {ex.Message}");
                Logger.Log.Error(ex, $"SaveOverride.SaveAs - overrideName={overrideName}.");
                return;
            }

            try
            {
                Application.DocumentManager.Open(overrideName, false);
                overrideName = null;
                doc.CloseAndDiscard();
                BackupOldFile(oldFile);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"SaveOverride - oldFile={oldFile}, overrideName={overrideName}.");
            }
        }

        private static void BackupOldFile(string oldFile)
        {
            if (!File.Exists(oldFile))
                return;
            var newName = $"{oldFile}.renamed";
            try
            {
                File.Move(oldFile, newName);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"BackupOldFile - oldFile={oldFile}, newName={newName}.");
            }
        }

        private static bool IsDwg(string fileName)
        {
            return Path.GetExtension(fileName).EqualsIgnoreCase(".dwg");
        }
    }
}