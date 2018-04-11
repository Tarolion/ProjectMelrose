using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMelrose
{
	public class PyStr : PyObject
	{
		readonly	public	string String;

		public PyStr(
			Int64 BaseAddress,
			IMemoryReader MemoryReader)
			:
			base(BaseAddress, MemoryReader)
		{
			String = MemoryReader.ReadStringAsciiNullTerminated(BaseAddress + 20);
		}
	}
}
