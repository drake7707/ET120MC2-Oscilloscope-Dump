using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace 示波器
{
	// Token: 0x0200001E RID: 30
	internal static class Program
	{
		// Token: 0x060000BB RID: 187 RVA: 0x00010480 File Offset: 0x0000E680
		[STAThread]
		private static void Main()
		{
			CultureInfo da = CultureInfo.GetCultureInfo("en-US");
			CultureInfo.DefaultThreadCurrentCulture = da;
			CultureInfo.DefaultThreadCurrentUICulture = da;
			Thread.CurrentThread.CurrentCulture = da;
			Thread.CurrentThread.CurrentUICulture = da;
			string i = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);
			int id = Process.GetCurrentProcess().Id;
			foreach (Process process in Process.GetProcesses())
			{
				if (process.ProcessName == i && process.Id != id)
				{
					process.Kill();
					break;
				}
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1ShiBoQi());
		}
	}
}
