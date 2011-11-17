using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Vici.Mvc.Test
{
     [TestFixture]
    public class ViewContainerTest
    {
         [Test]
         public void TestPropertyOfLinkedObject()
         {
             var obj = new {Test = "XXX"};

             ViewDataContainer viewData = new ViewDataContainer(obj);

             object value;
             Type type;

             Assert.AreEqual("XXX", viewData["Test"]);
             Assert.IsTrue(viewData.TryGetValue("Test", out value, out type));

             Assert.IsInstanceOfType(typeof(string),value);
             Assert.AreEqual(typeof(string), type);
             Assert.AreEqual("XXX",value);

             Assert.IsFalse(viewData.TryGetValue("Test2", out value, out type));

         }

         [Test]
         public void TestPropertyOfMultipleLinkedObject()
         {
             var obj1 = new { Test = "XXX" };
             var obj2 = new { Value = 15.5m };

             ViewDataContainer viewData = new ViewDataContainer(obj1,obj2);

             object value;
             Type type;

             Assert.AreEqual("XXX", viewData["Test"]);
             Assert.AreEqual(15.5m, viewData["Value"]);
             Assert.IsTrue(viewData.TryGetValue("Test", out value, out type));

             Assert.IsInstanceOfType(typeof(string), value);
             Assert.AreEqual(typeof(string), type);
             Assert.AreEqual("XXX", value);

             Assert.IsTrue(viewData.TryGetValue("Value", out value, out type));

             Assert.IsInstanceOfType(typeof(decimal), value);
             Assert.AreEqual(typeof(decimal), type);
             Assert.AreEqual(15.5m, value);

             Assert.IsFalse(viewData.TryGetValue("Test2", out value, out type));
             Assert.IsFalse(viewData.TryGetValue("Value2", out value, out type));

         }

         [Test]
         public void TestPropertyOfDictionaryEntry()
         {
             ViewDataContainer viewData = new ViewDataContainer();

             viewData["Test"] = "XXX";

             object value;
             Type type;

             Assert.AreEqual("XXX", viewData["Test"]);
             Assert.IsTrue(viewData.TryGetValue("Test", out value, out type));

             Assert.IsInstanceOfType(typeof(string), value);
             Assert.AreEqual(typeof(string), type);
             Assert.AreEqual("XXX", value);

             Assert.IsFalse(viewData.TryGetValue("Test2", out value, out type));

         }

         [Test]
         public void TestApply()
         {
             var obj1 = new { Test = "XXX" };
             var obj2 = new { Value = 15.5m };

             ViewDataContainer viewData1 = new ViewDataContainer(obj1);
             ViewDataContainer viewData2 = new ViewDataContainer(obj2);

             ViewDataContainer viewData = new ViewDataContainer();

             viewData.Apply(viewData1);
             viewData.Apply(viewData2);

             object value;
             Type type;


             Assert.AreEqual("XXX", viewData["Test"]);
             Assert.AreEqual(15.5m, viewData["Value"]);
             Assert.IsTrue(viewData.TryGetValue("Test", out value, out type));

             Assert.IsInstanceOfType(typeof(string), value);
             Assert.AreEqual(typeof(string), type);
             Assert.AreEqual("XXX", value);

             Assert.IsTrue(viewData.TryGetValue("Value", out value, out type));

             Assert.IsInstanceOfType(typeof(decimal), value);
             Assert.AreEqual(typeof(decimal), type);
             Assert.AreEqual(15.5m, value);

             Assert.IsFalse(viewData.TryGetValue("Test2", out value, out type));
             Assert.IsFalse(viewData.TryGetValue("Value2", out value, out type));

         }

    }

}
