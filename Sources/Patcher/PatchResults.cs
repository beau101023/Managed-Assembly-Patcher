using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAP
{
    public class PatchResults
    {
        public PatchStatus status;

        public string patchedText = null;

        public float patchSuccessPercentage = -1;

        public PatchResults(PatchStatus _status)
        {
            status = PatchStatus.Failure;
        }

        public PatchResults(string _patchedText, bool[] patchResults)
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
