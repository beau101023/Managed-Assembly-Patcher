using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAP.Analysis
{
    class AnalysisResults
    {
        AnalysisStatus status;

        string[] editScript;

        public AnalysisResults(AnalysisStatus _status)
        {
            status = _status;
        }

        public override string ToString()
        {
            string strRep = "";

            switch(status)
            {
                case AnalysisStatus.Success:
                    strRep += "S";
                    break;
                case AnalysisStatus.FilesAreEqual:
                    strRep += "E";
                    break;
            }

            return strRep;
        }

        public enum AnalysisStatus
        {
            Success,
            FilesAreEqual
        }
    }
}
