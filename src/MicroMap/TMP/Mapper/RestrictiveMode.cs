﻿using System;

namespace MicroMap.Mapper
{
    [Flags]
    public enum RestrictiveMode
    {
        None = 0,

        Ignore = 1,

        Log = 2,

        ThrowException = 4
    }
}
