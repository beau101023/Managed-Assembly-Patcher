using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectDiffer;
using System.IO;
using System.Reflection;

namespace MAP
{
    public static class DiffObjectPatcher
    {
        public static Difference GenerateDifference(object original, object modified)
        {
            DifferFactory factory = new DifferFactory();
            var differ = factory.GetDefault();

            return differ.Diff(original, modified);
        }

        public static Difference LoadDifference(string filePath)
        {
            return Serialization_Functions.TypeFromByteArray<Difference>(File.ReadAllBytes(filePath));
        }

        public static byte[] DifferenceToByteArray(Difference d)
        {
            return Serialization_Functions.ObjectToByteArray(d);
        }

        public static T ApplyDifference<T>(T baseObj, Difference d)
        {
            if(d == null || baseObj == null)
            {
                return default(T);
            }

            if(d.ChildDiffs == null || !d.ChildDiffs.Any())
            {
                // apply diff
                PropertyInfo info = baseObj.GetPropertyInfo(d.PropertyName);

                info.SetPropValue(baseObj, d.NewValue);
            }
            else
            {
                ApplyDifference(baseObj, d);
            }

            return baseObj;
        }

        // why did I make this again?
        private static List<ObjectPatch> FindLowLevelDiffs(Difference diff)
        {
            List<ObjectPatch> patches = new List<ObjectPatch>();

            if (diff.ChildDiffs != null)
            {
                foreach (Difference d in diff.ChildDiffs)
                {
                    patches.AddRange(FindLowLevelDiffs(d));
                }
            }
            else if(diff.OldValue != null && diff.NewValue != null)
            {
                patches.Add(new ObjectPatch(diff.OldValue, diff.NewValue));
            }
            
            return patches;
        }
    }
}
