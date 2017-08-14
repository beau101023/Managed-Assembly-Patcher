using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectDiffer;
using dnlib.DotNet;
using System.IO;

namespace MAP_Tests
{
    [TestClass]
    public class ObjectPatcherTests
    {
        [TestMethod]
        public void ObjectPatchPipeline()
        {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            ModuleDefMD original = ModuleDefMD.Load(projectDirectory + @"\TestFiles\Assembly-CSharp_RECOMPILED.dll");
            ModuleDefMD modified = ModuleDefMD.Load(projectDirectory + @"\TestFiles\Assembly-CSharp_REDS SPIT DADDIES.dll");

            Difference diff = DiffObjectPatcher.GenerateDifference(original.Assembly, modified.Assembly);

            AssemblyDef diffed = DiffObjectPatcher.ApplyDifference(original.Assembly, diff);

            // check for null difference
            if (diff == null)
            {
                Assert.Fail();
            }

            Assert.AreEqual(original, diffed);
        }
    }
}
