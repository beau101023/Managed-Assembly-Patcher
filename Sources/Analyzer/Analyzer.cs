using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Managed_Assembly_Patcher
{
    class Analyzer
    {
        ModuleDefMD module1, module2;

        // loads module
        void LoadModules(string path1, string path2)
        {
            module1 = ModuleDefMD.Load(path1);
            module2 = ModuleDefMD.Load(path2);
        }

        AnalysisResults Analyze()
        {
            return new AnalysisResults();
        }
    }
}
