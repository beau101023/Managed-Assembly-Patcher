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

            if (baseMod != modifiedMod)
            {
                //for (int currenttype = 0; currenttype <= basemod.types.count; currenttype++)
                //{
                //    if (basemod.types[currenttype] != modifiedmod.types[currenttype])
                //    {

                //    }
                //}
                System.Diagnostics.Debug.WriteLine("Modules not equal, running subdiff");
                // TODO: Implement algorithm to find differences in nested data.
            }
            else if(baseMod == modifiedMod)
            {
                return new AnalysisResults(AnalysisResults.AnalysisStatus.FilesAreEqual);
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

            /*
            StringBuilder baseSB = new StringBuilder(baseString.Length * 2);
            StringBuilder modSB = new StringBuilder(modifiedString.Length * 2);

            foreach (char c in baseString)
            {
                baseSB.Append(c);
                baseSB.Append(' ');
            }
            baseSB.Remove(baseSB.Length - 1, 1);

            foreach (char c in modifiedString)
            {
                modSB.Append(c);
                modSB.Append(' ');
            }
            modSB.Remove(modSB.Length - 1, 1);

            string editScript = GenerateEditScript(baseSB.ToString(), modSB.ToString());
            */

            string editScript = GenerateEditScript(baseString, modifiedString);

            return new AnalysisResults(AnalysisResults.AnalysisStatus.Success, AnalysisResults.AnalysisResultsType.RawFilePatch, editScript);
        }

        /// <summary>
        /// Generates a list of instructions to transform one string into another with the least modification.
        /// </summary>
        private static string GenerateEditScript(string baseString, string targetString)
        {
            diff_match_patch patcher = new diff_match_patch();

            patcher.Diff_Timeout = 0;

            List<Patch> patches = patcher.patch_make(baseString, targetString);

            string results = patcher.patch_toText(patches);

            return results;
        }
    }
}
