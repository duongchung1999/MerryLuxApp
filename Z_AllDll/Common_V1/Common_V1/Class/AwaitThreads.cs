using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common_V1
{
    internal class AwaitThreads
    {

        private object obj_lock = new object();
        Dictionary<int, int> Awaits = new Dictionary<int, int>();
        private bool first = true;
        public Dictionary<string, object> Config;
        /// <summary isPublicTestItem="true">
        /// 等待所有线程后开始测试
        /// </summary>
        /// <returns>True</returns>
        public string AwaitThread(Dictionary<string, object> OnceConfig)
        {
            //多加锁，不然寄存器容易出bug
            lock (obj_lock)
            {
                lock (Awaits)
                {
                    // 第一个到达的线程
                    if (first)
                    {
                        //统计一共有几个线程并记录
                        foreach (KeyValuePair<int, Dictionary<string, string>> item in (Dictionary<int, Dictionary<string, string>>)Config["TestControl"])
                        {
                            //先给线程设置一个状态  0代表已到达 ,1代表未到达
                            Awaits[item.Key] = 1;
                        }
                        first = false;
                    }

                    //将本线程的状态设置成 0 已到达
                    Awaits[Convert.ToInt32(OnceConfig["TestID"])] = 0;
                }
            }
            do
            {
                Thread.Sleep(100);
            }
            while (Awaits.ContainsValue(1));//如果有未到达的线程 那就先等等
            first = true;
            Awaits.Clear();
            return "True";
        }



    }
}
