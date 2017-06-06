using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAP
{
    class PatchResults
    {
        public PatchStatus status;

        public string patchedText;

        public float patchSuccessPercentage;

        PatchResults(PatchStatus _status, string _patchedText, bool[] patchSuccessful)
        {
            status = _status;
            patchedText = _patchedText;

            int successes = 0;

            foreach (bool b in patchSuccessful)
            {
                if (b == true)
                {
                    successes++;
                }
            }

            patchSuccessPercentage = successes / patchSuccessful.Count();

            if(patchSuccessPercentage == 1.00)
            {
                status = PatchStatus.Success;
            }
            else if(patchSuccessPercentage > 0.00 && patchSuccessPercentage < 1.00)
            {
                status = PatchStatus.PartialPatch;
            }
            else if(patchSuccessPercentage == 0.00)
            {
                status = PatchStatus.Failure;
            }
        }

        PatchResults(string _patchedText, bool[] patchResults)
        {
            patchedText = _patchedText;

            int successes = 0;

            foreach (bool patchSuccessful in patchResults)
            {
                if (patchSuccessful == true)
                {
                    successes++;
                }
            }

            patchSuccessPercentage = successes / patchResults.Count();

            if (patchSuccessPercentage == 1.00)
            {
                status = PatchStatus.Success;
            }
            else if (patchSuccessPercentage > 0.00 && patchSuccessPercentage < 1.00)
            {
                status = PatchStatus.PartialPatch;
            }
            else if (patchSuccessPercentage == 0.00)
            {
                status = PatchStatus.Failure;
            }
        }

        public enum PatchStatus
        {
            Success,
            PartialPatch,
            Failure,
            Error
        }
    }
}
