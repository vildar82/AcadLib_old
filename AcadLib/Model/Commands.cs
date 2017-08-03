using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AcadLib.PaletteCommands;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System.Text.RegularExpressions;
using AcadLib.Layers;
using System.Threading.Tasks;
using AcadLib.Statistic;
using Autodesk.AutoCAD.ApplicationServices;

[assembly: CommandClass(typeof(AcadLib.Commands))]
[assembly: ExtensionApplication(typeof(AcadLib.Commands))]

namespace AcadLib
{
    public class Commands : IExtensionApplication
    {
        internal static readonly string fileCommonBlocks = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Blocks\Блоки-оформления.dwg");
        public const string CommandBlockList = "PIK_BlockList";
        public const string CommandCleanZombieBlocks = "PIK_CleanZombieBlocks";
        public const string CommandColorBookNCS = "PIK_ColorBookNCS";
        public const string CommandDbJbjectsCountInfo = "PIK_DbObjectsCountInfo";
        //public const string CommandInsertBlockPikLogo = "PIK_InsertBlockLogo";
        public const string CommandXDataView = "PIK_XDataView";
        public const string Group = AutoCAD_PIK_Manager.Commands.Group;
        /// <summary>
        /// Общие команды для всех отделов определенные в этой сборке
        /// </summary>
        public static List<IPaletteCommand> CommandsPalette { get; set; }
        List<NetLib.IO.DllResolve> dllsResolve;
        public static string CurDllDir =Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

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

        [CommandMethod(Group, nameof(PIK_Start), CommandFlags.Modal)]
        public void PIK_Start()
        {
            try
            {
                PaletteSetCommands.Start();
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error(ex, "PIK_Start");
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        [CommandMethod(Group, CommandBlockList, CommandFlags.Modal)]
        public void BlockListCommand()
        {
            CommandStart.Start(doc =>
            {
                doc.Database.List();
            });
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
            CommandStart.Start(doc =>
            {
                Colors.ColorBookHelper.GenerateNCS();
            });
        }

        [CommandMethod(Group, CommandDbJbjectsCountInfo, CommandFlags.Modal)]
        public void DbObjectsCountInfo()
        {
            CommandStart.Start(doc =>
            {
                var db = doc.Database;
                var ed = doc.Editor;
                var allTypes = new Dictionary<string, int>();
                for (var i = db.BlockTableId.Handle.Value; i < db.Handseed.Value; i++)
                {
                    if (!db.TryGetObjectId(new Handle(i), out ObjectId id)) continue;
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

        [LispFunction(nameof(PIK_LispLog))]
        public void PIK_LispLog(ResultBuffer rb)
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
        public void PIK_LispInsertBlockFromFbDwg(ResultBuffer rb)
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
            catch(System.Exception ex)
            {
                Logger.Log.Error(ex,"PIK_LispInsertBlockFromFbDwg");
            }
        }

        public void Terminate()
        {            
        }

        [CommandMethod(Group, CommandXDataView, CommandFlags.Modal)]
        public void XDataView()
        {
            CommandStart.Start(doc =>
            {
                XData.Viewer.XDataView.View();
            });
        }

        [CommandMethod(Group, nameof(PIK_UpdateFieldsInObjects), CommandFlags.Modal)]
        public void PIK_UpdateFieldsInObjects()
        {
            CommandStart.Start(doc =>
            {
                Field.UpdateField.UpdateInSelected();
            });
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
            CommandStart.Start(doc =>
            {
                doc.Editor.WriteMessage($"\n{Layers.AutoLayers.AutoLayersService.GetInfo()}");                
            });
        }
        [CommandMethod(Group, nameof(PIK_AutoLayersAll), CommandFlags.Modal)]
        public void PIK_AutoLayersAll()
        {
            CommandStart.Start(doc =>
            {
                Layers.AutoLayers.AutoLayersService.AutoLayersAll();                
            });
        }

        public void Initialize()
        {
	        try
	        {
	            PluginStatisticsHelper.StartAutoCAD();

		        // Копирование вспомогательных сборок локально из шаровой папки packages
		        var task = Task.Run(() =>
		        {
			        LoadService.CopyPackagesLocal();
		        });
		        task.Wait(5000);

		        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		        // Загрузка сборок из текущей папки
		        foreach (var item in Directory.EnumerateFiles(CurDllDir, "*.dll"))
		        {
			        LoadService.LoadFromTry(item);
		        }
		        // Загрузка общей сборки - для всех специальностей             
		        var fileDll = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder,
			        @"Script\NET\PIK_Acad_Common.dll");
		        LoadService.LoadFromTry(fileDll);

		        // Загрузка сбороки для данного раздела
		        var groups = AutoCAD_PIK_Manager.Settings.PikSettings.UserGroupsCombined;
		        var fileGroups = new List<string>();
		        foreach (var group in groups)
		        {
			        var groupDll = string.Empty;
			        if (group.Equals("СС", StringComparison.OrdinalIgnoreCase))
			        {
				        groupDll = "PIK_SS_Acad.dll";
			        }
			        else if (group.Equals("ГП", StringComparison.OrdinalIgnoreCase))
			        {
				        groupDll = "PIK_GP_Acad.dll";
			        }
			        else if (group.Equals("ГП_Тест", StringComparison.OrdinalIgnoreCase))
			        {
				        groupDll = "PIK_GP_Civil.dll";
			        }
					else if (group.Equals("КР-СБ-ГК", StringComparison.OrdinalIgnoreCase))
			        {
				        groupDll = "Autocad_ConcerteList.dll";
			        }
			        else if (group.Equals("КР-МН", StringComparison.OrdinalIgnoreCase))
			        {
				        groupDll = "KR_MN_Acad.dll";
			        }
			        else if (group.Equals("НС", StringComparison.OrdinalIgnoreCase))
			        {
			            groupDll = "PIK_NS_Civil.dll";
			        }
                    if (!string.IsNullOrEmpty(groupDll))
			        {
				        fileGroups.Add($@"Script\NET\{group}\{groupDll}");
			        }
		        }
		        foreach (var fileGroup in fileGroups)
		        {
			        fileDll = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, fileGroup);
			        LoadService.LoadFromTry(fileDll);
		        }

		        // Коннекторы к базе MDM
		        fileDll = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder,
			        @"Script\NET\ASPADBConnector.dll");
		        LoadService.LoadFromTry(fileDll);
		        fileDll = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder,
			        @"Script\NET\ALDBConnector.dll");
		        LoadService.LoadFromTry(fileDll);

		        // Автослои
		        Layers.AutoLayers.AutoLayersService.Init();
	        }
	        catch (System.Exception ex)
	        {
		        Logger.Log.Error(ex, $"AcadLib Initialize.");
	        }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (dllsResolve == null)
            {
                // Сборки в основной папке dll
                dllsResolve = NetLib.IO.DllResolve.GetDllResolve(CurDllDir, SearchOption.TopDirectoryOnly);
                // Все сборки из папки Script\NET
                dllsResolve.AddRange(NetLib.IO.DllResolve.GetDllResolve(
                    Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Script\NET"), 
                    SearchOption.AllDirectories));
                // Все сборки из локальной папки packages
                dllsResolve.AddRange(NetLib.IO.DllResolve.GetDllResolve(LoadService.dllLocalPackages, SearchOption.AllDirectories));
            }
            var dllResolver = dllsResolve.FirstOrDefault(f => f.IsResolve(args.Name));
            if (dllResolver == null) return null;
            try
            {
                return dllResolver.LoadAssembly();
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error(ex, $"Ошибка AssemblyResolve - {dllResolver.DllFile}.");
            }
            return null;            
        }
    }
}