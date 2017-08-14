using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MAP
{
    public static class Reflection_Functions
    {
        public static T GetDefault<T>()
        {
            return default(T);
        }

        public static T CastToType<T>(object input)
        {
            return (T)input;
        }

        public static Object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }

        public static Object GetPropValue(this Object obj, String name)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public static void SetPropValue(this PropertyInfo info, Object obj, Object value)
        {
            info.SetValue(obj, value);
        }

        public static PropertyInfo GetPropertyInfo(this Object obj, String name)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                return type.GetProperty(part);
            }

            return null;
        }

        public static T GetPropValue<T>(this Object obj, String name)
        {
            Object retval = GetPropValue(obj, name);
            if (retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }
    }
}
