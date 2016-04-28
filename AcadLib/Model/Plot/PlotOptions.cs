using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace AcadLib.Plot
{    
    public class PlotOptions
    {
        private static string REGKEY = "PlotOptions";
        private static string KeySortTabOrName = "SortTabOrName";
        private static string KeyOnePdfOrEachDwg = "OnePdfOrEachDwg";
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

        public void Load()
        {            
            // загрузка настроек из реестра
            using (Registry.RegExt reg = new Registry.RegExt(REGKEY))
            {
                SortTabOrName = reg.Load(KeySortTabOrName, true);
                OnePdfOrEachDwg = reg.Load(KeyOnePdfOrEachDwg, true);
            }            
        }

        public void Save()
        {
            // Сохранение в реестр
            using (Registry.RegExt reg = new Registry.RegExt(REGKEY))
            {
                reg.Save(KeySortTabOrName, SortTabOrName);
                reg.Save(KeyOnePdfOrEachDwg, OnePdfOrEachDwg);
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
            return (bool)value ?
              "Вкладкам" : "Именам";
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
            return (bool)value ?
              "Общий" : "Для каждого dwg";
        }        
    }
}
