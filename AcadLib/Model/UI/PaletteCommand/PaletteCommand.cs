using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autodesk.AutoCAD.ApplicationServices;

namespace AcadLib.PaletteCommands
{
    public class PaletteCommand : IPaletteCommand
    {
        public string Description { get; set; }
        public ImageSource Image { get; set; }        
        public string Name { get; set; }
        public Action Command { get; set; } 


        public PaletteCommand() { }

        public PaletteCommand(string name, ImageSource image, Action command, string description)
        {
            this.Image = image;
            this.Name = name;
            this.Command = command;
            this.Description = description;            
        }       

        public void Execute()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            using (doc.LockDocument())
            {
                Command();
            }            
        }
    }
}
