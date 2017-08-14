using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffMatchPatch;

namespace MAP
{
    [Serializable]
    public class Patch
    {
        public Operation operation;

        public int startingIndex;

        public int endingIndex;

        public int length;

        public string content = null;

        public Patch(int _startingIndex, int _length)
        {
            operation = Operation.DELETE;
            startingIndex = _startingIndex;
            length = _length;
            endingIndex = startingIndex + length;
        }

        public Patch(int _startingIndex, string _content)
        {
            operation = Operation.INSERT;
            startingIndex = _startingIndex;
            content = _content;
            length = content.Length;
            endingIndex = startingIndex + length;
        }


        /// <summary>
        /// Moves the patch's index to targetIndex.
        /// </summary>
        public void Move(int targetIndex)
        {
            startingIndex = targetIndex;
            endingIndex = targetIndex + length;
        }

        /// <summary>
        /// Offsets the patch by offset. Negative values acceptable.
        /// </summary>
        public void Offset(int offset)
        {
            this.Move(startingIndex + offset);
        }

        
        /// <summary>
        /// Returns a string representation of the relevant parts of the patch.
        /// </summary>
        /// <returns>String representation of a patch.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(operation.ToString() + ',' + startingIndex);

            if(operation == Operation.DELETE)
            {
                sb.Append('-' + endingIndex);
            }

            if(operation == Operation.INSERT)
            {
                sb.Append(',' + content);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Equality comparison override.
        /// </summary>
        public override bool Equals(object obj)
        {
            if(obj.GetType() != typeof(Patch) )
            {
                return false;
            }

            // just compare everything ¯\_(ツ)_/¯
            return
                operation == (obj as Patch).operation &&
                startingIndex == (obj as Patch).startingIndex &&
                endingIndex == (obj as Patch).endingIndex &&
                length == (obj as Patch).length &&
                content == (obj as Patch).content;
        }
    }
}
