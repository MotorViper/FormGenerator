using Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HelpersTest
{
    [TestClass]
    public class IOCTests
    {
        [TestMethod]
        public void TestRegistrationWithParameters()
        {
            IOCContainer container = IOCContainer.Instance;

            container.Register<ITestInterface, TestClass>();
            container.Register<ITestInterface, TestClass>("a");
            container.Register<ITestInterface, TestClass>("x", new object[] {3});
            container.Register<ITestInterface, TestClass>("1", new object[] {3, "3"});

            Assert.AreEqual(15, container.Resolve<ITestInterface>().ID);
            Assert.AreEqual(15, container.Resolve<ITestInterface>("a").ID);
            Assert.AreEqual(42, container.Resolve<ITestInterface>("x").ID);
            Assert.AreEqual(87, container.Resolve<ITestInterface>("1").ID);
        }
    }

    public interface ITestInterface
    {
        int ID { get; }
    }

    public class TestClass : ITestInterface
    {
        public TestClass()
        {
            ID = 15;
        }

        public TestClass(int id)
        {
            ID = 42;
        }

        public TestClass(int id, string s)
        {
            ID = 87;
        }

        public TestClass(string s)
        {
            ID = 64;
        }

        public int ID { get; private set; }
    }
}
