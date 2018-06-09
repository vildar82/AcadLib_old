using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AcadLib.Properties;
using JetBrains.Annotations;
using NetLib.WPF;
using ReactiveUI;

namespace AcadLib.PaletteProps.UI
{
    public class PalettePropsVM : BaseModel
    {
        public PalettePropsVM()
        {
            this.WhenAnyValue(v => v.PropsHelpVisible).Subscribe(s =>
            {
                PropsHelpHeight = s ? Settings.Default.PalettePropsHelpHeight : 0d;
            });
            this.WhenAnyValue(v => v.PropsHelpHeight).Where(w => w > 0)
                .Subscribe(s => Settings.Default.PalettePropsHelpHeight = s);
        }

        public List<PalettePropsType> Types { get; set; }

        public PalettePropsType SelectedType { get; set; }

        public PalettePropVM SelectedProp { get; set;}

        public double PropsHelpHeight { get; set;}

        public bool PropsHelpVisible { get; set; } = Settings.Default.PalettePropsHelpVisible;

        public void Clear()
        {
            Types = null;
        }
    }

    public class DesignPalettePropsVM : PalettePropsVM
    {
        public DesignPalettePropsVM()
        {
            Types = GetTypes();
        }

        [NotNull]
        public static List<PalettePropsType> GetTypes()
        {
            var types = new List<PalettePropsType>();
            types.AddRange(Enumerable.Range(0,7).Select(s=> new PalettePropsType
            {
                Name = $"Type{s}",
                Groups = Enumerable.Range(0,3).Select(g=>new PalettePropsGroup
                {
                    Name = $"Group{g}",
                    Properties = Enumerable.Range(0,10).Select(p=> new PalettePropVM
                    {
                        Name = $"Prop{p}",
                        ValueControl = new IntValueView(new IntValueVM{ Value = 5, Min = 1, Max = 10})
                    }).ToList()
                }).ToList()
            }));
            return types;
        }
    }
}
