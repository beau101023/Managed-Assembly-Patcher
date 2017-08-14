using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAP
{
    // why did I make this?
    public class ObjectPatch
    {
        Object originalObject;
        Object modifiedObject;

        public ObjectPatch(object _originalObject, object _modifiedObject)
        {
            originalObject = _originalObject;
            modifiedObject = _modifiedObject;
        }

    }
}
