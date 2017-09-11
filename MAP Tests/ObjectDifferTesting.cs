using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAP;
using ObjectDiffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectDiffer.TypeDiffers;

namespace MAP_Tests
{
    [TestClass]
    public class ObjectDifferTesting
    {
        [TestMethod]
        public void ClassDiffTesting()
        {
            TestClass t1 = new TestClass(123, 321, "word", "drow");
            TestClass t2 = new TestClass(456, 654, "a different string", "and another one");

            Difference d = DiffObjectPatcher.GenerateDifference(t1, t2);

            Assert.IsNotNull(d);

            TestClass t3 = t1;

            DiffObjectPatcher.ApplyDifference(ref t3, d);

            Assert.AreEqual(t1, t3);
        }
    }

    public class TestClass
    {
        public TestMemberClass t { get; set; }

        public int aNumber { get; set; }

        public string aString { get; set; }

        public TestClass(int number, int nestedNumber, string word, string nestedWord)
        {
            aNumber = number;
            aString = word;

            t = new TestMemberClass(nestedNumber, nestedWord);
        }

        public override bool Equals(object obj)
        {
            return (obj as TestClass).Equals(this);
        }

        public override int GetHashCode()
        {
            return aNumber;
        }
    }

    public class TestMemberClass
    {
        public int anotherNumber { get; set; }

        public string anotherString { get; set; }

        public TestMemberClass(int number, string word)
        {
            anotherNumber = number;

            anotherString = word;
        }

        public override bool Equals(object obj)
        {
            return (obj as TestMemberClass).Equals(this);
        }

        public override int GetHashCode()
        {
            return anotherNumber;
        }
    }
}
