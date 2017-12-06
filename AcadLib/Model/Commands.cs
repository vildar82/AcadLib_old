using AcadLib.Layers;
using AcadLib.PaletteCommands;
using AcadLib.Statistic;
using AutoCAD_PIK_Manager.Settings;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using JetBrains.Annotations;
using NetLib.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Path = System.IO.Path;

[assembly: CommandClass(typeof(AcadLib.Commands))]
[assembly: ExtensionApplication(typeof(AcadLib.Commands))]

namespace AcadLib
{
    public class Commands : IExtensionApplication
    {
        internal static readonly string fileCommonBlocks = Path.Combine(PikSettings.LocalSettingsFolder, @"Blocks\Блоки-оформления.dwg");
        public const string CommandBlockList = "PIK_BlockList";
        public const string CommandCleanZombieBlocks = "PIK_CleanZombieBlocks";
        public const string CommandColorBookNCS = "PIK_ColorBookNCS";
        //public const string CommandInsertBlockPikLogo = "PIK_InsertBlockLogo";
        public const string CommandXDataView = "PIK_XDataView";
        public const string Group = AutoCAD_PIK_Manager.Commands.Group;
        /// <summary>
        /// Общие команды для всех отделов определенные в этой сборке
        /// </summary>
        public static List<IPaletteCommand> CommandsPalette { get; set; }

        private List<DllResolve> dllsResolve;
        public static readonly string CurDllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Список общих команд
        /// </summary>
        internal static void AllCommandsCommon()
        {
            try
            {
                CommandsPalette = new List<IPaletteCommand>()
                {
                    new PaletteInsertBlock("PIK_Logo", fileCommonBlocks, "Блок логотипа", Properties.Resources.logo, "Вставка блока логотипа ПИК."),
                    new PaletteCommand("Просмотр расширенных данных примитива", Properties.Resources.PIK_XDataView, CommandXDataView,"Просмотр расширенных данных (XData) примитива."),
                };
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error(ex, "AcadLib.AllCommandsCommon()");
                CommandsPalette = new List<IPaletteCommand>();
            }
        }

        [CommandMethod(Group, "PIK_Acadlib_About", CommandFlags.Modal)]
        public void About()
        {
            CommandStart.Start(doc =>
            {
                var ed = doc.Editor;
                var acadLibVer = Assembly.GetExecutingAssembly().GetName().Version;
                ed.WriteMessage($"\nБиблиотека AcadLib версии {acadLibVer}");
            });
        }

        public void Initialize()
        {
#if DEBUG
            // Отключение отладочных сообщений биндинга (тормозит сильно)
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Off;
#endif
            try
            {
                Logger.Log.Info("start Initialize AcadLib");
                PluginStatisticsHelper.StartAutoCAD();
                AllCommandsCommon();
                // Копирование вспомогательных сборок локально из шаровой папки packages
                var task = Task.Run(() =>
                {
                    LoadService.CopyPackagesLocal();
                });
                task.Wait(15000);
                // Автослоиtest
                Layers.AutoLayers.AutoLayersService.Init();
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                // Загрузка сборок из текущей папки
                //foreach (var item in Directory.EnumerateFiles(CurDllDir, "*.dll"))
                //{
                //    LoadService.LoadFromTry(item);
                //}
                // Загрузка сборок из папки ../Script/Net - без вложенных папок
                LoadService.LoadFromFolder(Path.Combine(PikSettings.LocalSettingsFolder, @"Script\NET"), SearchOption.TopDirectoryOnly);
                // Загрузка сборок из папки ../Script/Net/[UserGroup]
                foreach (var userGroup in PikSettings.UserGroupsCombined)
                {
                    var dirGroup = Path.Combine(PikSettings.LocalSettingsFolder, $@"Script\NET\{userGroup}");
                    LoadService.LoadFromFolder(dirGroup, SearchOption.TopDirectoryOnly);
                }
                Logger.Log.Info("end Initialize AcadLib");
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error(ex, "AcadLib Initialize.");
            }
        }

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
                dllsResolve.AddRange(DllResolve.GetDllResolve(LoadService.dllLocalPackages, SearchOption.AllDirectories));
            }
            var dllResolver = dllsResolve.FirstOrDefault(f => f.IsResolve(args.Name));
            if (dllResolver == null) return null;
            try
            {
                Logger.Log.Info($"resolve assembly - {dllResolver.DllFile}");
                return dllResolver.LoadAssembly();
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error(ex, $"Ошибка AssemblyResolve - {dllResolver.DllFile}.");
            }
            return null;
        }

        public void Terminate()
        {
            Logger.Log.Info($"Terminate AcadLib");
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
            CommandStart.Start(doc => Colors.ColorBookHelper.GenerateNCS());
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
                    var ms = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject<BlockTableRecord>();
                    foreach (var id in ms)
                    {
                        if (allTypes.ContainsKey(id.ObjectClass.Name))
                        {
                            allTypes[id.ObjectClass.Name]++;
                        }
                        else
                        {
                            allTypes.Add(id.ObjectClass.Name, 1);
                        }
                    }
                    var sortedByCount = allTypes.OrderBy(i => i.Value);
                    foreach (var item in sortedByCount)
                        ed.WriteMessage($"\n{item.Key} - {item.Value}");
                    t.Commit();
                }
            });
        }

        [LispFunction(nameof(PIK_LispLog))]
        public void PIK_LispLog([NotNull] ResultBuffer rb)
        {
            var tvs = rb.AsArray();
            if (tvs.Any())
            {
                Logger.Log.InfoLisp(tvs[0].Value.ToString());
            }
        }

        /// <summary>
        /// Визуальное окно для вставки блока из файла
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
                var file = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"flexBrics\dwg\", fileName);
                Blocks.Visual.VisualInsertBlock.InsertBlock(file,
                    n => matchs.Any(r => Regex.IsMatch(n, r.Value.ToString(), RegexOptions.IgnoreCase)),
                    layer);
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error(ex, "PIK_LispInsertBlockFromFbDwg");
            }
        }

        [CommandMethod(Group, CommandXDataView, CommandFlags.Modal)]
        public void XDataView()
        {
            CommandStart.Start(doc => XData.Viewer.XDataView.View());
        }

        [CommandMethod(Group, nameof(PIK_UpdateFieldsInObjects), CommandFlags.Modal)]
        public void PIK_UpdateFieldsInObjects()
        {
            CommandStart.Start(doc => Field.UpdateField.UpdateInSelected());
        }

        [CommandMethod(Group, nameof(PIK_PlotToPdf), CommandFlags.Session)]
        public void PIK_PlotToPdf()
        {
            CommandStart.Start(doc =>
            {
                using (doc.LockDocument())
                {
                    Plot.PlotDirToPdf.PromptAndPlot(doc);
                }
            });
        }

        [CommandMethod(Group, nameof(PIK_AutoLayersStart), CommandFlags.Modal)]
        public void PIK_AutoLayersStart()
        {
            CommandStart.Start(doc =>
            {
                Layers.AutoLayers.AutoLayersService.Start();
                doc.Editor.WriteMessage($"\n{Layers.AutoLayers.AutoLayersService.GetInfo()}");
            });
        }
        [CommandMethod(Group, nameof(PIK_AutoLayersStop), CommandFlags.Modal)]
        public void PIK_AutoLayersStop()
        {
            CommandStart.Start(doc =>
            {
                Layers.AutoLayers.AutoLayersService.Stop();
                doc.Editor.WriteMessage($"\n{Layers.AutoLayers.AutoLayersService.GetInfo()}");
            });
        }
        [CommandMethod(Group, nameof(PIK_AutoLayersStatus), CommandFlags.Modal)]
        public void PIK_AutoLayersStatus()
        {
            CommandStart.Start(doc => doc.Editor.WriteMessage($"\n{Layers.AutoLayers.AutoLayersService.GetInfo()}"));
        }

        [CommandMethod(Group, nameof(PIK_AutoLayersAll), CommandFlags.Modal)]
        public void PIK_AutoLayersAll()
        {
            CommandStart.Start(doc => Layers.AutoLayers.AutoLayersService.AutoLayersAll());
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
                    var ms = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject<BlockTableRecord>();
                    var entId = ms.Cast<ObjectId>().FirstOrDefault(f => f.OldId == id);
                    if (entId.IsNull)
                    {
                        "Элемент не найден в Моделе.".WriteToCommandLine();
                    }
                    else
                    {
                        entId.ShowEnt();
                    }
                    t.Commit();
                }
            });
        }
    }
}