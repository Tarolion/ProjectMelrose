using ProjectMelrose.Discord;
using ProjectMelrose.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectMelrose
{
	class Program
	{
		static void Main(string[] args)
		{
            MemoryManager manager = new MemoryManager();
            manager.OutputToFile("C:\\Temp\\data.txt");
        }

    }
}
