using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMap.Sql
{
    public static class CustomConfiguration
    {
        static CustomConfiguration()
        {
            TreatEnumAsInteger = true;
            StripUpperInLike = true;
        }

        static bool? _treatEnumAsInteger;
        public static bool TreatEnumAsInteger
        {
            get
            {
                return _treatEnumAsInteger != null ? _treatEnumAsInteger.Value : false;
            }
            set
            {
                _treatEnumAsInteger = value;
            }
        }

        public static bool StripUpperInLike { get; set; }
    }
}
