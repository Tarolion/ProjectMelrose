using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMelrose
{
    public class PyInt : PyObject
    {
        readonly public int Int;

        public PyInt(
            Int64 BaseAddress,
            IMemoryReader MemoryReader)
            :
            base(BaseAddress, MemoryReader)
        {
            Int = (int)MemoryReader.ReadInt(BaseAddress + 8);
        }
    }
}
