using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using DiffMatchPatch;

namespace MAP
{
    class Analyzer
    {
        public static void Test()
        {
            ModuleDefMD mod = ModuleDefMD.Load(typeof(Analyzer).Module);

            System.Diagnostics.Debug.WriteLine(mod.Assembly);

            
        }

        public static AnalysisResults DNLibAnalyze(string baseFilePath, string modifiedFilePath)
        {
            ModuleDefMD baseMod = ModuleDefMD.Load(baseFilePath);
            ModuleDefMD modifiedMod = ModuleDefMD.Load(modifiedFilePath);

            if (baseMod == modifiedMod)
            {
                return new AnalysisResults(AnalysisResults.AnalysisStatus.FilesAreEqual);
            }
            // if a type is not equal, check the data in it to see what isn't equal, continue doing this for all nested data
            for (int currentType = 0; currentType <= baseMod.Types.Count; currentType++)
            {
                if (baseMod.Types[currentType] != modifiedMod.Types[currentType])
                {
                    for(int currentMethod = 0; currentMethod <= baseMod.Types[currentType].Methods.Count; currentMethod++)
                    {
                        
                    }
                }
            }

            AnalysisResults results = new AnalysisResults(AnalysisResults.AnalysisStatus.Success, AnalysisResults.AnalysisResultsType.DNLibPatch);

            System.Diagnostics.Debug.WriteLine(results);

            return results;
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

            string editScript = GenerateEditScript(baseString, modifiedString);

            return new AnalysisResults(AnalysisResults.AnalysisStatus.Success, AnalysisResults.AnalysisResultsType.RawFilePatch, editScript);
        }

        /// <summary>
        /// Generates a list of instructions to transform one string into another with the least modification.
        /// </summary>
        private static string GenerateEditScript(string basePath, string modPath)
        {
            diff_match_patch engine = new diff_match_patch();

            engine.Diff_Timeout = 0;

            List<Diff> diffs = engine.diff_main(basePath, modPath, false);

            //List<Patch> patches = engine.patch_make(baseString, diffs);

            //string results = engine.patch_toText(patches);

            List<Patch> patches = Patcher.PatchMake(diffs);

            return Patcher.EncodePatchesToString(patches);
        }
    }
}
