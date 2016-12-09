using AcadLib.Blocks;
using Autodesk.AutoCAD.ApplicationServices;
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
    /// Команда вставки блока из списка
    /// </summary>
    public class PaletteVisualInsertBlocks : PaletteCommand
    {        
        string file;
        Predicate<string> filter;

        public PaletteVisualInsertBlocks(Predicate<string> filter, string file, string name, Bitmap image,
            string description, string group = "")
            : base(name, image, "", description, group)
        {            
            this.file = file;
            this.filter = filter;                                    
        }

        public override void Execute()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            using (doc.LockDocument())
            {
                Blocks.Visual.VisualInsertBlock.InsertBlock(file, filter);
            }
        }               
    }
}
