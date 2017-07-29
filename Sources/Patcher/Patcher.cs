using System;
using System.Collections.Generic;
using DiffMatchPatch;
using System.IO;
using System.Text;

namespace MAP
{
    public class Patcher
    {
        static public List<Patch> PatchMake(List<Diff> diffs)
        {
            List<Patch> patches = new List<Patch>();

            // what character are we on?
            int characterIndex = 0;

            for(int i = 0; i < diffs.Count; i++)
            {
                switch (diffs[i].operation)
                {
                    // don't add to the character index for DELETE
                    case Operation.DELETE:
                        Patch deleteP = new Patch(characterIndex, diffs[i].text.Length);
                        patches.Add(deleteP);
                        break;
                    // add insert length to character index
                    case Operation.INSERT:
                        Patch insertP = new Patch(characterIndex, diffs[i].text);
                        patches.Add(insertP);
                        characterIndex += diffs[i].text.Length;
                        break;
                    // add length, don't create patch for EQUAL diffs
                    case Operation.EQUAL:
                        characterIndex += diffs[i].text.Length;
                        break;
                }
            }

            return patches;
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
            if two diffs overlap perfectly (such that both the start and end positions of the diffs match), the lesser-priority diff will be offset forwards as above.
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
        // NOTE: Develop method for storing character indices (start and end positions of diffs) VERY IMPORTANT
        static public List<Patch> CombinePatches(List<List<Patch>> patchLists)
        {
            List<Patch> patches = new List<Patch>();

            foreach(List<Patch> pList in patchLists)
            {
                patches.AddRange(pList);
            }

            List<int> toBeRemoved = new List<int>();

            for (int i = 0; i < patches.Count; i++)
            {
                // compare results[i] against everything in front of it
                for (int j = i + 1; j < patches.Count; j++)
                {
                    if(patches[i].operation == Operation.INSERT && patches[j].operation == Operation.INSERT)
                    {
                        int endingIndexI = patches[i].startingIndex + patches[i].length;
                        int endingIndexJ = patches[j].startingIndex + patches[j].length;

                        // check for patch overlap where patches[i] is on the end of patches[j]
                        if (patches[i].startingIndex < endingIndexJ && patches[i].startingIndex > patches[j].startingIndex)
                        {

                        }
                    }
                    if (patches[i].operation == Operation.DELETE && patches[j].operation == Operation.DELETE)
                    {

                    }
                    if (patches[i].operation != patches[j].operation)
                    {

                    }
                }
            }

            foreach (int index in toBeRemoved)
            {
                patches.RemoveAt(index);
            }

            return patches;
        }

        /// <summary>
        /// Parses a string to a list of patches.
        /// </summary>
        static public List<Patch> ParseStringToPatches(string patchString)
        {
            List<Patch> patches = new List<Patch>();

            List<string> patchTemp = new List<string>(patchString.Split((char)9246));

            for (int i = 0; i < patchTemp.Count; i += 2)
            {
                if(patchTemp[i].Substring(0, 1) == "d")
                {
                    patches.Add
                    (
                        new Patch(int.Parse(patchTemp[i].Substring(1)), int.Parse(patchTemp[i + 1]))
                    );
                }
                if(patchTemp[i].Substring(0, 1) == "i")
                {
                    patches.Add
                    (
                        new Patch(int.Parse(patchTemp[i].Substring(1)), patchTemp[i + 1])
                    );
                }
            }

            return patches;
        }


        /// <summary>
        /// Encodes a list of patches to a string for export.
        /// </summary>
        static public string EncodePatchesToString(List<Patch> patches)
        {
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < patches.Count; i++)
            {

                if (patches[i].operation == Operation.DELETE)
                {
                    sb.Append('d');
                    sb.Append(patches[i].startingIndex.ToString());
                    sb.Append((char)9246);
                    sb.Append(patches[i].length.ToString());
                    sb.Append((char)9246);
                }
                else if (patches[i].operation == Operation.INSERT)
                {
                    sb.Append('i');
                    sb.Append(patches[i].startingIndex.ToString());
                    sb.Append((char)9246);
                    sb.Append(patches[i].content);
                    sb.Append((char)9246);
                }
            }

            // removes the extra seperator character at the end. Probably not the best solution.
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        static public PatchResults ApplyPatches(string filePath, List<Patch> patches)
        {
            StringBuilder tempContent = new StringBuilder(File.ReadAllText(filePath));

            patches.Sort
            (
                (x, y) => x.startingIndex.CompareTo(y.startingIndex)
            );

            for(int currentPatch = 0; currentPatch < patches.Count; currentPatch++)
            {
                if (patches[currentPatch].operation == Operation.DELETE)
                {
                    // p.endingIndex - p.startingIndex gives us the length of characters removed
                    tempContent.Remove(patches[currentPatch].startingIndex, patches[currentPatch].length);
                }
                if (patches[currentPatch].operation == Operation.INSERT)
                {
                    // if we've gone over the length limit, append the patch content to the end. This really shouldn't happen more than once, patcher might be broken if it does.
                    if (patches[currentPatch].startingIndex > tempContent.Length)
                    {
                        tempContent.Append(patches[currentPatch].content);
                    }
                    else
                    {
                        tempContent.Insert(patches[currentPatch].startingIndex, patches[currentPatch].content);
                    }
                }
            }

            return new PatchResults(tempContent.ToString(), PatchResults.PatchStatus.Success);
        }
    }
}