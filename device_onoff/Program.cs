//
// Author: https://github.com/dkxce
// .Net 2.0+
// WinXP+
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;

namespace device_onoff
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Device On/Off");
            Console.WriteLine("usage: device_onoff.exe");
            Console.WriteLine("usage: device_onoff.exe /setup");
            Console.WriteLine("   Or type any data in cosole to set DeviceID");

            string ln = Conso1e.ReadLine(500);
            if (!string.IsNullOrEmpty(ln))
            {
                SetDevice();
                return;
            };

            if ((args != null) && (args.Length > 0) && (args[0] == "/setup"))
                SetDevice();                
            else
                ToggleDevice();
        }

        static void ToggleDevice()
        {
            string DeviceID = null;
            bool enabled = false;

            ReadRegistry(ref DeviceID, ref enabled);
            if(string.IsNullOrEmpty(DeviceID)) { SetDevice(); return; };

            Console.Write("Switching {0} {1} ... ", enabled ? "off" : "on", DeviceID);
            EnableDevice(DeviceID, enabled = !enabled);
            WriteRegistry(DeviceID, enabled);
            Console.WriteLine("OK");
            string ln = Conso1e.ReadLine(1000);
            if (!string.IsNullOrEmpty(ln))
                SetDevice();
        }

        static void SetDevice()
        {
            List<string> Devices = DeviceSwitcher.ListDevices();
            string DeviceID = "";
            bool enabled = false;
            ReadRegistry(ref DeviceID, ref enabled);
            InputBox.defWidth = 500;
            InputBox.pShowInTaskBar = true;
            if (InputBox.Show("Select Device", "DeviceiD (from Device Manager):", Devices.ToArray(), ref DeviceID, true) != DialogResult.OK) return;
            DeviceID = DeviceID.Trim();
            if (string.IsNullOrEmpty(DeviceID)) return;
            WriteRegistry(DeviceID, true);
        }

        static void EnableDevice(string DeviceID, bool enable)
        {
            //Guid mouseGuid = new Guid("{745a17a0-74d3-11d0-b6fe-00a0c90f57da}");
            //string DeviceID = @"HID\ELAN072B&COL03\4&F53F679&1&0002";
            DeviceSwitcher.SetDeviceEnabled(DeviceID, enable);
        }

        static void ReadRegistry(ref string DeviceID, ref bool enabled)
        {
            try
            {
                RegistryKey bkx86 = RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentUser, Environment.MachineName);
                RegistryKey rk = bkx86.OpenSubKey(@"SOFTWARE\dkxce\device_onoff");
                DeviceID = rk.GetValue("DeviceID", "").ToString();
                enabled = rk.GetValue("Enabled", 0).ToString() == "1";
                rk.Close();
                bkx86.Close();
                if (string.IsNullOrEmpty(DeviceID)) throw new Exception("None");
            }
            catch
            { };
        }

        static void WriteRegistry(string DeviceID, bool enabled)
        {
            try
            {
                RegistryKey bkx86 = RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentUser, Environment.MachineName);
                RegistryKey rk = bkx86.CreateSubKey(@"SOFTWARE\dkxce\device_onoff");
                rk.SetValue("DeviceID", DeviceID);
                rk.SetValue("Enabled", enabled ? 1 : 0);
                rk.Close();
                bkx86.Close();
            }
            catch
            { };
        }
    }
}
