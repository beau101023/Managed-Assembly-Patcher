using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MAP;
using System.IO;
using DiffMatchPatch;
using System.Collections.Generic;

namespace MAP_Tests
{
    [TestClass]
    public class RawPatcherTests
    {
        /// <summary>
        /// Checks to see if the patcher successfully encodes and decodes patches with no unintended modification or formatting errors.
        /// </summary>
        [TestMethod]
        public void PatchEncodeAndDecode()
        {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            string testFilePath1 = projectDirectory + @"\TestFiles\Assembly-CSharp_RECOMPILED.dll";

            string testFilePath2 = projectDirectory + @"\TestFiles\Assembly-CSharp_REDS SPIT DADDIES.dll";

            string diffmodPath = projectDirectory + @"\TestFiles\REDS SPIT DADDIES.diffmod";

            string fileContent1 = File.ReadAllText(testFilePath1);
            string fileContent2 = File.ReadAllText(testFilePath2);

            diff_match_patch engine = new diff_match_patch();

            engine.Diff_Timeout = 0;

            List<Diff> diffs = engine.diff_main(fileContent1, fileContent2, false);

            List<Patch> patches = Patcher.PatchMake(diffs);

            byte[] patchBytes = Patcher.EncodePatchesToByteArray(patches);

            File.WriteAllBytes(diffmodPath, patchBytes);

            patchBytes = File.ReadAllBytes(diffmodPath);

            List<Patch> decodedPatches = Patcher.ParseByteArrayToPatches(patchBytes);

            CollectionAssert.AreEqual(patches, decodedPatches);
        }

        [TestMethod]
        public void PatchPipeline()
        {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            string baseFilePath = projectDirectory + @"\TestFiles\Assembly-CSharp_RECOMPILED.dll";
            string modifiedFilePath = projectDirectory + @"\TestFiles\Assembly-CSharp_REDS SPIT DADDIES.dll";
            string diffmodPath = projectDirectory + @"\TestFiles\REDS SPIT DADDIES.diffmod";

            string baseFileContent = File.ReadAllText(baseFilePath);
            string modFileContent = File.ReadAllText(modifiedFilePath);

            diff_match_patch engine = new diff_match_patch();
            engine.Diff_Timeout = 0;
            List<Diff> diffs = engine.diff_main(baseFileContent, modFileContent, false);

            List<Patch> patches = Patcher.PatchMake(diffs);
            byte[] fileData = Patcher.EncodePatchesToByteArray(patches);
            File.WriteAllBytes(diffmodPath, fileData);
            fileData = File.ReadAllBytes(diffmodPath);
            List<Patch> decodedPatches = Patcher.ParseByteArrayToPatches(fileData);

            string finalOutput = Patcher.ApplyPatches(baseFilePath, decodedPatches).patchedText;

            Assert.AreEqual(modFileContent, finalOutput);
        }
    }
}
