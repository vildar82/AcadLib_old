using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.PaletteCommands
{
    public class PaletteCommandBlock : PaletteCommand
    {
        /// <summary>
        /// Имя блока
        /// </summary>
        public string BlockName { get; set; }
        /// <summary>
        /// Относительный путь к файлу - от папки Settings (c:\Autodesk\AutoCAD\Pik\Settings\)
        /// Например: "Blocks\ГП\ГП_Блоки.dwg"
        /// </summary>
        public string BlockFileRef { get; set; }

        public PaletteCommandBlock(string name, Bitmap image, string command, string description, string group = "") 
            : base(name, image, command, description, group)
        {
            
        }
    }
}
