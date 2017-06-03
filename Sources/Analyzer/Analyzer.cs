using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace MAP.Analysis
{
    class Analyzer
    {
        ModuleDefMD baseModule, modifiedModule;

        // loads modules
        void LoadModules(string baseModulePath, string modifiedModulePath)
        {
            baseModule = ModuleDefMD.Load(baseModulePath);
            modifiedModule = ModuleDefMD.Load(modifiedModulePath);
        }

        void TestingMethod()
        {
            ModuleDefMD mod = ModuleDefMD.Load(typeof(Analyzer).Module);

            System.Diagnostics.Debug.WriteLine(mod.Assembly.ToString());
        }

        AnalysisResults Analyze()
        {
            List<TypeDef> modifiedTypes = new List<TypeDef>();

            foreach (TypeDef baseType in baseModule.GetTypes())
            {
                foreach(TypeDef modifiedType in modifiedModule.GetTypes())
                {
                    if(baseType != modifiedType)
                    {
                        modifiedTypes.Add(modifiedType);
                    }
                }
            }

            return new AnalysisResults();
        }
    }
}
