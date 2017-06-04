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

        public AnalysisResults(AnalysisStatus _status)
        {
            status = _status;
        }

        public enum AnalysisStatus
        {
            Success,
            FilesAreEqual
        }
    }
}
