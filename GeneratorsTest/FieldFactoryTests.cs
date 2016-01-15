using System;
using System.Collections.Generic;
using Generator;
using Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;

namespace GeneratorsTest
{
    [TestClass]
    public class FieldFactoryTests
    {
        //public class Field : IField
        //{
        //    public Field()
        //    {
        //    }

        //    protected Field(string name)
        //    {
        //        Name = name;
        //    }

        //    /// <summary>
        //    /// The fields children.
        //    /// </summary>
        //    public TokenTreeList Children { get; }

        //    /// <summary>
        //    /// The fields data, i.e. children and properties.
        //    /// </summary>
        //    public TokenTree Data { get; set; }

        //    /// <summary>
        //    /// Level of indentation.
        //    /// </summary>
        //    public int Level { get; set; }

        //    /// <summary>
        //    /// Field name.
        //    /// </summary>
        //    public string Name { get; set; }

        //    /// <summary>
        //    /// Fields parent.
        //    /// </summary>
        //    public IField Parent { get; set; }

        //    /// <summary>
        //    /// Outputs the field to the writer.
        //    /// </summary>
        //    /// <param name="level">The indentation level.</param>
        //    /// <param name="parameters">Parameters used for any calculations.</param>
        //    public void OutputField(int level, TokenTree parameters)
        //    {
        //    }

        //    /// <summary>
        //    /// Adds any child properties that are linked to the field.
        //    /// </summary>
        //    /// <param name="child">The child whose properties are being added.</param>
        //    public void AddChildProperties(IField child)
        //    {
        //    }

        //    /// <summary>
        //    /// Adds a property to the output.
        //    /// </summary>
        //    /// <typeparam name="T">The property type.</typeparam>
        //    /// <param name="name">The property name.</param>
        //    /// <param name="value">The property value.</param>
        //    public void AddProperty<T>(string name, T value)
        //    {
        //    }

        //    /// <summary>
        //    /// Add an element to the output.
        //    /// </summary>
        //    /// <param name="data">The data making up the element.</param>
        //    /// <param name="level">The indentation level.</param>
        //    /// <param name="parameters">Calculation parameters.</param>
        //    /// <param name="parent">The elements parent.</param>
        //    /// <param name="selected">The selected output element.</param>
        //    /// <param name="keys">List of available elements.</param>
        //    public void AddElement(TokenTree data, int level, TokenTree parameters, IField parent = null, TokenTree selected = null, List<string> keys = null)
        //    {
        //    }
        //}

        //public class Field1 : Field
        //{
        //    public Field1() : base("FieldB")
        //    { }
        //}

        //public class Field2 : Field
        //{
        //    public Field2() : base("FieldC")
        //    { }
        //}

        //public class Element : IElement
        //{
        //    public IEnumerable<IElement> Children { get; }
        //    public string ElementType { get; set; }
        //    public IPropertyList Properties { get; }
        //    public IList<IValue> GetDataList(string name)
        //    {
        //        return null;
        //    }

        //    public Element(string elementType)
        //    {
        //        ElementType = elementType;
        //    }
        //}

        //[TestMethod]
        //public void TestCreate()
        //{
        //    IOCContainer.Instance.Register<IField, Field1>("Test1");
        //    IOCContainer.Instance.Register<IField, Field2>("Test2");
        //    IOCContainer.Instance.Register<IField, Field>();

        //    Field field = FieldFactory.CreateField(new Element("Test1"), 0, null) as Field;
        //    Assert.AreEqual("FieldB", field.Name);
        //    Assert.AreEqual(0, field.Level);
        //    Assert.AreEqual(null, field.Parent);

        //    field = FieldFactory.CreateField(new Element("Test2"), 1, field) as Field;
        //    Assert.AreEqual("FieldC", field.Name);
        //    Assert.AreEqual(1, field.Level);
        //    Assert.AreEqual("FieldB", ((Field)field.Parent).Name);

        //    field = FieldFactory.CreateField(new Element("Test3"), 2, field) as Field;
        //    Assert.AreEqual("Test3", field.Name);
        //    Assert.AreEqual(2, field.Level);
        //    Assert.AreEqual("FieldC", ((Field)field.Parent).Name);
        //}
    }
}
