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

            // if setup
            string ln = Conso1e.ReadLine(500);
            if (!string.IsNullOrEmpty(ln)) { SetupDevice(); return; };
            if ((args != null) && (args.Length > 0) && (args[0] == "/setup")) { SetupDevice(); return; };
            
            ToggleDevice();
        }

        static void SetupDevice()
        {
            string DeviceID = "";
            bool enabled = true;

            List<string> Devices = DeviceSwitcher.ListDevices(ref DeviceID);
            ReadRegistry(ref DeviceID, ref enabled);

            InputBox.defWidth = 500;
            InputBox.pShowInTaskBar = true;
            if (InputBox.Show("Select Device", "DeviceiD (from Device Manager):", Devices.ToArray(), ref DeviceID, true) != DialogResult.OK) return;
            DeviceID = DeviceID.Trim();
            if (string.IsNullOrEmpty(DeviceID)) return;

            WriteRegistry(DeviceID, true);
        }

        #region ToggleDevice
        static void ToggleDevice()
        {
            string DeviceID = null;
            bool enabled = false;

            ReadRegistry(ref DeviceID, ref enabled);
            if(string.IsNullOrEmpty(DeviceID)) { SetupDevice(); return; }; // no device

            Console.Write("Switching {0} {1} ... ", enabled ? "off" : "on", DeviceID);
            ToggleDevice(DeviceID, enabled = !enabled);
            WriteRegistry(DeviceID, enabled);
            Console.WriteLine("OK");

            // if setup
            string ln = Conso1e.ReadLine(1000);
            if (!string.IsNullOrEmpty(ln))
                SetupDevice();
        }
       
        static void ToggleDevice(string DeviceID, bool on_or_off)
        {
            //Guid mouseGuid = new Guid("{745a17a0-74d3-11d0-b6fe-00a0c90f57da}");
            //string DeviceID = @"HID\ELAN072B&COL03\4&F53F679&1&0002";
            DeviceSwitcher.SetDeviceEnabled(DeviceID, on_or_off);
        }
        #endregion ToggleDevice

        #region Registry
        static void ReadRegistry(ref string DeviceID, ref bool enabled)
        {
            try
            {
                RegistryKey rkBase = RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentUser, Environment.MachineName);
                RegistryKey rkApp = rkBase.OpenSubKey(@"SOFTWARE\dkxce\device_onoff");
                DeviceID = rkApp.GetValue("DeviceID", DeviceID).ToString();
                enabled = rkApp.GetValue("Enabled", enabled ? 1 : 0).ToString() == "1";
                rkApp.Close();
                rkBase.Close();
                if (string.IsNullOrEmpty(DeviceID)) throw new Exception("None");
            }
            catch
            { };
        }

        static void WriteRegistry(string DeviceID, bool enabled)
        {
            try
            {
                RegistryKey rkBase = RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentUser, Environment.MachineName);
                RegistryKey rkApp = rkBase.CreateSubKey(@"SOFTWARE\dkxce\device_onoff");
                rkApp.SetValue("DeviceID", DeviceID);
                rkApp.SetValue("Enabled", enabled ? 1 : 0);
                rkApp.Close();
                rkBase.Close();
            }
            catch
            { };
        }
        #endregion Registry
    }
}
