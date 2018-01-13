using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using MicroMvvm;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using static Autodesk.AutoCAD.ApplicationServices.Core.Application;

// ReSharper disable once CheckNamespace
namespace AcadLib.PaletteCommands
{
    /// <summary>
    /// Кнопка для вставки одного блока
    /// </summary>
    public class PaletteInsertBlock : PaletteCommand
    {
        private readonly string blName;
        private readonly string file;
        private readonly List<Property> props;

        public PaletteInsertBlock(string blName, string file, string name, Bitmap image,
            string description, string group = "", [CanBeNull] List<Property> props = null, bool isTest = false)
            : base(name, image, "", description, group, isTest)
        {
            this.blName = blName;
            this.file = file;
            this.props = props;
            CreateContexMenu();
        }

        public override void Execute()
        {
            CopyBlock(DuplicateRecordCloning.Ignore);
            BlockInsert.Insert(blName, null, props);
        }

        private bool CanRedefine()
        {
            return Block.HasBlockThisDrawing(blName);
        }

        private void CopyBlock(DuplicateRecordCloning mode)
        {
            var doc = DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var db = doc.Database;
            using (doc.LockDocument())
            {
                // Выбор и вставка блока
                if (mode == DuplicateRecordCloning.Replace)
                {
                    Block.Redefine(blName, file, db);
                }
                else
                {
                    Block.CopyBlockFromExternalDrawing(new List<string> { blName }, file, db, mode);
                }
            }
        }

        private void CreateContexMenu()
        {
            ContexMenuItems = new List<MenuItemCommand>();
            var menu = new MenuItemCommand("Переопределить", new RelayCommand(Redefine, CanRedefine));
            ContexMenuItems.Add(menu);
        }

        /// <summary>
        /// Переопределение блока
        /// </summary>
        private void Redefine()
        {
            if (!CanRedefine())
            {
                MessageBox.Show($"В текущем чертеже нет блока {blName}.");
                return;
            }
            CopyBlock(DuplicateRecordCloning.Replace);
        }
    }
}