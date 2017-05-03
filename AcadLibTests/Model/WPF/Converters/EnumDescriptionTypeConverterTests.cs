using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var e = MyEnum.One;
            var val = EnumDescriptionTypeConverter.GetEnumDescription(e);
            Assert.AreEqual("Один", val);
        }

        [TestMethod()]
        public void TestEnumDescrValue ()
        {
            var e = MyEnum.One;
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