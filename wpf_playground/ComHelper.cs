﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpf_playground.Model;

namespace wpf_playground
{
    public class ComHelper
    {

        public static Dictionary<ComSignalConfig, int> MappingDict = new Dictionary<ComSignalConfig, int>
        {
            [new ComSignalConfig(SignalModeEnum.Visual, PQModeEnum.Visual, SOAEnum.Soa200)] = 1,
            [new ComSignalConfig(SignalModeEnum.Auditory, PQModeEnum.Visual, SOAEnum.Soa200)] = 2,
            [new ComSignalConfig(SignalModeEnum.Tactile, PQModeEnum.Visual, SOAEnum.Soa200)] = 3,
            [new ComSignalConfig(SignalModeEnum.Visual, PQModeEnum.Auditory, SOAEnum.Soa200)] = 4,
            [new ComSignalConfig(SignalModeEnum.Auditory, PQModeEnum.Auditory, SOAEnum.Soa200)] = 5,
            [new ComSignalConfig(SignalModeEnum.Tactile, PQModeEnum.Auditory, SOAEnum.Soa200)] = 6,
            [new ComSignalConfig(SignalModeEnum.Visual, PQModeEnum.Tactile, SOAEnum.Soa200)] = 7,
            [new ComSignalConfig(SignalModeEnum.Auditory, PQModeEnum.Tactile, SOAEnum.Soa200)] = 8,
            [new ComSignalConfig(SignalModeEnum.Tactile, PQModeEnum.Tactile, SOAEnum.Soa200)] = 9,
            [new ComSignalConfig(SignalModeEnum.Visual, PQModeEnum.Visual, SOAEnum.Soa600)] = 10,
            [new ComSignalConfig(SignalModeEnum.Auditory, PQModeEnum.Visual, SOAEnum.Soa600)] = 11,
            [new ComSignalConfig(SignalModeEnum.Tactile, PQModeEnum.Visual, SOAEnum.Soa600)] = 12,
            [new ComSignalConfig(SignalModeEnum.Visual, PQModeEnum.Auditory, SOAEnum.Soa600)] = 13,
            [new ComSignalConfig(SignalModeEnum.Auditory, PQModeEnum.Auditory, SOAEnum.Soa600)] = 14,
            [new ComSignalConfig(SignalModeEnum.Tactile, PQModeEnum.Auditory, SOAEnum.Soa600)] = 15,
            [new ComSignalConfig(SignalModeEnum.Visual, PQModeEnum.Tactile, SOAEnum.Soa600)] = 16,
            [new ComSignalConfig(SignalModeEnum.Auditory, PQModeEnum.Tactile, SOAEnum.Soa600)] = 17,
            [new ComSignalConfig(SignalModeEnum.Tactile, PQModeEnum.Tactile, SOAEnum.Soa600)] = 18,
            [new ComSignalConfig(SignalModeEnum.Visual, PQModeEnum.Visual, SOAEnum.Soa1000)] = 19,
            [new ComSignalConfig(SignalModeEnum.Auditory, PQModeEnum.Visual, SOAEnum.Soa1000)] = 20,
            [new ComSignalConfig(SignalModeEnum.Tactile, PQModeEnum.Visual, SOAEnum.Soa1000)] = 21,
            [new ComSignalConfig(SignalModeEnum.Visual, PQModeEnum.Auditory, SOAEnum.Soa1000)] = 22,
            [new ComSignalConfig(SignalModeEnum.Auditory, PQModeEnum.Auditory, SOAEnum.Soa1000)] = 23,
            [new ComSignalConfig(SignalModeEnum.Tactile, PQModeEnum.Auditory, SOAEnum.Soa1000)] = 24,
            [new ComSignalConfig(SignalModeEnum.Visual, PQModeEnum.Tactile, SOAEnum.Soa1000)] = 25,
            [new ComSignalConfig(SignalModeEnum.Auditory, PQModeEnum.Tactile, SOAEnum.Soa1000)] = 26,
            [new ComSignalConfig(SignalModeEnum.Visual, PQModeEnum.Tactile, SOAEnum.Soa1000)] = 27,
        };

        SerialPort sendPort;

        void createPort(string portName)
        {
            if (sendPort != null)
            {
                sendPort.Close();
            }
            sendPort = new SerialPort(portName, 57600, Parity.None, 8, StopBits.One) { Encoding = Encoding.ASCII };
            sendPort.WriteTimeout = 1000;
            sendPort.Open();
        }

        public ComHelper(string portName)
        {
            createPort(portName);
        }

        public void closePort()
        {
            sendPort.Close();
            sendPort = null;
        }

        ~ComHelper()
        {
            closePort();
        }

        public static string[] GetComportList()
        {
            var portList = SerialPort.GetPortNames();
            return portList;
        }

        public void send(int val)
        {
            try
            {
                sendPort.Write(new byte[] { Byte.Parse("0") }, 0, 1);
                byte[] buffer = new byte[] { Convert.ToByte(val) };
                sendPort.Write(buffer, 0, 1);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
        

    }
}