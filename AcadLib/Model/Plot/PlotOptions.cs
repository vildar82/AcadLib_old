using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
        private static string KeyFilterByNumbersDescending = "FilterByNumbersDescending";
        private static string KeyFilterByNames = "FilterByNames";
        private static string KeyFilterState = "FilterState";
        
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

        [Category("Фильтр")]
        [DisplayName("Фильтр по номерам вкладок:")]
        [Description("Печатать только указанные номера вкладок. Номера через запятую и/или тире.")]
        [DefaultValue("")]        
        public string FilterByNumbers { get; set; }

        [Category("Фильтр")]
        [DisplayName("Фильтр по номерам вкладок начиная:")]
        [Description("Указанные номера считать с конца вкладок.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(DescendingConverter))]
        public bool FilterByNumbersDescending { get; set; }

        private List<int> _filterNumbers;
        private string _filterByNumbers;
        [XmlIgnore]
        [Browsable(false)]
        public List<int> FilterNumbers
        {
            get
            {
                if(_filterByNumbers != null && string.Equals(_filterByNumbers, FilterByNumbers) )
                {
                    return _filterNumbers;
                }
                else
                {
                    _filterNumbers = MathExt.ParseRangeNumbers(FilterByNumbers);
                    _filterByNumbers = FilterByNumbers;
                    return _filterNumbers;
                }
            }
        }

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
            using (Registry.RegExt reg = new Registry.RegExt(REGKEY))
            {
                SortTabOrName = reg.Load(KeySortTabOrName, true);
                OnePdfOrEachDwg = reg.Load(KeyOnePdfOrEachDwg, true);
                FilterByNumbers = reg.Load(KeyFilterByNumbers, "");
                FilterByNumbersDescending = reg.Load(KeyFilterByNumbersDescending, false);
                FilterByNames = reg.Load(KeyFilterByNames, "");
                FilterState = reg.Load(KeyFilterState, false);
            }            
        }

        public void Save()
        {
            // Сохранение в реестр
            using (Registry.RegExt reg = new Registry.RegExt(REGKEY))
            {
                reg.Save(KeySortTabOrName, SortTabOrName);
                reg.Save(KeyOnePdfOrEachDwg, OnePdfOrEachDwg);
                reg.Save(KeyFilterByNumbers, FilterByNumbers);
                reg.Save(KeyFilterByNumbersDescending, FilterByNumbersDescending);                
                reg.Save(KeyFilterByNames, FilterByNames);
                reg.Save(KeyFilterState, FilterState);
            }
        }

        public void Show()
        {
            UI.FormProperties formOpt = new UI.FormProperties();
            var copyOptions = (PlotOptions)this.MemberwiseClone();
            formOpt.propertyGrid1.SelectedObject = copyOptions;
            if (Application.ShowModalDialog(formOpt) == System.Windows.Forms.DialogResult.OK)
            {
                SortTabOrName = copyOptions.SortTabOrName;
                OnePdfOrEachDwg = copyOptions.OnePdfOrEachDwg;
                FilterState = copyOptions.FilterState;
                FilterByNames = copyOptions.FilterByNames;
                FilterByNumbers = copyOptions.FilterByNumbers;
                FilterByNumbersDescending = copyOptions.FilterByNumbersDescending;
                Save();
            }            
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
    public class DescendingConverter : BooleanConverter
    {
        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (string)value == "С конца";
        }

        public override object ConvertTo (ITypeDescriptorContext context,
                CultureInfo culture,
          object value,
          Type destType)
        {
            return (bool)value ? "С конца" : "С начала";
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
}
