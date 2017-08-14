using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectDiffer;

namespace MAP
{
    public class AnalysisResults
    {
        public AnalysisStatus status;

        public AnalysisResultsType resultType;

        public byte[] editScript = null;

        public AnalysisResults(AnalysisStatus _status, AnalysisResultsType _resultType, byte[] _editScript)
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

        public enum AnalysisStatus
        {
            Success,
            FilesAreEqual,
            FilesSamePath,
            PatchError
        }

        public enum AnalysisResultsType
        {
            RawFilePatch,
            ILPatch
        }
    }
}
