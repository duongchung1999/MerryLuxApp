using Common_V1;
using MerryTestFramework.testitem.Headset;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace MerryDllFramework
{
    /// <summary dllName="Common_V1">
    /// 通用测试类 Common_V1
    /// </summary>
    public class MerryDll : IMerryAllDll
    {
        #region 接口方法
        public static KeyboardHook k_hook = new KeyboardHook();
        Graphics currentGraphics = Graphics.FromHwnd(new WindowInteropHelper(new System.Windows.Window()).Handle);
        MessageBoxColor MessageBoxColor;
        Dictionary<string, object> OnceConfig = new Dictionary<string, object>();
        Dictionary<string, object> Config;
        _image img = new _image();
        ButtonTest buttonTest;
        public object Interface(Dictionary<string, object> Config) => this.Config = img.Config = Config;
        public string[] GetDllInfo()
        {
            string dllname = "DLL 名称       ：Common_V1";
            string dllfunction = "Dll功能说明 ：弹出窗体，串口调试";
            string dllVersion = "当前Dll版本：24.02.01.0";
            string dllChangeInfo = "Dll改动信息：";
            string dllChangeInfo1 = "22.11.24：增加启用和禁用USB设备，增加处理电脑字符集不一样导致抓取返回值失败";
            string dllChangeInfo2 = "23.3.24：增加获取虚拟SN入口";
            string dllChangeInfo3 = "23.4.26.0：awaitThread线程注释错了";
            string dllChangeInfo4 = "23.8.3.0：修改提示弹框的大小";
            string dllChangeInfo5 = "23.10.25.0：连板模式的弹框优化，分开好位置和确认按键";
            string dllChangeInfo6 = "23.11.01.0：MessageBox优化弹窗，连板模式的窗体显示的时候，程序自动寻找MoreTest界面";
            string[] info = { dllname, dllfunction,
                dllVersion, dllChangeInfo,dllChangeInfo1
            };
            return info;
        }
        #endregion
        public string Run(object[] Command)
        {
            SplitCMD(Command, out string[] cmd);

            switch (cmd[1])
            {
                case "MessageBox": return MessageBox(cmd[2]).ToString();
                case "Sleep": return Sleep(cmd[2]);
                case "MessageBox_Color": return MessageBox_Color(cmd[2], cmd[3]).ToString();
                case "Close_Box": return Close_Box(cmd[2]);
                case "LockSleep": return LockSleep(cmd[2]);
                case "WriteTextSerialPort":
                    return WriteTextSerialPort
                        (OnceConfig.ContainsKey("ComPort") ? (string)OnceConfig["ComPort"] : cmd[2],
                       int.Parse(cmd[3]), cmd[4]);
                case "SerialPort_Write":
                    return SerialPort_Write
                        (OnceConfig.ContainsKey("ComPort") ? (string)OnceConfig["ComPort"] : cmd[2],
                       int.Parse(cmd[3]), cmd[4]);
                case "WriteHexSerialPort":
                    return WriteHexSerialPort
                        (OnceConfig.ContainsKey("ComPort") ? (string)OnceConfig["ComPort"] : cmd[2],
                       int.Parse(cmd[3]), cmd[4]);
                case "ResponseSerialPort":
                    return ResponseSerialPort
                        (OnceConfig.ContainsKey("ComPort") ? (string)OnceConfig["ComPort"] : cmd[2],
                        int.Parse(cmd[3]), cmd[4], cmd[5], int.Parse(cmd[6]), cmd[7]);
                case "PowerFrequency":
                    return PowerFrequency(OnceConfig.ContainsKey("ComPort") ? (string)OnceConfig["ComPort"] : cmd[2],
                     bool.Parse(cmd[3]), bool.Parse(cmd[4]), int.Parse(cmd[5]));
                case "LockItem":
                    return LockItem(cmd[2]);

                case "UnlockItem":
                    return UnlockItem(cmd[2]);

                case "AwaitThreads":
                    return AwaitThreads(cmd[2]);
                case "EqualsDynamicCode":
                    return EqualsDynamicCode(cmd[2], int.Parse(cmd[3]));

                case "DisableDevice":
                    return DisableDevice();

                case "EnableDevice":
                    return EnableDevice();
                case "GetVirtualSerialNumber": return GetVirtualSerialNumber();

                case "Screenshot": return Screenshot(cmd[2], bool.Parse(cmd[3]));

                case "GetNewSN": return GetNewSN();

                case "AwaitDevice": return AwaitDevice(cmd[2], cmd[3], int.Parse(cmd[4]), cmd[5]);

                case "AwaitRemoveDevice": return AwaitRemoveDevice(cmd[2], cmd[3], int.Parse(cmd[4]), cmd[5]);

                case "CallCMD":
                    return CallCMD(bool.Parse(cmd[2]), cmd[3], cmd[4], cmd[5], bool.Parse(cmd[6]));
                case "ResponseCallCMD":
                    return ResponseCallCMD(cmd[2], cmd[3], cmd[4], cmd[5], bool.Parse(cmd[6]));
                case "Lock":
                    return Lock();
                case "Unlock":
                    return Unlock();
                case "AwaitThread":
                    return AwaitThread();
                default:
                    return "Connend Error False";
            }
        }
        void SplitCMD(object[] Command, out string[] CMD)
        {
            List<string> listCMD = new List<string>();
            foreach (var item in Command)
            {
                Type type = item.GetType();
                if (type == typeof(Dictionary<string, object>))
                    OnceConfig = (Dictionary<string, object>)item;
                if (type != typeof(string)) continue;
                listCMD = new List<string>(item.ToString().Split('&'));
                for (int i = 1; i < listCMD.Count; i++) listCMD[i] = listCMD[i].Split('=')[1];
            }
            Device_Location = OnceConfig.ContainsKey("Device_Location") ? (string)OnceConfig["Device_Location"] : "FFFFFFFFFFFFFFF";
            CMD = listCMD.ToArray();
        }


        /// <summary isPublicTestItem="true">
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ 串口区 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// </summary>
        /// <returns></returns>
        public string Cut_OffRule____()
        {
            return "True";
        }

        /// <summary isPublicTestItem="true">
        /// 串口下字符串指令 SerialPort_WriteLine
        /// </summary>
        /// <param name="PortName">串口号 比如COM1 连扳可填Null 连扳字段ComPort</param>
        /// <param name="Baudrate">波特率 比如9600</param>
        /// <param name="Command"> 指令 可输入\r 或 \n 程序会对应转译</param>
        /// <returns>info</returns>
        public string WriteTextSerialPort(string PortName, int Baudrate, string Command)
        {
            using (SerialPort Comport = new SerialPort())
            {
                try
                {
                    Comport.PortName = PortName;
                    Comport.BaudRate = Baudrate;
                    Comport.DataBits = 8;
                    Comport.Parity = Parity.None;
                    Comport.StopBits = StopBits.One;
                    Command = Command.Replace(@"\r", "\r").Replace(@"\n", "\n");
                    Comport.Open();
                    Comport.WriteLine(Command);
                    if (Comport.IsOpen) Comport.Close();
                    return $"{PortName} True";
                }
                catch (Exception ex)
                {
                    return $"{PortName} {ex.Message} Error";
                }
                finally
                {
                    Comport.Dispose();
                }
            }

        }

        /// <summary isPublicTestItem="true">
        /// 串口下字符串指令 SerialPort_Write
        /// </summary>
        /// <param name="PortName">串口号 比如COM1 连扳可填Null 连扳字段ComPort</param>
        /// <param name="Baudrate">波特率 比如9600</param>
        /// <param name="Command"> 指令 可输入\r 或 \n 程序会对应转译</param>
        /// <returns>info</returns>
        public string SerialPort_Write(string PortName, int Baudrate, string Command)
        {
            using (SerialPort Comport = new SerialPort())
            {
                try
                {
                    Comport.PortName = PortName;
                    Comport.BaudRate = Baudrate;
                    Comport.DataBits = 8;
                    Comport.Parity = Parity.None;
                    Comport.StopBits = StopBits.One;
                    Command = Command.Replace(@"\r", "\r").Replace(@"\n", "\n");
                    Comport.Open();
                    Comport.Write(Command);
                    if (Comport.IsOpen) Comport.Close();
                    return true.ToString();
                }
                catch (Exception ex)
                {
                    return $"{PortName} {ex.Message} False";
                }
                finally
                {
                    Comport.Dispose();
                }
            }

        }
        /// <summary isPublicTestItem="true">
        /// 串口下十六进制指令
        /// </summary>
        /// <param name="PortName">串口号 比如COM1 连扳可填Null 连扳字段ComPort</param>
        /// <param name="Baudrate">波特率 比如9600</param>
        /// <param name="Command">十六进制指令 空格分隔</param>
        /// <returns>info</returns>
        public string WriteHexSerialPort(string PortName, int Baudrate, string Command)
        {
            using (SerialPort Comport = new SerialPort())
            {
                try
                {
                    List<byte> hexCMD = new List<byte>();
                    Comport.PortName = PortName;
                    Comport.BaudRate = Baudrate;
                    Comport.DataBits = 8;
                    Comport.Parity = Parity.None;
                    Comport.StopBits = StopBits.One;
                    foreach (var item in Command.Trim().Split(' '))
                    {
                        hexCMD.Add(Convert.ToByte(item, 16));
                    }
                    Comport.Open();
                    Comport.Write(hexCMD.ToArray(), 0, hexCMD.Count);
                    if (Comport.IsOpen) Comport.Close();
                    return true.ToString();
                }
                catch (Exception ex)
                {
                    return $"{PortName} {ex.Message} False";
                }
                finally
                {
                    Comport.Dispose();
                }
            }

        }

        /// <summary isPublicTestItem="true">
        /// 串口下指令 并 检查包含返回值
        /// </summary>
        /// <param name="PortName">串口号 比如COM1 连扳可填Null 连扳字段ComPort</param>
        /// <param name="Baudrate">波特率 比如9600</param>
        /// <param name="Command">字符串指令</param>
        /// <param name="ContentString">返回值包含的字符串</param>
        /// <param name="TimeOut">检查超时/秒 常规 10</param>
        /// <param name="MSG">提示信息</param>
        /// 
        /// <returns>info</returns>
        public string ResponseSerialPort(string PortName, int Baudrate, string Command, string ContentString, int TimeOut, string MSG)
        {

            using (SerialPort Comport = new SerialPort())
            {
                try
                {
                    string Report = "TimeOut False";
                    Comport.PortName = PortName;
                    Comport.BaudRate = Baudrate;
                    Comport.DataBits = 8;
                    Comport.Parity = Parity.None;
                    Comport.StopBits = StopBits.One;
                    Comport.ReadTimeout = 1000;
                    Comport.WriteTimeout = 1000;
                    Comport.Open();
                    Comport.DiscardInBuffer();
                    ProgressBars pro = new ProgressBars();
                    Func<bool> func = new Func<bool>(() =>
                    {
                        int count = TimeOut * 1000 / 500;
                        for (int i = 0; i < count; i++)
                        {
                            Comport.WriteLine(Command);
                            Thread.Sleep(200);
                            for (int x = 0; x < 3; x++)
                            {
                                byte[] ReadByte = new byte[Comport.BytesToRead];
                                if (Comport.BytesToRead > 0)
                                {
                                    var a = Comport.Read(ReadByte, 0, ReadByte.Length);
                                    string Value = Encoding.ASCII.GetString(ReadByte);
                                    pro.MsgList.Add(Value);
                                    if (Value.Contains(ContentString))
                                        return true;
                                }
                                Thread.Sleep(100);
                            }
                        }
                        return false;
                    });
                    if (pro.CountDown_(func, $"{MSG}", "提示", TimeOut))
                    {
                        Report = $"{true}";
                    };

                    if (Comport.IsOpen) Comport.Close();
                    return $"{Report}";
                }
                catch (Exception ex)
                {
                    return $"{PortName} {ex.Message} Error";
                }
            }


        }

        /// <summary isPublicTestItem="true">
        /// 串口电频调节
        /// </summary>
        /// <param name="ComPort">串口号 比如 COM1 连扳可填Null 连扳字段ComPort</param>
        /// <param name="Dtr" options="True,False">4脚 True为高电频</param>
        /// <param name="Rts" options="True,False">7脚 True为高电频</param>
        /// <param name="millisecond">几毫秒后恢复</param>
        /// <returns>True or False</returns>
        public string PowerFrequency(string ComPort, bool Dtr, bool Rts, int millisecond)
        {

            using (SerialPort Comport = new SerialPort(ComPort))
            {
                try
                {
                    Comport.Open();
                    //  Com口 4脚设置是否高电频
                    Comport.DtrEnable = Dtr;
                    //  Com口 7脚设置是否高电频
                    Comport.RtsEnable = Rts;
                    return "True";
                }
                catch (Exception ex)
                {
                    return $"{ComPort} {ex.Message} Error";
                }
                finally
                {
                    Thread.Sleep(millisecond);
                    if (Comport.IsOpen) Comport.Close();
                }
            }
        }
        /// <summary isPublicTestItem="true">
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ 呼叫CMD区 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// </summary>
        /// <returns></returns>
        public string Cut_OffRule_____()
        {
            return "True";
        }

        /// <summary isPublicTestItem="true">
        /// 呼叫 CMD 运行程序
        /// </summary>
        /// <param name="isWait" options="True,False"> 是否等待完成 不用等待选“False”</param>
        /// <param name="WorkingDirectory">文件夹路径 相对路径输“default” 相对路径会切到机型目录下</param>
        /// <param name="Command">比如 TransportUnlock.exe args1 args2 args3</param>
        /// <param name="TimeOut">超时 单位/秒 选择异步完成可输入 “1”</param>
        /// <param name="isSaveLog" options="True,False">是否保存交互记录  选“True”可以在LOG文件夹找到CMDlog txt 默选“False”</param>
        /// <returns></returns>
        public string CallCMD(bool isWait, string WorkingDirectory, string Command, string TimeOut, bool isSaveLog)
        {
            return ResponseCallCMD(WorkingDirectory, Command, "MerryTest_Run_CMD_END", TimeOut, isSaveLog, isWait);
        }
        /// <summary isPublicTestItem="true">
        /// 呼叫 CMD 运行程序抓返回值
        /// </summary>
        /// <param name="WorkingDirectory">文件夹路径 相对路径输“default” 相对路径会切到机型目录下</param>
        /// <param name="Command">比如 TransportUnlock.exe args1 args2 args3</param>
        /// <param name="ResponseStr">响应的字符串用“|”分割 比如 succeed|success|100%</param>
        /// <param name="TimeOut">超时 单位/秒 选择异步完成可输入 “1”</param>
        /// <param name="isSaveLog" options="True,False">是否保存交互记录  选“True”可以在LOG文件夹找到CMDlog txt 默选“False”</param>
        /// <returns></returns>

        public string ResponseCallCMD(string WorkingDirectory, string Command, string ResponseStr, string TimeOut, bool isSaveLog, bool isWait = true)
        {
            string Result = "TimeOut False";
            ProcessLog.Clear();
            listLog.Clear();
            if (p == null)
            {
                p = new Process();
                p.StartInfo.FileName = $"cmd";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.CreateNoWindow = true;//
                p.StartInfo.RedirectStandardError = true;
                p.Start();
                p.StandardOutput.DiscardBufferedData();
                Thread.Sleep(500);
                Thread ReadThread = new Thread(() =>
                {
                    while (!p.StandardOutput.EndOfStream)
                    {
                        string readstr = p.StandardOutput.ReadLine();
                        lock (listLog)
                        {
                            listLog.Add(readstr);
                        }
                        Console.WriteLine(readstr);
                        ProcessLog.AppendLine(readstr);
                    };

                });
                ReadThread.Start();
            }
            if (!WorkingDirectory.Contains("default"))
            {
                p.StandardInput.WriteLine($"cd {WorkingDirectory.Substring(0, 2)}");
                p.StandardInput.WriteLine($"cd {WorkingDirectory}");

            }
            else
            {
                string logDir = $@"{Config["adminPath"]}\TestItem\{Config["Name"]}";
                p.StandardInput.WriteLine($"cd {logDir.Substring(0, 2)}");
                string dicp = $@"cd {logDir}\{WorkingDirectory.Replace(@"default\", "")}";
                p.StandardInput.WriteLine(dicp);
            }
            p.StandardInput.WriteLine($"{Command}");
            if (isWait)
            {
                p.StandardInput.WriteLine("MerryTest_Run_CMD_END");
                int Count = int.Parse(TimeOut) * 1000;
                ProgressBars pro = new ProgressBars();
                Func<bool> func = new Func<bool>(() =>
                {
                    string[] ResponseStrs = ResponseStr.Split('|');

                    for (int i = 0; i < Count; i += 100)
                    {
                        foreach (var item in ResponseStrs)
                        {
                            if (ProcessLog.ToString().Contains(item))//("MerryTest_Run_CMD_END"))
                            {
                                Result = "True";
                                return true;
                            }
                        }
                        if (ProcessLog.ToString().Contains("MerryTest_Run_CMD_END"))
                        {
                            Result = "end of run False ";
                            return true;
                        }
                        lock (listLog)
                        {
                            lock (pro.MsgList)
                            {
                                foreach (var item in listLog)
                                {
                                    pro.MsgList.Add(item);
                                }
                                listLog.Clear();
                            }

                        }

                        Thread.Sleep(100);
                    }

                    return false;
                });
                pro.CountDown_(func, $"正在调用“{Command}”", "提示", int.Parse(TimeOut));

            }
            else
            {
                Result = "True";
            }
            if (isSaveLog)
            {
                string logDir = $@"{Config["adminPath"]}\Log";
                Directory.CreateDirectory(logDir);
                string str = $"###################### {DateTime.Now} ######################\r\n{ProcessLog.ToString()}";
                File.AppendAllText($@"{logDir}\CMDlog_{DateTime.Now:yy_MM_dd}.txt", str);
            }
            Thread.Sleep(100);
            ProcessLog.Clear();
            return Result;
        }
        /// <summary isPublicTestItem="true">
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ 线程区 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// </summary>
        /// <returns></returns>
        public string Cut_OffRule()
        {
            return "True";
        }

        /// <summary isPublicTestItem="true">
        /// 延时
        /// </summary>
        /// <param name="millisecond">毫秒 千分之一秒</param>
        /// <returns> True </returns>
        public string Sleep(string millisecond)
        {
            int I = 500;
            I = int.TryParse(millisecond, out I) ? I : 500;
            if (I >= 5000)
            {

                ProgressBars pro = new ProgressBars();
                Func<bool> func = new Func<bool>(() =>
                {
                    Thread.Sleep(I);
                    return true;
                });
                if (pro.CountDown_(func, $"Await\r\nVui lòng đợi", "提示", (I + 100) / 1000))
                {
                    return $"{true}";
                };
            }
            else
            {
                Thread.Sleep(I);
            }
            return "True";
        }
        /// <summary isPublicTestItem="true">
        /// 等待关箱
        /// </summary>
        /// <param name="ComPort">PortName</param>
        /// <returns>True or False</returns>
        public string Close_Box(string ComPort)
        {
            string key = "close\r";
            string tip = " Vui lòng đóng nắp thùng\n请关箱测试 ";
            SerialPort serialPort = new SerialPort(ComPort, 9600, Parity.None, 8, StopBits.One);
            serialPort.Open();
            byte[] commandBytes = Encoding.ASCII.GetBytes("status\r");
            serialPort.Write(commandBytes, 0, commandBytes.Length);
            Thread.Sleep(500);
            string status = serialPort.ReadExisting();
            if (status.Contains("close"))
            {
                serialPort.Close();
                return true.ToString();
            }
            buttonTest = new ButtonTest();
            var result = buttonTest.Buttontest(() => {
               
                var btnKey = serialPort.ReadExisting();
                return key.Equals(btnKey);
            }, tip);
            serialPort.Close();
            return result.ToString();
        }
        /// <summary isPublicTestItem="true">
        /// 线程排队延时
        /// </summary>
        /// <param name="millisecond">毫秒 千分之一秒</param>
        /// <returns>True</returns>
        public string LockSleep(string millisecond)
        {
            lock (obj_lock)
            {
                Thread.Sleep(int.TryParse(millisecond, out int i) ? i : 1000);
                return "True";
            }

        }
        /// <summary isPublicTestItem="true">
        /// 项目锁 用于需要排队测试的项目
        /// </summary>
        /// <param name="number">给锁赐予的编号  1号锁只能1号解锁</param>
        /// <returns>True</returns>
        public string LockItem(string number)
              => LockThread_.LockNumberThread(number).ToString();

        /// <summary isPublicTestItem="true">
        /// 项目解锁
        /// </summary>
        /// <param name="number">锁的编号 1号解锁只能解1号锁</param>
        /// <returns>True</returns>
        public string UnlockItem(string number)
           => LockThread_.UnLockNumberThread(number).ToString();

        static Dictionary<string, AwaitThreads> DicLock = new Dictionary<string, AwaitThreads>();
        /// <summary isPublicTestItem="true">
        /// 等待所有线程
        /// </summary>
        /// <param name="number">等待的编号</param>
        /// <returns></returns>
        public string AwaitThreads(string number)
        {
            lock (ObjDicLock)
            {
                lock (DicLock)
                {
                    if (!DicLock.ContainsKey(number))
                    {
                        DicLock[number] = new AwaitThreads();
                        DicLock[number].Config = this.Config;
                    }
                }
            }
            return DicLock[number].AwaitThread(this.OnceConfig);
        }
        /// <summary isPublicTestItem="true">
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ 辅助区 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// </summary>
        /// <returns></returns>
        public string Cut_OffRule_()
        {
            return "True";
        }
        /// <summary isPublicTestItem="true">
        /// 截屏
        /// </summary>
        /// <param name="IsSaveImg" options="Fail,Pass,All">“Fail”表示测试失败才会截图储存</param>
        /// <param name="isUploadServer" options="True,False">“True”表示上传服务器</param>
        /// <returns></returns>
        public string Screenshot(string IsSaveImg, bool isUploadServer)
        {
            bool flag = (bool)Config["TestResultFlag"];
            switch (IsSaveImg)
            {
                case "Fail":
                    if (!flag)
                        img.CompressImage(isUploadServer);
                    return "True";

                case "Pass":
                    if (flag)
                        img.CompressImage(isUploadServer);
                    return "True";
                case "All":
                    img.CompressImage(isUploadServer);
                    return "True";
                default:
                    return $"Save CMD Error {false}";
            }
        }
        /// <summary isPublicTestItem="true">
        /// 获取时间进制21码SN
        /// </summary>
        /// <returns></returns>
        public string GetNewSN()
        {
            string datatime = DateTime.Now.ToString("yyyyMMddHHmmssFFFFFFF");
            return datatime;
        }
        /// <summary isPublicTestItem="true">
        /// 获取TE库有编码原则的SN
        /// </summary>
        /// <returns></returns>
        public string GetVirtualSerialNumber()
        {
            string Str = HttpPostAPI.HttpPost($"http://10.55.2.25:8086/api/LGVirtualSN/GenerateLGVirtualSN?ModelName={Config["Name"]}&StationName={Config["Station"]}", "");
            return Str;
        }
        string DynamicCode = "";
        /// <summary isPublicTestItem="true">
        /// 检测程序动态密码
        /// </summary>
        /// <param name="Message">弹窗信息</param>
        /// <param name="Length">密码长度</param>
        /// <returns>info</returns>
        public string EqualsDynamicCode(string Message, int Length)
        {
            string SQL = $@"
SELECT a1.`code` 
FROM dynamic_code a1, model a2
WHERE a1.model_id=a2.id AND a2.`name`='{Config["Name"]}'
";
            string SqlResult = Common_V1.MySqlHelper.GetData(SQL, out string[][] dataTable);
            if (SqlResult != "True")
                return SqlResult;
            if (dataTable.Length <= 0)
                return $"Not Found {Config["Name"]} Dynamic Code False";

            if (DynamicCode == "")
            {
                if (!MessageBoxs.BarCodeBox(Message, Length, out DynamicCode))
                    return "Scan Barcode False";
            }
            if (DynamicCode != dataTable[0][0])
                return $"{DynamicCode} Wrong Password False";
            byte[] ascii = Encoding.ASCII.GetBytes(DynamicCode);
            string AsciiHex = "";
            foreach (var item in ascii)
            {
                AsciiHex += $"{Convert.ToString(item * 5, 16)}";
            }
            return "True";//$"{AsciiHex}";

        }
        /// <summary isPublicTestItem="true">
        /// 弹框提示信息
        /// </summary>
        /// <param name="Message">显示在弹框的信息</param>
        /// <returns>True or False</returns>
        public bool MessageBox(string Message)
        {

            if (OnceConfig.ContainsKey("SN"))
            {
                int TestID = Convert.ToInt16(OnceConfig["TestID"]);
                Box boxs = new Box(TestID, Message);
                int Width = (int)currentGraphics.VisibleClipBounds.Width / 3;
                int Height = (int)currentGraphics.VisibleClipBounds.Height / 2;
                boxs.Width = (int)(Width * 0.9);
                boxs.Height = (int)(Height * 0.8);
                var newx = boxs.Width;
                boxs.uias.UpdateSize(boxs.Width, boxs.Height, boxs);
                boxs.uias.X = newx;
                boxs.label1.Font = new System.Drawing.Font("微软雅黑", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                boxs.NO.Font = new System.Drawing.Font("微软雅黑", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                boxs.YES.Font = new System.Drawing.Font("微软雅黑", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));


                boxs.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
                //boxs.Location = TestID <= 3
                //    ? boxs.Location = (Point)new Size(Width / 20 + (TestID - 1) * Width, Height / 10)
                //    : boxs.Location = (Point)new Size(Width / 20 + (TestID - 4) * Width, Height / 10 + Height);
                if (TestID <= 3)
                {
                    boxs.Location = (Point)new Size(Width / 20 + (TestID - 1) * Width, Height / 7);
                }
                else if (TestID <= 6)
                {
                    boxs.Location = (Point)new Size(Width / 20 + (TestID - 4) * Width, Height / 20 + Height);
                }
                else if (TestID <= 9)
                {
                    boxs.Location = (Point)new Size(Width / 20 + (TestID - 7) * Width, Height / 20 + Height / 2);

                }
                else
                {
                    boxs.StartPosition = FormStartPosition.WindowsDefaultLocation; //窗体的位置由Location属性决定

                }



                boxs.ShowDialog();
                var result = boxs.DialogResult;//先关闭会获取不到值
                return result == DialogResult.Yes;


            }
            else
            {
                Box boxs = new Box(Message);
                boxs.StartPosition = FormStartPosition.CenterScreen;
                if(boxs.label1.Text.Length > 40)
                {
                    boxs.label1.Font = new System.Drawing.Font("微软雅黑", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                }
                boxs.ShowDialog();
                var result = boxs.DialogResult;//先关闭会获取不到值
                return result == DialogResult.Yes;
            }

        }
        /// <summary isPublicTestItem="true">
        /// MessageBox_Color
        /// </summary>
        /// <param name="Message">Message information</param>
        /// <param name="value" options="BLACK,WHITE,BLUE,GREEN,ORANGE" >Color</param>
        /// <returns>info</returns>
        public bool MessageBox_Color(string Message,string value)
        {
            MessageBoxColor = new MessageBoxColor(Message, value);
            MessageBoxColor.Show();
            int iValue = MessageBoxColor.IValue;
            MessageBoxColor.Close();
            if (iValue == 1)return true;
            return false;
        }
        /// <summary isPublicTestItem="true">
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ USB设备区 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// </summary>
        /// <returns></returns>
        public string Cut_OffRule__()
        {
            return "True";
        }

        /// <summary isPublicTestItem="true">
        /// 禁用USB装置 必须跟启用USB装置搭配使用（只支持连扳，需要设定Device_ID）
        /// </summary>
        /// <returns></returns>
        public string DisableDevice()
        {
            CallCmd($"cd {DllPath}", "", 1);
            string Device_ID = Common_V1.WindowsAPI.SelectLocation(Device_Location);
            if (Device_ID.Contains("False"))
                return "Not Found Device False";
            string Result = "Not Send False";
            foreach (var item in Device_ID.Split(' '))
            {
                Result = CallCmd($"pnputil.exe /disable-device \"{item}\"", "", 1000);
                //break;
            }
            return Result;

        }
        /// <summary isPublicTestItem="true">
        /// 启用USB装置必须设定必须执行项目（只支持连扳，需要设定Device_ID）
        /// </summary>
        /// <returns></returns>
        public string EnableDevice()
        {
            string Device_ID = Common_V1.WindowsAPI.SelectLocation(Device_Location);
            if (Device_ID.Contains("False"))
                return "Not Found Device False";
            string Result = "Not Send False";
            foreach (var item in Device_ID.Split(' '))
            {
                Result = CallCmd($"pnputil.exe /enable-device \"{item}\"", "", 1000);
                // break;

            }
            return Result;
        }


        /// <summary isPublicTestItem="true">
        /// 等待装置识别
        /// </summary>
        /// <param name="VID">VID</param>
        /// <param name="PID">PID</param>
        /// <param name="TimeOut">检查超时/秒 常规 8</param>
        /// <param name="MSG">提示信息</param>
        /// <returns>info</returns>
        public string AwaitDevice(string VID, string PID, int TimeOut, string MSG)
        {


            try
            {
                int count = TimeOut * 1000 / 250;
                ProgressBars pro = new ProgressBars();
                Func<bool> func = new Func<bool>(() =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (GetHandle.GetHidDevicePath(VID, PID, ""))
                        {
                            return true;
                        };
                        Thread.Sleep(250);
                    }
                    return false;
                });
                if (pro.CountDown_(func, $"正在识别装置 PID:{PID} VID：{VID}”", "提示", TimeOut))
                {
                    return $"{true}";
                };
                return $"Await {VID}_{PID} Time Out False";
            }
            catch (Exception ex)
            {
                return $"{ex.Message} Error";
            }

        }
        /// <summary isPublicTestItem="true">
        /// 等待装置移除
        /// </summary>
        /// <param name="VID">VID</param>
        /// <param name="PID">PID</param>
        /// <param name="TimeOut">检查超时/秒 常规 8</param>
        /// <param name="MSG">提示信息</param>
        /// <returns>info</returns>
        public string AwaitRemoveDevice(string VID, string PID, int TimeOut, string MSG)
        {

            try
            {
                int count = TimeOut * 1000 / 250;


                ProgressBars pro = new ProgressBars();
                Func<bool> func = new Func<bool>(() =>
                {
                    for (int i = 0; i < count; i++)
                    {

                        if (!GetHandle.GetHidDevicePath(VID, PID, ""))
                        {
                            return true;
                        };

                        Thread.Sleep(250);

                    }
                    return false;
                });
                if (pro.CountDown_(func, $"正在识别装置 PID:{PID} VID：{VID}”", "提示", TimeOut))
                {
                    return $"{true}";
                };





                return $"Await Remove {VID}_{PID} Time Out False";
            }
            catch (Exception ex)
            {
                return $"{ex.Message} Error";
            }
        }





        public string Lock()
            => LockThread_.Lock().ToString();
        public string Unlock()
            => LockThread_.UnLock().ToString();

        object ObjDicLock = new object();
        private static object obj_lock = new object();
        private static List<int> TestFlags = null;
        private static List<int> testID = new List<int>();
        private static bool first = true;
        public string AwaitThread()
        {
            lock (obj_lock)
            {
                if (TestFlags == null)
                {
                    foreach (KeyValuePair<int, Dictionary<string, string>> item in (Dictionary<int, Dictionary<string, string>>)Config["TestControl"])
                    {
                        testID.Add(item.Key);
                    }
                    TestFlags = new List<int>(new int[500]);
                }
                if (first)
                {
                    foreach (int item2 in testID)
                    {
                        TestFlags[item2] = 1;
                    }
                    first = false;
                }
            }
            do
            {
                TestFlags[Convert.ToInt32(OnceConfig["TestID"])] = 0;
                Thread.Sleep(100);
            }
            while (TestFlags.Contains(1));
            first = true;
            return "True";
        }


        Process p;
        StringBuilder ProcessLog = new StringBuilder();
        List<string> listLog = new List<string>();
        string CallCmd(string args, string ResultStr = "", int TimeOut = 0)
        {
            return CallCmd(args, ResultStr, TimeOut, out string[] strs);
        }
        string CallCmd(string args, string ResultStr, int TimeOut, out string[] info)
        {
            info = null;
            string Result = "";
            try
            {
                ProcessLog.Clear();
                if (p == null)
                {
                    p = new Process();
                    p.StartInfo.FileName = $"cmd";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.CreateNoWindow = true;//
                    p.StartInfo.RedirectStandardError = true;
                    p.Start();
                    p.StandardOutput.DiscardBufferedData();
                    Thread.Sleep(500);
                    Thread ReadThread = new Thread(() =>
                    {
                        while (!p.StandardOutput.EndOfStream)
                        {
                            string readstr = p.StandardOutput.ReadLine();
                            ProcessLog.Append(readstr);
                        };

                    });
                    ReadThread.Start();

                }
                p.StandardInput.WriteLine($"{args}");

                for (int i = 0; i < TimeOut; i += 100)
                {
                    foreach (var item in ResultStr.Split('|'))
                    {
                        if (ProcessLog.ToString().Contains(item))
                        {
                            Result = "True";
                            break;
                        }
                    }

                    Thread.Sleep(100);
                }
                Thread.Sleep(100);
                return Result == "True" ? Result : "TimeOut False";

            }
            catch (Exception ex)
            {
                return $"{ex.Message} False";
            }
        }




        string DllPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        string Device_Location = "";

    }
    static class LockThread_
    {
        private static bool Flag;

        public static bool Lock()
        {
            while (Flag)
            {
                Thread.Sleep(100);
            }

            Flag = true;
            return true;
        }

        public static bool UnLock()
        {
            Flag = false;
            return true;
        }
        static Dictionary<string, bool> lockbool = new Dictionary<string, bool>();
        static Dictionary<string, object> lockObj = new Dictionary<string, object>();

        public static bool LockNumberThread(string number)
        {
            if (!lockObj.ContainsKey(number))
            {
                lockObj[number] = new object();
                lockbool[number] = false;
            }
            lock (lockObj[number])
            {

                while (lockbool[number])
                {
                    Thread.Sleep(100);
                }
                lockbool[number] = true;
            }
            return true;
        }

        public static bool UnLockNumberThread(string number)
        {
            lockbool[number] = false;
            return true;
        }
    }

}
