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

        AnalysisResultsType resultType;

        string[] editScript = null;

        public AnalysisResults(AnalysisStatus _status, AnalysisResultsType _resultType, string[] _editScript)
        {
            status = _status;

            resultType = _resultType;

            editScript = _editScript;
        }

        public AnalysisResults(AnalysisStatus _status, AnalysisResultsType _resultType)
        {
            status = _status;

            resultType = _resultType;
        }

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

        public enum AnalysisResultsType
        {
            RawFilePatch,
            DNLibPatch,
            PatchError,
            NoPatch
        }
    }
}
