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
    public class EnumDescriptionTypeConverterTests
    {
        [TestMethod()]
        public void GetEnumDescriptionTest ()
        {
            MyEnum e = MyEnum.One;
            var val = AcadLib.WPF.Converters.EnumDescriptionTypeConverter.GetEnumDescription(e);
            Assert.AreEqual("Один", val);
        }

        public enum MyEnum
        {
            [System.ComponentModel.Description("Один")]
            One
        }
    }    
}