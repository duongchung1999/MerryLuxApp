﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HanOpticSens_V1
{
    internal class ControlHanOptic
    {

        SerialPort sp;

        public string SendHanOpti(string ComPort, string cmd, int Channel, double AddNumber, double Coefficient = 1)
        {
            string ReturnValue = ResponseHanOpti(ComPort, cmd);
            if (ReturnValue.Contains("False")) return ReturnValue;
            string[] ReturnValues = ReturnValue.Split('=');
            string[] Result = ReturnValues[1].Split(',');
            double DoubleValue = Math.Round(double.Parse(Result[Channel - 1]) * Coefficient, 2) + AddNumber;
            return DoubleValue.ToString();
        }
        public string ResponseHanOpti(string ComPort, string cmd)
        {
            try
            {
                if (sp == null)
                    sp = new SerialPort
                    {
                        PortName = ComPort,
                        BaudRate = 115200,
                        DataBits = 8
                    };

                if (!sp.IsOpen) sp.Open();
                sp.DiscardInBuffer();
                Thread.Sleep(50); //毫秒内数据接收完毕，可根据实际情况调整
                sp.Write(cmd);
                sp.Write(new Byte[] { 0X0A }, 0, 1);
                Thread.Sleep(50); //毫秒内数据接收完毕，可根据实际情况调整
                var recData = new byte[sp.BytesToRead];
                sp.Read(recData, 0, recData.Length);
                return Encoding.ASCII.GetString(recData);//将byte数组转换为ASCll OK = 4F 4B
            }
            catch (Exception ex)
            {
                if (sp.IsOpen) sp.Close();
                sp?.Dispose();
                sp = null;
                return $"{ex.Message} Error False";
            }


        }

    }
}