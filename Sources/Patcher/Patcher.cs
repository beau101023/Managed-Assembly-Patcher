using System;
using System.Collections.Generic;
using DiffMatchPatch;
using System.IO;

namespace MAP
{
    public class Patcher
    {
        /// <summary>
        /// Takes lists of patches and combines them into a single list.
        /// </summary>
        static public List<Patch> CombinePatches(List<List<Patch>> patchLists)
        {
            List<Patch> tempList = new List<Patch>();

            List<int> toBeRemoved = new List<int>();

            foreach(List<Patch> plist in patchLists)
            {
                tempList.AddRange(plist);
            }

            for (int i = 0; i < tempList.Count; i++)
            {
                // compare i against everything in front of it
                for (int j = i + 1; j < tempList.Count; j++)
                {
                    if (tempList[i] == tempList[j])
                    {
                        // mark all entries equal to i for removal.
                        toBeRemoved.Add(j);
                    }
                }
            }
            
            foreach(int index in toBeRemoved)
            {
                tempList.RemoveAt(index);
            }

            List<Patch> resultList = tempList;

            return tempList;
        }

        /// <summary>
        /// Takes an edit script and applies it to the file specified by filePath.
        /// </summary>
        static public PatchResults ApplyPatches(string filePath, string editScript)
        {
            diff_match_patch patcher = new diff_match_patch();

            List<Patch> patches = patcher.patch_fromText(editScript);

            Object[] patcherResults = patcher.patch_apply(patches, File.ReadAllText(filePath));

            return new PatchResults((string)patcherResults[0], (bool[])patcherResults[1]);
        }

        /// <summary>
        /// Takes a list of patch objects and applies them to the file specified by filePath.
        /// </summary>
        static public PatchResults ApplyPatches(string filePath, List<Patch> editScript)
        {
            diff_match_patch patcher = new diff_match_patch();

            Object[] patcherResults = patcher.patch_apply(editScript, File.ReadAllText(filePath));

            return new PatchResults((string) patcherResults[0], (bool[]) patcherResults[1]);
        }
    }
}
