namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;
    using NetLib.WPF;
    using ReactiveUI;

    public class PalettePropsGroup : BaseModel
    {
        public PalettePropsGroup()
        {
            this.WhenAnyValue(v => v.IsExpanded).Subscribe(s =>
            {
                ButtonExpandContent = s ? "-" : "+";
                ButtonExpandTooltip = s ? "Свернуть" : "Развернуть";
            });
            ButtonExpandCommand = CreateCommand(() => IsExpanded = !IsExpanded);
        }

        /// <summary>
        /// Название группы
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Кол-во объектов
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Связанный объект
        /// </summary>
        public object Tag { get; set; }

        public bool IsExpanded { get; set; } = true;

        public string ButtonExpandContent { get; set; }

        public string ButtonExpandTooltip { get; set; }

        public ReactiveCommand ButtonExpandCommand { get; set; }

        public List<PalettePropVM> Properties { get; set; }

        public Brush Background { get; set; }
    }
}