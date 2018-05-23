// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2017 06 27 10:07

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using AcadLib;
using AcadLib.Blocks.Visual;
using AcadLib.Colors;
using AcadLib.DbYouTubeTableAdapters;
using AcadLib.Editors;
using AcadLib.Errors;
using AcadLib.Field;
using AcadLib.Layers;
using AcadLib.Layers.AutoLayers;
using AcadLib.Layers.LayersSelected;
using AcadLib.Lisp;
using AcadLib.PaletteCommands;
using AcadLib.Plot;
using AcadLib.Properties;
using AcadLib.Statistic;
using AcadLib.Template;
using AcadLib.UI.Ribbon;
using AcadLib.UI.StatusBar;
using AcadLib.Utils;
using AutoCAD_PIK_Manager.Settings;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using JetBrains.Annotations;
using NetLib;
using NetLib.IO;
using Exception = System.Exception;
using Path = System.IO.Path;

[assembly: CommandClass(typeof(Commands))]
[assembly: ExtensionApplication(typeof(Commands))]

namespace AcadLib
{
    [PublicAPI]
    public class Commands : IExtensionApplication
    {
        public const string CommandBlockList = "PIK_BlockList";
        public const string CommandCleanZombieBlocks = "PIK_CleanZombieBlocks";

        public const string CommandColorBookNCS = "PIK_ColorBookNCS";

        //public const string CommandInsertBlockPikLogo = "PIK_InsertBlockLogo";
        public const string CommandXDataView = "PIK_XDataView";

        public const string GroupCommon = "Общие";
        public const string Group = AutoCAD_PIK_Manager.Commands.Group;

        internal static readonly string fileCommonBlocks =
            Path.Combine(PikSettings.LocalSettingsFolder, @"Blocks\Блоки-оформления.dwg");
        
        public static readonly Assembly AcadLibAssembly = Assembly.GetExecutingAssembly();
        public static readonly Version AcadLibVersion = AcadLibAssembly.GetName().Version;
        public static readonly string CurDllDir = Path.GetDirectoryName(AcadLibAssembly.Location);

        private readonly Timer timer = new Timer();

        private List<DllResolve> dllsResolve;
        private C_PlayStatisticTableAdapter player;

        /// <summary>
        ///     Общие команды для всех отделов определенные в этой сборке
        /// </summary>
        public static List<IPaletteCommand> CommandsPalette { get; set; }

        public void Initialize()
        {
#if DEBUG // Отключение отладочных сообщений биндинга (тормозит сильно)
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Off;
#endif
            try
            {
                Logger.Log.Info("start Initialize AcadLib");
                try
                {
                    StatusBarEx.AddPaneUserGroup();
                }
                catch
                {
                    //
                }
                PluginStatisticsHelper.StartAutoCAD();
                CheckUpdates.Start();
                if (Settings.Default.UpgradeRequired)
                {
                    Settings.Default.Upgrade();
                    Settings.Default.UpgradeRequired = false;
                    Settings.Default.Save();
                }
                AllCommandsCommon();
                // Автослоиtest
                AutoLayersService.Init();
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                // Загрузка сборок из папки ../Script/Net - без вложенных папок
                LoadService.LoadFromFolder(Path.Combine(PikSettings.LocalSettingsFolder, @"Script\NET"),
                    SearchOption.TopDirectoryOnly);
                // Загрузка сборок из папки ../Script/Net/[UserGroup]
                foreach (var userGroup in PikSettings.UserGroupsCombined)
                {
                    var dirGroup = Path.Combine(PikSettings.LocalSettingsFolder, $@"Script\NET\{userGroup}");
                    LoadService.LoadFromFolder(dirGroup, SearchOption.TopDirectoryOnly);
                }
                // автозагрузка стартового общего лиспа
                LispAutoloader.Start();
                if (PaletteSetCommands._paletteSets.Any()) RibbonBuilder.InitRibbon();
                Logger.Log.Info("end Initialize AcadLib");
                YoutubeStatisticInit();
                EventsStatisticService.Start();
                AcadLibAssembly.AcadLoadInfo();
            }
            catch (Exception ex)
            {
                $"PIK. Ошибка загрузки AcadLib, версия:{AcadLibVersion} - {ex.Message}.".WriteToCommandLine();
                Logger.Log.Error(ex, "AcadLib Initialize.");
            }
        }

        private void YoutubeStatisticInit()
        {
            try
            {
                var procsR = Process.GetProcessesByName("Acad");
                if (procsR.Length == 1)
                {
                    player = new C_PlayStatisticTableAdapter();
                    timer.Interval = 60000 * 3;
                    timer.Tick += Timer_Tick;
                    timer.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "YoutubeStatisticInit");
            }
        }

        public void Terminate()
        {
            Logger.Log.Info("Terminatenh AcadLib");
        }

        [CommandMethod(Group, "PIK_Acadlib_About", CommandFlags.Modal)]
        public void About()
        {
            CommandStart.Start(doc =>
            {
                var ed = doc.Editor;
                ed.WriteMessage($"\nБиблиотека AcadLib версии {AcadLibVersion}");
            });
        }

        /// <summary>
        ///     Список общих команд
        /// </summary>
        internal static void AllCommandsCommon()
        {
            try
            {
                CommandsPalette = new List<IPaletteCommand>
                {
                    new PaletteInsertBlock("PIK_Project-Logo", fileCommonBlocks, "Блок логотипа", Resources.logo,
                        "Вставка блока логотипа ПИК.", GroupCommon),
                    new PaletteCommand("Просмотр расширенных данных примитива", Resources.PIK_XDataView, CommandXDataView,
                        "Просмотр расширенных данных (XData) примитива.", GroupCommon),
                    new PaletteCommand("Проверка и очистка", Resources.purge, nameof(PIK_PurgeAuditRegen),
                        "Очистка (_purge), проверка (_audit), сброс списка масштабов аннотации (_scalelistedit) и регенерация чертежа.",
                        GroupCommon),
                    new PaletteCommand("Последние ошибки", Resources.error, nameof(PIK_Errors), "Показать окно последних ошибок", GroupCommon)
                };
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "AcadLib.AllCommandsCommon()");
                CommandsPalette = new List<IPaletteCommand>();
            }
        }

        [CommandMethod(Group, CommandBlockList, CommandFlags.Modal)]
        public void BlockListCommand()
        {
            CommandStart.Start(doc => doc.Database.List());
        }

        [CommandMethod(Group, CommandCleanZombieBlocks, CommandFlags.Modal)]
        public void CleanZombieBlocks()
        {
            CommandStart.Start(doc =>
            {
                var db = doc.Database;
                var countZombie = db.CleanZombieBlock();
                doc.Editor.WriteMessage($"\nУдалено {countZombie} зомби!☻");
            });
        }

        [CommandMethod(Group, CommandColorBookNCS, CommandFlags.Modal | CommandFlags.Session)]
        public void ColorBookNCS()
        {
            CommandStart.Start(doc => ColorBookHelper.GenerateNCS());
        }

        [CanBeNull]
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (dllsResolve == null)
            {
                // Сборки в основной папке dll
                dllsResolve = DllResolve.GetDllResolve(CurDllDir, SearchOption.TopDirectoryOnly);
                // Все сборки из папки Script\NET
                dllsResolve.AddRange(DllResolve.GetDllResolve(
                    Path.Combine(PikSettings.LocalSettingsFolder, @"Script\NET"),
                    SearchOption.AllDirectories));
                // Все сборки из локальной папки packages
                //dllsResolve.AddRange(DllResolve.GetDllResolve(LoadService.dllLocalPackages, SearchOption.AllDirectories));
                // Оставить только сборки под текущую версию автокада
                dllsResolve = FilterDllResolveVersions(dllsResolve);
            }
            var dllResolver = dllsResolve.FirstOrDefault(f => f.IsResolve(args.Name));
            if (dllResolver == null) return null;
            try
            {
                var asm = dllResolver.LoadAssembly();
                Logger.Log.Info($"resolve assembly - {asm.FullName}");
                return asm;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"Ошибка AssemblyResolve - {dllResolver.DllFile}.");
            }
            return null;
        }

        [NotNull]
        private List<DllResolve> FilterDllResolveVersions(List<DllResolve> dllResolves)
        {
            return LoadService.GetDllsForCurVerAcad(dllsResolve.Select(s => s.DllFile).ToList())
                .Select(s => new DllResolve(s.Dll) {DllName = s.FileWoVer}).ToList();
        }

        [CommandMethod(Group, nameof(PIK_AutoLayersAll), CommandFlags.Modal)]
        public void PIK_AutoLayersAll()
        {
            CommandStart.Start(doc => AutoLayersService.AutoLayersAll());
        }

        [CommandMethod(Group, nameof(PIK_AutoLayersStart), CommandFlags.Modal)]
        public void PIK_AutoLayersStart()
        {
            CommandStart.Start(doc =>
            {
                AutoLayersService.Start();
                doc.Editor.WriteMessage($"\n{AutoLayersService.GetInfo()}");
            });
        }

        [CommandMethod(Group, nameof(PIK_AutoLayersStatus), CommandFlags.Modal)]
        public void PIK_AutoLayersStatus()
        {
            CommandStart.Start(doc => doc.Editor.WriteMessage($"\n{AutoLayersService.GetInfo()}"));
        }

        [CommandMethod(Group, nameof(PIK_AutoLayersStop), CommandFlags.Modal)]
        public void PIK_AutoLayersStop()
        {
            CommandStart.Start(doc =>
            {
                AutoLayersService.Stop();
                doc.Editor.WriteMessage($"\n{AutoLayersService.GetInfo()}");
            });
        }

        [CommandMethod(Group, nameof(PIK_BlocksUnitsless), CommandFlags.Modal)]
        public void PIK_BlocksUnitsless()
        {
            CommandStart.Start(doc =>
            {
                var db = doc.Database;
                using (var t = db.TransactionManager.StartTransaction())
                {
                    var bt = (BlockTable) db.BlockTableId.GetObject(OpenMode.ForRead);
                    foreach (var id in bt)
                    {
                        var btr = (BlockTableRecord) id.GetObject(OpenMode.ForRead);
                        if (btr.IsLayout || btr.IsAnonymous || btr.IsDependent) continue;
                        if (btr.Units != UnitsValue.Undefined)
                        {
                            btr = (BlockTableRecord) id.GetObject(OpenMode.ForWrite);
                            btr.Units = UnitsValue.Undefined;
                        }
                    }
                    t.Commit();
                }
            });
        }

        [CommandMethod(Group, nameof(PIK_DbObjectsCountInfo), CommandFlags.Modal)]
        public void PIK_DbObjectsCountInfo()
        {
            CommandStart.Start(doc =>
            {
                var db = doc.Database;
                var ed = doc.Editor;
                var allTypes = new Dictionary<string, int>();
                for (var i = db.BlockTableId.Handle.Value; i < db.Handseed.Value; i++)
                {
                    if (!db.TryGetObjectId(new Handle(i), out var id)) continue;
                    if (allTypes.ContainsKey(id.ObjectClass.Name))
                        allTypes[id.ObjectClass.Name]++;
                    else
                        allTypes.Add(id.ObjectClass.Name, 1);
                }
                var sortedByCount = allTypes.OrderBy(i => i.Value);
                foreach (var item in sortedByCount)
                    ed.WriteMessage($"\n{item.Key} - {item.Value}");
            });
        }

        [CommandMethod(Group, nameof(PIK_ExportTemplateToJson), CommandFlags.Modal)]
        public void PIK_ExportTemplateToJson()
        {
            CommandStart.Start(doc =>
            {
                if (!doc.IsNamedDrawing) throw new Exception("Чертеж не сохранен на диске");
                var tData = TemplateManager.LoadFromDb(doc.Database);
                var file = Path.ChangeExtension(doc.Name, "json");
                tData.ExportToJson(file ?? throw new InvalidOperationException());
                Process.Start(file);
            });
        }

        [CommandMethod(Group, nameof(PIK_LayersSelectedObjects), CommandFlags.UsePickSet)]
        public void PIK_LayersSelectedObjects()
        {
            CommandStart.Start(LayersSelectedService.Show);
        }

        /// <summary>
        ///     Визуальное окно для вставки блока из файла
        /// </summary>
        /// <param name="rb">Парметры: Имя файла, имя слоя, соответствия имен блоков</param>
        [LispFunction(nameof(PIK_LispInsertBlockFromFbDwg))]
        public void PIK_LispInsertBlockFromFbDwg([CanBeNull] ResultBuffer rb)
        {
            try
            {
                if (rb == null) return;
                var tvs = rb.AsArray();
                if (!tvs.Any()) return;
                var fileName = tvs[0].Value.ToString();
                var layerName = tvs[1].Value.ToString();
                var layer = new LayerInfo(layerName);
                var matchs = tvs.Skip(2).ToList();
                var file = Path.Combine(PikSettings.LocalSettingsFolder, @"flexBrics\dwg\", fileName);
                VisualInsertBlock.InsertBlock(file,
                    n => matchs.Any(r => Regex.IsMatch(n, r.Value.ToString(), RegexOptions.IgnoreCase)),
                    layer);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "PIK_LispInsertBlockFromFbDwg");
            }
        }

        [LispFunction(nameof(PIK_LispLog))]
        public void PIK_LispLog([NotNull] ResultBuffer rb)
        {
            var tvs = rb.AsArray();
            if (tvs.Any())
                Logger.Log.InfoLisp(tvs[0].Value.ToString());
        }

        [CommandMethod(Group, nameof(PIK_ModelObjectsCountInfo), CommandFlags.Modal)]
        public void PIK_ModelObjectsCountInfo()
        {
            CommandStart.Start(doc =>
            {
                var db = doc.Database;
                var ed = doc.Editor;
                using (var t = db.TransactionManager.StartTransaction())
                {
                    var allTypes = new Dictionary<string, int>();
                    var ms = (BlockTableRecord) SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject(OpenMode.ForRead);
                    foreach (var id in ms)
                    {
                        if (allTypes.ContainsKey(id.ObjectClass.Name))
                            allTypes[id.ObjectClass.Name]++;
                        else
                            allTypes.Add(id.ObjectClass.Name, 1);
                    }
                    var sortedByCount = allTypes.OrderBy(i => i.Value);
                    foreach (var item in sortedByCount)
                        ed.WriteMessage($"\n{item.Key} - {item.Value}");
                    t.Commit();
                }
            });
        }

        [CommandMethod(Group, nameof(PIK_PlotToPdf), CommandFlags.Session)]
        public void PIK_PlotToPdf()
        {
            CommandStart.Start(doc =>
            {
                using (doc.LockDocument())
                    PlotDirToPdf.PromptAndPlot(doc);
            });
        }

        [CommandMethod(Group, nameof(PIK_PurgeAuditRegen), CommandFlags.Modal)]
        public void PIK_PurgeAuditRegen()
        {
            CommandStart.Start(doc =>
            {
                var ed = doc.Editor;
                ed.Command("_-purge", "_All", "*", "_No");
                ed.Command("_audit", "_Yes");
                ed.Command("_-scalelistedit", "_R", "_Y", "_E");
                ed.Regen();
            });
        }

        [CommandMethod(Group, nameof(PIK_Ribbon), CommandFlags.Modal)]
        public void PIK_Ribbon()
        {
            CommandStart.Start(d => RibbonBuilder.CreateRibbon());
        }

        [CommandMethod(Group, nameof(PIK_SearchById), CommandFlags.Modal)]
        public void PIK_SearchById()
        {
            CommandStart.Start(doc =>
            {
                var ed = doc.Editor;
                var res = ed.GetString("\nВведи ObjectID, например:8796086050096");
                if (res.Status != PromptStatus.OK) return;
                var id = long.Parse(res.StringResult);
                var db = doc.Database;
                using (var t = db.TransactionManager.StartTransaction())
                {
                    var ms = (BlockTableRecord) SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject(OpenMode.ForRead);
#pragma warning disable 618
                    var entId = ms.Cast<ObjectId>().FirstOrDefault(f => f.OldId == id);
#pragma warning restore 618
                    if (entId.IsNull)
                        "Элемент не найден в Моделе.".WriteToCommandLine();
                    else
                        entId.ShowEnt();
                    t.Commit();
                }
            });
        }

        [CommandMethod(Group, nameof(PIK_Test), CommandFlags.Modal)]
        public void PIK_Test()
        {
            CommandStart.Start(doc =>
            {
                //var templateFile = TemplateManager.GetTemplateFile("PIK_Masterplan_24.08.17_на согласование.dwt");
                //doc.Database.ImportLayerFilterTree(templateFile);
                //doc.Database.ImportLayerStates(templateFile);
            });
        }

        [CommandMethod(Group, nameof(PIK_UpdateFieldsInObjects), CommandFlags.Modal)]
        public void PIK_UpdateFieldsInObjects()
        {
            CommandStart.Start(doc => UpdateField.UpdateInSelected());
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                var procsChrome = Process.GetProcessesByName("chrome");
                if (procsChrome.Length <= 0)
                {
                }
                else
                {
                    foreach (var proc in procsChrome)
                    {
                        if (proc.MainWindowHandle == IntPtr.Zero) continue;
                        var root = AutomationElement.FromHandle(proc.MainWindowHandle);
                        var activeTabName = root.Current.Name;
                        if (activeTabName.ToLower().Contains("youtube"))
                        {
                            try
                            {
                                player.Insert(Environment.UserName, "AutoCAD", activeTabName, DateTime.Now);
                                break;
                            }
                            catch (Exception ex)
                            {
                                Logger.Log.Error(ex, "Video Statistic");
                            }
                        }
                    }
                }
            });
        }

        [CommandMethod(Group, CommandXDataView, CommandFlags.Modal)]
        public void XDataView()
        {
            CommandStart.Start(doc => XData.Viewer.XDataView.View());
        }

        [CommandMethod(Group, nameof(PIK_ListModules), CommandFlags.Modal)]
        public void PIK_ListModules()
        {
            CommandStart.Start(doc => ListModules.List());
        }

        [CommandMethod(Group, nameof(PIK_Errors), CommandFlags.Modal)]
        public void PIK_Errors()
        {
            CommandStart.StartWoStat(d => Inspector.ShowLast());
        }

        [CommandMethod(Group, nameof(PIK_StyleManager), CommandFlags.Modal)]
        public void PIK_StyleManager()
        {
            CommandStart.Start(d => Styles.StyleManager.StyleManagerService.ManageStyles());
        }
    }
}