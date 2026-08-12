using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace 示波器.Properties
{
	// Token: 0x0200001F RID: 31
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x060000BC RID: 188 RVA: 0x0000268C File Offset: 0x0000088C
		internal Resources()
		{
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x060000BD RID: 189 RVA: 0x000026BB File Offset: 0x000008BB
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("示波器.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060000BE RID: 190 RVA: 0x000026E7 File Offset: 0x000008E7
		// (set) Token: 0x060000BF RID: 191 RVA: 0x000026EE File Offset: 0x000008EE
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x000026F6 File Offset: 0x000008F6
		internal static Bitmap _
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("-", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x00002711 File Offset: 0x00000911
		internal static Bitmap _new
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("new", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x0000272C File Offset: 0x0000092C
		internal static Bitmap dinode
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("dinode", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x00002747 File Offset: 0x00000947
		internal static Icon ScopeMeter
		{
			get
			{
				return (Icon)Resources.ResourceManager.GetObject("ScopeMeter", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x00002762 File Offset: 0x00000962
		internal static string String1
		{
			get
			{
				return Resources.ResourceManager.GetString("String1", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00002778 File Offset: 0x00000978
		internal static Bitmap Winter
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("Winter", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x00002793 File Offset: 0x00000993
		internal static Bitmap 新建
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("新建", Resources.resourceCulture);
			}
		}

		// Token: 0x040001C4 RID: 452
		private static ResourceManager resourceMan;

		// Token: 0x040001C5 RID: 453
		private static CultureInfo resourceCulture;
	}
}
