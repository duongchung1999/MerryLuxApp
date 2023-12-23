using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MerryTestFramework.testitem.Forms;

namespace MerryTestFramework.testitem.Headset
{
    public class ButtonTest
    {
        private ProgressBars1 msgbox;

        public bool Buttontest(Func<bool> func, string name)
        {
            bool flag = true;
            Task task = Task.Run(delegate
            {
                Thread.Sleep(50);
                while (flag)
                {
                    if (func())
                    {
                        msgbox.DialogResult = DialogResult.OK;
                    }

                    Thread.Sleep(100);
                }
            });
            bool result = ProgressBarsBox(name);
            flag = false;
            return result;
        }

        private bool ProgressBarsBox(string name)
        {
            try
            {
                msgbox = new ProgressBars1(name);
                return msgbox.ShowDialog() == DialogResult.OK;
            }
            catch (Exception value)
            {
                Console.WriteLine(value);
                return false;
            }
        }
    }
}