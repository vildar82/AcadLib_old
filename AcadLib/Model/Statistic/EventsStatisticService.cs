namespace AcadLib.Statistic
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.Runtime;
    using FileLog.Entities;
    using JetBrains.Annotations;
    using NetLib;
    using PathChecker;
    using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
    using Exception = System.Exception;

    public static class EventsStatisticService
    {
        private static bool veto;
        private static string sn;
        private static Eventer eventer;
        private static string overrideName;
        private static Document _currentDoc;
        private static string lastModeChange;
        private static string lastSaveAsFile;

        public static void Start()
        {
            try
            {
                CheckExcludeUser();
                Application.DocumentManager.DocumentLockModeChanged += DocumentManager_DocumentLockModeChanged;
                Task.Run(() => { eventer = new Eventer(GetApp(), HostApplicationServices.Current.releaseMarketVersion); });
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

        private static void CheckExcludeUser()
        {
            // Департамент продукта
            var isProductUser = UserInfo.IsProductUser;
            if (isProductUser)
                throw new Exception("Пользователь из Деп.Продукта - Статистика и нейминг пропущен.");

            // Индустрия
            if (Environment.UserDomainName.EqualsIgnoreCase("DSK2"))
                throw new Exception("Пользователь из Индустрии - Статистика и нейминг пропущен.");

            if (Environment.UserName.EqualsIgnoreCase("egorov_ps"))
                throw new Exception("Пользователь из исключений - Фаталит автокад при сохранении.");
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
            eventer?.Start(Case.Default, null);
        }

        private static void DocumentManager_DocumentLockModeChanged(object sender, DocumentLockModeChangedEventArgs e)
        {
            Debug.WriteLine($"DocumentManager_DocumentLockModeChanged {e.GlobalCommandName} {e.Document.Name}");
            try
            {
                short dbmod = (short)Application.GetSystemVariable("DBMOD");
                switch (e.GlobalCommandName)
                {
                    case "QSAVE":
                        StopSave(e, Case.Default);
                        lastModeChange = "QSAVE";
                        break;
                    case "SAVEAS":
                        if (lastModeChange != "SAVEAS")
                        {
                            lastModeChange = "SAVEAS";
                            StopSave(e, Case.SaveAs);
                        }

                        break;
                    case "#SAVEAS":
                        if (lastModeChange != "SAVEAS" || lastSaveAsFile != e.Document.Name)
                        {
                            StopSave(e, Case.SaveAs);
                        }

                        lastModeChange = "#SAVEAS";
                        break;
                    case "CLOSE":
                    case "#CLOSE":
                        if (dbmod != 0 && lastModeChange != "CLOSE")
                        {
                            switch (MessageBox.Show("Файл изменен. Хотите сохранить изменения?", "Внимание!",
                                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
                            {
                                case MessageBoxResult.Yes:
                                    if (!StopSave(e, Case.Default))
                                    {
                                        e.Veto();
                                        CloseSave(e.Document);
                                    }
                                    else
                                    {
                                        e.Veto();
                                        CloseDiscard(e.Document);
                                    }

                                    lastModeChange = "CLOSE";
                                    break;
                                case MessageBoxResult.No:
                                    e.Veto();
                                    CloseDiscard(e.Document);
                                    break;
                                case MessageBoxResult.Cancel:
                                    e.Veto();
                                    break;
                            }
                        }

                        lastModeChange = "CLOSE";
                        break;
                    default:
                        lastModeChange = null;
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                // Отмена
                e.Veto();
            }
            catch (Exception ex)
            {
                Logger.Log.Fatal($"EventsStatisticService DocumentManager_DocumentLockModeChanged, GlobalCommandName={e?.GlobalCommandName}", ex);
            }
        }

        private static void CloseDiscard(Document doc)
        {
            _currentDoc = doc;
            Application.Idle += CloseDiscardOnIdle;
        }

        private static void CloseDiscardOnIdle(object sender, EventArgs e)
        {
            try
            {
                Application.Idle -= CloseDiscardOnIdle;
                _currentDoc.CloseAndDiscard();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("EventsStatisticService CloseDiscardOnIdle", ex);
            }
        }

        private static void CloseSave(Document doc)
        {
            _currentDoc = doc;
            Application.Idle += CloseSaveOnIdle;
        }

        private static void CloseSaveOnIdle(object sender, EventArgs e)
        {
            try
            {
                Application.Idle -= CloseSaveOnIdle;
                _currentDoc.CloseAndSave(_currentDoc.Name);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("EventsStatisticService CloseSaveOnIdle", ex);
            }
        }

        private static bool StopSave(DocumentLockModeChangedEventArgs e, Case @case)
        {
            lastSaveAsFile = e.Document.Name;
            BeginSave(e.Document.Name, @case);
            if (veto)
            {
                e.Veto();
                Debug.WriteLine($"StopSave Veto {e.GlobalCommandName}");
                return true;
            }

            Debug.WriteLine($"StopSave no veto {e.GlobalCommandName}");
            return false;
        }

        private static void DocumentManager_DocumentDestroyed(object sender, [NotNull] DocumentDestroyedEventArgs e)
        {
            eventer?.Finish(EventType.Close, e.FileName, sn);
        }

        private static void DocumentManager_DocumentCreateStarted(object sender, DocumentCollectionEventArgs e)
        {
            eventer?.Start(Case.Default, null);
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
                    Logger.Log.Info($"EventsStatisticService (_pkser) SerialNumber = {sn}");
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, "EventsStatisticService - GetSystemVariable(\"_pkser\")");
                }

                if (sn == null || sn.StartsWith("000"))
                {
                    sn = GetRegistrySerialNumber();
                    Logger.Log.Info($"EventsStatisticService (Registry) SerialNumber = {sn}");
                }
            }

            try
            {
                var db = doc.Database;
                db.SaveComplete += Db_SaveComplete;

                // Если запустили автокад открытием файла dwg из проводника.
                eventer?.Start(Case.Default, null);
                eventer?.Finish(EventType.Open, doc.Name, sn);
                Logger.Log.Info("SubscribeDoc end");
            }
            catch (Exception ex)
            {
                Logger.Log.Error("EventsStatisticService SubscribeDoc", ex);
            }
        }

        private static string GetRegistrySerialNumber()
        {
            try
            {
                var prod = Registry.LocalMachine.OpenSubKey(HostApplicationServices.Current.MachineRegistryProductRootKey);
                return prod.GetValue("SerialNumber")?.ToString();
            }
            catch
            {
                return null;
            }
        }

        private static void BeginSave(string file, Case @case)
        {
            veto = false;
            Debug.WriteLine($"Db_BeginSave {file}");
            if (!IsDwg(file))
                return;
            if (IsCheckError(eventer?.Start(@case, file)))
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
            eventer?.Finish(EventType.Save, e.FileName, sn);
        }

        private static bool IsCheckError(PathCheckerResult checkRes)
        {
            Debug.WriteLine($"checkRes FilePathOverride={checkRes?.FilePathOverride}");
            if (checkRes != null)
            {
                switch (checkRes.NexAction)
                {
                    case NexAction.Proceed:
                        return false;
                    case NexAction.SaveOverride:
                        SaveOverride(checkRes.FilePathOverride);
                        return true;
                    case NexAction.Cancel:
                        throw new OperationCanceledException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return false;
        }

        private static void SaveIdle(object sender, EventArgs e)
        {
            Application.Idle -= SaveIdle;
            var doc = AcadHelper.Doc;
            using (doc.LockDocument())
            {
                doc.Database.SaveAs(doc.Name, true, DwgVersion.Current, doc.Database.SecurityParameters);
            }
        }

        private static void SaveOverride(string overrideName)
        {
            EventsStatisticService.overrideName = overrideName;
            Application.Idle += SaveOverride_Idle;
        }

        private static void SaveOverride_Idle(object sender, EventArgs e)
        {
            Application.Idle -= SaveOverride_Idle;
            Logger.Log.Info("EventsStatisticService SaveOverride_Idle");
            if (string.IsNullOrEmpty(overrideName))
                return;
            var doc = AcadHelper.Doc;
            var oldFile = doc.Name;
            try
            {
                using (doc.LockDocument())
                {
                    doc.Database.SaveAs(overrideName, DwgVersion.Current);
                }
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
            try
            {
                return Path.GetExtension(fileName).EqualsIgnoreCase(".dwg");
            }
            catch
            {
                return false;
            }
        }
    }
}