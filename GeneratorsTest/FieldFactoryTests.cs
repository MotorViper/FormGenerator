using System.Collections.Generic;
using Generator;
using Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratorsTest
{
    [TestClass]
    public class FieldFactoryTests
    {
        [TestMethod]
        public void TestCreate()
        {
            IOCContainer.Instance.Register<IField, Field1>("Test1");
            IOCContainer.Instance.Register<IField, Field2>("Test2");
            IOCContainer.Instance.Register<IField, Field>();

            Field field = FieldFactory.CreateField(new SimpleElement("Test1"), 0, null) as Field;
            Assert.AreEqual("FieldB", field.Name);
            Assert.AreEqual(0, field.Level);
            Assert.AreEqual(null, field.Parent);

            field = FieldFactory.CreateField(new SimpleElement("Test2"), 1, field) as Field;
            Assert.AreEqual("FieldC", field.Name);
            Assert.AreEqual(1, field.Level);
            Assert.AreEqual("FieldB", ((Field)field.Parent).Name);

            field = FieldFactory.CreateField(new SimpleElement("Test3"), 2, field) as Field;
            Assert.AreEqual("Test3", field.Name);
            Assert.AreEqual(2, field.Level);
            Assert.AreEqual("FieldC", ((Field)field.Parent).Name);
        }

        public class Field : IField
        {
            // ReSharper disable once UnusedMember.Global - used by IOC.
            public Field()
            {
            }

            protected Field(string name)
            {
                Name = name;
            }

            public IElement Element { get; set; }
            public int Level { get; set; }
            public string Name { get; set; }
            public IField Parent { get; set; }

            public void OutputField(int level)
            {
            }

            public void AddChildProperties(IField child)
            {
            }

            public void AddTypedProperty<T>(string name, T value)
            {
            }

            public void AddElement(IElement data, int level, IField parent = null, List<string> keys = null)
            {
            }
        }

        public class Field1 : Field
        {
            public Field1() : base("FieldB")
            {
            }
        }

        public class Field2 : Field
        {
            public Field2() : base("FieldC")
            {
            }
        }
    }
}
