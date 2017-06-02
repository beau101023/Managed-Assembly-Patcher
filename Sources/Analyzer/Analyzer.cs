using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace MAP.Analyzer
{
    class Analyzer
    {
        ModuleDefMD module1, module2;

        // loads modules
        void LoadModules(string path1, string path2)
        {
            module1 = ModuleDefMD.Load(path1);
            module2 = ModuleDefMD.Load(path2);
        }

        AnalysisResults Analyze()
        {
            module1.Assembly.ToString();

            return new AnalysisResults();
        }
    }
}
