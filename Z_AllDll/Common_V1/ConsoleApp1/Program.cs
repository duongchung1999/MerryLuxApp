using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string str = "055B3C00000A38000CFE000000000000080708075802580208070807000000000000000000000000080708070807080758025802B004B0040807080708070807\r\n";
            Console.WriteLine(str.Length / 2);


            Console.WriteLine(str.Split(' ').Length);


            MerryDllFramework.MerryDll dll = new MerryDllFramework.MerryDll();
            dll.Interface(new Dictionary<string, object>());
            Stopwatch sp = new Stopwatch();
            sp.Start();
            Console.WriteLine(dll.Run(new object[] { "dllname=Common_V1&method=MessageBox&Message=请插入充电线，放入RX、Iphone、iWatch并检查是否有充电符号/Khi dùng sạc pin, dùng cho RX、iphone、iwatch hãy kiểm tra xem có biểu tượng sạc không" }));
            Console.WriteLine(sp.ElapsedMilliseconds);
            Console.ReadKey();
        }
    }
}
