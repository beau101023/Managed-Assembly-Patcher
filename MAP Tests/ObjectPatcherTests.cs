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
using System.Collections;

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

            List<Difference> TypeDifferences = new List<Difference>();

            List<TypeDef> originalTypes = new List<TypeDef>(original.GetTypes());
            List<TypeDef> modifiedTypes = new List<TypeDef>(modified.GetTypes());

            // handle this later!
            if(originalTypes.Count != modifiedTypes.Count)
            {
                Assert.Inconclusive();
            }

            Difference diff = DiffObjectPatcher.GenerateDifference(original, modified);

            Assert.IsNotNull(diff);

            ModuleDefMD diffedModule = original;

            List<TypeDef> diffedTypes = new List<TypeDef>();

            for (int i = 0; i < TypeDifferences.Count; i++)
            {
                TypeDef tempType = originalTypes[i];

                DiffObjectPatcher.ApplyDifference<TypeDef>(ref tempType, TypeDifferences[i]);

                diffedTypes[i] = tempType;
            }

            // find out how to set types in ModuleDefMD
            diffedModule.Types.Clear();

            foreach(TypeDef t in diffedTypes)
            {
                diffedModule.Types.Add(t);
            }

            Assert.AreEqual(original, diffedModule);
        }
    }
}