using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMelrose
{
	public class PyUnicode : PyObject
	{
		readonly public	string String;
        readonly public uint? StringPtr;
        readonly public int Length;

		public PyUnicode(
			Int64 BaseAddress,
			IMemoryReader MemoryReader)
			:
			base(BaseAddress, MemoryReader)
		{
            Length = (int)MemoryReader.ReadInt(BaseAddress + 8);
            StringPtr = MemoryReader.ReadUInt32(BaseAddress + 12);
            String = MemoryReader.ReadUnicodeString((Int64)StringPtr, Length);

		}
	}
}
