using System;
using System.Collections.Generic;

namespace Rockabilly.Common
{
    public class EnumUtil
    {
        //http://stackoverflow.com/questions/972307/can-you-loop-through-all-enum-values – Şafak Gür
        public static IReadOnlyList<T> GetValues<T>() { return (T[])Enum.GetValues(typeof(T)); }
    }
}
