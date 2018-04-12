using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMelrose.Memory
{
    public class MemoryManager
    {
        private static MemoryManager Instance = null;
        public static MemoryManager GetInstance()
        {
            if(Instance == null)
            {
                Instance = new MemoryManager();
            }

            return Instance;
        }

        public ProcessMemoryReader MemoryReader;
        public PythonMemoryReader PyMemoryReader;

        public UITreeNode RootNode;
        public Dictionary<long, MemoryNode> Nodes;

        public MemoryManager()
        {
            try
            {
                // Prep:
                RootNode = null;
                Nodes = new Dictionary<long, MemoryNode>();

                ReadRootNode();
                if (RootNode == null) { return; }

                var AllNodes =
                    new UITreeNode[] { RootNode }
                    .Concat(RootNode.EnumerateChildrenTransitive(PyMemoryReader)).ToArray();

                //	show off the spoils....
                foreach (var Node in AllNodes)
                {
                    MemoryNode memNode = new MemoryNode();
                    memNode.BaseAddress = Node.BaseAddress;

                    var Dict = Node.Dict;

                    if (null == Dict)
                    {
                        continue;
                    }

                    var DictSlots = Dict.Slots;

                    if (null == DictSlots)
                    {
                        continue;
                    }
                    
                    //	show info for each entry in dict that has a String as Key.
                    foreach (var Entry in DictSlots)
                    {
                        var EntryKeyStr = Entry.KeyStr;

                        if (null == EntryKeyStr)
                        {
                            continue;
                        }

                        var me_value = Entry.me_value;
                        PyTypeObject typeObject = new PyObject(me_value.Value, MemoryReader).LoadType(PyMemoryReader);
                        
                        MemoryDictEntry memEntry = new MemoryDictEntry(Entry.BaseAddress, EntryKeyStr, GetTypeName(me_value), ReadValue(me_value));
                        memNode.AddMemoryDictEntry(memEntry);
                    }

                    Nodes.Add(memNode.BaseAddress, memNode);
                }
            }
            finally
            {
            }
        }

        public MemoryNode GetMemoryNodeWithName(string name)
        {
            foreach(MemoryNode node in Nodes.Values)
            {
                foreach (MemoryDictEntry entry in node.Dict.Values)
                {
                    if(entry.Key.ToLower().Equals("_name"))
                    {
                        if(GetStringValueFromMemoryDictEntry(entry).ToLower().Equals(name.ToLower()))
                        {
                            return node;
                        }
                    }
                }
            }

            return null;
        }

        public string GetStringValueFromObject(PyObject obj)
        {
            string type = GetTypeName(obj);

            switch (type)
            {
                case "str":
                    return new PyStr(obj.BaseAddress, MemoryReader).String;
                case "unicode":
                    return new PyUnicode(obj.BaseAddress, MemoryReader).String;

                case "float":
                    return new PyFloat(obj.BaseAddress, MemoryReader).Float.ToString();
                case "int":
                    return new PyInt(obj.BaseAddress, MemoryReader).Int.ToString();
                case "bool":
                    return new PyBool(obj.BaseAddress, MemoryReader).Bool.ToString();

                case "list":
                    PyList list = new PyList(obj.BaseAddress, MemoryReader);
                    return "List with " + ((list.Items == null) ? 0 : list.Items.Length).ToString() + " elements";

                default:
                    return obj.BaseAddress.ToString("x");
            }
        }
        public string GetStringValueFromMemoryDictEntry(MemoryDictEntry entry)
        {
            switch (entry.Type)
            {
                case "str":
                    return ((PyStr)entry.Value).String;
                case "unicode":
                    return ((PyUnicode)entry.Value).String;

                case "float":
                    return ((PyFloat)entry.Value).Float.ToString();
                case "int":
                    return ((PyInt)entry.Value).Int.ToString();
                case "bool":
                    return ((PyBool)entry.Value).Bool.ToString();
                case "list":
                    return entry.BaseAddress.ToString("x");

                default:
                    return entry.BaseAddress.ToString("x");
            }
        }
        public string[] GetStringValuesFromPyList(PyList list)
        {
            uint[] items = list.Items;
            List<string> stringValues = new List<string>();

            if (items == null) return null;
            
            foreach (uint address in items)
            {
                PyObject obj = new PyObject(address, MemoryReader);
                stringValues.Add(GetStringValueFromObject(obj));
            }

            return stringValues.ToArray();
        }

        public void ReadRootNode()
        {
            try
            {
                var MengeCandidateProcess = System.Diagnostics.Process.GetProcessesByName("exefile");

                var EveProcess = MengeCandidateProcess.FirstOrDefault();

                if (null == EveProcess)
                {
                    return;
                }

                MemoryReader = new ProcessMemoryReader(EveProcess);
                PyMemoryReader = new PythonMemoryReader(MemoryReader);

                RootNode = EveOnline.UIRoot(PyMemoryReader);
            }
            catch(Exception e)
            {
                RootNode = null;
            }
        }
        
        public object ReadValue(uint? me_value)
        {
            PyTypeObject typeObject = new PyObject(me_value.Value, MemoryReader).LoadType(PyMemoryReader);
            object value = null;

            switch (typeObject.tp_name_Val)
            {
                case "str":
                    value = new PyStr(me_value.Value, MemoryReader);
                    break;

                case "float":
                    value = new PyFloat(me_value.Value, MemoryReader);
                    break;

                case "int":
                    value = new PyInt(me_value.Value, MemoryReader);
                    break;

                case "bool":
                    value = new PyBool(me_value.Value, MemoryReader);
                    break;

                case "unicode":
                    value = new PyUnicode(me_value.Value, MemoryReader);
                    break;

                case "list":
                    value = new PyList(me_value.Value, MemoryReader);
                    break;

                default:
                    break;

            }

            if (value != null) return value;
            
            return me_value?.ToString("x");
        }
        public object ReadValue(PyObject obj)
        {
            PyTypeObject typeObject = obj.LoadType(PyMemoryReader);
            object value = null;

            switch (typeObject.tp_name_Val)
            {
                case "str":
                    value = new PyStr(obj.BaseAddress, MemoryReader);
                    break;

                case "float":
                    value = new PyFloat(obj.BaseAddress, MemoryReader);
                    break;

                case "int":
                    value = new PyInt(obj.BaseAddress, MemoryReader);
                    break;

                case "bool":
                    value = new PyBool(obj.BaseAddress, MemoryReader);
                    break;

                case "unicode":
                    value = new PyUnicode(obj.BaseAddress, MemoryReader);
                    break;

                case "list":
                    value = new PyList(obj.BaseAddress, MemoryReader);
                    break;

                default:
                    break;

            }

            if (value != null) return value;

            return obj.BaseAddress.ToString("x");
        }
        
        public string GetTypeName(uint? me_value)
        {
            PyTypeObject typeObject = new PyObject(me_value.Value, MemoryReader).LoadType(PyMemoryReader);
            return typeObject.tp_name_Val;
        }
        public string GetTypeName(PyObject obj)
        {
            PyTypeObject typeObject = obj.LoadType(PyMemoryReader);
            return typeObject.tp_name_Val;
        }


        public void OutputToFile(string path)
        {
            List<string> data = new List<string>();

            try
            {
                ReadRootNode();

                if (RootNode == null) { return; }
                else
                {
                    data.Add("UIRoot found at " + RootNode.BaseAddress.ToString("x"));
                    data.Add("");
                }

                var AllNodes =
                    new UITreeNode[] { RootNode }
                    .Concat(RootNode.EnumerateChildrenTransitive(PyMemoryReader)).ToArray();

                //	show off the spoils....
                foreach (var Node in AllNodes)
                {
                    data.Add("==================================================================================");
                    data.Add("= UITree Node at " + Node.BaseAddress.ToString("x"));
                    data.Add("==================================================================================");
                    data.Add("");

                    var Dict = Node.Dict;

                    if (null == Dict)
                    {
                        continue;
                    }

                    var DictSlots = Dict.Slots;

                    if (null == DictSlots)
                    {
                        continue;
                    }

                    int itterator = 0;

                    //	show info for each entry in dict that has a String as Key.
                    foreach (var Entry in DictSlots)
                    {
                        var EntryKeyStr = Entry.KeyStr;

                        if (null == EntryKeyStr)
                        {
                            continue;
                        }

                        var me_value = Entry.me_value;
                        PyObject obj = new PyObject(me_value.Value, MemoryReader);
                        PyTypeObject typeObject = obj.LoadType(PyMemoryReader);

                        data.Add
                            (
                                itterator +
                                " : " +
                                Entry.BaseAddress.ToString("x") +
                                " + Entry[\"" +
                                EntryKeyStr +
                                "\"].Value(" +
                                GetTypeName(me_value) +
                                ") = " +
                                (me_value.HasValue ? GetStringValueFromObject(obj) : "null")
                            );

                        if (GetTypeName(me_value) == "list")
                        {
                            uint[] items = ((PyList)ReadValue(me_value)).Items;

                            if (items == null) continue;

                            foreach (uint address in items)
                            {
                                obj = new PyObject(address, MemoryReader);
                                String type = GetTypeName(obj);

                                data.Add("+ List Item: (" + type + ") = " + GetStringValueFromObject(obj));
                            }
                        }

                        itterator += 1;
                    }

                    data.Add("");
                }

                System.IO.File.WriteAllLines(path, data.ToArray());
            }
            finally
            {
            }
        }

    }

}
