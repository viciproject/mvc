using System;
using System.Linq;
using NUnit.Framework;

namespace Vici.Mvc.Test
{
    [TestFixture]
    public class MinifyTest
    {
        [Test]
        public void Test()
        {
            string js = JavaScriptMinifier.Minify(@"if (Test.Ajax( 5 ) == 1) {
  alert( 'x' );
}
");

            Assert.AreEqual("\nif(Test.Ajax(5)==1){alert('x');}\n",js);
            


        }
        
    }
}