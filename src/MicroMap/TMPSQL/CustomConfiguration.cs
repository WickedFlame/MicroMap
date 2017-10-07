using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMap.TMP
{
    public static class CustomConfiguration
    {
        static CustomConfiguration()
        {
            TreatEnumAsInteger = true;
            StripUpperInLike = true;
        }

        static bool? treatEnumAsInteger;
        public static bool TreatEnumAsInteger
        {
            get
            {
                return treatEnumAsInteger != null ? treatEnumAsInteger.Value : false;
            }
            set
            {
                treatEnumAsInteger = value;
            }
        }

        public static bool StripUpperInLike { get; set; }
    }
}
