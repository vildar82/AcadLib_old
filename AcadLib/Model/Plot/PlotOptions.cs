using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using AcadLib.UI.Properties;
using Autodesk.AutoCAD.ApplicationServices;

namespace AcadLib.Plot
{    
    [Serializable]
    public class PlotOptions
    {
        private static string REGKEY = "PlotOptions";
        private static string KeySortTabOrName = "SortTabOrName";
        private static string KeyOnePdfOrEachDwg = "OnePdfOrEachDwg";
        private static string KeyFilterByNumbers = "FilterByNumbers";        
        private static string KeyFilterByNames = "FilterByNames";
        private static string KeyFilterState = "FilterState";
        private static string KeyDefaultPlotSource = "DefaultPlotSource";
        private static string KeyIncludeSubdirs = "IncludeSubdirs";

        //private static PlotOptions _instance;
        //public static PlotOptions Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            _instance = new PlotOptions();                    
        //        }
        //        return _instance;
        //    }
        //}

        public PlotOptions()
        {
            Load();
        }
        
        [Category("Печать")]
        [DisplayName("Сортировка по")]
        [Description("Сортировка листов - по порядку вкладок в чертеже или по алфавиту.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(SortLayoutConverter))]
        public bool SortTabOrName { get; set; }

        [Category("Печать")]
        [DisplayName("Файл PDF")]
        [Description("Создавать pdf для каждого чертежа dwg или один для всех.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(OnePdfOrEachConverter))]
        public bool OnePdfOrEachDwg { get; set; }

        [Category("Печать")]
        [DisplayName("Поумолчанию печать из:")]
        [Description("При вызове команды установить опцию поумолчанию печати из текущего чертежа или выбор папки.")]
        [DefaultValue("Текущего")]
        [TypeConverter(typeof(PlotSourceConverter))]
        public string DefaultPlotSource { get; set; }

        [Category("Печать")]
        [DisplayName("Включая подпапки")]
        [Description("Если выбрана печать всей папки, то включать все файлы в подпапках удовлетворяющие фильтру.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool IncludeSubdirs { get; set; }

        [Category("Фильтр")]
        [DisplayName("Фильтр по номерам вкладок:")]
        [Description("Печатать только указанные номера вкладок. Номера через запятую и/или тире. Отрицательные числа считаются с конца вкладок.\n\r Например: 16--4 печать с 16 листа до 4 с конца; -1--3 печать трех последних листов.")]
        [DefaultValue("")]        
        public string FilterByNumbers { get; set; }        

        [Category("Фильтр")]
        [DisplayName("Фильтр по названию вкладок:")]
        [Description("Печатать только вкладки соответствующим заданной строке поиска. Через | можно складывать условия ИЛИ.")]
        [DefaultValue("")]
        public string FilterByNames { get; set; }

        [Category("Фильтр")]
        [DisplayName("Использовать фильтр?")]
        [Description("Включение и отключение фильтров.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]        
        public bool FilterState { get; set; }

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
                copyOptions = (PlotOptions) MemberwiseClone();
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
            //formOpt.propertyGrid1.SelectedObject = copyOptions;
            //if (Application.ShowModalDialog(formOpt) == System.Windows.Forms.DialogResult.OK)
            //{
            //    SortTabOrName = copyOptions.SortTabOrName;
            //    OnePdfOrEachDwg = copyOptions.OnePdfOrEachDwg;
            //    FilterState = copyOptions.FilterState;
            //    FilterByNames = copyOptions.FilterByNames;
            //    FilterByNumbers = copyOptions.FilterByNumbers;
            //    DefaultPlotSource = copyOptions.DefaultPlotSource;
            //    IncludeSubdirs = copyOptions.IncludeSubdirs;
            //    Save();
            //}            
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

        public override object ConvertTo(ITypeDescriptorContext context,
                CultureInfo culture,
          object value,
          Type destType)
        {
            return (bool)value ? "Вкладкам" : "Именам";
        }        
    }
        
    public class YesNoConverter : BooleanConverter
    {
        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (string)value == "Да";
        }

        public override object ConvertTo (ITypeDescriptorContext context,
                CultureInfo culture,
          object value,
          Type destType)
        {
            return (bool)value ? "Да" : "Нет";
        }
    }

    public class OnePdfOrEachConverter : BooleanConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            return (string)value == "Общий";
        }

        public override object ConvertTo(ITypeDescriptorContext context,
                CultureInfo culture,
          object value,
          Type destType)
        {
            return (bool)value ? "Общий" : "Для каждого dwg";
        }        
    }

    public class PlotSourceConverter : TypeConverter
    {
        private List<string> values = new List<string>() { "Текущего", "Папки" };
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(values);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value;
        }
    }
}
