using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAP
{
    public class PatchResults
    {
        /// <summary>
        /// Status of the patch job.
        /// </summary>
        public PatchStatus status;

        /// <summary>
        /// Post-patch text. May not be patched correctly. Check PatchStatus. Is null if an error occurs.
        /// </summary>
        public string patchedText = null;

        /// <summary>
        /// Percentage of the patches that succeeded. Is -1 if no percentage has been calculated.
        /// </summary>
        public float patchSuccessPercentage = -1;

        /// <summary>
        /// An array representing whether individual patches have failed or succeeded. True for success, false for failure.
        /// </summary>
        public bool[] patchStates;
        
        /// <summary>
        /// Only call this constructor if there is a problem.
        /// </summary>
        public PatchResults()
        {
            status = PatchStatus.Error;
        }

        /// <summary>
        /// Class for storing patch data and status.
        /// </summary>
        public PatchResults(string _patchedText, bool[] patches)
        {
            patchedText = _patchedText;

            int successes = 0;

            foreach (bool patchSuccessful in patches)
            {
                if (patchSuccessful == true)
                {
                    successes++;
                }
            }

            patchSuccessPercentage = successes / patches.Count();

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

        public PatchResults(string _patchedText, PatchStatus _status)
        {
            patchedText = _patchedText;

            status = _status;
        }

        /// <summary>
        /// Status of the Patch job.
        /// </summary>
        public enum PatchStatus
        {

            /// <summary>
            /// 100% Successful patch.
            /// </summary>
            Success,
            /// <summary>
            /// Partial patch, not 100% but not 0% successful. Running a partially patched file will probably crash things/cause weird behaviour.
            /// </summary>
            PartialPatch,
            /// <summary>
            /// 0% Patch success. :(
            /// </summary>
            Failure,
            /// <summary>
            /// Something went horribly wrong, maybe an invalid parameter or another strange thing.
            /// </summary>
            Error
        }
    }
}
