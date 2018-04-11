using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectMelrose
{
	class Program
	{
		static void Main(string[] args)
		{
            Console.SetWindowSize(100, 60);

            List<string> data = new List<string>();

            try
            {
                Console.WriteLine("Demo Program reading Eve Online UI Tree");
                data.Add("Demo Program reading Eve Online UI Tree");
                Console.WriteLine();
                data.Add("");

                var MengeCandidateProcess = System.Diagnostics.Process.GetProcessesByName("exefile");

                var EveProcess = MengeCandidateProcess.FirstOrDefault();

                if (null == EveProcess)
                {
                    Console.WriteLine("EveProcess not found");
                    return;
                }

                using (var MemoryReader = new ProcessMemoryReader(EveProcess))
                {
                    var PyMemoryReader = new PythonMemoryReader(MemoryReader);

                    var UIRoot = EveOnline.UIRoot(PyMemoryReader);

                    if (null == UIRoot)
                    {
                        Console.WriteLine("UIRoot not found");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("UIRoot found at {0}", UIRoot.BaseAddress.ToString("x"));
                        data.Add("UIRoot found at " + UIRoot.BaseAddress.ToString("x"));
                    }

                    var AllNodes =
                        new UITreeNode[] { UIRoot }
                        .Concat(UIRoot.EnumerateChildrenTransitive(PyMemoryReader)).ToArray();

                    //	show off the spoils....
                    foreach(var Node in AllNodes)
                    {
                        Console.WriteLine("UIRoot found at {0}", UIRoot.BaseAddress.ToString("x"));
                        data.Add("UIRoot found at " + UIRoot.BaseAddress.ToString("x"));



                        Console.WriteLine();
                        data.Add("");
                        Console.WriteLine("UITree Node at {0}:", Node.BaseAddress.ToString("x"));
                        data.Add("UITree Node at " + Node.BaseAddress.ToString("x"));

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
                            PyTypeObject typeObject = new PyObject(me_value.Value, MemoryReader).LoadType(PyMemoryReader);

                            Console.WriteLine("{0} : {1} + Entry[\"{2}\"].Value({3}) = {4}", 
                                itterator,
                                Entry.BaseAddress.ToString("x"),
                                EntryKeyStr, 
                                GetTypeName(me_value, MemoryReader, PyMemoryReader),// (typeObject == null ? typeObject.tp_name_Val : "null"), 
                                (me_value.HasValue ?  ReadValue(me_value, MemoryReader, PyMemoryReader) : "null"));

                            data.Add
                                (
                                    itterator + 
                                    " : " + 
                                    Entry.BaseAddress.ToString("x") + 
                                    " + Entry[\"" + 
                                    EntryKeyStr +
                                    "\"].Value(" +
                                    GetTypeName(me_value, MemoryReader, PyMemoryReader) + 
                                    ") = " +
                                    (me_value.HasValue ? ReadValue(me_value, MemoryReader, PyMemoryReader) : "null")
                                );

                            itterator += 1;
                        }
                    }
                }

                System.IO.File.WriteAllLines("C:\\Temp\\data.txt", data.ToArray());
            }
            finally
            {
                Console.ReadKey();
            }
        }



        public static string ReadValue(uint? me_value, IMemoryReader MemoryReader, IPythonMemoryReader PyMemoryReader)
        {
            PyTypeObject typeObject = new PyObject(me_value.Value, MemoryReader).LoadType(PyMemoryReader);
            PyObject value = null;

            switch(typeObject.tp_name_Val)
            {
                case "str":
                    value = new PyStr(me_value.Value, MemoryReader);
                    return ((PyStr)value).String;

                case "float":
                    value = new PyFloat(me_value.Value, MemoryReader);
                    return ((PyFloat)value).Float.ToString();

                case "int":
                    value = new PyInt(me_value.Value, MemoryReader);
                    return ((PyInt)value).Int.ToString();

                case "bool":
                    value = new PyBool(me_value.Value, MemoryReader);
                    return ((PyBool)value).Bool.ToString();

                case "unicode":
                    value = new PyUnicode(me_value.Value, MemoryReader);
                    return ((PyUnicode)value).String;

                default:
                    break;

            }



            return me_value?.ToString("x");
        }
        public static string GetTypeName(uint? me_value, IMemoryReader MemoryReader, IPythonMemoryReader PyMemoryReader)
        {
            PyTypeObject typeObject = new PyObject(me_value.Value, MemoryReader).LoadType(PyMemoryReader);
            return typeObject.tp_name_Val;
        }
    }
}
