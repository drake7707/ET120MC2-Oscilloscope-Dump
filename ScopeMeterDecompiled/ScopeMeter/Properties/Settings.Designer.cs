using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace 示波器.Properties
{
	// Token: 0x02000020 RID: 32
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.4.0.0")]
	[CompilerGenerated]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x000027AE File Offset: 0x000009AE
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x040001C6 RID: 454
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
