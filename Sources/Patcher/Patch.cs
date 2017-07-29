using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffMatchPatch;

namespace MAP
{
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
        /// Offsets the patch's index to targetIndex.
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
    }
}
