using AcadLib.UI.Properties;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace AcadLib.Plot
{
    public class OnePdfOrEachConverter : BooleanConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            return (string)value == "Общий";
        }

        [NotNull]
        public override object ConvertTo(ITypeDescriptorContext context,
                CultureInfo culture,
          [NotNull] object value,
          Type destType)
        {
            return (bool)value ? "Общий" : "Для каждого dwg";
        }
    }

    [PublicAPI]
    [Serializable]
    public class PlotOptions
    {
        private const string KeyDefaultPlotSource = "DefaultPlotSource";
        private const string KeyFilterByNames = "FilterByNames";
        private const string KeyFilterByNumbers = "FilterByNumbers";
        private const string KeyFilterState = "FilterState";
        private const string KeyIncludeSubdirs = "IncludeSubdirs";
        private const string KeyOnePdfOrEachDwg = "OnePdfOrEachDwg";
        private const string KeySortTabOrName = "SortTabOrName";
        private const string REGKEY = "PlotOptions";
        [Category("Печать")]
        [DisplayName("Печать по умолчанию:")]
        [Description("При вызове команды установить опцию поумолчанию печати из текущего чертежа или выбор папки.")]
        [DefaultValue("Текущего")]
        [TypeConverter(typeof(PlotSourceConverter))]
        public string DefaultPlotSource { get; set; }

        [Category("Фильтр")]
        [DisplayName("Фильтр по названию вкладок:")]
        [Description("Печатать только вкладки соответствующим заданной строке поиска. Через | можно складывать условия ИЛИ.")]
        [DefaultValue("")]
        public string FilterByNames { get; set; }


        [Category("Фильтр")]
        [DisplayName("Фильтр по номерам вкладок:")]
        [Description("Печатать только указанные номера вкладок. Номера через запятую и/или тире. Отрицательные числа считаются с конца вкладок.\n\r Например: 16--4 печать с 16 листа до 4 с конца; -1--3 печать трех последних листов.")]
        [DefaultValue("")]
        public string FilterByNumbers { get; set; }

        [Category("Фильтр")]
        [DisplayName("Использовать фильтр?")]
        [Description("Включение и отключение фильтров.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool FilterState { get; set; }

        [Category("Печать")]
        [DisplayName("Единый файл PDF")]
        [Description("Создавать один общий файл pdf или для каждого чертежа dwg отдельно.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(OnePdfOrEachConverter))]
        public bool OnePdfOrEachDwg { get; set; }

        [Category("Печать")]
        [DisplayName("С подпапками")]
        [Description("Если выбрана печать всей папки, то включать все файлы в подпапках удовлетворяющие фильтру.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool IncludeSubdirs { get; set; }

        [Category("Печать")]
        [DisplayName("Сортировка по")]
        [Description("Сортировка листов - по порядку вкладок в чертеже или по алфавиту.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(SortLayoutConverter))]
        public bool SortTabOrName { get; set; }

        public PlotOptions()
        {
            Load();
        }

        public void Load()
        {
            // загрузка настроек из реестра
            using (var reg = new Registry.RegExt(REGKEY))
            {
                SortTabOrName = reg.Load(KeySortTabOrName, true);
                OnePdfOrEachDwg = reg.Load(KeyOnePdfOrEachDwg, true);
                FilterByNumbers = reg.Load(KeyFilterByNumbers, "");
                FilterByNames = reg.Load(KeyFilterByNames, "");
                FilterState = reg.Load(KeyFilterState, false);
                DefaultPlotSource = reg.Load(KeyDefaultPlotSource, "Текущего");
                IncludeSubdirs = reg.Load(KeyIncludeSubdirs, false);
            }
        }

        public void Save()
        {
            // Сохранение в реестр
            using (var reg = new Registry.RegExt(REGKEY))
            {
                reg.Save(KeySortTabOrName, SortTabOrName);
                reg.Save(KeyOnePdfOrEachDwg, OnePdfOrEachDwg);
                reg.Save(KeyFilterByNumbers, FilterByNumbers);
                reg.Save(KeyFilterByNames, FilterByNames);
                reg.Save(KeyFilterState, FilterState);
                reg.Save(KeyDefaultPlotSource, DefaultPlotSource);
                reg.Save(KeyIncludeSubdirs, IncludeSubdirs);
            }
        }

        public void Show()
        {
            //var formOpt = new UI.FormProperties();
            var copyOptions = (PlotOptions)MemberwiseClone();
            if (PropertiesService.Show(copyOptions, v =>
            {
                copyOptions = (PlotOptions)MemberwiseClone();
                return copyOptions;
            }) == true)
            {
                SortTabOrName = copyOptions.SortTabOrName;
                OnePdfOrEachDwg = copyOptions.OnePdfOrEachDwg;
                FilterState = copyOptions.FilterState;
                FilterByNames = copyOptions.FilterByNames;
                FilterByNumbers = copyOptions.FilterByNumbers;
                DefaultPlotSource = copyOptions.DefaultPlotSource;
                IncludeSubdirs = copyOptions.IncludeSubdirs;
                Save();
            }
        }
    }

    public class PlotSourceConverter : TypeConverter
    {
        private readonly List<string> values = new List<string> { "Текущего", "Папки" };

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value;
        }

        [NotNull]
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(values);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    public class SortLayoutConverter : BooleanConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            return (string)value == "Вкладкам";
        }

        [NotNull]
        public override object ConvertTo(ITypeDescriptorContext context,
                CultureInfo culture,
          [NotNull] object value,
          Type destType)
        {
            return (bool)value ? "Вкладкам" : "Именам";
        }
    }

    public class YesNoConverter : BooleanConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (string)value == "Да";
        }

        [NotNull]
        public override object ConvertTo(ITypeDescriptorContext context,
                CultureInfo culture,
          [NotNull] object value,
          Type destType)
        {
            return (bool)value ? "Да" : "Нет";
        }
    }
}