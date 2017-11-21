using System;

namespace MicroMap.Diagnostics
{
    public interface ILogWriter
    {
        void Write(string message, string source = null, string category = null, DateTime? logtime = null);
    }
}
