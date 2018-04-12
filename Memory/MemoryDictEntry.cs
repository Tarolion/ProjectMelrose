using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMelrose.Memory
{
    public class MemoryDictEntry
    {
        public long BaseAddress;

        public string Key;
        public string Type;
        public object Value;

        public MemoryDictEntry(long a, string k, string t, object v)
        {
            BaseAddress = a;
            Key = k;
            Type = t;
            Value = v;
        }
    }
}
