using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AcadLib.PaletteCommands;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;

[assembly: CommandClass(typeof(AcadLib.Commands))]
[assembly: ExtensionApplication(typeof(AcadLib.Commands))]

namespace AcadLib
{
    public class Commands : IExtensionApplication
    {
        /// <summary>
        /// Общие команды для всех отделов определенные в этой сборке
        /// </summary>
        public static List<IPaletteCommand> CommandsPalette { get; set; } 

        public const string Group = AutoCAD_PIK_Manager.Commands.Group;
        public const string CommandAbout = "PIK_Acadlib_About";
        public const string CommandDbJbjectsCountInfo = "PIK_DbObjectsCountInfo";
        public const string CommandBlockList = "PIK_BlockList";
        public const string CommandCleanZombieBlocks = "PIK_CleanZombieBlocks";
        public const string CommandColorBookNCS = "PIK_ColorBookNCS";
        public const string CommandInsertBlockPikLogo = "PIK_InsertBlockLogo";        

        public void InitCommands()
        {
            CommandsPalette = new List<IPaletteCommand>();
        }

        [CommandMethod(Group, CommandInsertBlockPikLogo, CommandFlags.Modal)]
        //[PaletteCommand("Блок логотипа", "Вставка блока логотипа ПИК")]
        public void InsertBlockPikLogo()
        {
            CommandStart.Start(doc =>
            {
                Blocks.BlockInsert.InsertCommonBlock("PIK_Logo", doc.Database);
            });
        }

        [CommandMethod(Group, CommandAbout, CommandFlags.Modal)]
        public void About()
        {
            CommandStart.Start(doc =>
            {
                Editor ed = doc.Editor;
                var acadLibVer = Assembly.GetExecutingAssembly().GetName().Version;
                ed.WriteMessage($"\nБиблиотека AcadLib версии {acadLibVer}");
            });
        }        

        [CommandMethod(Group, CommandDbJbjectsCountInfo, CommandFlags.Modal)]
        public void DbObjectsCountInfo()
        {
            CommandStart.Start(doc =>
            {
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

        [CommandMethod(Group, CommandBlockList, CommandFlags.Modal)]
        public void BlockListCommand()
        {
            CommandStart.Start(doc =>
            {
                BlockList.List(doc.Database);
            });       
        }

        [CommandMethod(Group, CommandCleanZombieBlocks, CommandFlags.Modal)]
        public void CleanZombieBlocks()
        {
            CommandStart.Start(doc =>
            {
                Database db = doc.Database;
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

        public void Initialize()
        {
            // Загрузка сбороки для данного раздела
            var group = AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup;
            // пока только для ГП
            if (group == "ГП") 
            {
                var fileGroup = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder,
                                "Script\\NET\\ГП\\", "PIK_" + "GP" + "_Acad.dll");
                // Загрузка сбороки ГП                                                        
                var assGroup = Assembly.LoadFrom(fileGroup);
                // Список общих команд
                InitCommands();
            }
            //else if (group == "КР-СБ-ГК")
            //{
            //    var fileGroup = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder,
            //                    "Script\\NET\\КР-СБ-ГК\\Autocad_ConcerteList.dll");
            //    // Загрузка сбороки                                                       
            //    var assGroup = Assembly.LoadFrom(fileGroup);
            //}
        }

        public void Terminate()
        {
            // Сохранение счетчика команд пользователя            
            CommandCounter.Counter?.Save();            
        }
    }
}
