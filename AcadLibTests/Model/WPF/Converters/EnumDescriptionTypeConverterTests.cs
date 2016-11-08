using Microsoft.VisualStudio.TestTools.UnitTesting;
using AcadLib.WPF.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.WPF.Converters.Tests
{
    [TestClass()]
    [System.ComponentModel.Description("Имя")]
    public class EnumDescriptionTypeConverterTests
    {
        [System.ComponentModel.Description("Имя")]
        public string Name { get; set; }

        [TestMethod()]
        public void GetEnumDescriptionTest ()
        {
            MyEnum e = MyEnum.One;
            var val = AcadLib.WPF.Converters.EnumDescriptionTypeConverter.GetEnumDescription(e);
            Assert.AreEqual("Один", val);
        }

        [TestMethod()]
        public void TestEnumDescrValue ()
        {
            MyEnum e = MyEnum.One;
            var res = e.Description();
            Assert.AreEqual("Один", res);
        }        

        public enum MyEnum
        {
            [System.ComponentModel.Description("Один")]
            One
        }
    }    
}