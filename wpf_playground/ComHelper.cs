using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using wpf_playground.Model;

namespace wpf_playground
{
    public static class ComHelper
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
            [new ComSignalConfig(SignalModeEnum.Visual, PQModeEnum.Visual, SOAEnum.Soa400)] = 19,
            [new ComSignalConfig(SignalModeEnum.Auditory, PQModeEnum.Visual, SOAEnum.Soa400)] = 20,
            [new ComSignalConfig(SignalModeEnum.Tactile, PQModeEnum.Visual, SOAEnum.Soa400)] = 21,
            [new ComSignalConfig(SignalModeEnum.Visual, PQModeEnum.Auditory, SOAEnum.Soa400)] = 22,
            [new ComSignalConfig(SignalModeEnum.Auditory, PQModeEnum.Auditory, SOAEnum.Soa400)] = 23,
            [new ComSignalConfig(SignalModeEnum.Tactile, PQModeEnum.Auditory, SOAEnum.Soa400)] = 24,
            [new ComSignalConfig(SignalModeEnum.Visual, PQModeEnum.Tactile, SOAEnum.Soa400)] = 25,
            [new ComSignalConfig(SignalModeEnum.Auditory, PQModeEnum.Tactile, SOAEnum.Soa400)] = 26,
            [new ComSignalConfig(SignalModeEnum.Tactile, PQModeEnum.Tactile, SOAEnum.Soa400)] = 27,
        };

        static SerialPort sendPort;

        public static void createPort(string portName)
        {
            try
            {
                if (sendPort != null && sendPort.IsOpen) return;
                sendPort = new SerialPort(portName, 57600, Parity.None, 8, StopBits.One) { Encoding = Encoding.ASCII };
                sendPort.WriteTimeout = 1000;
                sendPort.Open();
            }catch(Exception ex)
            {
                MessageBox.Show("Failed to create com port: " + ex.Message);
                throw ex;
            }
        }
        public static void closePort()
        {
            try
            {
                if (sendPort != null && sendPort.IsOpen) sendPort.Close();
            }catch(Exception ex)
            {
                MessageBox.Show("Failed to close com port: " + ex.Message);
                throw ex;
            }
        }


        public static string[] GetComportList()
        {
            var portList = SerialPort.GetPortNames();
            return portList;
        }

        public static void send(int val)
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
