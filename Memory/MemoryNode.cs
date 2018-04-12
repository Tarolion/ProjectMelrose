using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMelrose.Memory
{
    public class MemoryNode
    {
        public long BaseAddress;

        /// <summary>
        /// All Entries stored in dictionary with key = MemoryAddress, and data stored as MemoryDictEntry
        /// </summary>
        public Dictionary<string, MemoryDictEntry> Dict;
        public MemoryNode Parent;
        public List<MemoryNode> Children;

        public MemoryNode()
        {
            Dict = new Dictionary<string, MemoryDictEntry>();
            Parent = null;
            Children = new List<MemoryNode>();
        }

        public void Reset()
        {
            Dict.Clear();
            Parent = null;
            Children.Clear();
        }

        public void AddMemoryDictEntry(MemoryDictEntry entry)
        {
            Dict.Add(entry.Key, entry);
        }
    }
}
