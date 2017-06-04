using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace MAP.Analysis
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
                // TODO: Implement subdiff algorithm to find differences in nested data.
            }
            else if(baseMod == modifiedMod)
            {
                return new AnalysisResults(AnalysisResults.AnalysisStatus.FilesAreEqual);
            }

            return new AnalysisResults(AnalysisResults.AnalysisStatus.Success);
        }

        /// <summary>
        /// Returns a bytewise edit script to transform the base file into the modified file specified with baseFilePath and modifiedFilePath respectively.
        /// </summary>
        public static AnalysisResults BytewiseAnalyze(string baseFilePath, string modifiedFilePath)
        {
            byte[] baseBytes = File.ReadAllBytes(baseFilePath);
            byte[] modifiedBytes = File.ReadAllBytes(modifiedFilePath);

            GenerateEditScript(baseBytes, modifiedBytes);

            return new AnalysisResults();
        }


        /// <summary>
        /// Generates a list of instructions to transform one baseBytes into targetBytes with the least modification.
        /// </summary>
        static private string[] GenerateEditScript(byte[] baseBytes, byte[] targetBytes)
        {
            /*
            format of edit script:
            A{byte},{int} - insert {byte} from target to base after index {int}
            R{int} - remove a byte from base at index {int}
            */ 
            List<string> editScript = new List<string>();

            // implementation of https://neil.fraser.name/software/diff_match_patch/myers.pdf
            int baseSize, targetSize;

            baseSize = baseBytes.Length;
            targetSize = targetBytes.Length;

            int MAX = baseSize + targetSize;

            int[] kValues = new int[MAX*2+1];

            bool solutionFound = false;

            for(int d = 0; d <= MAX; d++)
            {
                for(int k = -d; k <= d; k++)
                {
                    // down or right?
                    bool down = (k == -d || (k != d && kValues[k - 1] < kValues[k + 1]));

                    int kPrev = down ? k + 1 : k - 1;

                    // start point
                    int xStart = kValues[kPrev];
                    int yStart = xStart - kPrev;

                    // mid point
                    int xMid = down ? xStart : xStart + 1;
                    int yMid = xMid - k;

                    // end point
                    int xEnd = xMid;
                    int yEnd = yMid;

                    // follow diagonal
                    int snake = 0;
                    while (xEnd < baseSize && yEnd < targetSize && baseBytes[xEnd] == targetBytes[yEnd]) { xEnd++; yEnd++; snake++; }

                    // save end point
                    kValues[k] = xEnd;

                    // check for solution
                    if (xEnd >= baseSize && yEnd >= targetSize) /* solution has been found */
                    {
                        solutionFound = true;
                    }
                }
                if(solutionFound)
                {
                    break;
                }
            }

            foreach(int value in kValues)
            {

            }
        }
    }
}
