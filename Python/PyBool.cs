using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMelrose
{
    public class PyBool : PyObject
    {
        readonly public bool Bool;

        public PyBool(
            Int64 BaseAddress,
            IMemoryReader MemoryReader)
            :
            base(BaseAddress, MemoryReader)
        {
            Bool = (bool)MemoryReader.ReadBool(BaseAddress + 8);
        }
    }
}
