using System;
using System.Collections.Generic;

namespace AcadLib.CommandLock.Data
{
    public class CaseInsensitiveDictionary : Dictionary<string, CommandLockInfo>
    {
        public CaseInsensitiveDictionary() : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}