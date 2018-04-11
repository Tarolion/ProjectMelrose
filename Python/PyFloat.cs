using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMelrose
{
    public class PyFloat : PyObject
    {
        readonly public double Float;

        public PyFloat(
            Int64 BaseAddress,
            IMemoryReader MemoryReader)
            :
            base(BaseAddress, MemoryReader)
        {
            Float = (double)MemoryReader.ReadDouble(BaseAddress + 8);
        }
    }
}
