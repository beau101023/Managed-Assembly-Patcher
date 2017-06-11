﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAP
{
    class AnalysisResults
    {
        public AnalysisStatus status;

        public AnalysisResultsType resultType;

        public string editScript = null;

        public AnalysisResults(AnalysisStatus _status, AnalysisResultsType _resultType, string _editScript)
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
            DNLibPatch
        }
    }
}
