using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AcadLib.PaletteCommands;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(AcadLib.Commands))]
[assembly: ExtensionApplication(typeof(AcadLib.Commands))]

namespace AcadLib {

	public class Commands : IExtensionApplication 
		{
		public const string CommandBlockList = "PIK_BlockList";
		public const string CommandCleanZombieBlocks = "PIK_CleanZombieBlocks";
		public const string CommandColorBookNCS = "PIK_ColorBookNCS";
		public const string CommandDbJbjectsCountInfo = "PIK_DbObjectsCountInfo";
		public const string CommandInsertBlockPikLogo = "PIK_InsertBlockLogo";
		public const string CommandXDataView = "PIK_XDataView";
		public const string Group = AutoCAD_PIK_Manager.Commands.Group;
		/// <summary>
		/// Общие команды для всех отделов определенные в этой сборке
		/// </summary>
		public static List<IPaletteCommand> CommandsPalette { get; set; }

		[CommandMethod(Group, "PIK_Acadlib_About", CommandFlags.Modal)]
		public void About ()
		{
			CommandStart.Start(doc => {
				Editor ed = doc.Editor;
				var acadLibVer = Assembly.GetExecutingAssembly().GetName().Version;
				ed.WriteMessage($"\nБиблиотека AcadLib версии {acadLibVer}");
			});
		}

		[CommandMethod(Group, CommandBlockList, CommandFlags.Modal)]
		public void BlockListCommand ()
		{
			CommandStart.Start(doc => {
				BlockList.List(doc.Database);
			});
		}

		[CommandMethod(Group, CommandCleanZombieBlocks, CommandFlags.Modal)]
		public void CleanZombieBlocks ()
		{
			CommandStart.Start(doc => {
				Database db = doc.Database;
				var countZombie = db.CleanZombieBlock();
				doc.Editor.WriteMessage($"\nУдалено {countZombie} зомби!☻");
			});
		}

		[CommandMethod(Group, CommandColorBookNCS, CommandFlags.Modal | CommandFlags.Session)]
		public void ColorBookNCS ()
		{
			CommandStart.Start(doc => {
				Colors.ColorBookHelper.GenerateNCS();
			});
		}

		[CommandMethod(Group, CommandDbJbjectsCountInfo, CommandFlags.Modal)]
		public void DbObjectsCountInfo ()
		{
			CommandStart.Start(doc => {
				Database db = doc.Database;
				Editor ed = doc.Editor;
				Dictionary<string, int> allTypes = new Dictionary<string, int>();
				for (long i = db.BlockTableId.Handle.Value; i < db.Handseed.Value; i++)
				{
					ObjectId id;
					if (db.TryGetObjectId(new Handle(i), out id))
					{
						if (allTypes.ContainsKey(id.ObjectClass.Name))
							allTypes[id.ObjectClass.Name]++;
						else
							allTypes.Add(id.ObjectClass.Name, 1);
					}
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

		[CommandMethod(Group, CommandInsertBlockPikLogo, CommandFlags.Modal)]
		public void InsertBlockPikLogo ()
		{
			CommandStart.Start(doc => {
				Blocks.BlockInsert.InsertCommonBlock("PIK_Logo", doc.Database);
			});
		}

		public void Terminate ()
		{
			// Сохранение счетчика команд пользователя
			CommandCounter.Counter?.Save();
		}

		[CommandMethod(Group, CommandXDataView, CommandFlags.Modal)]
		public void XDataView ()
		{
			CommandStart.Start(doc => {
				XData.Viewer.XDataView.View();
			});
		}

		[CommandMethod(Group, nameof(PIK_UpdateFieldsInObjects), CommandFlags.Modal)]
		public void PIK_UpdateFieldsInObjects ()
		{
			CommandStart.Start(doc => {
				Field.UpdateField.UpdateInSelected();
			});
		}

        [CommandMethod(Group, nameof(PIK_PlotToPdf), CommandFlags.Session)]
        public void PIK_PlotToPdf ()
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
        internal static void AllCommandsCommon ()
		{
			CommandsPalette = new List<IPaletteCommand>()
			{
				new PaletteCommand("Блок логотипа", Properties.Resources.logo, CommandInsertBlockPikLogo,"Вставка блока логотипа ПИК."),
				new PaletteCommand("Просмотр расширенных данных примитива", Properties.Resources.PIK_XDataView, CommandXDataView,"Просмотр расширенных данных (XData) примитива."),
			};
		}

        public void Initialize ()
        {
            // MicroMvvm            
            var fileDll = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Dll\MicroMvvm.dll");
            LoadService.LoadFromTry(fileDll);
            // Загрузка общей сборки - для всех специальностей             
            fileDll = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Script\NET\PIK_Acad_Common.dll");
            LoadService.LoadFromTry(fileDll);

            // Загрузка сбороки для данного раздела
            var group = AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup;
            string fileGroup = string.Empty;
            if (group == "СС")
            {
                fileGroup = @"Script\NET\СС\PIK_SS_Acad.dll";
            }
            else if (group == "ГП")
            {
                fileGroup = @"Script\NET\ГП\PIK_GP_Acad.dll";
            }
            else if (group == "КР-СБ-ГК")
            {
                fileGroup = @"Script\NET\КР-СБ-ГК\Autocad_ConcerteList.dll";
            }
            else if (group == "КР-МН")
            {
                fileGroup = @"Script\NET\КР-МН\KR_MN_Acad.dll";
            }
            else if (group == "ВК")
            {
                fileGroup = @"Script\NET\ВК\PIK_VK_Acad.dll";
            }
            if (!string.IsNullOrEmpty(fileGroup))
            {
                fileDll = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, fileGroup);
                LoadService.LoadFromTry(fileDll);
                //Assembly.LoadFrom(fileDll);
            }

            // Загрузка базы MDM
            fileDll = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Script\NET\PIK_DB_Projects.dll");
            LoadService.LoadFromTry(fileDll);
            try
            {
                LoadService.LoadEntityFramework();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "LoadEntityFramework");
            }

            // Загрузка общей библмотеки NetLib
            fileDll = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Dll\NetLib.dll");
            LoadService.LoadFromTry(fileDll);

            // Коннекторы к базе MDM
            fileDll = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Script\NET\MDM_Connector.dll");
            LoadService.LoadFromTry(fileDll);
            fileDll = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Script\NET\MDBCToLISP.dll");
            LoadService.LoadFromTry(fileDll);
        }
    }
}