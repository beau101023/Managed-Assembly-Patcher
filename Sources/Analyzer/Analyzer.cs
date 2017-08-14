using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using DiffMatchPatch;
using ObjectDiffer;
using System.Runtime.Serialization.Formatters.Binary;

namespace MAP
{
    public class Analyzer
    {

        /// <summary>
        /// Analyzes IL code decompiled by DNLib
        /// </summary>
        public static AnalysisResults ILAnalyze(string baseFilePath, string modifiedFilePath)
        {
            // load assemblies with dnSpy
            ModuleDefMD baseMod = ModuleDefMD.Load(baseFilePath);
            ModuleDefMD modifiedMod = ModuleDefMD.Load(modifiedFilePath);

            // check straight away for equality, we don't need to waste time with this.
            if (baseMod == modifiedMod)
            {
                return new AnalysisResults(AnalysisResults.AnalysisStatus.FilesAreEqual);
            }

            // new diffing library is nice and simple
            DifferFactory factory = new DifferFactory();
            var d = factory.GetDefault();

            Difference diffObj = d.Diff(baseMod, modifiedMod);

            byte[] editScript;

            // serialize diffObj to a byte array for writing to disk
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, diffObj);
                editScript = ms.ToArray();
            }

            return new AnalysisResults(AnalysisResults.AnalysisStatus.Success, AnalysisResults.AnalysisResultsType.ILPatch, editScript);
        }

        /// <summary>
        /// Returns a bytewise edit script to transform the base file into the modified file specified with baseFilePath and modifiedFilePath respectively.
        /// </summary>
        public static AnalysisResults RawAnalyze(string baseFilePath, string modifiedFilePath)
        {
            if (baseFilePath == modifiedFilePath)
            {
                return new AnalysisResults(AnalysisResults.AnalysisStatus.FilesSamePath);
            }

            string baseString = File.ReadAllText(baseFilePath);
            string modifiedString = File.ReadAllText(modifiedFilePath);

            if (baseString == modifiedString)
            {
                return new AnalysisResults(AnalysisResults.AnalysisStatus.FilesAreEqual);
            }

            byte[] editScript = GenerateEditScript(baseString, modifiedString);

            return new AnalysisResults(AnalysisResults.AnalysisStatus.Success, AnalysisResults.AnalysisResultsType.RawFilePatch, editScript);
        }

        /// <summary>
        /// Generates a list of instructions to transform one string into another with the least modification.
        /// </summary>
        private static byte[] GenerateEditScript(string basePath, string modPath)
        {
            diff_match_patch engine = new diff_match_patch();

            engine.Diff_Timeout = 0;

            List<Diff> diffs = engine.diff_main(basePath, modPath, false);

            //List<Patch> patches = engine.patch_make(baseString, diffs);

            //string results = engine.patch_toText(patches);

            List<Patch> patches = Patcher.PatchMake(diffs);

            return Patcher.EncodePatchesToByteArray(patches);
        }
    }
}
