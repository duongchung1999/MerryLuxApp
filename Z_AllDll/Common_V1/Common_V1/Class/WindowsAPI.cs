using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Common_V1
{
    internal class WindowsAPI
    {
        #region 参数及引用区
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int CloseHandle(IntPtr hFile);
        [DllImport("hid.dll")]//获得GUID
        private static extern void HidD_GetHidGuid(ref Guid hidGuid);
        [DllImport("setupapi.dll", SetLastError = true)]//过滤设备，获取需要的设备
        private static extern IntPtr SetupDiGetClassDevs(ref Guid gClass, [MarshalAs(UnmanagedType.LPStr)] string strEnumerator, IntPtr hParent, Digcf nFlags);
        private enum Digcf  //3
        {
            DigcfDefault = 0x1,//返回与系统默认设备相关的设备
            DigcfPresent = 0x2,//返回当前存在的设备
            DigcfAllclasses = 0x4,//返回所有安装的设备
            DigcfProfile = 0x8,//只返回当前硬件配置文件的设备
            DigcfDeviceinterface = 0x10//返回所有支持的设备
        }
        internal struct SpDeviceInterfaceData
        {
            internal int Size;
            internal Guid InterfaceClassGuid;
            internal int Flags;
            internal int Reserved;
        }
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, uint devInfo, ref Guid interfaceClassGuid, uint memberIndex, ref SpDeviceInterfaceData deviceInterfaceData);
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SpDeviceInterfaceData deviceInterfaceData, IntPtr deviceInterfaceDetailData,
                                                                 uint deviceInterfaceDetailDataSize, ref uint requiredSize, IntPtr deviceInfoData);
        [StructLayout(LayoutKind.Sequential, Pack = 2)]//2
        internal struct SpDeviceInterfaceDetailData
        {
            internal int Size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            internal string DevicePath;
        }

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr lpDeviceInfoSet, ref SpDeviceInterfaceData oInterfaceData, ref SpDeviceInterfaceDetailData oDetailData, uint nDeviceInterfaceDetailDataSize, ref uint nRequiredSize, IntPtr lpDeviceInfoData);
        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern IntPtr SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);
        #endregion
        #region 参数区
        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, int MemberIndex, ref SpDeviceInterfaceData DeviceInfoData);
        [DllImport("setupapi.dll", SetLastError = true)]
        unsafe static extern bool SetupDiGetDevicePropertyW(
                                IntPtr DeviceInfoSet,
                                ref SpDeviceInterfaceData DeviceInfoData,
                                ref DEVPKEY_Device_RemovalRelations PropertyKey,
                                ref uint PropertyType,
                                byte* PropertyBuffer,
                                byte[] PropertyBufferSize,
                                ref int RequiredSize,
                                int Flags
                                  );
        struct DEVPKEY_Device_RemovalRelations
        {
            public Guid fmtid;
            public uint pid;

        }
        static DEVPKEY_Device_RemovalRelations DEVPKEY_Device_BusRelations = new DEVPKEY_Device_RemovalRelations()
        {
            fmtid = new Guid("4340A6C5-93FA-4706-972C-7B648008A5A7"),
            pid = 7
        };
        static DEVPKEY_Device_RemovalRelations DEVPKEY_Device_LocationInfo = new DEVPKEY_Device_RemovalRelations()
        {
            fmtid = new Guid("A45C254E-DF1C-4EFD-8020-67D146A850E0"),
            pid = 15
        };

        #endregion


        #region Location
        unsafe public bool SelectLocation(string pid, string vid, string Location, string QueryStr, out string HidInfo)
        {
            HidInfo = "";
            bool Flag = false;
            Guid guid = Guid.Empty;
            //获取HID的GUID
            HidD_GetHidGuid(ref guid);
            //          系统装置类指针                                选择USB                          返回当前存在且 所安装的设备
            IntPtr DeviceInfoSet = SetupDiGetClassDevs(ref guid, "USB", IntPtr.Zero, Digcf.DigcfPresent | Digcf.DigcfAllclasses);
            //指针==0的时候说明获取失败或者没有
            if (DeviceInfoSet == IntPtr.Zero) return false;
            for (int i = 0; ; i++)
            {
                //系统定义装置结构
                SpDeviceInterfaceData DeviceInterfaceData = new SpDeviceInterfaceData();
                //获取非托管变量的大小
                DeviceInterfaceData.Size = Marshal.SizeOf(DeviceInterfaceData);
                //系统装置会有个列表排列装置    指针         我们不知道第几个需要是哪个设备只能每个遍历（i）
                if (!SetupDiEnumDeviceInfo(DeviceInfoSet, i, ref DeviceInterfaceData)) break;

                int RequiredSize = 0;
                uint PropertyType = 0;
                //确保变量有足够的容量储存返回信息
                byte[] PathBuffer = new byte[1024];
                if (PathBuffer.Length < 1024) Array.Resize(ref PathBuffer, 1024);
                byte[] LocationBuffer = new byte[1024];
                if (LocationBuffer.Length < 1024) Array.Resize(ref LocationBuffer, 1024);

                //C#使用指针的方式
                fixed (byte* sTransData = &PathBuffer[0])
                {
                    SetupDiGetDevicePropertyW(
                                     DeviceInfoSet,
                                     ref DeviceInterfaceData,
                                     ref DEVPKEY_Device_BusRelations,//系统变量存储路径的GUID和PID的编号
                                     ref PropertyType,
                                    sTransData,                      //储存容量的指针
                                    (PathBuffer),                    //返回的内容是ASCII
                                     ref RequiredSize,
                                     0
                                     );
                }
                fixed (byte* sTransData = &LocationBuffer[0])
                {
                    SetupDiGetDevicePropertyW(
                               DeviceInfoSet,
                               ref DeviceInterfaceData,
                               ref DEVPKEY_Device_LocationInfo,
                               ref PropertyType,
                              sTransData,
                                (LocationBuffer),
                               ref RequiredSize,
                               0
                               );
                }
                //转换格式就不用解释
                List<byte> Pathbyte = new List<byte>();
                List<byte> LocationByte = new List<byte>();
                foreach (var item in PathBuffer)
                    if (item > 0) Pathbyte.Add(item);
                foreach (var item in LocationBuffer)
                    if (item > 0) LocationByte.Add(item);
                string locationstr = Encoding.ASCII.GetString(LocationByte.ToArray());
                string Pathstr = Encoding.ASCII.GetString(Pathbyte.ToArray()).ToLower();
                //选择包含的位置              //包含HID字眼用于下指令，因为装置会有很多 USB Path
                if (locationstr.Contains(Location) & Pathstr.Contains("hid"))
                {
                    foreach (var item in Pathstr.Split('h'))
                    {
                        //包含PID                          //包含VID                //包含需要的索引字眼
                        if (item.Contains(pid.ToLower()) && item.Contains(vid.ToLower()) && item.Contains(QueryStr))
                        {
                            HidInfo = $"h{item}".Replace("\\", "#");
                            Flag = true;
                            break;

                        }
                    }
                }

            }
            return Flag;
        }
        unsafe public bool SelectLocation(out List<string> HidInfos)
        {

            HidInfos = new List<string>();
            Guid guid = Guid.Empty;
            //获取HID的GUID
            HidD_GetHidGuid(ref guid);
            //          系统装置类指针                                选择USB                          返回当前存在且 所安装的设备
            IntPtr DeviceInfoSet = SetupDiGetClassDevs(ref guid, "USB", IntPtr.Zero, Digcf.DigcfPresent | Digcf.DigcfAllclasses);
            //指针==0的时候说明获取失败或者没有
            if (DeviceInfoSet == IntPtr.Zero) return false;
            for (int i = 0; ; i++)
            {
                //系统定义装置结构
                SpDeviceInterfaceData DeviceInterfaceData = new SpDeviceInterfaceData();
                //获取非托管变量的大小
                DeviceInterfaceData.Size = Marshal.SizeOf(DeviceInterfaceData);
                //系统装置会有个列表排列装置    指针         我们不知道第几个需要是哪个设备只能每个遍历（i）
                if (!SetupDiEnumDeviceInfo(DeviceInfoSet, i, ref DeviceInterfaceData)) break;

                int RequiredSize = 0;
                uint PropertyType = 0;
                //确保变量有足够的容量储存返回信息
                byte[] PathBuffer = new byte[2048];
                if (PathBuffer.Length < 2048) Array.Resize(ref PathBuffer, 2048);
                byte[] LocationBuffer = new byte[2048];
                if (LocationBuffer.Length < 2048) Array.Resize(ref LocationBuffer, 2048);

                //C#使用指针的方式
                fixed (byte* sTransData = &PathBuffer[0])
                {
                    SetupDiGetDevicePropertyW(
                                     DeviceInfoSet,
                                     ref DeviceInterfaceData,
                                     ref DEVPKEY_Device_BusRelations,//系统变量存储路径的GUID和PID的编号
                                     ref PropertyType,
                                    sTransData,                      //储存容量的指针
                                    (PathBuffer),                    //返回的内容是ASCII
                                     ref RequiredSize,
                                     0
                                     );
                }
                fixed (byte* sTransData = &LocationBuffer[0])
                {
                    SetupDiGetDevicePropertyW(
                               DeviceInfoSet,
                               ref DeviceInterfaceData,
                               ref DEVPKEY_Device_LocationInfo,
                               ref PropertyType,
                              sTransData,
                                (LocationBuffer),
                               ref RequiredSize,
                               0
                               );
                }
                //转换格式就不用解释
                List<byte> Pathbyte = new List<byte>();
                List<byte> LocationByte = new List<byte>();
                foreach (var item in PathBuffer)
                    if (item > 0) Pathbyte.Add(item);
                foreach (var item in LocationBuffer)
                    if (item > 0) LocationByte.Add(item);
                string locationstr = Encoding.ASCII.GetString(LocationByte.ToArray());
                string Pathstr = Encoding.ASCII.GetString(Pathbyte.ToArray()).ToLower();
                //选择包含的位置              //包含HID字眼用于下指令，因为装置会有很多 USB Path
                if (Pathstr.Contains("hid"))
                {
                    foreach (var item in Pathstr.Split('h'))
                    {
                        HidInfos.Add($"{locationstr}={$"h{item}".Replace("\\", "#")}");
                    }
                }

            }
            return true;
        }
        unsafe public static string SelectLocation(string Location)
        {

            Guid guid = Guid.Empty;
            //获取HID的GUID
            HidD_GetHidGuid(ref guid);
            //          系统装置类指针                                选择USB                          返回当前存在且 所安装的设备
            IntPtr DeviceInfoSet = SetupDiGetClassDevs(ref guid, "USB", IntPtr.Zero, Digcf.DigcfPresent | Digcf.DigcfAllclasses);
            //指针==0的时候说明获取失败或者没有
            if (DeviceInfoSet == IntPtr.Zero) return "IntPtr==0 False";
            for (int i = 0; ; i++)
            {
                //系统定义装置结构
                SpDeviceInterfaceData DeviceInterfaceData = new SpDeviceInterfaceData();
                //获取非托管变量的大小
                DeviceInterfaceData.Size = Marshal.SizeOf(DeviceInterfaceData);
                //系统装置会有个列表排列装置    指针         我们不知道第几个需要是哪个设备只能每个遍历（i）
                if (!SetupDiEnumDeviceInfo(DeviceInfoSet, i, ref DeviceInterfaceData)) break;

                int RequiredSize = 0;
                uint PropertyType = 0;
                //确保变量有足够的容量储存返回信息
                byte[] PathBuffer = new byte[2048];
                if (PathBuffer.Length < 2048) Array.Resize(ref PathBuffer, 2048);
                byte[] LocationBuffer = new byte[2048];
                if (LocationBuffer.Length < 2048) Array.Resize(ref LocationBuffer, 2048);

                //C#使用指针的方式
                fixed (byte* sTransData = &PathBuffer[0])
                {
                    SetupDiGetDevicePropertyW(
                                     DeviceInfoSet,
                                     ref DeviceInterfaceData,
                                     ref DEVPKEY_Device_BusRelations,//系统变量存储路径的GUID和PID的编号
                                     ref PropertyType,
                                    sTransData,                      //储存容量的指针
                                    (PathBuffer),                    //返回的内容是ASCII
                                     ref RequiredSize,
                                     0
                                     );
                }
                fixed (byte* sTransData = &LocationBuffer[0])
                {
                    SetupDiGetDevicePropertyW(
                               DeviceInfoSet,
                               ref DeviceInterfaceData,
                               ref DEVPKEY_Device_LocationInfo,
                               ref PropertyType,
                              sTransData,
                                (LocationBuffer),
                               ref RequiredSize,
                               0
                               );
                }
                //转换格式就不用解释
                List<byte> Pathbyte = new List<byte>();
                List<byte> LocationByte = new List<byte>();
                foreach (var item in PathBuffer)
                    if (item > 0) Pathbyte.Add(item);
                foreach (var item in LocationBuffer)
                    if (item > 0) LocationByte.Add(item);
                string locationstr = Encoding.ASCII.GetString(LocationByte.ToArray());
                string Pathstr = Encoding.ASCII.GetString(Pathbyte.ToArray()).ToLower();

                // Console.WriteLine($"{locationstr.PadRight(50, ' ')}|{Pathstr}");
                //选择包含的位置              //包含HID字眼用于下指令，因为装置会有很多 USB Path
                string Device_Path = "";
                Console.WriteLine($"{locationstr.PadRight(20, ' ')}|{Pathstr}");
                if (locationstr.Contains(Location))
                {
                    if (Pathstr.Contains("usb"))
                    {
                        foreach (var item in Pathstr.Split('u'))
                        {
                            if (!item.Contains("sb"))
                                continue;
                            Device_Path += $"u{item} ";
                        }
                        return Device_Path.Trim();
                    }
                    if (Pathstr.Contains("h"))
                    {
                        foreach (var item in Pathstr.Split('h'))
                        {
                            if (!item.Contains("id"))
                                continue;
                            Device_Path += $"h{item} ";
                        }
                        return Device_Path.Trim();
                    }
                }


            }
            return "Read False";
        }


        #endregion

    }
}
