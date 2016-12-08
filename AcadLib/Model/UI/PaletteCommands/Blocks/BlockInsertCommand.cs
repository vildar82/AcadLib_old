using AcadLib.Blocks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.PaletteCommands
{
    /// <summary>
    /// Кнопка для вставки одного блока
    /// </summary>
    public class PaletteInsertBlock : PaletteCommand
    {
        string blName;
        string file;
        List<Property> props;

        public PaletteInsertBlock(string blName, string file, string name, Bitmap image,
            string description, string group = "", List<Property> props = null)
            : base(name, image, "", description, group)
        {
            this.blName = blName;
            this.file = file;
            this.props = props;
            CreateContexMenu();
        }

        public override void Execute()
        {
            CopyBlock( DuplicateRecordCloning.Ignore);
            BlockInsert.Insert(blName, null, props);
        }

        private void CreateContexMenu()
        {
            ContexMenuItems = new List<MenuItemCommand>();
            var menu = new MenuItemCommand("Переопределить", new RelayCommand(Redifine, CanRedifine));
            ContexMenuItems.Add(menu);
        }

        /// <summary>
        /// Переопределение блока
        /// </summary>
        private void Redifine()
        {
            CopyBlock(DuplicateRecordCloning.Replace);
        }

        private bool CanRedifine()
        {
            var resCan = false;
            // Проверить, есть ли в текущем чертеже такой блок
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return resCan;
            var db = doc.Database;
            using (var bt = db.BlockTableId.Open(OpenMode.ForRead) as BlockTable)
            {
                resCan = bt.Has(blName);
            }
            return resCan;
        }

        private void CopyBlock(DuplicateRecordCloning mode)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var db = doc.Database;
            using (doc.LockDocument())
            {
                // Выбор и вставка блока                 
                Block.CopyBlockFromExternalDrawing(new List<string> { blName }, file, db, mode);
            }
        }
    }
}
