using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectDiffer;
using System.IO;
using System.Reflection;
using ObjectDiffer.TypeDiffers;
using dnlib.DotNet;
using System.Collections;

namespace MAP
{
    public static class DiffObjectPatcher
    {
        public static Difference GenerateDifference<T>(T original, T modified)
        {
            var differ = GetDiffer(true);

            return differ.Diff(original, modified);
        }

        static Differ GetDiffer(bool diffByIndex)
        {
            return new Differ(
            new List<ITypeDiffer>
            {
                new PrimativeDiffer(),
                diffByIndex ? (ITypeDiffer)new IndexEnumerableDiffer() : new ObjectEqualityEnumerableDiffer(new DefaultEqualityComparer()),
                new ObjectTypeDiffer()
            });
        }

        public static Difference LoadDifference(string filePath)
        {
            return Serialization_Functions.TypeFromByteArray<Difference>(File.ReadAllBytes(filePath));
        }

        public static byte[] DifferenceToByteArray(Difference d)
        {
            return Serialization_Functions.ObjectToByteArray(d);
        }

        public static void ApplyDifference<T>(ref T baseObj, Difference d, string propertyPath = null)
        {
            // note: add logic to find out if difference references array and use SetNestedArrayElement if so

            if (d.ChildDiffs != null && d.ChildDiffs.Any())
            {
                foreach (Difference diff in d.ChildDiffs)
                {
                    string temp;

                    if (propertyPath == null)
                    {
                        temp = diff.PropertyName;
                    }
                    else
                    {
                        temp = propertyPath + "." + diff.PropertyName;
                    }

                    ApplyDifference(ref baseObj, diff, temp);
                }
            }

            SetNestedProperty(propertyPath, baseObj, d.NewValue);
        }

        /// <summary>
        /// Utility for setting the properties of objects within objects
        /// </summary>
        /// <param name="path">A string that specifies the path to the property. Argument should be formatted like "Object.NestedObject.Property"</param>
        /// <param name="target">The Object to modify.</param>
        /// <param name="value">The value to set.</param>
        public static void SetNestedProperty(string path, object target, object value)
        {
            string[] bits = path.Split('.');
            for (int i = 0; i < bits.Length - 1; i++)
            {
                PropertyInfo propertyToGet = target.GetType().GetProperty(bits[i]);
                target = propertyToGet.GetValue(target, null);
            }
            PropertyInfo propertyToSet = target.GetType().GetProperty(bits.Last());
            propertyToSet.SetValue(target, value, null);
        }

        /// <summary>
        /// Utility for setting the elements of arrays within nested objects
        /// </summary>
        /// <param name="path">A string that specifies the path to the array property. Argument should be formatted like "Object.NestedObject.Property"</param>
        /// <param name="target">The object which hosts the array.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="index">The index of the object to set.</param>
        public static void SetNestedArrayElement(string path, object target, object value, int index)
        {
            PropertyInfo propertyToGet = null;
            string[] bits = path.Split('.');
            for (int i = 0; i < bits.Length - 1; i++)
            {
                propertyToGet = target.GetType().GetProperty(bits[i]);
                target = propertyToGet.GetValue(target, null);
            }
            // get the array as an IList and change it
            IList targetArray = (IList) propertyToGet.GetValue(target, null);
            targetArray[index] = value;
        }
    }
}
