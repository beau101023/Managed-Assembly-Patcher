using System;
using System.Collections.Generic;
using DiffMatchPatch;
using System.IO;
using System.Text;

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
                    if (tempList[i] == tempList[j] && !toBeRemoved.Contains(j))
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

            patcher.Patch_DeleteThreshold = 0f;

            patcher.Match_Threshold = 0f;

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

            patcher.Patch_DeleteThreshold = 0f;

            patcher.Match_Threshold = 0f;

            patcher.Match_Distance = 0;

            Object[] patcherResults = patcher.patch_apply(editScript, File.ReadAllText(filePath));

            return new PatchResults((string) patcherResults[0], (bool[]) patcherResults[1]);
        }

        static public List<Patch> GetPatchesFromString(string patchString)
        {
            diff_match_patch patcher = new diff_match_patch();

            return patcher.patch_fromText(patchString);
        }

        static public string PatchMake(List<Diff> diffs)
        {
            StringBuilder sb = new StringBuilder();

            // what character are we on?
            int characterIndex = 0;

            for(int i = 0; i < diffs.Count; i++)
            {

            }

            /*
            Future: Develop custom patcher using diffs to:
            reduce file size
            make patcher more rigid and less prone to error
            make applying patches *much* faster

            New patch making procedure:

            get diffs (diff_main)
            MakePatches:
            iterate through diffs, format them into a string which only stores DELETE and INSERT operations (with starting indices for insert and start + end indices for delete)
            write string to file

            CombinePatches:

            combination modes:
            1. if two diffs start on the same character or overlap, offset the lesser priority one behind the higher priority one
            2. if two diffs start on the same character or overlap, remove the lesser priority one

            ApplyPatches:
            read string and interpret into diffs and starting character indices (and accept raw diffs and indices as params)
            read file target file contents
            
            for INSERT operations insert the diff's text after the starting character index
            for DELETE operations delete the text starting at the character index and ending at the length of the text to be deleted

            write file contents to disk

            Patch encoding format:

            i/r (insert/delete)
            +


            */


        }
    }
}
