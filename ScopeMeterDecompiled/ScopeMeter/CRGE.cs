using System;
using Microsoft.Win32;

namespace 示波器
{
	// Token: 0x0200001A RID: 26
	internal static class CRGE
	{
		// Token: 0x060000B3 RID: 179 RVA: 0x00010118 File Offset: 0x0000E318
		public static string findComHuada()
		{
			string result = "";
			try
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\ControlSet001\\Enum\\USB\\VID_2E88&PID_4603&MI_00");
				string[] subKeyNames = registryKey.GetSubKeyNames();
				if (subKeyNames.Length == 0)
				{
					return "";
				}
				registryKey = registryKey.OpenSubKey(subKeyNames[0]);
				registryKey = registryKey.OpenSubKey("Device Parameters");
				if (registryKey != null)
				{
					result = registryKey.GetValue("PortName").ToString();
					registryKey.Close();
				}
			}
			catch
			{
			}
			return result;
		}
	}
}
