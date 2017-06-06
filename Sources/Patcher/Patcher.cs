using System;
using System.Collections.Generic;
using DiffMatchPatch;
using System.IO;

namespace MAP
{
    public class Patcher
    {
        static public PatchResults ApplyPatches(string filePath, string editScript)
        {
            diff_match_patch patcher = new diff_match_patch();

            List<Patch> patches = patcher.patch_fromText(editScript);

            Object[] patcherResults = patcher.patch_apply(patches, File.ReadAllText(filePath));

            return patcherResults;
        }

        static public PatchResults ApplyPatches(string filePath, List<Patch> editScript)
        {
            diff_match_patch patcher = new diff_match_patch();

            Object[] patcherResults = patcher.patch_apply(editScript, File.ReadAllText(filePath));

            return patcherResults;
        }
    }
}
