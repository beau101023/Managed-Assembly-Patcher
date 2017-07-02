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
        [Obsolete("Use CombineDiffs")]
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
        [Obsolete("Old patching method which does not use the new, custom encoding system.")]
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
        [Obsolete("Old patching method which does not use the new, custom encoding system.")]
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
                if(diffs[i].operation == Operation.DELETE)
                {
                    // start of the deletion
                    sb.Append(characterIndex);
                    // seperator (placeholder for when I figure out what characters are actually acceptable)
                    sb.Append('♪');
                    // end of the deletion (start + deleted text length)
                    sb.Append(characterIndex + diffs[i].text.Length);
                    // statement end
                    sb.Append('☺');
                }
                if(diffs[i].operation == Operation.INSERT)
                {
                    // starting index of the insert
                    sb.Append(characterIndex);
                    // seperator
                    sb.Append('♪');
                    // text to insert
                    sb.Append(diffs[i].text);
                    // statement end
                    sb.Append('☺');
                }

                // keep track of how far along we are, we don't need to do anything special for EQUAL diffs
                characterIndex += diffs[i].text.Length;
            }

            return sb.ToString();
        }

        /*
        Algorithm abstract:

        This algorithm will compare lists of diffs

        it will add these lists of diffs into a single list, with the last diff added having lesser priority than the first.
        
        Thus, diff lists should be passed in order of priority from greatest to least, with the greatest priority having the lowest index 

        The algorithm will then iterate over these diffs, comparing the current diff to every diff 'in front' of it. This allows us to assume that the diffs in front of it are all lower-priority diffs

        Sorting and sifting rules:

        IF both are insert diffs:
            it will offset overlapping diffs away from each other, such that they no longer overlap (given 0 being an overlapping character and - being a non-overlapping character, it will transform -0- to ----, -00- to ------, etc)
            if two diffs overlap perfectly (such that both the start and end positions of the diffs match), the lesser-priority diff will be removed (the one with the greatest index in the list).
            if two diffs overlap such that one is contained in the other, but the sizes of the diffs are not equal, the smaller one will be offset to the side of the larger diff that it is the closest to.
            if two diffs overlap such that one is contained in the other and is exactly centered inside the other, the smaller diff will be removed (note: this is not optimal, find a better way to do this)
        IF both are delete diffs:
            if two diffs overlap such that one is contained in the other or they overlap exactly, the smaller diff will be removed
            if two diffs overlap partially or touch each other, the higher-priority diff will be modified to encompass both diffs
        IF one is insert and one is delete:
            if diffs overlap, offset insert behind delete
            if delete encompasses both the insert and 1 char behind and in front of it, remove insert, likely PATCH INCOMPATABILITY
        for space inbetween diffs, assume equality
        */

        // INCOMPLETE
        /// <summary>
        /// Compares lists of diffs, lists should be passed in order of priority from greatest to least, such that the item with greatest priority is at index 0, and least priority at the end of the list
        /// </summary>
        List<Diff> CombinePatches(List<List<Diff>> diffs)
        {
            List<Diff> results = new List<Diff>();

            List<int> toBeRemoved = new List<int>();

            foreach (List<Diff> dList in diffs)
            {
                results.AddRange(dList);
            }

            for (int i = 0; i < results.Count; i++)
            {
                // compare results[i] against everything in front of it
                for (int j = i + 1; j < results.Count; j++)
                {
                    if (results[i] == results[j] && !toBeRemoved.Contains(j))
                    {
                        // mark all instances of i other than the primary one for removal
                        toBeRemoved.Add(j);
                    }
                    if (results[i].operation == results[j].operation && results[i].text == results[j].text && !toBeRemoved.Contains(j))
                    {
                        // mark all entries equal to i in content for removal
                        toBeRemoved.Add(j);
                    }
                }
            }

            foreach (int index in toBeRemoved)
            {
                results.RemoveAt(index);
            }

            return results;
        }
    }
}

/*
Future: Develop custom patcher using diffs to:
reduce file size
make patcher more rigid and less prone to error
make applying patches *much* faster

New patch making procedure:

get diffs (diff_main)

MakePatches: DONE
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

(placeholder for now)
*/