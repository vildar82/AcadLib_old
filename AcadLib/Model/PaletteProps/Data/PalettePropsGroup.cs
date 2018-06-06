using System;
using System.Collections.Generic;
using System.Windows.Media;
using Autodesk.AutoCAD.DatabaseServices;
using NetLib.WPF;
using ReactiveUI;

namespace AcadLib.PaletteProps
{
    public class PalettePropsGroup : BaseModel
    {
        public PalettePropsGroup()
        {
            this.WhenAnyValue(v => v.IsExpanded).Subscribe(s=> { ButtonExpandContent = s ? "-" : "+"; });
            ButtonExpandCommand = CreateCommand(() => IsExpanded = !IsExpanded);
        }

        /// <summary>
        /// Название группы
        /// </summary>
        public string Name { get; set; }

        public List<ObjectId> Ids { get; set; }

        public bool IsExpanded { get; set; } = true;
        public string ButtonExpandContent { get; set; }
        public ReactiveCommand ButtonExpandCommand { get; set; }

        public List<PalettePropVM> Properties { get; set; }

        public Brush Background { get; set; }
    }
}