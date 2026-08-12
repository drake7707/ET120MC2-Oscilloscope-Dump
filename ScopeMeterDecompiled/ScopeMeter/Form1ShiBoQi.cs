using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using HandleINI;
using LEDConTroler;
using Microsoft.Win32;
using USBClassLibrary;
using ZedGraph;
using 示波器.Properties;

namespace 示波器
{
	// Token: 0x02000005 RID: 5
	public partial class Form1ShiBoQi : Form
	{
		// Token: 0x0600000F RID: 15 RVA: 0x000032E4 File Offset: 0x000014E4
		private void onDraw(Graphics dc, Rectangle si, bool drawControl)
		{
			if (this.macheType2 == Form1ShiBoQi.MachineType2.DMM)
			{
				switch (this.dmmMode)
				{
				case Form1ShiBoQi.DMM_Mode.DMM_realTimeData:
				case Form1ShiBoQi.DMM_Mode.AutoRecoreData:
				case Form1ShiBoQi.DMM_Mode.HoldData:
				case Form1ShiBoQi.DMM_Mode.Calibation:
					break;
				default:
					return;
				}
			}
			else if (this.dsoData1.dao[0] != null && (this.dsoData1.dao[0].bValid || this.dsoData1.dao[1].bValid))
			{
				Bitmap b;
				Graphics g;
				if (this.tempx == 0)
				{
					b = new Bitmap(si.Width, si.Height);
					g = Graphics.FromImage(b);
				}
				else
				{
					b = new Bitmap(10, 10);
					this.tempx = 0;
					g = dc;
				}
				g.FillRectangle(Brushes.Black, si.X, si.Y, si.Width, si.Height);
				float temp = (float)(si.Width + si.Height) / 80f;
				if (this.tempx == 0)
				{
					g.TranslateTransform(temp + (float)si.X, (float)si.Y + temp);
				}
				else
				{
					g.TranslateTransform(temp, temp);
				}
				GraphicsState state = g.Save();
				float t = (float)si.Width - 2f * temp;
				if (t <= 0f)
				{
					return;
				}
				float t2 = (float)si.Height - 2f * temp;
				if (t2 <= 0f)
				{
					return;
				}
				g.ScaleTransform(t / 10f, t2 / 8f);
				for (int i = 1; i < 8; i++)
				{
					g.DrawLine(this.peGray, 0, i, 10, i);
				}
				for (int j = 1; j < 12; j++)
				{
					g.DrawLine(this.peGray, (float)j / 1.2f, 0f, (float)j / 1.2f, 8f);
				}
				g.DrawLine(this.peBlack, 0, 4, 10, 4);
				g.DrawLine(this.peBlack, 5, 0, 5, 8);
				g.DrawRectangle(this.peBlack, 0, 0, 10, 8);
				if (this.holdScreen)
				{
					switch (this.clickButtonID)
					{
					case 2:
					{
						float per = 0.5f - this.getButtonPercent(this.button2);
						g.DrawLine(this.peGrayN, 0f, 8f * per, 10f, 8f * per);
						break;
					}
					case 3:
					{
						float per = 0.5f - this.getButtonPercent(this.button3);
						g.DrawLine(this.peGrayN, 0f, 8f * per, 10f, 8f * per);
						break;
					}
					case 4:
					{
						float per = this.getButtonPercent(this.button4);
						g.DrawLine(this.peGrayN, 10f * per, 0f, 10f * per, 8f);
						break;
					}
					case 5:
					{
						float per = this.getButtonPercent(this.button5);
						g.DrawLine(this.peGrayN, 10f * per, 0f, 10f * per, 8f);
						break;
					}
					}
				}
				g.Restore(state);
				g.ScaleTransform(t / 300f, t2 / 192f);
				Form1ShiBoQi.DSO_DATAS obj = this.dsoData1;
				lock (obj)
				{
					int[] array = new int[2];
					if (this.dsoData1.Channel_processing == 0)
					{
						array[0] = 0;
						array[1] = 1;
					}
					else
					{
						array[0] = 0;
						array[1] = 1;
					}
					int k = 0;
					if (!this.bValid)
					{
						this.bValid = this.bValidFile();
						k = 1;
					}
					while (k < 2)
					{
						int l = array[k];
						if (this.dsoData1.dao[l].bValid)
						{
							Pen pen;
							if (l == 0)
							{
								pen = this.peRed;
							}
							else
							{
								pen = this.peYellow;
							}
							if (!this.hScrollBar1.Visible)
							{
								g.DrawLines(pen, this.dsoData1.dao[l].dso_Data);
							}
							else
							{
								g.DrawLines(pen, this.dsoData1.dao[l].dso_Data);
							}
						}
						k++;
					}
				}
				g.DrawLine(this.p, 0, 0, 300, 0);
				g.DrawLine(this.p, 0, 200, 300, 200);
				g.DrawLine(this.p, 0, 0, 0, 200);
				g.DrawLine(this.p, 300, 0, 300, 200);
				Font textFont = new Font("宋体", 6f);
				for (int m = 0; m < 2; m++)
				{
					if (this.dsoData1.dao[m].bValid)
					{
						g.DrawString(string.Format("CH{1}  Vrms:{0:F2}v", this.dsoData1.dao[m].VRms, m + 1), textFont, this.blackBrush, 1f, (float)(1 + 169 * m));
						g.DrawString(string.Format("+Vp:{0:f2}v", this.dsoData1.dao[m].vp0), textFont, this.blackBrush, 1f, (float)(8 + 169 * m));
						g.DrawString(string.Format("-Vp:{0:f2}v", this.dsoData1.dao[m].vp1), textFont, this.blackBrush, 45f, (float)(8 + 169 * m));
						g.DrawString(string.Format("Vpp:{0:f2}v", this.dsoData1.dao[m].vpp), textFont, this.blackBrush, 90f, (float)(8 + 169 * m));
						string chString;
						if (this.dsoData1.dao[m].ch_value < 1.0)
						{
							chString = (this.dsoData1.dao[m].ch_value * 1000.0).ToString("F0") + "m";
						}
						else if (this.dsoData1.dao[m].ch_value >= 1000000.0)
						{
							chString = (this.dsoData1.dao[m].ch_value / 1000000.0).ToString("F1") + "M";
						}
						else if (this.dsoData1.dao[m].ch_value >= 1000.0)
						{
							chString = (this.dsoData1.dao[m].ch_value / 1000.0).ToString("F1") + "K";
						}
						else
						{
							chString = this.dsoData1.dao[m].ch_value.ToString("F1");
						}
						g.DrawString(string.Format("CH:{0}V", chString), textFont, this.blackBrush, 1f, (float)(15 + 169 * m));
						if (this.dsoData1.timBase < this.timeBaseS.Length)
						{
							g.DrawString(string.Format("M:{0}", this.timeBaseS[this.dsoData1.timBase]), textFont, this.blackBrush, 45f, (float)(15 + 169 * m));
						}
						float frq = this.dsoData1.dao[m].Freq;
						int n = 0;
						for (int j2 = 0; j2 < 2; j2++)
						{
							if (frq > 1000f)
							{
								frq /= 1000f;
								n++;
							}
						}
						g.DrawString(string.Format("Freq:{0:f3}{1}Hz", frq, " KM"[n]), textFont, this.blackBrush, 90f, (float)(15 + 169 * m));
						float per2 = this.dsoData1.dao[m].Period;
						n = 0;
						for (int j3 = 0; j3 < 2; j3++)
						{
							if ((double)per2 < 0.001)
							{
								per2 *= 1000f;
								n++;
							}
						}
						g.DrawString(string.Format("Peri:{0:f3}{1}s", per2, " mun"[n]), textFont, this.blackBrush, 163f, (float)(15 + 169 * m));
					}
				}
				if (this.tempx == 0)
				{
					dc.DrawImage(b, si.X, si.Y, b.Width, b.Height);
					g.Dispose();
				}
				this.tempx = 0;
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00003BA0 File Offset: 0x00001DA0
		private bool bValidFile()
		{
			// PATCHED: the original was a time bomb --
			//     return dt.Year * 365 + dt.Month * 30 < 739550;
			// 2026*365 = 739490, so this went false once Month reached 2, i.e. from
			// February 2026 onwards. In onDraw() a false result pins the trace loop's
			// start index to 1, which silently skips channel 0 -- so CH1 stopped being
			// drawn entirely and the screen looked dead.
			return true;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00003BD8 File Offset: 0x00001DD8
		public void DEMON()
		{
			this.sendCommand(this.serialPort1, 1);
			Thread.Sleep(500);
			for (;;)
			{
				if (this.macheType2 == Form1ShiBoQi.MachineType2.DSO)
				{
					switch (this.dsoMode)
					{
					case Form1ShiBoQi.DSO_MODE.RealtimeData:
						this.sendCommand(this.serialPort1, 3);
						break;
					case Form1ShiBoQi.DSO_MODE.RealTimeDataForAnalyse:
						this.sendCommand(this.serialPort1, 4);
						break;
					}
					Thread.Sleep(1000);
				}
				else if (this.macheType2 == Form1ShiBoQi.MachineType2.DMM)
				{
					switch (this.dmmMode)
					{
					case Form1ShiBoQi.DMM_Mode.DMM_realTimeData:
						this.sendCommand(this.serialPort1, 5);
						break;
					case Form1ShiBoQi.DMM_Mode.AutoRecoreData:
						this.sendCommand(this.serialPort1, 6);
						break;
					case Form1ShiBoQi.DMM_Mode.GET_PRODUCT_MESSAGE:
						this.sendCommand(this.serialPort1, 9);
						this.dmmMode = Form1ShiBoQi.DMM_Mode.GET_PRODUCT_MESSAGE_Finished;
						break;
					}
					Thread.Sleep(500);
				}
				else
				{
					Thread.Sleep(1000);
				}
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00003CC4 File Offset: 0x00001EC4
		private void getData()
		{
			for (;;)
			{
				IL_00:
				Form1ShiBoQi.ret_data data = this.getAData();
				if (!data.overtime && data.ret_data1 == 165)
				{
					data = this.getAData();
					if (!data.overtime)
					{
						byte data2 = data.ret_data1;
						if (data2 != 33)
						{
							if (data2 - 34 <= 7 || data2 == 58)
							{
								if (data2 == 34)
								{
									this.dsoMode = Form1ShiBoQi.DSO_MODE.NULL;
								}
								if (data2 >= 32)
								{
									byte sum = 165;
									sum += data2;
									byte[] ByteLength = new byte[2];
									Form1ShiBoQi.ret_data da;
									for (int i = 0; i < 2; i++)
									{
										da = this.getAData();
										if (da.overtime)
										{
											goto IL_00;
										}
										ByteLength[i] = da.ret_data1;
										sum += ByteLength[i];
									}
									da = this.getAData();
									if (!da.overtime)
									{
										sum += da.ret_data1;
										if (sum == 0)
										{
											ushort length = BitConverter.ToUInt16(ByteLength, 0);
											if (length <= 5000)
											{
												byte[] tempPackage = new byte[(int)(1 + length)];
												tempPackage[0] = data2;
												for (ushort j = 0; j < length; j += 1)
												{
													da = this.getAData();
													if (da.overtime)
													{
														goto IL_00;
													}
													tempPackage[(int)(j + 1)] = da.ret_data1;
													sum += tempPackage[(int)(j + 1)];
												}
												da = this.getAData();
												if (!da.overtime && sum + da.ret_data1 == 0 && !this.holdScreen)
												{
													Queue<byte[]> obj = this.packageQueue;
													lock (obj)
													{
														this.packageQueue.Enqueue(tempPackage);
													}
												}
											}
										}
									}
								}
							}
						}
						else
						{
							Form1ShiBoQi.ret_data data_ = this.getAData();
							if (!data_.overtime)
							{
								byte data3 = data_.ret_data1;
								data_ = this.getAData();
								if (!data_.overtime && data3 + data_.ret_data1 + 165 + data2 == 0)
								{
									byte[] tempPackage = new byte[]
									{
										33,
										data3
									};
									Queue<byte[]> obj = this.packageQueue;
									lock (obj)
									{
										this.packageQueue.Enqueue(tempPackage);
									}
									bool b = false;
									if (data3 == 0)
									{
										if (this.macheType2 != Form1ShiBoQi.MachineType2.DSO || this.dMMToolStripMenuItem.Enabled)
										{
											this.macheType2 = Form1ShiBoQi.MachineType2.DSO;
											this.dsoMode = Form1ShiBoQi.DSO_MODE.RealtimeData;
											b = true;
										}
									}
									else
									{
										this.macheType2 = Form1ShiBoQi.MachineType2.DMM;
										b = true;
									}
									if (b)
									{
										this.bSerialConnected = true;
										this.textBox2.Clear();
										this.textBox2.AppendText("    Record\r\n");
										this.num2 = 0;
										this.numx = 0;
										this.newData = 1;
										if (this.macheType2 == Form1ShiBoQi.MachineType2.DSO)
										{
											this.仪表.Text = "DSO ";
											this.dMM记录ToolStripMenuItem.Enabled = false;
											this.dMMToolStripMenuItem.Checked = false;
											this.dSOToolStripMenuItem1.Enabled = true;
											this.dSOToolStripMenuItem.Enabled = true;
											this.toolStripButton7.Enabled = true;
											this.toolStripButton9.Enabled = false;
											this.toolStripButton4.Enabled = true;
											this.toolStripButton6.Enabled = true;
											this.toolStrip实时波形.Enabled = true;
											if (this.dsoMode != Form1ShiBoQi.DSO_MODE.RealTimeDataForAnalyse && this.dsoMode != Form1ShiBoQi.DSO_MODE.OldDrawingData)
											{
												this.dsoMode = Form1ShiBoQi.DSO_MODE.RealtimeData;
												byte[] array = new byte[2];
												array[0] = 3;
												byte[] by = array;
												this.sendBytes(this.serialPort1, by);
											}
											Form1ShiBoQi.MultimeterType type = Form1ShiBoQi.MultimeterType.DCvoltage;
											this.showLED(0.0, this.ledMin, this.ledMinPoint, this.label5, type, -2);
											this.showLED(0.0, this.ledMax, this.ledMaxPoint, this.label4, type, -2);
											this.showLED(0.0, this.ledCurrent, this.ledCurrentPoint, this.label2, type, -2);
											if (this.dsoMode != Form1ShiBoQi.DSO_MODE.RealTimeDataForAnalyse)
											{
												this.GraphControl1.Visible = false;
											}
											this.timer定时测量.Stop();
											this.textBox2.Text = "";
											this.ToDSOGraphControl();
										}
										else
										{
											this.仪表.Text = "DMM ";
											this.dMM记录ToolStripMenuItem.Enabled = true;
											this.dMMToolStripMenuItem.Checked = true;
											this.dSOToolStripMenuItem1.Enabled = false;
											this.dSOToolStripMenuItem.Enabled = false;
											this.toolStripButton7.Enabled = false;
											this.toolStripButton9.Enabled = true;
											this.toolStripButton4.Enabled = false;
											this.toolStripButton6.Enabled = false;
											this.toolStrip实时波形.Enabled = false;
											this.hScrollBar1.Visible = false;
											this.macheType2 = Form1ShiBoQi.MachineType2.DMM;
											this.dmmMode = Form1ShiBoQi.DMM_Mode.DMM_realTimeData;
											this.comMenu2_Click(null, null);
											this.addToright = false;
											this.ToDMMGraphControl();
										}
										this.newData = 1;
										if (this.bReturn)
										{
											break;
										}
									}
								}
							}
						}
					}
					if (this.bReturn)
					{
						return;
					}
				}
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000041B4 File Offset: 0x000023B4
		private Form1ShiBoQi.ret_data getAData()
		{
			int i = 0;
			while (!this.serialPort1.IsOpen || this.serialPort1.BytesToRead == 0)
			{
				i++;
				Thread.Sleep(1);
				if (i >= 100)
				{
					Form1ShiBoQi.ret_data data;
					data.overtime = true;
					data.ret_data1 = 0;
					return data;
				}
			}
			this.vacancyTimes = 0;
			Form1ShiBoQi.ret_data data2;
			data2.overtime = false;
			data2.ret_data1 = (byte)this.serialPort1.ReadByte();
			return data2;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00004224 File Offset: 0x00002424
		private void ToDSOGraphControl()
		{
			GraphPane myPane = this.GraphControl1.GraphPane;
			myPane.Title.Text = "DMM RealTime Data";
			myPane.XAxis.Title.Text = "Time";
			myPane.XAxis.Color = Color.White;
			myPane.YAxis.Title.Text = "Data";
			myPane.XAxis.Color = Color.White;
			this.GraphControl1.GraphPane.XAxis.IsAxisSegmentVisible = true;
			this.GraphControl1.GraphPane.XAxis.Color = Color.YellowGreen;
			myPane.CurveList.Clear();
			this.myCurve = myPane.AddCurve("RealTime Data 1", new PointPairList(), Color.Red, SymbolType.None);
			this.myCurveYellow = myPane.AddCurve("RealTime Data 2", new PointPairList(), Color.Yellow, SymbolType.None);
			this.myCurve.Symbol.Fill = new Fill(Color.Black);
			this.myCurveYellow.Symbol.Fill = new Fill(Color.Black);
			myPane.Chart.Fill = new Fill(Color.Black, Color.Black, 45f);
			myPane.Fill = new Fill(Color.Black, Color.Black, 45f);
			this.GraphControl1.AxisChange();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00004384 File Offset: 0x00002584
		private void ToDMMGraphControl()
		{
			GraphPane myPane = this.GraphControl1.GraphPane;
			myPane.XAxis.Type = AxisType.Linear;
			myPane.Title.Text = "DMM RealTime Data";
			myPane.XAxis.Title.Text = "Time";
			myPane.XAxis.Color = Color.White;
			myPane.YAxis.Title.Text = "Data";
			myPane.XAxis.Color = Color.White;
			this.GraphControl1.GraphPane.XAxis.IsAxisSegmentVisible = true;
			this.GraphControl1.GraphPane.XAxis.Color = Color.YellowGreen;
			myPane.CurveList.Clear();
			this.myCurve = myPane.AddCurve("RealTime Data 1", new PointPairList(), Color.Red, SymbolType.None);
			this.myCurve.Symbol.Fill = new Fill(Color.Black);
			this.myCurveYellow.Symbol.Fill = new Fill(Color.Black);
			myPane.Chart.Fill = new Fill(Color.Black, Color.Black, 45f);
			myPane.Fill = new Fill(Color.Black, Color.Black, 45f);
			this.GraphControl1.AxisChange();
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000044D4 File Offset: 0x000026D4
		private string[] handleNumber(double num, Form1ShiBoQi.MultimeterType type, int dotPosition)
		{
			string[] str = new string[2];
			switch (type)
			{
			case Form1ShiBoQi.MultimeterType.ACvoltage:
			case Form1ShiBoQi.MultimeterType.dinode:
			case Form1ShiBoQi.MultimeterType.DCvoltage:
				str[0] = "V";
				break;
			case Form1ShiBoQi.MultimeterType.ACcurrent:
			case Form1ShiBoQi.MultimeterType.DCcurrent:
				str[0] = "A";
				break;
			case Form1ShiBoQi.MultimeterType.frequency:
				str[0] = "Hz";
				break;
			case Form1ShiBoQi.MultimeterType.resistance:
			case Form1ShiBoQi.MultimeterType.FengMen:
				str[0] = "Ω";
				break;
			case Form1ShiBoQi.MultimeterType.Lx:
				str[0] = "H";
				break;
			case Form1ShiBoQi.MultimeterType.Cx:
				str[0] = "F";
				break;
			case Form1ShiBoQi.MultimeterType.Percent:
				str[0] = "%";
				break;
			}
			if (num > 1000000000.0)
			{
				str[1] = "over";
				if (type == Form1ShiBoQi.MultimeterType.resistance)
				{
					str[0] = "M" + str[0];
				}
				return str;
			}
			double num2 = Math.Log10(Math.Abs(num)) / 3.0;
			int exp = (int)num2;
			if (num2 < 0.0)
			{
				exp--;
			}
			if (exp > -5 && exp < 100)
			{
				num /= Math.Pow(10.0, (double)(exp * 3));
				double num3 = Math.Abs(num);
				int x = (int)num3;
				if (num3 - (double)x > 0.999999)
				{
					if (num > 0.0)
					{
						num = (double)((int)num + 1);
					}
					else
					{
						num = (double)((int)num - 1);
					}
				}
				switch (exp)
				{
				case -4:
					str[0] = "p" + str[0];
					break;
				case -3:
					str[0] = "n" + str[0];
					break;
				case -2:
					str[0] = "μ" + str[0];
					break;
				case -1:
					str[0] = "m" + str[0];
					break;
				case 0:
					break;
				case 1:
					str[0] = "k" + str[0];
					break;
				case 2:
					str[0] = "M" + str[0];
					break;
				default:
					str[0] = "m" + str[0];
					break;
				}
			}
			else
			{
				switch (type)
				{
				case Form1ShiBoQi.MultimeterType.ACvoltage:
				case Form1ShiBoQi.MultimeterType.dinode:
				case Form1ShiBoQi.MultimeterType.DCvoltage:
					str[0] = "mV";
					break;
				case Form1ShiBoQi.MultimeterType.ACcurrent:
				case Form1ShiBoQi.MultimeterType.DCcurrent:
					str[0] = "mA";
					break;
				case Form1ShiBoQi.MultimeterType.frequency:
					str[0] = "Hz";
					break;
				case Form1ShiBoQi.MultimeterType.resistance:
					str[0] = "mΩ";
					break;
				case Form1ShiBoQi.MultimeterType.Lx:
					str[0] = "uH";
					break;
				case Form1ShiBoQi.MultimeterType.Cx:
					str[0] = "uF";
					break;
				}
			}
			if (dotPosition < 0)
			{
				if (num < 10.0)
				{
					str[1] = num.ToString("F3", CultureInfo.InvariantCulture);
				}
				else if (num < 100.0)
				{
					str[1] = num.ToString("F2", CultureInfo.InvariantCulture);
				}
				else if (num < 1000.0)
				{
					str[1] = num.ToString("F1", CultureInfo.InvariantCulture);
				}
				else
				{
					str[1] = num.ToString("F0", CultureInfo.InvariantCulture);
				}
			}
			else
			{
				str[1] = num.ToString("F" + dotPosition.ToString(), CultureInfo.InvariantCulture);
			}
			if (type == Form1ShiBoQi.MultimeterType.dinode && num > 1000.0)
			{
				str[0] = "V";
				str[1] = "F";
			}
			return str;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000047EC File Offset: 0x000029EC
		private void initalizeLED()
		{
			this.axLED13.setNum(10);
			this.ledMin[0] = this.axLED8;
			this.ledMin[1] = this.axLED5;
			this.ledMin[2] = this.axLED7;
			this.ledMin[3] = this.axLED6;
			this.initLEDs(this.ledMin);
			this.ledMinPoint[0] = this.pictureBox6;
			this.ledMinPoint[1] = this.pictureBox4;
			this.ledMinPoint[2] = this.pictureBox7;
			this.ledMinPoint[3] = this.pictureBox5;
			this.ledMax[0] = this.axLED12;
			this.ledMax[1] = this.axLED11;
			this.ledMax[2] = this.axLED10;
			this.ledMax[3] = this.axLED9;
			this.initLEDs(this.ledMax);
			this.ledMaxPoint[0] = this.pictureBox11;
			this.ledMaxPoint[1] = this.pictureBox10;
			this.ledMaxPoint[2] = this.pictureBox9;
			this.ledMaxPoint[3] = this.pictureBox8;
			this.ledCurrent[0] = this.axLED1;
			this.ledCurrent[1] = this.axLED2;
			this.ledCurrent[2] = this.axLED4;
			this.ledCurrent[3] = this.axLED3;
			this.initLEDs(this.ledCurrent);
			this.ledCurrentPoint[0] = this.pictureBox12;
			this.ledCurrentPoint[1] = this.pictureBox2;
			this.ledCurrentPoint[2] = this.pictureBox3;
			this.ledCurrentPoint[3] = this.pictureBox1;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000497C File Offset: 0x00002B7C
		private void initLEDs(LED[] axLED1)
		{
			int i = axLED1.Length;
			for (int j = 0; j < i; j++)
			{
				axLED1[j].setFrontColor(0, 0, 0);
				axLED1[j].setBckColor(135, 206, 250);
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000049BC File Offset: 0x00002BBC
		private string showLED(double number, LED[] led, PictureBox[] box, System.Windows.Forms.Label lab, Form1ShiBoQi.MultimeterType type, int dotPosition)
		{
			if (number < 0.0)
			{
				box[3].Show();
				number = -number;
			}
			else
			{
				box[3].Hide();
			}
			string[] str = this.handleNumber(number, type, dotPosition);
			if (dotPosition != -2 && str[1] == "over")
			{
				byte[] data = new byte[]
				{
					12,
					0,
					11,
					12
				};
				for (int i = 0; i < 4; i++)
				{
					led[i].setNum((short)data[i]);
				}
				for (int j = 0; j < 3; j++)
				{
					box[j].BackColor = this.backColor;
				}
				this.label2.Text = str[0];
				return "over";
			}
			lab.Text = str[0];
			if (str[1] != "F")
			{
				short[] data2 = new short[4];
				int k = 4;
				int l = 0;
				if (dotPosition >= 0)
				{
					l = 4 - str[1].Length;
					if (str[1].Contains("."))
					{
						l++;
					}
				}
				foreach (char ch in str[1])
				{
					if (ch == '.')
					{
						k = l - 1;
					}
					else if (l < 5)
					{
						data2[l++] = (short)(ch - '0');
					}
				}
				if (k != 0)
				{
					int m = 0;
					while (m < 4 && data2[m] == 0)
					{
						data2[0] = 12;
						m++;
					}
				}
				for (int n = 0; n < 4; n++)
				{
					if (data2[n] >= 14)
					{
						data2[n] = 0;
					}
					led[n].setNum(data2[n]);
				}
				for (int i2 = 0; i2 < 3; i2++)
				{
					if (i2 == k)
					{
						box[i2].Show();
					}
					else
					{
						box[i2].Hide();
					}
				}
			}
			else
			{
				led[0].setNum(0);
				led[1].setNum(11);
				led[2].setNum(12);
				led[3].setNum(12);
				for (int i3 = 0; i3 < 3; i3++)
				{
					box[i3].Hide();
				}
			}
			return str[1] + str[0];
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00004BC4 File Offset: 0x00002DC4
		private void addMultimeterData(Form1ShiBoQi.MultimeterData curData)
		{
			if (this.minMultimeterData.tail.Length > 0 && curData.tail.Length > 0 && this.minMultimeterData.tail[this.minMultimeterData.tail.Length - 1] != curData.tail[curData.tail.Length - 1])
			{
				this.newData = 1;
			}
			if (this.newData == 1)
			{
				if (curData.bValid)
				{
					this.newData = 0;
					this.minMultimeterData.toEqual(curData);
					this.maxMultimeterData.toEqual(curData);
					this.minMultimeterData.showLED(this.ledMax, this.ledMaxPoint, this.label4);
					this.maxMultimeterData.showLED(this.ledMin, this.ledMinPoint, this.label5);
				}
				else
				{
					this.minMultimeterData.data = 0;
					this.maxMultimeterData.data = 0;
				}
				this.minMultimeterData.tail = (string)curData.tail.Clone();
				this.maxMultimeterData.tail = (string)curData.tail.Clone();
			}
			else if (this.minMultimeterData.Comparer(curData) == Form1ShiBoQi.compareResult.bigger)
			{
				this.minMultimeterData.toEqual(curData);
				this.minMultimeterData.showLED(this.ledMax, this.ledMaxPoint, this.label4);
			}
			else if (this.maxMultimeterData.Comparer(curData) == Form1ShiBoQi.compareResult.smaller)
			{
				this.maxMultimeterData.toEqual(curData);
				this.maxMultimeterData.showLED(this.ledMin, this.ledMinPoint, this.label5);
			}
			string str = curData.toString();
			string type;
			switch (curData.m_type)
			{
			case Form1ShiBoQi.MultimeterType.ACvoltage:
			case Form1ShiBoQi.MultimeterType.ACcurrent:
				type = "AC";
				goto IL_1F0;
			case Form1ShiBoQi.MultimeterType.DCvoltage:
			case Form1ShiBoQi.MultimeterType.DCcurrent:
				type = "DC";
				goto IL_1F0;
			case Form1ShiBoQi.MultimeterType.Cx:
				type = "Cap";
				goto IL_1F0;
			}
			type = "";
			IL_1F0:
			string obj = this.textbox2string;
			lock (obj)
			{
				this.textbox2string = type + " " + str;
			}
			curData.showLED(this.ledCurrent, this.ledCurrentPoint, this.label2);
			this.maxMultimeterData.showLED(this.ledMin, this.ledMinPoint, this.label5);
			this.minMultimeterData.showLED(this.ledMax, this.ledMaxPoint, this.label4);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00004E54 File Offset: 0x00003054
		public static string FunToString(Form1ShiBoQi.FUC f)
		{
			switch (f)
			{
			case Form1ShiBoQi.FUC.dcvoltage:
				return "DC";
			case Form1ShiBoQi.FUC.acVoltage:
				return "AC";
			case Form1ShiBoQi.FUC.Res:
				return "Res";
			case Form1ShiBoQi.FUC.cont:
				return "Cont";
			case Form1ShiBoQi.FUC.diode:
				return "Diode";
			case Form1ShiBoQi.FUC.capacity:
				return "Cap";
			case Form1ShiBoQi.FUC.dcMa:
				return "DC";
			case Form1ShiBoQi.FUC.acma:
				return "AC";
			case Form1ShiBoQi.FUC.DCA:
				return "DC";
			case Form1ShiBoQi.FUC.ACA:
				return "AC";
			case Form1ShiBoQi.FUC.HZ:
				return "Freq";
			case Form1ShiBoQi.FUC.duty:
				return "Duty";
			}
			return "";
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00004F10 File Offset: 0x00003110
		private void forDMM_realTimeData(byte[] by)
		{
			this.addElement = true;
			Form1ShiBoQi.DMM_RECORD dr = new Form1ShiBoQi.DMM_RECORD(by.Skip(1).ToArray<byte>(), 0);
			this.currentLedValue = dr.getValue();
			if (dr.Function != this.oldFunc)
			{
				this.minLedVale = (this.maxLedValue = this.currentLedValue);
			}
			else if (this.currentLedValue > this.maxLedValue)
			{
				this.maxLedValue = this.currentLedValue;
			}
			else if (this.currentLedValue < this.minLedVale)
			{
				this.minLedVale = this.currentLedValue;
			}
			this.oldFunc = dr.Function;
			Form1ShiBoQi.MultimeterData currentData = new Form1ShiBoQi.MultimeterData();
			currentData.data = dr.value;
			currentData.floatValue = dr.fvalue;
			currentData.VBase = dr.Vbase;
			currentData.bValid = (dr.rangeOUt == 0);
			currentData.pointNum = (int)(4 - dr.PointNum);
			if (currentData.data < 0)
			{
				if (currentData.m_type == Form1ShiBoQi.MultimeterType.frequency)
				{
					return;
				}
				currentData.plus = false;
			}
			else
			{
				currentData.plus = true;
			}
			switch (dr.Function)
			{
			case Form1ShiBoQi.FUC.dcvoltage:
				currentData.frontType = Form1ShiBoQi.FrontType.DC;
				currentData.m_type = Form1ShiBoQi.MultimeterType.DCvoltage;
				if (dr.range == 0)
				{
					currentData.tail = "mV";
				}
				else
				{
					currentData.tail = "V";
				}
				break;
			case Form1ShiBoQi.FUC.acVoltage:
				currentData.frontType = Form1ShiBoQi.FrontType.ac;
				currentData.m_type = Form1ShiBoQi.MultimeterType.ACvoltage;
				if (dr.range == 0)
				{
					currentData.tail = "mV";
				}
				else
				{
					currentData.tail = "V";
				}
				break;
			case Form1ShiBoQi.FUC.Res:
				currentData.frontType = Form1ShiBoQi.FrontType.RES;
				currentData.m_type = Form1ShiBoQi.MultimeterType.resistance;
				switch (dr.range)
				{
				case 0:
					currentData.tail = "Ω";
					break;
				case 1:
					currentData.tail = "KΩ";
					break;
				case 2:
					currentData.tail = "KΩ";
					break;
				case 3:
					currentData.tail = "KΩ";
					break;
				case 4:
					currentData.tail = "MΩ";
					break;
				case 5:
					currentData.tail = "MΩ";
					break;
				}
				break;
			case Form1ShiBoQi.FUC.cont:
				currentData.frontType = Form1ShiBoQi.FrontType.fengmi;
				currentData.m_type = Form1ShiBoQi.MultimeterType.FengMen;
				currentData.tail = "Ω";
				break;
			case Form1ShiBoQi.FUC.diode:
				currentData.frontType = Form1ShiBoQi.FrontType.dinode;
				currentData.m_type = Form1ShiBoQi.MultimeterType.dinode;
				currentData.tail = "V";
				break;
			case Form1ShiBoQi.FUC.capacity:
				currentData.frontType = Form1ShiBoQi.FrontType.capcity;
				currentData.m_type = Form1ShiBoQi.MultimeterType.Cx;
				switch (dr.range)
				{
				case 0:
					currentData.tail = "nF";
					break;
				case 1:
					currentData.tail = "uF";
					break;
				case 2:
					currentData.tail = "uF";
					break;
				case 3:
					currentData.tail = "mF";
					break;
				}
				break;
			case Form1ShiBoQi.FUC.dcMa:
				currentData.frontType = Form1ShiBoQi.FrontType.DC;
				currentData.m_type = Form1ShiBoQi.MultimeterType.DCcurrent;
				currentData.tail = "mA";
				break;
			case Form1ShiBoQi.FUC.acma:
				currentData.frontType = Form1ShiBoQi.FrontType.ac;
				currentData.m_type = Form1ShiBoQi.MultimeterType.ACcurrent;
				currentData.tail = "mA";
				break;
			case Form1ShiBoQi.FUC.DCA:
				currentData.frontType = Form1ShiBoQi.FrontType.DC;
				currentData.m_type = Form1ShiBoQi.MultimeterType.DCcurrent;
				currentData.tail = "A";
				break;
			case Form1ShiBoQi.FUC.ACA:
				currentData.frontType = Form1ShiBoQi.FrontType.ac;
				currentData.m_type = Form1ShiBoQi.MultimeterType.ACcurrent;
				currentData.tail = "A";
				break;
			case Form1ShiBoQi.FUC.HZ:
				currentData.frontType = Form1ShiBoQi.FrontType.nullType;
				currentData.m_type = Form1ShiBoQi.MultimeterType.frequency;
				if (dr.Vbase == 1000000f)
				{
					currentData.tail = "MHz";
				}
				else if (dr.Vbase == 1000f)
				{
					currentData.tail = "KHz";
				}
				else
				{
					currentData.tail = "Hz";
				}
				break;
			case Form1ShiBoQi.FUC.duty:
				currentData.frontType = Form1ShiBoQi.FrontType.duty;
				currentData.m_type = Form1ShiBoQi.MultimeterType.Percent;
				currentData.tail = "%";
				break;
			}
			if (currentData.frontType != this.oldMultiMachintType || currentData.tail != this.oldTail)
			{
				this.newData = 1;
				this.comMenu2_Click(null, null);
			}
			this.oldMultiMachintType = currentData.frontType;
			this.oldTail = currentData.tail;
			this.addMultimeterData(currentData);
			this.currentMultimeterData = currentData;
			if (currentData.frontType != Form1ShiBoQi.FrontType.percent)
			{
				if (currentData.auto_mode)
				{
					this.AutoMode.Text = "Auto";
				}
				else
				{
					this.AutoMode.Text = "Manu";
				}
			}
			else
			{
				this.AutoMode.Text = "    ";
			}
			switch (currentData.frontType)
			{
			case Form1ShiBoQi.FrontType.dinode:
				this.axLED13.setNum(10);
				break;
			case Form1ShiBoQi.FrontType.fengmi:
				this.axLED13.setNum(13);
				break;
			case Form1ShiBoQi.FrontType.nullType:
			case Form1ShiBoQi.FrontType.DC:
				this.axLED13.setNum(12);
				break;
			case Form1ShiBoQi.FrontType.percent:
				this.axLED13.setNum(15);
				break;
			case Form1ShiBoQi.FrontType.ac:
				this.axLED13.setNum(14);
				break;
			case Form1ShiBoQi.FrontType.duty:
				this.axLED13.drawSpace();
				break;
			case Form1ShiBoQi.FrontType.capcity:
				this.axLED13.setCapacity();
				break;
			case Form1ShiBoQi.FrontType.RES:
				this.axLED13.drawSpace();
				break;
			}
			float temp = 100000f;
			if (!this.oldMultiMachineData.getSameType(currentData))
			{
				this.myCurve.Clear();
				Scale scale3 = this.GraphControl1.GraphPane.XAxis.Scale;
				scale3.Min = 0.0;
				scale3.MinAuto = true;
				scale3.MaxAuto = true;
				scale3.MajorStep = 2.0;
				scale3.MinorStep = 1.0;
				this.textBox2.Text = "";
				this.num2 = 0;
				this.numx = 0;
				float scale = currentData.getMaxScaleWithoutUnit();
				this.maxscale = scale;
				if (currentData.m_type != Form1ShiBoQi.MultimeterType.DCcurrent)
				{
					Form1ShiBoQi.MultimeterType type = currentData.m_type;
				}
				this.GraphControl1.GraphPane.YAxis.Scale.Min = (double)this.minLedVale * 0.3;
				this.GraphControl1.GraphPane.YAxis.Scale.Max = (double)this.maxLedValue * 1.5 + 1.0;
				this.GraphControl1.GraphPane.YAxis.Scale.MaxAuto = true;
				this.GraphControl1.GraphPane.YAxis.Scale.MinAuto = true;
				this.oldScale = 0f;
				this.addElementToDrawing();
			}
			else
			{
				float scale2 = currentData.getMaxScaleWithoutUnit();
				this.maxscale = scale2;
				if (temp > this.oldScale)
				{
					float mindcale;
					if (currentData.m_type == Form1ShiBoQi.MultimeterType.DCcurrent || currentData.m_type == Form1ShiBoQi.MultimeterType.DCvoltage)
					{
						mindcale = -scale2;
					}
					else
					{
						mindcale = 0f;
					}
					this.GraphControl1.GraphPane.YAxis.Scale.Min = (double)mindcale;
					this.GraphControl1.GraphPane.YAxis.Scale.MaxAuto = true;
					this.oldScale = temp;
				}
				this.addElementToDrawing();
			}
			this.GraphControl1.GraphPane.XAxis.Scale.MaxAuto = true;
			float minStep = (float)(this.num2 / 10) / 5f;
			if (minStep < 1f)
			{
			}
			this.GraphControl1.GraphPane.YAxis.Scale.Min = (double)this.minLedVale;
			this.GraphControl1.GraphPane.YAxis.Scale.Max = (double)this.maxLedValue * 1.2 + 1.0;
			this.oldMultiMachineData = currentData;
			this.GraphControl1.AxisChange();
			this.GraphControl1.Invalidate();
			this.dataSaved = false;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000056D8 File Offset: 0x000038D8
		private void forDSODataForAnalyse(byte[] by)
		{
			if (by.Length != 4254 || by[1] == 0)
			{
				return;
			}
			Form1ShiBoQi.DSO_DATAS data = new Form1ShiBoQi.DSO_DATAS();
			data.dao[0].bValid = (by[1] == 1 || by[1] == 3);
			data.dao[1].bValid = (by[1] == 2 || by[1] == 3);
			for (int i = 0; i < 2; i++)
			{
				if (data.dao[i].bValid)
				{
					data.dao[i].dso_Data = new Point[2048];
					data.dao[i].TD = new COMPLEX[2048];
					for (int j = 0; j < 2048; j++)
					{
						data.dao[i].dso_Data[j].X = j;
						data.dao[i].dso_Data[j].Y = (int)by[2 + j + 2048 * i];
					}
				}
			}
			by = by.Skip(4098).ToArray<byte>();
			data.channel = (Form1ShiBoQi.Channel)by[0];
			data.timBase = (int)by[2];
			data.Channel_processing = (int)by[1];
			if (data.dao[0].bValid)
			{
				byte[] ch0 = by.Skip(10).Take(73).ToArray<byte>();
				this.getChannelData(ch0, data.dao[0]);
			}
			if (data.dao[1].bValid)
			{
				byte[] ch = by.Skip(83).Take(73).ToArray<byte>();
				this.getChannelData(ch, data.dao[1]);
			}
			Form1ShiBoQi.DSO_DATAS obj = this.dsoData1;
			lock (obj)
			{
				this.dsoData1 = data;
			}
			double[] temp = new double[]
			{
				this.voltaBaseF[data.dao[0].ch],
				this.voltaBaseF[data.dao[1].ch]
			};
			int leng = 2048;
			for (int k = 0; k < 2; k++)
			{
				if (data.dao[k].bValid)
				{
					for (int l = 0; l < leng; l++)
					{
						data.dao[k].TD[l].re = (double)data.dao[k].dso_Data[l].Y * temp[k];
						data.dao[k].TD[l].im = 0.0;
					}
				}
			}
			for (int m = 0; m < 2; m++)
			{
				if (data.dao[m].bValid)
				{
					switch (this.currentWindowMode)
					{
					case Form1ShiBoQi.DSOWindowMode.Hanning:
					{
						double d0 = (double)((float)(6.2831853071795862 / (double)(leng - 1)));
						for (int n = 0; n < leng; n++)
						{
							COMPLEX[] td = data.dao[m].TD;
							int num = n;
							td[num].re = td[num].re * (double)((float)(0.5 * (1.0 - Math.Cos(d0 * (double)n))));
						}
						break;
					}
					case Form1ShiBoQi.DSOWindowMode.Hamming:
					{
						double d = (double)((float)(6.2831853071795862 / (double)(leng - 1)));
						for (int i2 = 0; i2 < leng; i2++)
						{
							COMPLEX[] td2 = data.dao[m].TD;
							int num2 = i2;
							td2[num2].re = td2[num2].re * (double)((float)(0.53836 - 0.46164 * Math.Cos(d * (double)i2)));
						}
						break;
					}
					case Form1ShiBoQi.DSOWindowMode.Blackman:
					{
						double d2 = (double)((float)(6.2831853071795862 / (double)(leng - 1)));
						for (int i3 = 0; i3 < leng; i3++)
						{
							COMPLEX[] td3 = data.dao[m].TD;
							int num3 = i3;
							td3[num3].re = td3[num3].re * (double)((float)(0.42 - 0.5 * Math.Cos(d2 * (double)i3) + 0.08 * Math.Cos(2.0 * d2 * (double)i3)));
						}
						break;
					}
					case Form1ShiBoQi.DSOWindowMode.Flattop:
					{
						double d3 = (double)((float)(6.2831853071795862 / (double)(leng - 1)));
						for (int i4 = 0; i4 < leng; i4++)
						{
							COMPLEX[] td4 = data.dao[m].TD;
							int num4 = i4;
							td4[num4].re = td4[num4].re * (double)((float)(1.0 - 1.93 * Math.Cos(d3 * (double)i4) + 1.29 * Math.Cos(2.0 * d3 * (double)i4) - 0.388 * Math.Cos(3.0 * d3 * (double)i4) + 0.0032 * Math.Cos(4.0 * d3 * (double)i4)));
						}
						break;
					}
					case Form1ShiBoQi.DSOWindowMode.Bartlett:
					{
						double d4 = (double)((float)(leng - 1) / 2f);
						for (int i5 = 0; i5 < leng; i5++)
						{
							COMPLEX[] td5 = data.dao[m].TD;
							int num5 = i5;
							td5[num5].re = td5[num5].re * ((d4 - Math.Abs((double)i5 - d4)) / d4);
						}
						break;
					}
					}
				}
			}
			int len = (int)Math.Log((double)leng, 2.0);
			leng = (int)Math.Pow(2.0, (double)len);
			double[] data_ = new double[leng];
			double[] data2 = new double[leng];
			if (data.dao[0].bValid)
			{
				for (int i6 = 0; i6 < leng; i6++)
				{
					data_[i6] = data.dao[0].TD[i6].re;
				}
			}
			if (data.dao[1].bValid)
			{
				for (int i7 = 0; i7 < leng; i7++)
				{
					data2[i7] = data.dao[1].TD[i7].re;
				}
			}
			double[] data_im = new double[leng];
			double[] data_im2 = new double[leng];
			double[] fr = new double[leng];
			double[] fi = new double[leng];
			double[] fr2 = new double[leng];
			double[] fi2 = new double[leng];
			if (data.dao[0].bValid)
			{
				this.kbfft(data_, data_im, leng, len, fr, fi, 0, 1);
			}
			if (data.dao[1].bValid)
			{
				this.kbfft(data2, data_im2, leng, len, fr2, fi2, 0, 1);
			}
			double[] obj2 = data_;
			lock (obj2)
			{
				GraphPane myPane = this.GraphControl1.GraphPane;
				this.myCurve.Clear();
				this.myCurveYellow.Clear();
				myPane.Title.Text = "DSO RealTime Data";
				myPane.YAxis.Title.Text = "Volt,V";
				myPane.XAxis.Title.Text = "Frequency,Hz";
				this.myCurve.Label.Text = "RealTime Data";
				if (this.dsoXLinearMode == Form1ShiBoQi.DSOXLinearMode.Linear)
				{
					myPane.XAxis.Type = AxisType.Linear;
				}
				else
				{
					myPane.XAxis.Type = AxisType.Log;
				}
				this.GraphControl1.GraphPane.YAxis.Scale.MaxAuto = true;
				double xIncData = 1.0 / (this.timeBaseF[data.timBase] * (double)this.calibration_ratio_x);
				double y_ratio = (double)this.calibration_ratio_y * this.voltaBaseF[data.dao[1].ch];
				double y_ratio2 = (double)this.calibration_ratio_y * this.voltaBaseF[data.dao[0].ch];
				if (data.dao[0].bValid)
				{
					for (int i8 = 1; i8 < leng / 3; i8++)
					{
						this.myCurve.AddPoint((double)i8 * xIncData, data_[i8] * y_ratio2);
					}
				}
				if (data.dao[1].bValid)
				{
					for (int i9 = 1; i9 < leng / 3; i9++)
					{
						this.myCurveYellow.AddPoint((double)i9 * xIncData, data2[i9] * y_ratio);
					}
				}
				this.GraphControl1.Dock = DockStyle.Fill;
				this.GraphControl1.Visible = true;
				this.GraphControl1.GraphPane.YAxis.Scale.Min = 0.0;
				this.GraphControl1.GraphPane.XAxis.Scale.MaxAuto = true;
				this.GraphControl1.GraphPane.XAxis.Scale.MinAuto = true;
				this.GraphControl1.GraphPane.XAxis.Scale.MajorStepAuto = true;
				this.GraphControl1.GraphPane.XAxis.Scale.MinorStepAuto = true;
				this.GraphControl1.AxisChange();
				this.panel4.Visible = false;
			}
			this.GraphControl1.Invalidate();
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000022B7 File Offset: 0x000004B7
		private void forOldDraw(byte[] by)
		{
			new Form1ShiBoQi.DSO_DATAS();
			by = by.Skip(4).ToArray<byte>();
			this.forDSORealTimeData(by);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00005FF0 File Offset: 0x000041F0
		private void getChannelData(byte[] data, Form1ShiBoQi.DSO_DATA ds)
		{
			ds.ch = (int)data[0];
			ds.ch_value = this.voltaBaseF[ds.ch] * Math.Pow(10.0, (double)data[4]);
			ds.VRms = BitConverter.ToSingle(data, 25);
			ds.Period = BitConverter.ToSingle(data, 45);
			ds.vp0 = BitConverter.ToSingle(data, 37);
			ds.vp1 = BitConverter.ToSingle(data, 41);
			ds.vpp = BitConverter.ToSingle(data, 29);
			ds.Freq = BitConverter.ToSingle(data, 49);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00006080 File Offset: 0x00004280
		private void forDSORealTimeData(byte[] by)
		{
			Form1ShiBoQi.DSO_DATAS data = new Form1ShiBoQi.DSO_DATAS();
			data.dao[0].bValid = (by[1] == 1 || by[1] == 3);
			data.dao[1].bValid = (by[1] == 2 || by[1] == 3);
			for (int i = 0; i < 2; i++)
			{
				if (data.dao[i].bValid)
				{
					data.dao[i].dso_Data = new Point[600];
					for (int j = 0; j < 300; j++)
					{
						data.dao[i].dso_Data[2 * j].X = j;
						data.dao[i].dso_Data[2 * j + 1].X = j;
						data.dao[i].dso_Data[2 * j].Y = (int)by[2 + 2 * j + 600 * i];
						data.dao[i].dso_Data[2 * j + 1].Y = (int)by[2 + 2 * j + 1 + 600 * i];
					}
				}
			}
			by = by.Skip(1202).ToArray<byte>();
			data.channel = (Form1ShiBoQi.Channel)by[0];
			data.timBase = (int)by[2];
			data.Channel_processing = (int)by[1];
			if (data.dao[0].bValid)
			{
				byte[] ch0 = by.Skip(10).Take(73).ToArray<byte>();
				this.getChannelData(ch0, data.dao[0]);
			}
			if (data.dao[1].bValid)
			{
				byte[] ch = by.Skip(83).Take(73).ToArray<byte>();
				this.getChannelData(ch, data.dao[1]);
			}
			Form1ShiBoQi.DSO_DATAS obj = this.dsoData1;
			lock (obj)
			{
				this.dsoData1 = data;
			}
			this.panel4.Dock = DockStyle.Fill;
			this.panel4.Visible = true;
			this.panel4.Invalidate();
		}

		// Token: 0x06000021 RID: 33 RVA: 0x0000629C File Offset: 0x0000449C
		private void PackageHandleThread()
		{
			for (;;)
			{
				if (this.packageQueue.Count > 0)
				{
					Queue<byte[]> obj = this.packageQueue;
					byte[] by;
					lock (obj)
					{
						if (this.packageQueue.Count <= 0)
						{
							continue;
						}
						by = this.packageQueue.Dequeue();
						goto IL_4F;
					}
					goto IL_47;
					IL_4F:
					if (by.Length <= 2)
					{
						continue;
					}
					byte[] obj2 = this.saveData;
					lock (obj2)
					{
						this.saveData = (by.Clone() as byte[]);
					}
					byte b = by[0];
					switch (b)
					{
					case 34:
						this.forOldDraw(by);
						continue;
					case 35:
						this.forDSORealTimeData(by);
						continue;
					case 36:
						this.forDSODataForAnalyse(by);
						continue;
					case 37:
						this.forDMM_realTimeData(by);
						continue;
					case 38:
						this.forDMMAutoRecordHoldRecord(by);
						continue;
					case 39:
						this.forDMMAutoRecordHoldRecord(by);
						continue;
					case 40:
						this.forCalibrationData(by);
						continue;
					case 41:
						this.forProductionMessage(by);
						continue;
					default:
						if (b != 160)
						{
							continue;
						}
						continue;
					}
				}
				IL_47:
				Thread.Sleep(1);
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000063E8 File Offset: 0x000045E8
		public string[] getSerialPorts()
		{
			List<string> ls = new List<string>();
			RegistryKey key = Registry.LocalMachine;
			key = key.OpenSubKey("HARDWARE\\DEVICEMAP\\SERIALCOMM");
			foreach (string port in key.GetValueNames())
			{
				if (port.Contains("USBSER"))
				{
					string s = key.GetValue(port).ToString();
					ls.Add(s);
				}
			}
			key.Close();
			return ls.ToArray();
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00006458 File Offset: 0x00004658
		public Form1ShiBoQi()
		{
			Control.CheckForIllegalCrossThreadCalls = false;
			this.InitializeComponent();
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			base.UpdateStyles();
			this.timer定时测量.Elapsed += new ElapsedEventHandler(this.OnTimer定时测量);
			this.timer定时测量.Interval = 1000.0;
			this.timer定时测量.Enabled = false;
			this.panel4.Dock = DockStyle.Fill;
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Class\\{4D36E978-E325-11CE-BFC1-08002BE10318}", false);
			if (registryKey == null)
			{
				MessageBox.Show("can NOT find the serialport driver,Please install it first");
				base.Close();
			}
			registryKey.Close();
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00006C00 File Offset: 0x00004E00
		private void getParaFromReg()
		{
			IniRW rw = ClassINI.getIni();
			this.macheType2 = Form1ShiBoQi.MachineType2.DSO;
			this.dmmMode = (Form1ShiBoQi.DMM_Mode)rw.ReadInt("0", "DmmMode");
			this.dsoMode = (Form1ShiBoQi.DSO_MODE)rw.ReadInt("0", "DSOMode");
			this.dsoXLinearMode = (Form1ShiBoQi.DSOXLinearMode)rw.ReadInt("0", "dsoXLineMode");
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00006C5C File Offset: 0x00004E5C
		private void writeParaToReg()
		{
			IniRW ini = ClassINI.getIni();
			ini.WriteValue("0", "DmmMode", (int)this.dmmMode);
			ini.WriteValue("0", "DSOMode", (int)this.dsoMode);
			ini.WriteValue("0", "dsoXLineMode", (int)this.dsoXLinearMode);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00006CC4 File Offset: 0x00004EC4
		private void getAllPointsSizes()
		{
			this.panel1Point.X = this.panel1.Width;
			this.panel1Point.Y = this.panel1.Height;
			this.panel1Size.Width = this.panel1.Width;
			this.panel1Size.Height = this.panel1.Height;
			int i = this.panel1.Controls.Count;
			this.controlPoints = new Point[i];
			this.controlSizes = new Size[i];
			this.fontSize = new float[i];
			for (int j = 0; j < i; j++)
			{
				Control con = this.panel1.Controls[j];
				this.controlPoints[j].X = con.Location.X;
				this.controlPoints[j].Y = con.Location.Y;
				this.controlSizes[j].Width = con.Size.Width;
				this.controlSizes[j].Height = con.Size.Height;
				if (con.GetType() == this.label2.GetType())
				{
					System.Windows.Forms.Label lab = (System.Windows.Forms.Label)con;
					this.fontSize[j] = lab.Font.Size;
				}
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000022D4 File Offset: 0x000004D4
		private void toolStripMenuItem2_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002290 File Offset: 0x00000490
		private void exitWindow(object sender, EventArgs e)
		{
			base.Close();
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00006E34 File Offset: 0x00005034
		private void selectInterval(object sender, EventArgs e)
		{
			ToolStripItem itemMenuItem = (ToolStripItem)sender;
			int.Parse(itemMenuItem.Tag.ToString());
			foreach (object obj in this.dMM记录ToolStripMenuItem.DropDownItems)
			{
				ToolStripMenuItem item = (ToolStripMenuItem)obj;
				item.Enabled = (itemMenuItem != item);
				item.CheckState = ((itemMenuItem == item) ? CheckState.Checked : CheckState.Unchecked);
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000022D6 File Offset: 0x000004D6
		private void 结束ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripItem toolStripItem = (ToolStripItem)sender;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00006EC0 File Offset: 0x000050C0
		private void 伊万_Click(object sender, EventArgs e)
		{
			Form1ShiBoQi.b = !Form1ShiBoQi.b;
			if (Form1ShiBoQi.b)
			{
				this.DmmAutorecordTextBox.Hide();
				base.WindowState = FormWindowState.Maximized;
				string currPath = Environment.CurrentDirectory.ToString();
				currPath += "\\欧亚集团伊万科技---产品园地.mht";
				this.webBrowser1.Navigate(currPath);
				this.webBrowser1.Show();
				this.webBrowser1.Dock = DockStyle.Fill;
				return;
			}
			this.webBrowser1.Hide();
			base.WindowState = FormWindowState.Normal;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00006F40 File Offset: 0x00005140
		private void 打开_Click(object sender, EventArgs e)
		{
			this.holdScreen = true;
			this.openFileDialog1.Filter = "DSO File (*.dat)|*.dat|DSO File (*.txt)|*.txt|DMM File(*.txt)|*.txt";
			this.openFileDialog1.Title = "Open DSO or DMM file";
			if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				string path = this.openFileDialog1.FileName;
				if (path.Contains(".txt"))
				{
					this.DmmAutorecordTextBox.Clear();
					StreamReader sr = new StreamReader(path);
					this.textBox2.Text = sr.ReadToEnd();
					sr.Close();
					return;
				}
				if (path.Contains(".dat"))
				{
					Stream s = new FileStream(path, FileMode.Open);
					BinaryReader binaryReader = new BinaryReader(s);
					byte[] by = binaryReader.ReadBytes((int)s.Length);
					binaryReader.Close();
					if (by.Length != 0)
					{
						switch (by[0])
						{
						case 34:
							this.forOldDraw(by);
							return;
						case 35:
							this.forDSORealTimeData(by);
							return;
						case 36:
							this.forDSODataForAnalyse(by);
							return;
						case 37:
							this.forDMM_realTimeData(by);
							return;
						case 38:
							this.forDMMAutoRecordHoldRecord(by);
							return;
						case 39:
							this.forDMMAutoRecordHoldRecord(by);
							return;
						case 40:
							this.forCalibrationData(by);
							return;
						case 41:
							this.forProductionMessage(by);
							break;
						default:
							return;
						}
					}
				}
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00007078 File Offset: 0x00005278
		private void newFile(object sender, EventArgs e)
		{
			this.webBrowser1.Hide();
			this.textBox2.Clear();
			this.textBox2.AppendText("    Record\r\n");
			CurveList cl = this.GraphControl1.GraphPane.CurveList;
			if (cl.Count > 0)
			{
				cl[0].Clear();
			}
			this.num2 = 0;
			this.numx = 0;
			this.newData = 1;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000022DF File Offset: 0x000004DF
		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			this.dsoMode = Form1ShiBoQi.DSO_MODE.OldDrawingData;
			this.comMenu2_Click(new object(), new EventArgs());
			this.textBox2.Text = "";
			this.addToright = false;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000230F File Offset: 0x0000050F
		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			this.splitContainer1.SplitterDistance = 100000;
			this.splitContainer2.Invalidate();
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000022D4 File Offset: 0x000004D4
		private void LedsPaint(object sender, PaintEventArgs e)
		{
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000070E8 File Offset: 0x000052E8
		private void DMMPaint(Graphics dc, Rectangle si, bool drawControl)
		{
			float temp = (float)(si.Width + si.Height) / 80f;
			dc.Save();
			dc.TranslateTransform((float)si.X + temp, (float)si.Y + temp);
			float t = (float)si.Width - 2f * temp;
			if (t <= 0f)
			{
				return;
			}
			float t2 = (float)si.Height - 2f * temp;
			if (t2 <= 0f)
			{
				return;
			}
			dc.ScaleTransform(t / 10f, t2 / 8f);
			dc.DrawRectangle(this.peBlack, 0, 0, 10, 8);
			for (int i = 1; i < 8; i++)
			{
				dc.DrawLine(this.peGray, 0, i, 10, i);
			}
			for (int j = 1; j < 10; j++)
			{
				dc.DrawLine(this.peGray, j, 0, j, 8);
			}
			dc.DrawLine(this.peBlack, 0, 4, 10, 4);
			dc.DrawLine(this.peBlack, 5, 0, 5, 8);
			string str = DateTime.Now.ToString("s");
			str = str.Replace("T", "  ");
			dc.DrawString("Model: " + this.machineType, this.textFont, this.blueBrush, 0.1f, 0.1f);
			dc.DrawString(str, this.textFont, this.blueBrush, 5f, 0.1f);
			str = string.Concat(new string[]
			{
				this.平移,
				" ",
				this.平移V.ToString("%.3f"),
				" V      t:",
				this.timeBase.ToString(),
				"ms/div        y:",
				this.widthBase.ToString(),
				"v/div          DC"
			});
			dc.DrawString(str, this.textFont, this.blueBrush, 0.1f, 0.4f);
			dc.DrawString("2.02Vp-p   +Vp  1.01V  -Vp  1.01V  5.00Khz 0.2ms", this.textFont, this.blueBrush, 0.1f, 9.6f);
			PointF[] obj = this.multimeterdata;
			lock (obj)
			{
				dc.DrawLines(this.peRed, this.multimeterdata);
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000022D4 File Offset: 0x000004D4
		private void FFTPaint(Graphics dc)
		{
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000022D4 File Offset: 0x000004D4
		private void WAVEPaint(Graphics dc)
		{
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000022D4 File Offset: 0x000004D4
		private void DsoPaintLinear(Graphics dc, Rectangle si)
		{
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000022D4 File Offset: 0x000004D4
		private void DsoPaintLogicial(Graphics dc, Rectangle si)
		{
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00007348 File Offset: 0x00005548
		private Form1ShiBoQi.processDataType processData3(int m)
		{
			string str = this.timeBaseS[m];
			int i = 0;
			while (i < str.Length && str[i] >= '0' && str[i] <= '9')
			{
				i++;
			}
			int data = int.Parse(str.Substring(0, i));
			char mm = str[i];
			if (mm != 'S')
			{
				return this.processData1((double)data, mm);
			}
			return new Form1ShiBoQi.processDataType
			{
				ch = ' ',
				data = (double)data
			};
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000073C8 File Offset: 0x000055C8
		private Form1ShiBoQi.processDataType processData4(double m)
		{
			Form1ShiBoQi.processDataType type = default(Form1ShiBoQi.processDataType);
			if (m > 1000000.0)
			{
				type.ch = 'M';
				type.data = m / 1000000.0;
			}
			else if (m > 1000.0)
			{
				type.ch = 'K';
				type.data = m / 1000.0;
			}
			else if (m < 1.0)
			{
				type.ch = 'm';
				type.data = m * 1000.0;
			}
			else if (m < 0.001)
			{
				type.ch = 'u';
				type.data = m * 1000000.0;
			}
			else
			{
				type.ch = ' ';
				type.data = m;
			}
			return type;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00007498 File Offset: 0x00005698
		private Form1ShiBoQi.processDataType processData2(int m)
		{
			string str = this.volateBaseS[m];
			int i = 0;
			while (i < str.Length && str[i] >= '0' && str[i] <= '9')
			{
				i++;
			}
			int data = int.Parse(str.Substring(0, i));
			char mm = str[i];
			if (mm != 'V')
			{
				return this.processData1((double)data, mm);
			}
			return new Form1ShiBoQi.processDataType
			{
				ch = ' ',
				data = (double)data
			};
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00007518 File Offset: 0x00005718
		private Form1ShiBoQi.processDataType processData1(double data, char gree)
		{
			double exp = Math.Log10(Math.Abs(data)) / 3.0;
			if (exp < 0.0)
			{
				exp -= 0.99999;
			}
			int exp2 = (int)exp;
			int i = this.degree.IndexOf(gree);
			Form1ShiBoQi.processDataType type;
			if (exp2 > -5)
			{
				data /= Math.Pow(10.0, (double)(exp2 * 3));
				type.ch = this.degree[i + exp2];
				type.data = data;
				return type;
			}
			type.data = 0.0;
			type.ch = ' ';
			return type;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000075B8 File Offset: 0x000057B8
		private string processData(double data, char gree)
		{
			if (Math.Abs(data) < 1E-08)
			{
				return "0.0m";
			}
			double exp = Math.Log10(Math.Abs(data)) / 3.0;
			if (exp < 0.0)
			{
				exp -= 0.99999;
			}
			int exp2 = (int)exp;
			int i = this.degree.IndexOf(gree);
			if (exp2 <= -5)
			{
				return "";
			}
			data /= Math.Pow(10.0, (double)(exp2 * 3));
			if (i + exp2 > this.degree.Length)
			{
				return "";
			}
			return data.ToString("F1") + this.degree[i + exp2].ToString();
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00007684 File Offset: 0x00005884
		private string processData1(double data, char gree, string str)
		{
			if (Math.Abs(data) < 1E-08)
			{
				return "0.0m";
			}
			double exp = Math.Log10(Math.Abs(data)) / 3.0;
			if (exp < 0.0)
			{
				exp -= 0.99999;
			}
			int exp2 = (int)exp;
			int i = this.degree.IndexOf(gree);
			if (exp2 <= -5)
			{
				return "";
			}
			data /= Math.Pow(10.0, (double)(exp2 * 3));
			if (i + exp2 > this.degree.Length)
			{
				return "";
			}
			try
			{
				char c = this.degree[i + exp2];
			}
			catch (IndexOutOfRangeException)
			{
				return "";
			}
			string s = data.ToString("F1");
			if (s.Length >= 5)
			{
				return s.Substring(0, 5) + this.degree[i + exp2].ToString();
			}
			return s + this.degree[i + exp2].ToString();
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000077A8 File Offset: 0x000059A8
		private void 示波器Paint(object sender, PaintEventArgs e)
		{
			Graphics dc = e.Graphics;
			Rectangle si = this.panel4.ClientRectangle;
			if (si.Height <= 0 || si.Width <= 0)
			{
				return;
			}
			if (si.Width < 200)
			{
				return;
			}
			this.onDraw(dc, si, false);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000077F8 File Offset: 0x000059F8
		private void MoveLeds(object sender, EventArgs e)
		{
			Point point = this.panel1Point;
			Size si = this.panel1Size;
			if (si.Width == 0)
			{
				return;
			}
			float xRatio = (float)this.splitContainer2.Panel2.Width / (float)si.Width;
			float yRatio = (float)this.splitContainer2.Panel2.Height / (float)si.Height;
			int i = this.panel1.Controls.Count;
			for (int j = 0; j < i; j++)
			{
				Control con = this.panel1.Controls[j];
				con.Location = new Point((int)((float)this.controlPoints[j].X * xRatio), (int)((float)this.controlPoints[j].Y * yRatio));
				con.Size = new Size((int)((float)this.controlSizes[j].Width * xRatio), (int)((float)this.controlSizes[j].Height * yRatio));
				if (con.GetType() == this.label2.GetType())
				{
					System.Windows.Forms.Label lab = (System.Windows.Forms.Label)con;
					float xsize = this.fontSize[j] * xRatio;
					float ysize = this.fontSize[j] * yRatio;
					if (xsize > ysize)
					{
						xsize = ysize;
					}
					if (xsize > 0f)
					{
						lab.Font = new Font("黑体", xsize, FontStyle.Regular, GraphicsUnit.Point, 134);
					}
				}
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00007968 File Offset: 0x00005B68
		private void PaintLEDLines(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			Pen pen = new Pen(Color.Black, 3f);
			pen = new Pen(Color.Black, 1f);
			Point po = this.label3.Location;
			Size si = this.label3.Size;
			Point p0 = new Point(po.X + si.Width / 2, po.Y + si.Height);
			Point p = new Point(this.axLED8.Location.X - 3, (int)((double)this.axLED8.Location.Y + (double)this.axLED8.Size.Height * 0.75));
			Point p2 = new Point(p0.X, p.Y);
			graphics.DrawLine(pen, p0, p2);
			graphics.DrawLine(pen, p2, p);
			Point po2 = this.label6.Location;
			Size si2 = this.label6.Size;
			Point p3 = new Point(po2.X + si2.Width / 2, po2.Y + si2.Height);
			Point p4 = new Point(this.axLED12.Location.X - 3, (int)((double)this.axLED12.Location.Y + (double)this.axLED12.Size.Height * 0.75));
			Point p5 = new Point(p3.X, p4.Y);
			graphics.DrawLine(pen, p3, p5);
			graphics.DrawLine(pen, p5, p4);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x0000232C File Offset: 0x0000052C
		private void resize(object sender, EventArgs e)
		{
			this.panel1.Invalidate();
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00007B18 File Offset: 0x00005D18
		private bool selectRightCom()
		{
			this.checkCOM("");
			return true;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00007B34 File Offset: 0x00005D34
		private bool checkCOM(string com = "")
		{
			this.CloseThreads();
			this.macheType2 = Form1ShiBoQi.MachineType2.NULL;
			byte[] by = new byte[]
			{
				165,
				1,
				0,
				90
			};
			if (com != "")
			{
				if (this.serialPort1.IsOpen)
				{
					this.serialPort1.Close();
				}
				this.serialPort1.PortName = com;
				this.serialPort1.BaudRate = 115200;
				this.serialPort1.Parity = Parity.None;
				this.serialPort1.StopBits = StopBits.One;
				this.serialPort1.DataBits = 8;
				try
				{
					this.serialPort1.Open();
					goto IL_A3;
				}
				catch
				{
					return false;
				}
				goto IL_99;
				IL_A3:
				CultureInfo inf = CultureInfo.CurrentUICulture;
				if (!this.serialPort1.IsOpen)
				{
					return false;
				}
				this.serialPort1.Write(by, 0, by.Length);
				Thread.Sleep(200);
				if (this.serialPort1.BytesToRead > 2)
				{
					this.bReturn = true;
					this.getData();
					if (this.macheType2 != Form1ShiBoQi.MachineType2.NULL)
					{
						this.currentCOMID = int.Parse(this.serialPort1.PortName.Substring(3));
						if (inf.NativeName.Contains("中文"))
						{
							this.通讯状态.Text = "通讯成功!";
						}
						else
						{
							this.通讯状态.Text = "COM SUCEED!";
						}
						this.saveComIDtoRegistry();
						this.StartThreads();
					}
					return true;
				}
				if (inf.NativeName.Contains("中文"))
				{
					this.通讯状态.Text = "通讯失败!";
				}
				else
				{
					this.通讯状态.Text = "COM Failed!";
				}
				MessageBox.Show(this.通讯状态.Text);
				return false;
			}
			IL_99:
			this.串口_Click(null, null);
			return false;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00007CF0 File Offset: 0x00005EF0
		private void 串口_Click(object sender, EventArgs e)
		{
			Point po = this.toolStrip1.Location;
			int i;
			for (i = 0; i < this.toolStrip1.Items.Count; i++)
			{
				ToolStripItem item = this.toolStrip1.Items[i];
				if (item.Tag != null && !(item.Tag.ToString() != "串口"))
				{
					break;
				}
				po.X += item.Size.Width;
			}
			po.X += 40;
			po.Y += 30;
			this.SerialPortContextMenuStrip2.Items.Clear();
			string[] serialPorts = this.getSerialPorts();
			i = 0;
			foreach (string port in serialPorts)
			{
				this.SerialPortContextMenuStrip2.Items.Add(port);
				this.SerialPortContextMenuStrip2.Items[i++].Click += this.Form1ShiBoQi_Click;
			}
			if (this.SerialPortContextMenuStrip2.Items.Count == 0)
			{
				MessageBox.Show("没有找到合适的USB 串口设备!/No USB COM found!");
				return;
			}
			this.SerialPortContextMenuStrip2.Show(base.PointToScreen(po));
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00007E30 File Offset: 0x00006030
		private void Form1ShiBoQi_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem it = sender as ToolStripMenuItem;
			this.checkCOM(it.Text);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00007E54 File Offset: 0x00006054
		private void selectComMenu(object sender, EventArgs e)
		{
			foreach (object obj in this.SerialPortContextMenuStrip2.Items)
			{
				((ToolStripMenuItem)obj).Checked = false;
			}
			ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
			string str = toolStripMenuItem.Text;
			string str2 = str.Substring(3);
			this.currentCOMID = int.Parse(str2);
			toolStripMenuItem.Checked = true;
			this.toolStripStatusLabel1.Text = str;
			this.CloseThreads();
			Thread.Sleep(50);
			this.OpenSerialport(str);
			this.StartThreads();
			this.panel4.Dock = DockStyle.Fill;
			this.panel4.Visible = true;
			this.panel4.Invalidate();
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00007F20 File Offset: 0x00006120
		private void invalideMenu(object sender, EventArgs e)
		{
			this.panel4.Dock = DockStyle.Fill;
			this.panel4.Invalidate();
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			if (item.Tag != null)
			{
				string tag = item.Tag.ToString();
				if (tag == "通信")
				{
					this.连接ToolStripMenuItem.Checked = this.serialPort1.IsOpen;
					this.断开ToolStripMenuItem.Checked = !this.serialPort1.IsOpen;
					this.断开ToolStripMenuItem.Enabled = false;
					this.communicationTestToolStripMenuItem.Enabled = true;
					this.连接ToolStripMenuItem.Enabled = false;
					return;
				}
				if (tag == "Running")
				{
					this.dMMToolStripMenuItem.Checked = (this.macheType2 == Form1ShiBoQi.MachineType2.DMM);
					this.dSOToolStripMenuItem.Checked = (this.macheType2 != Form1ShiBoQi.MachineType2.DMM);
					this.dMMToolStripMenuItem.Enabled = this.dMMToolStripMenuItem.Checked;
					this.dSOToolStripMenuItem.Enabled = this.dSOToolStripMenuItem.Checked;
					return;
				}
				if (tag == "DSO")
				{
					this.WaveToolStripMenuItem.Checked = (this.dsoMode == Form1ShiBoQi.DSO_MODE.RealtimeData);
					this.fTTToolStripMenuItem.Checked = !this.WaveToolStripMenuItem.Checked;
					return;
				}
				if (!(tag == "编辑"))
				{
					return;
				}
				bool b = false;
				if (this.macheType2 != Form1ShiBoQi.MachineType2.DMM)
				{
					b = true;
				}
				this.复制波形ToolStripMenuItem.Enabled = b;
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00008090 File Offset: 0x00006290
		private void toolStripButton6_Click(object sender, EventArgs e)
		{
			this.dsoMode = Form1ShiBoQi.DSO_MODE.RealTimeDataForAnalyse;
			this.GraphControl1.GraphPane.YAxis.Title.Text = "Volt,(V)";
			this.comMenu2_Click(new object(), new EventArgs());
			this.textBox2.Text = "";
			this.addToright = false;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000080EC File Offset: 0x000062EC
		private void sendString(SerialPort port, string str)
		{
			if (!port.IsOpen)
			{
				return;
			}
			byte[] by = new byte[str.Length + 2];
			by[0] = 165;
			Encoding.Default.GetBytes(str, 0, str.Length, by, 1);
			int i = by.GetLength(0);
			byte temp = 0;
			for (int j = 0; j < i - 1; j++)
			{
				temp += by[j];
			}
			by[i - 1] = (byte)-temp;
			object obj = this.serialportObject;
			lock (obj)
			{
				if (port.IsOpen)
				{
					port.Write(by, 0, by.GetLength(0));
				}
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000081A0 File Offset: 0x000063A0
		private void sendBytes(SerialPort port, byte[] by)
		{
			if (!port.IsOpen)
			{
				return;
			}
			byte[] b = new byte[]
			{
				165
			};
			int i = by.Length;
			byte temp = 165;
			for (int j = 0; j < i; j++)
			{
				temp += by[j];
			}
			object obj = this.serialportObject;
			lock (obj)
			{
				port.Write(b, 0, 1);
				port.Write(by, 0, by.Length);
				b[0] = (byte)-temp;
				port.Write(b, 0, 1);
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00008238 File Offset: 0x00006438
		private void sendCommand(SerialPort port, byte command)
		{
			if (!port.IsOpen)
			{
				return;
			}
			byte[] by = new byte[4];
			by[0] = 165;
			by[1] = command;
			if (command == 2)
			{
				by[2] = this.commandParamter;
			}
			else
			{
				by[2] = 0;
			}
			by[3] = 165;
			byte[] array = by;
			int num = 3;
			array[num] += by[1];
			byte[] array2 = by;
			int num2 = 3;
			array2[num2] += by[2];
			by[3] = (byte)-by[3];
			object obj = this.serialportObject;
			lock (obj)
			{
				if (port.IsOpen)
				{
					try
					{
						port.Write(by, 0, 4);
					}
					catch
					{
						this.CloseThreads();
					}
				}
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000022D4 File Offset: 0x000004D4
		private static void emptyFun()
		{
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000082F8 File Offset: 0x000064F8
		private void OpenSerialport(string port)
		{
			this.serialPort1.Close();
			this.serialPort1.PortName = port;
			try
			{
				this.serialPort1.Open();
			}
			catch (UnauthorizedAccessException)
			{
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x0000833C File Offset: 0x0000653C
		private void CloseThreads()
		{
			if (this.getDataThread.ThreadState.GetTypeCode() != TypeCode.Empty)
			{
				this.getDataThread.Abort();
			}
			if (this.daemonThread.ThreadState.GetTypeCode() != TypeCode.Empty)
			{
				this.daemonThread.Abort();
			}
			if (this.demonSaveThread.ThreadState.GetTypeCode() != TypeCode.Empty)
			{
				this.demonSaveThread.Abort();
			}
			Queue<byte[]> obj = this.packageQueue;
			lock (obj)
			{
				this.packageHandleThread.Abort();
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000083F4 File Offset: 0x000065F4
		private void StartThreads()
		{
			this.bReturn = false;
			if (this.getDataThread.ThreadState.GetTypeCode() != TypeCode.Empty)
			{
				this.getDataThread.Abort();
			}
			this.getDataThread = new Thread(new ThreadStart(this.getData));
			this.getDataThread.Start();
			if (this.daemonThread.ThreadState.GetTypeCode() != TypeCode.Empty)
			{
				this.daemonThread.Abort();
			}
			this.daemonThread = new Thread(new ThreadStart(this.DEMON));
			this.daemonThread.Priority = ThreadPriority.Lowest;
			this.daemonThread.IsBackground = true;
			this.daemonThread.Start();
			Queue<byte[]> obj = this.packageQueue;
			lock (obj)
			{
				this.packageHandleThread.Abort();
				this.packageHandleThread = new Thread(new ThreadStart(this.PackageHandleThread));
				this.packageHandleThread.Start();
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002339 File Offset: 0x00000539
		private void saveTextboxData()
		{
			for (;;)
			{
				Thread.Sleep(300000);
				File.WriteAllLines(".\\e-one.txt", this.textBox2.Lines);
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x0000850C File Offset: 0x0000670C
		private void toolStripButton14_Click(object sender, EventArgs e)
		{
			Point po = this.toolStrip1.Location;
			for (int i = 0; i < this.toolStrip1.Items.Count; i++)
			{
				ToolStripItem item = this.toolStrip1.Items[i];
				if (item.Tag != null && !(item.Tag.ToString() != "定时"))
				{
					break;
				}
				po.X += item.Size.Width;
			}
			po.X += 40;
			po.Y += 30;
			string str = this.intervalTime.ToString() + "s";
			foreach (object obj in this.TimerContextMenuStrip1.Items)
			{
				ToolStripMenuItem item2 = (ToolStripMenuItem)obj;
				if (item2.Text == str)
				{
					item2.Checked = true;
				}
				else
				{
					item2.Checked = false;
				}
			}
			this.TimerContextMenuStrip1.Show(base.PointToScreen(po));
		}

		// Token: 0x06000050 RID: 80 RVA: 0x0000864C File Offset: 0x0000684C
		private void changeIntervalTime(object sender, EventArgs e)
		{
			string str = ((ToolStripMenuItem)sender).Text;
			str = str.Replace('s', ' ');
			this.intervalTime = int.Parse(str);
			if (this.timer定时测量.Enabled)
			{
				this.timer定时测量.Stop();
				this.timer定时测量.Interval = (double)(this.intervalTime * 1000);
				this.timer定时测量.Start();
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x0000235B File Offset: 0x0000055B
		private void FormCloseing(object sender, FormClosingEventArgs e)
		{
			this.CloseThreads();
			this.saveComIDtoRegistry();
			this.writeParaToReg();
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000022D4 File Offset: 0x000004D4
		private void getComIDFormRegister()
		{
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000022D4 File Offset: 0x000004D4
		private void saveComIDtoRegistry()
		{
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000086B8 File Offset: 0x000068B8
		private void CommunicationTestThread()
		{
			if (this.currentThread.IsAlive)
			{
				Queue<byte[]> obj = this.packageQueue;
				lock (obj)
				{
					this.currentThread.Abort();
				}
			}
			this.currentThread = new Thread(() => 
			{
				this.threadRunResult = false;
				if (this.serialPort1.IsOpen)
				{
					byte[] command = null;
					if (command == null)
					{
						Queue<byte[]> obj2 = this.packageQueue;
						lock (obj2)
						{
							this.packageQueue.Clear();
						}
						this.sendCommand(this.serialPort1, 1);
						this.sendCommand(this.serialPort1, 1);
						this.sendCommand(this.serialPort1, 1);
						int i = 0;
						while (i < 100 && this.packageQueue.Count <= 0)
						{
							Thread.Sleep(5);
							i++;
						}
						obj2 = this.packageQueue;
						lock (obj2)
						{
							if (this.packageQueue.Count > 0)
							{
								command = this.packageQueue.Dequeue();
							}
						}
					}
					if (command != null && command[0] == 33)
					{
						this.threadRunResult = true;
						return;
					}
				}
			});
			this.currentThread.Start();
		}

		// Token: 0x06000055 RID: 85 RVA: 0x0000872C File Offset: 0x0000692C
		private void OnTimer定时测量(object sender, EventArgs e)
		{
			string obj = this.textbox2string;
			string str;
			lock (obj)
			{
				str = this.numx.ToString("D3") + ". " + this.textbox2string + "  ";
			}
			if (this.addToright)
			{
				DateTime time = DateTime.Now;
				string sec = time.Second.ToString();
				if (sec.Length < 2)
				{
					sec = "0" + sec;
				}
				str = string.Concat(new string[]
				{
					str,
					time.ToShortDateString(),
					" ",
					time.ToShortTimeString(),
					":",
					sec,
					"\r\n"
				});
				this.textBox2.AppendText(str);
				this.numx++;
				this.addElement = true;
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x0000236F File Offset: 0x0000056F
		private void toolStripButton13_Click(object sender, EventArgs e)
		{
			this.setStopState(false, true);
			this.holdScreen = true;
			this.maxscale = 0f;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000882C File Offset: 0x00006A2C
		private void saveMultimeterData()
		{
			string path = ".\\multimeter files\\";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			SaveFileDialog sd = new SaveFileDialog();
			sd.Filter = "multimeter files (*.mtm.txt)|*.mtm.txt|All files (*.*)|*.*";
			sd.FilterIndex = 1;
			sd.InitialDirectory = path;
			if (sd.ShowDialog() == DialogResult.OK)
			{
				File.WriteAllLines(sd.FileName, this.textBox2.Lines);
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x0000888C File Offset: 0x00006A8C
		private void addElementToDrawing()
		{
			if (!this.addElement)
			{
				return;
			}
			this.addElement = false;
			float currentMaxScale = this.currentMultimeterData.getMaxScale();
			int num;
			if (this.oldMultimeterData.m_type != this.currentMultimeterData.m_type)
			{
				CurveItem curveItem = this.myCurve;
				num = this.num2;
				this.num2 = num + 1;
				curveItem.AddPoint((double)num, (double)this.currentMultimeterData.getfloatDataWithoutUnit());
				this.myCurve.Label.Text = "RealTime Data(" + this.currentMultimeterData.tail + ")";
				this.oldMultimeterData = this.currentMultimeterData;
				return;
			}
			float oldmax = this.oldMultimeterData.getMaxScale();
			if (currentMaxScale == oldmax)
			{
				CurveItem curveItem2 = this.myCurve;
				num = this.num2;
				this.num2 = num + 1;
				curveItem2.AddPoint((double)num, (double)this.currentMultimeterData.getfloatDataWithoutUnit());
				this.myCurve.Label.Text = "RealTime Data(" + this.currentMultimeterData.tail + ")";
				return;
			}
			if (currentMaxScale > oldmax)
			{
				float i = this.oldMultimeterData.getUnitPara() / this.currentMultimeterData.getUnitPara();
				int j = this.myCurve.Points.Count;
				for (int k = 0; k < j; k++)
				{
					this.myCurve.Points[k].Y *= (double)i;
				}
				CurveItem curveItem3 = this.myCurve;
				num = this.num2;
				this.num2 = num + 1;
				curveItem3.AddPoint((double)num, (double)this.currentMultimeterData.getfloatDataWithoutUnit());
				this.myCurve.Label.Text = "RealTime Data(" + this.currentMultimeterData.tail + ")";
				this.oldMultimeterData = this.currentMultimeterData;
				return;
			}
			float data = this.currentMultimeterData.getfloatData();
			float d = this.oldMultimeterData.getUnitPara();
			data /= d;
			CurveItem curveItem4 = this.myCurve;
			num = this.num2;
			this.num2 = num + 1;
			curveItem4.AddPoint((double)num, (double)data);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00008A98 File Offset: 0x00006C98
		private void forDMMAutoRecordHoldRecord(byte[] by)
		{
			this.dmmMode = Form1ShiBoQi.DMM_Mode.NULL;
			this.oldMode = Form1ShiBoQi.DMM_Mode.AutoRecoreData;
			BitConverter.ToInt16(by, 1);
			short currentTransNum = BitConverter.ToInt16(by, 3);
			BitConverter.ToInt16(by, 5);
			this.DmmAutorecordTextBox.Text = "";
			for (int i = 0; i < (int)currentTransNum; i++)
			{
				Form1ShiBoQi.DMM_RECORD dm = new Form1ShiBoQi.DMM_RECORD(by, 7 + 9 * i);
				string str = (i + 1).ToString("D4") + " " + dm.toString();
				while (str.Length < 20)
				{
					str += "   ";
				}
				this.DmmAutorecordTextBox.AppendText(str);
				if (i % 2 == 1)
				{
					this.DmmAutorecordTextBox.AppendText("\r\n");
				}
				else
				{
					this.DmmAutorecordTextBox.AppendText("\t");
				}
			}
			if (this.maxScreen)
			{
				this.DmmAutorecordTextBox.Location = new Point(0, 0);
				this.DmmAutorecordTextBox.Size = new Size(base.Size.Width, base.Size.Height);
				this.DmmAutorecordTextBox.Dock = DockStyle.Fill;
				return;
			}
			this.DmmAutorecordTextBox.Location = new Point(0, this.menuStrip1.Size.Height + this.toolStrip1.Size.Height);
			this.DmmAutorecordTextBox.Size = new Size(base.Size.Width, base.Size.Height - this.menuStrip1.Size.Height - this.toolStrip1.Size.Height);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00008C4C File Offset: 0x00006E4C
		private void forCalibrationData(byte[] by)
		{
			if (by.Length != 1301)
			{
				return;
			}
			this.DmmAutorecordTextBox.Clear();
			string s = "Items\tCoeff\t\tSValue\t\tRDValue\tLockFlag\tstring\r\n";
			this.DmmAutorecordTextBox.AppendText(s);
			for (int i = 0; i < 40; i++)
			{
				if (!this.caliString[i].Contains("Res"))
				{
					string str = string.Concat(new string[]
					{
						this.caliString[i],
						"\t",
						BitConverter.ToSingle(by, 237 + 4 * i).ToString("F7"),
						"\t",
						BitConverter.ToSingle(by, 397 + 4 * i).ToString("F7"),
						"\t",
						BitConverter.ToUInt32(by, 557 + 4 * i).ToString(),
						"\t",
						by[717 + i].ToString(),
						"\t",
						Encoding.Default.GetString(by, 757 + i * 13, 13).Replace('\0', ' '),
						"\r\n"
					});
					this.DmmAutorecordTextBox.AppendText(str);
				}
			}
			string data = Encoding.Default.GetString(by, 1277, 9) + "\r\n";
			this.DmmAutorecordTextBox.AppendText(data);
			if (this.maxScreen)
			{
				this.DmmAutorecordTextBox.Location = new Point(0, 0);
				this.DmmAutorecordTextBox.Size = new Size(base.Size.Width, base.Size.Height);
			}
			else
			{
				this.DmmAutorecordTextBox.Location = new Point(0, this.menuStrip1.Size.Height + this.toolStrip1.Size.Height);
				this.DmmAutorecordTextBox.Size = new Size(base.Size.Width, base.Size.Height - this.menuStrip1.Size.Height - this.toolStrip1.Size.Height);
			}
			this.DmmAutorecordTextBox.AppendText("\r\n\r\n密码：\r\n");
			string code = string.Empty;
			for (int j = 0; j < 3; j++)
			{
				code = code + Encoding.Default.GetString(by, 1 + j * 13, 12) + "\t";
			}
			this.DmmAutorecordTextBox.AppendText(code + "\r\n检验员：\t");
			code = Encoding.Default.GetString(by, 40, 3);
			this.DmmAutorecordTextBox.AppendText(code + "\r\n出厂编号：\t");
			code = Encoding.Default.GetString(by, 44, 8);
			this.DmmAutorecordTextBox.AppendText(code + "\r\n客户编号：\t");
			code = Encoding.Default.GetString(by, 53, 8);
			this.DmmAutorecordTextBox.AppendText(code + "\r\n生产日期：\t");
			code = Encoding.Default.GetString(by, 62, 8);
			this.DmmAutorecordTextBox.AppendText(code + "\r\n维修信息：\r\n");
			for (int k = 0; k < 10; k++)
			{
				code = string.Concat(new string[]
				{
					"编号：",
					BitConverter.ToInt16(by, 77 + k * 16).ToString(),
					"\t维修员:",
					Encoding.Default.GetString(by, 79 + k * 16, 3),
					"\t维修日期:",
					Encoding.Default.GetString(by, 83 + k * 16, 8),
					"\r\n"
				});
				this.DmmAutorecordTextBox.AppendText(code);
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00009028 File Offset: 0x00007228
		private void trimString(ref string type)
		{
			int index = type.IndexOf('\0');
			if (index > 0)
			{
				type = type.Substring(0, index);
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00009050 File Offset: 0x00007250
		private void forProductionMessage(byte[] bys)
		{
			bys = bys.Skip(1).ToArray<byte>();
			string type = Encoding.Default.GetString(bys);
			this.trimString(ref type);
			bys = bys.Skip(16).ToArray<byte>();
			string time = Encoding.Default.GetString(bys);
			this.trimString(ref time);
			bys = bys.Skip(7).ToArray<byte>();
			string hardVersion = Encoding.Default.GetString(bys);
			this.trimString(ref hardVersion);
			bys = bys.Skip(10).ToArray<byte>();
			string softVersion = Encoding.Default.GetString(bys);
			this.trimString(ref softVersion);
			this.DmmAutorecordTextBox.Text = string.Format(" Type:{0} Time:{1} Hardware Ver:{2} Software Ver:{3} {4}", new object[]
			{
				type,
				time,
				hardVersion,
				softVersion,
				Resources.String1
			});
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000238B File Offset: 0x0000058B
		private void changeToGraphMode()
		{
			this.panel4.Visible = false;
			this.panel4.Dock = DockStyle.None;
			this.GraphControl1.Visible = true;
			this.GraphControl1.Dock = DockStyle.Fill;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000023BD File Offset: 0x000005BD
		private void changeToPanel4Mode()
		{
			this.GraphControl1.Visible = false;
			this.panel4.Visible = true;
			this.GraphControl1.Dock = DockStyle.None;
			this.panel4.Dock = DockStyle.Fill;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000023EF File Offset: 0x000005EF
		private void changeToOtherMode()
		{
			this.GraphControl1.Visible = false;
			this.panel4.Visible = false;
			this.GraphControl1.Dock = DockStyle.None;
			this.panel4.Dock = DockStyle.None;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x0000911C File Offset: 0x0000731C
		private void comMenu2_Click(object sender, EventArgs e)
		{
			this.oldMultimeterData.m_type = Form1ShiBoQi.MultimeterType.Null;
			this.oldMaxScale = 0f;
			this.maxscale = 0f;
			this.oldScale = 0f;
			this.addToright = true;
			this.num2 = 0;
			this.numx = 0;
			this.endButton.Enabled = true;
			this.holdScreen = false;
			this.hScrollBar1.Visible = false;
			this.panel2.Visible = false;
			this.panel3.Visible = false;
			if (this.panel4.Visible)
			{
				this.panel4.Invalidate();
			}
			if (!this.serialPort1.IsOpen)
			{
				return;
			}
			this.serialPort1.DiscardInBuffer();
			this.timer定时测量.Stop();
			this.webBrowser1.Hide();
			this.DmmAutorecordTextBox.Hide();
			if (this.macheType2 == Form1ShiBoQi.MachineType2.DMM)
			{
				this.newData = 1;
				this.GraphControl1.GraphPane.YAxis.Title.Text = "";
				byte[] by = new byte[2];
				switch (this.dmmMode)
				{
				case Form1ShiBoQi.DMM_Mode.DMM_realTimeData:
				{
					this.textBox2.AppendText("Realtime Data\r\n");
					by[0] = 5;
					this.timer定时测量.Stop();
					this.timer定时测量.Interval = (double)(this.intervalTime * 1000);
					this.timer定时测量.Start();
					if (this.myCurve != null)
					{
						this.myCurve.Clear();
					}
					this.GraphControl1.Dock = DockStyle.Fill;
					this.changeToGraphMode();
					GraphPane graphPane = this.GraphControl1.GraphPane;
					graphPane.Title.Text = "DMM RealTime Data";
					graphPane.XAxis.Title.Text = "Time";
					this.panel4.Dock = DockStyle.None;
					this.panel4.Size = new Size(0, 0);
					this.GraphControl1.AxisChange();
					break;
				}
				case Form1ShiBoQi.DMM_Mode.AutoRecoreData:
					this.DmmAutorecordTextBox.Clear();
					this.DmmAutorecordTextBox.Visible = true;
					by[0] = 6;
					this.changeToOtherMode();
					break;
				case Form1ShiBoQi.DMM_Mode.HoldData:
					by[0] = 7;
					this.DmmAutorecordTextBox.Clear();
					this.DmmAutorecordTextBox.Visible = true;
					this.changeToPanel4Mode();
					break;
				case Form1ShiBoQi.DMM_Mode.Calibation:
					this.DmmAutorecordTextBox.Clear();
					this.DmmAutorecordTextBox.Visible = true;
					by[0] = 8;
					this.changeToOtherMode();
					break;
				case Form1ShiBoQi.DMM_Mode.GET_PRODUCT_MESSAGE:
				case Form1ShiBoQi.DMM_Mode.GET_PRODUCT_MESSAGE_Finished:
					this.DmmAutorecordTextBox.Clear();
					this.DmmAutorecordTextBox.Visible = true;
					by[0] = 9;
					this.changeToOtherMode();
					break;
				}
				Queue<byte[]> obj = this.packageQueue;
				lock (obj)
				{
					this.packageQueue.Clear();
				}
				this.sendBytes(this.serialPort1, by);
				return;
			}
			if (this.macheType2 == Form1ShiBoQi.MachineType2.DSO)
			{
				this.textBox2.Text = "";
			}
			byte[] by2 = null;
			switch (this.dsoMode)
			{
			case Form1ShiBoQi.DSO_MODE.OldDrawingData:
			{
				LoadDrawingIDDialog dlg = new LoadDrawingIDDialog();
				if (dlg.ShowDialog() != DialogResult.OK)
				{
					return;
				}
				this.timer定时测量.Stop();
				by2 = new byte[]
				{
					2,
					(byte)dlg.selectedNumber
				};
				this.sendBytes(this.serialPort1, by2);
				this.changeToPanel4Mode();
				break;
			}
			case Form1ShiBoQi.DSO_MODE.RealtimeData:
			{
				byte[] array = new byte[2];
				array[0] = 3;
				by2 = array;
				this.changeToPanel4Mode();
				break;
			}
			case Form1ShiBoQi.DSO_MODE.RealTimeDataForAnalyse:
			{
				byte[] array2 = new byte[2];
				array2[0] = 4;
				by2 = array2;
				this.changeToGraphMode();
				break;
			}
			}
			if (by2 != null)
			{
				Queue<byte[]> obj = this.packageQueue;
				lock (obj)
				{
					this.packageQueue.Clear();
				}
				this.sendBytes(this.serialPort1, by2);
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000022D4 File Offset: 0x000004D4
		private void st_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000094EC File Offset: 0x000076EC
		private void toolStripButton9_Click(object sender, EventArgs e)
		{
			Point po = this.toolStrip1.Location;
			for (int i = 0; i < this.toolStrip1.Items.Count; i++)
			{
				ToolStripItem item = this.toolStrip1.Items[i];
				if (item.Tag != null && !(item.Tag.ToString() != "万用表"))
				{
					break;
				}
				po.X += item.Size.Width;
			}
			po.X += 40;
			po.Y += 50;
			for (int j = 0; j < this.MultimeterContextMenuStrip1.Items.Count; j++)
			{
				if (this.dmmMode == (Form1ShiBoQi.DMM_Mode)j)
				{
					((ToolStripMenuItem)this.MultimeterContextMenuStrip1.Items[j]).Checked = true;
				}
				else
				{
					((ToolStripMenuItem)this.MultimeterContextMenuStrip1.Items[j]).Checked = false;
				}
			}
			this.MultimeterContextMenuStrip1.Show(base.PointToScreen(po));
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00009604 File Offset: 0x00007804
		private void changeDMM_mode(object sender, EventArgs e)
		{
			this.comMenu2_Click(new object(), new EventArgs());
			this.setStopState(false, true);
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			for (int i = 0; i < this.MultimeterContextMenuStrip1.Items.Count; i++)
			{
				if (((ToolStripMenuItem)this.MultimeterContextMenuStrip1.Items[i]).Equals(item))
				{
					this.dmmMode = (Form1ShiBoQi.DMM_Mode)i;
					break;
				}
			}
			this.FileFath = string.Empty;
			this.comMenu2_Click(new object(), new EventArgs());
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00009690 File Offset: 0x00007890
		private void toolStripButton7_Click(object sender, EventArgs e)
		{
			Point po = this.toolStrip1.Location;
			for (int i = 0; i < this.toolStrip1.Items.Count; i++)
			{
				ToolStripItem item = this.toolStrip1.Items[i];
				if (item.Tag != null && !(item.Tag.ToString() != "示波器"))
				{
					break;
				}
				po.X += item.Size.Width;
			}
			po.X += 40;
			po.Y += 50;
			for (int j = 0; j < this.OscillographContextMenuStrip2.Items.Count; j++)
			{
				if (this.dsoMode == (Form1ShiBoQi.DSO_MODE)j)
				{
					((ToolStripMenuItem)this.OscillographContextMenuStrip2.Items[j]).Checked = true;
				}
				else
				{
					((ToolStripMenuItem)this.OscillographContextMenuStrip2.Items[j]).Checked = false;
				}
			}
			this.OscillographContextMenuStrip2.Show(base.PointToScreen(po));
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000097A8 File Offset: 0x000079A8
		private void OscillographContextMenuStrip2_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			for (int i = 0; i < this.OscillographContextMenuStrip2.Items.Count; i++)
			{
				if (((ToolStripMenuItem)this.OscillographContextMenuStrip2.Items[i]).Equals(item))
				{
					this.dsoMode = (Form1ShiBoQi.DSO_MODE)i;
					break;
				}
			}
			this.FileFath = string.Empty;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000980C File Offset: 0x00007A0C
		private void changeWindowMode(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			for (int i = 0; i < this.fTTToolStripMenuItem1.DropDownItems.Count; i++)
			{
				if (item.Equals(this.fTTToolStripMenuItem1.DropDownItems[i]))
				{
					this.dsoWindowMode = (Form1ShiBoQi.DSOWindowMode)i;
					return;
				}
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x0000985C File Offset: 0x00007A5C
		private void onClickFttWindowMenu(object sender, EventArgs e)
		{
			for (int i = 0; i < this.fTTToolStripMenuItem1.DropDownItems.Count; i++)
			{
				((ToolStripMenuItem)this.fTTToolStripMenuItem1.DropDownItems[i]).Checked = (i == (int)this.dsoWindowMode);
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00002421 File Offset: 0x00000621
		private void changeDMM_DSO_Mode(object sender, EventArgs e)
		{
			if (((ToolStripMenuItem)sender).Text == "DMM")
			{
				this.macheType2 = Form1ShiBoQi.MachineType2.DMM;
				return;
			}
			this.macheType2 = Form1ShiBoQi.MachineType2.DSO;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000098A8 File Offset: 0x00007AA8
		private void onConnectSerialPort(object sender, EventArgs e)
		{
			this.checkCOM("");
			this.OpenSerialport("COM" + this.currentCOMID.ToString());
			this.StartThreads();
			if (this.selectRightCom())
			{
				this.startButton.Enabled = true;
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00002449 File Offset: 0x00000649
		private void onCloseSerialPort(object sender, EventArgs e)
		{
			if (this.serialPort1.IsOpen)
			{
				this.serialPort1.Close();
				this.timer定时测量.Stop();
				this.startButton.Enabled = false;
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000098F8 File Offset: 0x00007AF8
		private void toolStripButton8_Click(object sender, EventArgs e)
		{
			this.maxScreen = !this.maxScreen;
			ToolStripButton item = this.toolStripButton8;
			if (this.maxScreen)
			{
				this.splitterDistance1 = this.splitContainer1.SplitterDistance;
				this.splitterDistance2 = this.splitContainer2.SplitterDistance;
				this.toolStrip1.Hide();
				this.menuStrip1.Hide();
				this.splitContainer1.SplitterDistance = 10000;
				this.splitContainer2.SplitterDistance = 10000;
				item.Text = "Normal Screen";
				base.WindowState = FormWindowState.Maximized;
			}
			else
			{
				base.WindowState = FormWindowState.Normal;
				this.toolStrip1.Show();
				this.menuStrip1.Show();
				this.splitContainer1.SplitterDistance = this.splitterDistance1;
				this.splitContainer2.SplitterDistance = this.splitterDistance2;
				item.Text = "Max Screen";
			}
			if (this.maxScreen)
			{
				if (this.DmmAutorecordTextBox.Visible)
				{
					this.DmmAutorecordTextBox.Location = new Point(0, 0);
					this.DmmAutorecordTextBox.Size = new Size(base.Size.Width, base.Size.Height);
					return;
				}
			}
			else if (this.DmmAutorecordTextBox.Visible)
			{
				this.DmmAutorecordTextBox.Location = new Point(0, this.menuStrip1.Size.Height + this.toolStrip1.Size.Height);
				this.DmmAutorecordTextBox.Size = new Size(base.Size.Width, base.Size.Height - this.menuStrip1.Size.Height - this.toolStrip1.Size.Height);
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000247A File Offset: 0x0000067A
		private void onChangeWaveMode(object sender, EventArgs e)
		{
			if (((ToolStripMenuItem)sender).Tag.ToString() == "WAVE")
			{
				this.dsoMode = Form1ShiBoQi.DSO_MODE.RealtimeData;
				return;
			}
			this.dsoMode = Form1ShiBoQi.DSO_MODE.RealTimeDataForAnalyse;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000024A7 File Offset: 0x000006A7
		private void onChangeDSOLineralogicalMode(object sender, EventArgs e)
		{
			this.线性ToolStripMenuItem.Checked = (this.dsoXLinearMode == Form1ShiBoQi.DSOXLinearMode.Linear);
			this.对数ToolStripMenuItem.Checked = !this.线性ToolStripMenuItem.Checked;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000024D6 File Offset: 0x000006D6
		private void 线性ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.dsoXLinearMode = Form1ShiBoQi.DSOXLinearMode.Linear;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000024DF File Offset: 0x000006DF
		private void 对数ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.dsoXLinearMode = Form1ShiBoQi.DSOXLinearMode.logarithm;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000024E8 File Offset: 0x000006E8
		private void setStopState(bool stopStartButton, bool stopTimer)
		{
			if (stopTimer)
			{
				this.timer定时测量.Stop();
			}
			this.endButton.Enabled = stopStartButton;
			this.startButton.Enabled = !stopStartButton;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00009ACC File Offset: 0x00007CCC
		private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
		{
			bool oldHoldScreen = this.holdScreen;
			this.holdScreen = false;
			Graphics dc = e.Graphics;
			Rectangle si = default(Rectangle);
			si.Width = e.MarginBounds.Width;
			si.Height = (int)((float)si.Width * 9f / 11f);
			si.X = e.MarginBounds.X;
			si.Y = e.MarginBounds.Y + 40;
			dc.DrawString(this.Text, new Font("宋体", 15f), this.blackBrush, 10f, 30f);
			if (this.macheType2 == Form1ShiBoQi.MachineType2.DSO)
			{
				if (this.dsoMode != Form1ShiBoQi.DSO_MODE.RealTimeDataForAnalyse)
				{
					this.onDraw(dc, si, true);
				}
			}
			else if (this.DmmAutorecordTextBox.Visible)
			{
				int i = this.DmmAutorecordTextBox.Lines.Length;
				string[] lines = this.DmmAutorecordTextBox.Lines;
				int leng = 0;
				foreach (string line in this.DmmAutorecordTextBox.Lines)
				{
					if (line.Length > leng)
					{
						leng = line.Length;
					}
				}
				Font font = new Font("宋体", 12f);
				for (int j = 0; j < i; j++)
				{
					dc.DrawString(lines[j], font, this.blackBrush, (float)((e.MarginBounds.Width - leng * 8) / 2), (float)(30 + (j + 3) * 20));
				}
			}
			else if (this.macheType2 == Form1ShiBoQi.MachineType2.DMM && this.dmmMode == Form1ShiBoQi.DMM_Mode.DMM_realTimeData)
			{
				int k = this.textBox2.Lines.Length;
				string[] lines2 = this.textBox2.Lines;
				int leng2 = 0;
				foreach (string line2 in this.textBox2.Lines)
				{
					if (line2.Length > leng2)
					{
						leng2 = line2.Length;
					}
				}
				Font font2 = new Font("宋体", 12f);
				for (int l = 0; l < k; l++)
				{
					dc.DrawString(lines2[l], font2, this.blackBrush, (float)((e.MarginBounds.Width - leng2 * 8) / 2), (float)(30 + (l + 3) * 20));
				}
			}
			this.holdScreen = oldHoldScreen;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00009D34 File Offset: 0x00007F34
		private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.macheType2 == Form1ShiBoQi.MachineType2.DSO && this.dsoMode == Form1ShiBoQi.DSO_MODE.RealTimeDataForAnalyse)
			{
				this.GraphControl1.DoPrintPreview();
				return;
			}
			this.tempx = 1;
			this.printPreviewDialog1.Document = this.printDocument1;
			this.printPreviewDialog1.ShowDialog();
			this.panel4.Dock = DockStyle.Fill;
			this.panel4.Visible = true;
			this.panel4.Invalidate();
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00009DA8 File Offset: 0x00007FA8
		private void 打印ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.toolStripButton3.Enabled = false;
			this.printDialog1.Document = this.printDocument1;
			if (this.printDialog1.ShowDialog() == DialogResult.OK)
			{
				this.printDialog1.Document.Print();
			}
			this.toolStripButton3.Enabled = true;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00002513 File Offset: 0x00000713
		private void button1_Click(object sender, EventArgs e)
		{
			this.toolStripButton8_Click(new object(), new EventArgs());
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00009DFC File Offset: 0x00007FFC
		private void ToClipboard(object sender, EventArgs e)
		{
			if (!this.DmmAutorecordTextBox.Visible)
			{
				Bitmap bitmap = new Bitmap(this.splitContainer2.Panel1.Bounds.Width, this.splitContainer2.Panel1.Height);
				this.splitContainer2.Panel1.DrawToBitmap(bitmap, this.splitContainer2.Panel1.Bounds);
				Clipboard.SetData("Bitmap", bitmap);
				MessageBox.Show("Have copy to Clipboard!");
				this.panel4.Invalidate();
				return;
			}
			Clipboard.SetDataObject(this.DmmAutorecordTextBox.Text);
			MessageBox.Show("Have copy to Clipboard!");
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00009EA4 File Offset: 0x000080A4
		private void OnMouseMoveButton2(object sender, MouseEventArgs e)
		{
			if (!this.mouseIsDown)
			{
				return;
			}
			Button bu;
			if (sender.GetType().Name != "Button")
			{
				if (sender.Equals(this.pictureBox13))
				{
					bu = this.button2;
				}
				else if (sender.Equals(this.pictureBox14))
				{
					bu = this.button3;
				}
				else if (sender.Equals(this.pictureBox15))
				{
					bu = this.button4;
				}
				else
				{
					bu = this.button5;
				}
			}
			else
			{
				bu = (sender as Button);
			}
			Point newPoint = (sender as PictureBox).PointToScreen(e.Location);
			string text = bu.Tag.ToString();
			float per = this.getButtonPercent(bu);
			if (text.Contains("Ver"))
			{
				if (bu.Equals(this.button2))
				{
					if (((double)per >= -0.5 || newPoint.Y < this.mousePoint.Y) && ((double)per < 0.5 || newPoint.Y > this.mousePoint.Y))
					{
						bu.Location = new Point(bu.Location.X, bu.Location.Y + newPoint.Y - this.mousePoint.Y);
						this.pictureBox13.Location = new Point(bu.Location.X, bu.Location.Y + bu.Height);
					}
				}
				else if (((double)per >= -0.5 || newPoint.Y < this.mousePoint.Y) && ((double)per < 0.5 || newPoint.Y > this.mousePoint.Y))
				{
					bu.Location = new Point(bu.Location.X, bu.Location.Y + newPoint.Y - this.mousePoint.Y);
					this.pictureBox14.Location = new Point(bu.Location.X, bu.Location.Y - bu.Height);
				}
				Form1ShiBoQi.processDataType type = this.processData4(this.dsoData1.dao[this.dsoData1.Channel_processing].ch_value);
				bu.Text = ((double)(this.getButtonPercent(bu) * 8f) * type.data).ToString("F1") + type.ch.ToString() + "V";
				this.button6.Text = ((double)((this.getButtonPercent(this.button2) - this.getButtonPercent(this.button3)) * 8f) * type.data).ToString("F1") + type.ch.ToString() + "V";
				new Rectangle(new Point(0, bu.Location.Y - 100), new Size(this.splitContainer2.Panel1.Bounds.Size.Width, 200));
			}
			else
			{
				if (bu.Equals(this.button4))
				{
					if ((per >= 0f || newPoint.X > this.mousePoint.X) && (per < 1f || newPoint.X < this.mousePoint.X))
					{
						bu.Location = new Point(bu.Location.X + newPoint.X - this.mousePoint.X, bu.Location.Y);
						this.pictureBox15.Location = new Point(bu.Location.X - this.pictureBox15.Width, bu.Location.Y);
					}
				}
				else if ((per >= 0f || newPoint.X > this.mousePoint.X) && (per < 1f || newPoint.X < this.mousePoint.X))
				{
					bu.Location = new Point(bu.Location.X + newPoint.X - this.mousePoint.X, bu.Location.Y);
					this.pictureBox16.Location = new Point(bu.Location.X + bu.Width, bu.Location.Y);
				}
				Form1ShiBoQi.processDataType type2 = this.processData3(this.dsoData1.timBase);
				bu.Text = ((double)(this.getButtonPercent(bu) * 12f) * type2.data).ToString("F1") + type2.ch.ToString() + "s";
				this.button6.Text = ((double)((this.getButtonPercent(this.button4) - this.getButtonPercent(this.button5)) * 12f) * type2.data).ToString("F1") + type2.ch.ToString() + "s";
				new Rectangle(new Point(bu.Location.X - 100, 0), new Size(200, this.splitContainer2.Panel1.Bounds.Size.Height));
			}
			this.mousePoint = newPoint;
			this.panel4.Dock = DockStyle.Fill;
			this.panel4.Invalidate();
		}

		// Token: 0x06000077 RID: 119 RVA: 0x0000A474 File Offset: 0x00008674
		private void OnmouseDown(object sender, MouseEventArgs e)
		{
			this.GraphControl1.Visible = true;
			this.GraphControl1.Visible = false;
			this.GraphControl1.Size = new Size(0, 0);
			this.mouseIsDown = true;
			if (sender.GetType().Name == "Button")
			{
				Button bu = sender as Button;
				this.mousePoint = bu.PointToScreen(e.Location);
				if (sender.Equals(this.button2))
				{
					this.clickButtonID = 2;
					return;
				}
				if (sender.Equals(this.button3))
				{
					this.clickButtonID = 3;
					return;
				}
				if (sender.Equals(this.button4))
				{
					this.clickButtonID = 4;
					return;
				}
				if (sender.Equals(this.button5))
				{
					this.clickButtonID = 5;
					return;
				}
				this.clickButtonID = -1;
				return;
			}
			else
			{
				PictureBox bu2 = sender as PictureBox;
				this.mousePoint = bu2.PointToScreen(e.Location);
				if (sender.Equals(this.pictureBox13))
				{
					this.clickButtonID = 2;
				}
				else if (sender.Equals(this.pictureBox14))
				{
					this.clickButtonID = 3;
				}
				else if (sender.Equals(this.pictureBox15))
				{
					this.clickButtonID = 4;
				}
				else if (sender.Equals(this.pictureBox16))
				{
					this.clickButtonID = 5;
				}
				else
				{
					this.clickButtonID = -1;
				}
				if (this.clickButtonID == 2 || this.clickButtonID == 3)
				{
					this.button6.BackColor = Color.YellowGreen;
					return;
				}
				if (this.clickButtonID == 4 || this.clickButtonID == 5)
				{
					this.button6.BackColor = Color.LightSkyBlue;
				}
				return;
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00002525 File Offset: 0x00000725
		private void OnMouseLeave(object sender, MouseEventArgs e)
		{
			this.mouseIsDown = false;
			this.clickButtonID = -1;
			this.panel4.Invalidate();
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00002525 File Offset: 0x00000725
		private void OnMouseLeave(object sender, EventArgs e)
		{
			this.mouseIsDown = false;
			this.clickButtonID = -1;
			this.panel4.Invalidate();
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000022D4 File Offset: 0x000004D4
		private void button2_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000022D4 File Offset: 0x000004D4
		private void button5_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000022D4 File Offset: 0x000004D4
		private void button3_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000022D4 File Offset: 0x000004D4
		private void button4_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000A604 File Offset: 0x00008804
		private void splitContainer2_Panel1_SizeChanged(object sender, EventArgs e)
		{
			SplitterPanel pan = sender as SplitterPanel;
			foreach (Button bu in this.buttons)
			{
				if (bu == null)
				{
					break;
				}
				if (bu.Tag.ToString().Contains("Ver"))
				{
					bu.Location = new Point(bu.Location.X, pan.Bounds.Height - bu.Size.Height - 4);
				}
				else
				{
					bu.Location = new Point(pan.Bounds.Width - bu.Size.Width - 4, bu.Location.Y);
				}
			}
			this.button6.Location = new Point(0, this.panel2.Height - this.button3.Height);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000022D4 File Offset: 0x000004D4
		protected override void OnPaintBackground(PaintEventArgs e)
		{
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00002540 File Offset: 0x00000740
		private void communicationTestToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.selectRightCom();
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00002549 File Offset: 0x00000749
		private void GetProductionMEessage(object sender, EventArgs e)
		{
			this.setStopState(false, true);
			this.timer定时测量.Stop();
			this.macheType2 = Form1ShiBoQi.MachineType2.DMM;
			this.dmmMode = Form1ShiBoQi.DMM_Mode.GET_PRODUCT_MESSAGE;
			this.comMenu2_Click(new object(), new EventArgs());
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000A6F4 File Offset: 0x000088F4
		private void showHoldMenu(object sender, MouseEventArgs e)
		{
			if (this.macheType2 == Form1ShiBoQi.MachineType2.DSO && this.dsoMode != Form1ShiBoQi.DSO_MODE.RealTimeDataForAnalyse)
			{
				this.holdScreen = !this.holdScreen;
				this.panel2.Visible = this.holdScreen;
				this.panel3.Visible = this.holdScreen;
				if (this.holdScreen && this.dsoData1.dao != null && this.dsoData1.dao[0].dso_Data.Length > 300)
				{
					this.CloseThreads();
					this.hScrollBar1.Maximum = this.dsoData1.dao[0].dso_Data.Length - 300;
					this.hScrollBar1.Visible = true;
				}
				else
				{
					this.hScrollBar1.Visible = false;
					this.StartThreads();
				}
				if (this.holdScreen)
				{
					this.GraphControl1.Visible = true;
					this.GraphControl1.Visible = false;
					this.GraphControl1.Size = new Size(0, 0);
					int ch = this.dsoData1.dao[this.dsoData1.Channel_processing].ch;
					Form1ShiBoQi.processDataType type = this.processData4(this.dsoData1.dao[this.dsoData1.Channel_processing].ch_value);
					float per2 = this.getButtonPercent(this.button2);
					float per3 = this.getButtonPercent(this.button3);
					this.button2.Text = ((double)(per2 * 8f) * type.data).ToString("F1") + type.ch.ToString() + "V";
					this.button3.Text = ((double)(per3 * 8f) * type.data).ToString("F1") + type.ch.ToString() + "V";
					this.button6.Location = new Point(0, this.panel2.Height - this.button3.Height);
					this.button6.Text = ((double)((this.getButtonPercent(this.button2) - this.getButtonPercent(this.button3)) * 8f) * type.data).ToString("F1") + type.ch.ToString() + "V";
					this.button6.BackColor = Color.YellowGreen;
					type = this.processData3(this.dsoData1.timBase);
					this.button4.Text = ((double)(this.getButtonPercent(this.button4) * 12f) * type.data).ToString("F1") + type.ch.ToString() + "s";
					this.button5.Text = ((double)(this.getButtonPercent(this.button5) * 12f) * type.data).ToString("F1") + type.ch.ToString() + "s";
				}
				this.panel4.Dock = DockStyle.Fill;
				this.panel4.Visible = true;
				this.panel4.Invalidate();
				return;
			}
			this.panel2.Visible = false;
			this.panel3.Visible = false;
			this.hScrollBar1.Visible = false;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000022D4 File Offset: 0x000004D4
		private void changeHoldState(object sender, EventArgs e)
		{
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000AA44 File Offset: 0x00008C44
		private float getButtonPercent(Button bu)
		{
			int move;
			if (bu.Equals(this.button2))
			{
				move = (bu.Height + this.pictureBox13.Height) / 2;
			}
			else if (bu.Equals(this.button3))
			{
				move = -(bu.Height + this.pictureBox14.Height) / 2;
			}
			else if (bu.Equals(this.button4))
			{
				move = -(bu.Width + this.pictureBox15.Width) / 2;
			}
			else
			{
				move = (bu.Width + this.pictureBox16.Width) / 2;
			}
			float tem = (float)(this.panel4.Size.Width + this.panel4.Size.Height) / 80f;
			if (bu.Tag.ToString().Contains("Ver"))
			{
				return 0.5f - ((float)bu.Location.Y + (float)bu.Size.Height / 2f - tem + (float)move) / ((float)this.panel4.Size.Height - 2f * tem);
			}
			return ((float)bu.Location.X + (float)bu.Size.Width / 2f - tem + (float)move) / ((float)this.panel4.Size.Width - 2f * tem);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000257C File Offset: 0x0000077C
		private void onClosedContextMenuStrip(object sender, ToolStripDropDownClosedEventArgs e)
		{
			this.panel4.Invalidate();
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000257C File Offset: 0x0000077C
		private void dSOToolStripMenuItem1_DropDownClosed(object sender, EventArgs e)
		{
			this.panel4.Invalidate();
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000257C File Offset: 0x0000077C
		private void menuStrip1_DragOver(object sender, DragEventArgs e)
		{
			this.panel4.Invalidate();
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00002589 File Offset: 0x00000789
		private void toolStrip实时波形_Click(object sender, EventArgs e)
		{
			this.dsoMode = Form1ShiBoQi.DSO_MODE.RealtimeData;
			this.comMenu2_Click(new object(), new EventArgs());
			this.addToright = false;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000ABB4 File Offset: 0x00008DB4
		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			if (this.FileFath == string.Empty)
			{
				this.SaveAs();
				return;
			}
			if (this.macheType2 != Form1ShiBoQi.MachineType2.DMM)
			{
				Stream s = new FileStream(this.FileFath, FileMode.Create);
				BinaryWriter bw = new BinaryWriter(s);
				byte[] obj = this.saveData;
				lock (obj)
				{
					bw.Write(this.saveData);
				}
				bw.Close();
				s.Close();
				return;
			}
			if (this.dmmMode == Form1ShiBoQi.DMM_Mode.DMM_realTimeData)
			{
				StreamWriter sw = new StreamWriter(this.FileFath);
				foreach (string str in this.textBox2.Lines)
				{
					sw.WriteLine(str);
				}
				sw.Close();
				return;
			}
			StreamWriter sw2 = new StreamWriter(this.FileFath);
			foreach (string str2 in this.DmmAutorecordTextBox.Lines)
			{
				sw2.WriteLine(str2);
			}
			sw2.Close();
		}

		// Token: 0x0600008A RID: 138 RVA: 0x0000ACC4 File Offset: 0x00008EC4
		protected override void WndProc(ref Message m)
		{
			bool IsHandled = false;
			try
			{
				this.USBPort.ProcessWindowsMessage(m.Msg, m.WParam, m.LParam, ref IsHandled);
			}
			catch
			{
			}
			base.WndProc(ref m);
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000AD10 File Offset: 0x00008F10
		private void Form1ShiBoQi_Load(object sender, EventArgs e)
		{
			base.Icon = Resources.ScopeMeter;
			this.DmmAutorecordTextBox.Dock = DockStyle.Fill;
			this.macheType2 = Form1ShiBoQi.MachineType2.NULL;
			this.dmmMode = Form1ShiBoQi.DMM_Mode.DMM_realTimeData;
			this.dsoMode = Form1ShiBoQi.DSO_MODE.OldDrawingData;
			this.dsoXLinearMode = Form1ShiBoQi.DSOXLinearMode.logarithm;
			this.minMultimeterData.tail = " ";
			Form1ShiBoQi.MultimeterDisplayData obj = this.multimeterDisplayData;
			lock (obj)
			{
				this.multimeterDisplayData.Init();
			}
			this.initalizeLED();
			this.ListOfUSBDeviceProperties = new List<USBClass.DeviceProperties>();
			this.USBPort.USBDeviceAttached += this.USBPort_USBDeviceAttached;
			this.USBPort.USBDeviceRemoved += this.USBPort_USBDeviceRemoved;
			this.USBPort.RegisterForDeviceChange(true, base.Handle);
			this.currentCOMID = -1;
			this.USBPort_USBDeviceAttached(null, null);
			bool succeed = this.currentCOMID != -1;
			if (!succeed)
			{
				foreach (string port in this.getSerialPorts())
				{
					succeed = this.checkCOM(port);
					if (succeed)
					{
						break;
					}
				}
			}
			if (succeed)
			{
				this.checkCOM("COM" + this.currentCOMID.ToString());
				this.toolStripStatusLabel1.Text = "COM" + this.currentCOMID.ToString();
			}
			else
			{
				MessageBox.Show("Failed to connect to SerialPort！");
				this.通讯状态.Text = "Failed to connect to SerialPort";
				this.panel4.Dock = DockStyle.Fill;
				this.GraphControl1.Visible = false;
				this.panel4.Invalidate();
			}
			this.getParaFromReg();
			this.toolStrip1.Dock = DockStyle.Top;
			this.textBox2.Dock = DockStyle.Fill;
			this.DmmAutorecordTextBox.Visible = false;
			this.machineType = "ET521A";
			this.getAllPointsSizes();
			string str = Environment.CurrentDirectory.ToString() + "\\欧亚集团伊万科技---产品园地.mht";
			try
			{
				this.webBrowser1.Url = new Uri(str, UriKind.RelativeOrAbsolute);
			}
			catch (ObjectDisposedException)
			{
				base.Close();
				return;
			}
			this.textBox2.Dock = DockStyle.Fill;
			this.panel1.Dock = DockStyle.Fill;
			this.webBrowser1.Dock = DockStyle.Fill;
			base.CancelButton = this.button1;
			this.toolStrip1.Dock = DockStyle.Top;
			base.CenterToScreen();
			this.ToDMMGraphControl();
			this.OnResize(this, new EventArgs());
			base.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			if (File.Exists(".\\data"))
			{
				StreamReader streamReader = File.OpenText(".\\data");
				string x_ratio = streamReader.ReadLine();
				string y_ratio = streamReader.ReadLine();
				this.calibration_ratio_x = float.Parse(x_ratio);
				this.calibration_ratio_y = float.Parse(y_ratio);
				streamReader.Close();
				if ((double)this.calibration_ratio_x < 1E-06)
				{
					this.calibration_ratio_x = 1f;
				}
				if ((double)this.calibration_ratio_y < 1E-06)
				{
					this.calibration_ratio_y = 1f;
				}
			}
			this.label2.Text = "";
			this.label4.Text = "";
			this.label5.Text = "";
			this.axLED13.setNum(12);
			if (this.macheType2 == Form1ShiBoQi.MachineType2.DSO)
			{
				this.changeToPanel4Mode();
			}
			this.getAllPointsSizes();
			if (succeed)
			{
				this.StartThreads();
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000022D4 File Offset: 0x000004D4
		private void USBPort_USBDeviceRemoved(object sender, USBClass.USBDeviceEventArgs e)
		{
		}

		// Token: 0x0600008D RID: 141 RVA: 0x0000B06C File Offset: 0x0000926C
		private void USBPort_USBDeviceAttached(object sender, USBClass.USBDeviceEventArgs e)
		{
			this.serialPort1.Parity = Parity.None;
			this.serialPort1.StopBits = StopBits.One;
			this.serialPort1.DataBits = 8;
			List<USBClass.DeviceProperties> ls = new List<USBClass.DeviceProperties>();
			USBClass.GetUSBDevice(6790u, 29987u, ref ls, true, null);
			if (ls.Count > 0)
			{
				this.serialPort1.BaudRate = 57600;
			}
			else
			{
				USBClass.GetUSBDevice(11912u, 17923u, ref ls, true, null);
				this.serialPort1.BaudRate = 115200;
			}
			if (ls.Count > 0)
			{
				if (ls[0].COMPort == "")
				{
					this.serialPort1.PortName = CRGE.findComHuada();
				}
				else
				{
					this.serialPort1.PortName = ls[0].COMPort;
				}
				try
				{
					this.serialPort1.Close();
					this.serialPort1.Open();
					this.currentCOMID = int.Parse(this.serialPort1.PortName.Substring(3));
					this.toolStripStatusLabel1.Text = "COM" + this.currentCOMID.ToString();
				}
				catch
				{
					this.currentCOMID = -1;
				}
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x0000B1C4 File Offset: 0x000093C4
		private void OnResize(object sender, EventArgs e)
		{
			Form con = sender as Form;
			int data = 0;
			if (base.WindowState != FormWindowState.Maximized)
			{
				data = this.toolStrip1.Size.Height + this.menuStrip1.Size.Height;
				this.menuStrip1.Visible = true;
				this.toolStrip1.Visible = true;
			}
			else
			{
				this.menuStrip1.Visible = false;
				this.toolStrip1.Visible = false;
			}
			int h = con.Size.Height - data - this.statusStrip1.Size.Height - 40;
			this.splitContainer1.Location = new Point(0, data);
			this.splitContainer1.Size = new Size(con.Size.Width, h);
			this.button6.Location = new Point(0, this.panel2.Height - this.button3.Height);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000025A9 File Offset: 0x000007A9
		private void toolStripMenuItem3_Click(object sender, EventArgs e)
		{
			this.SaveAs();
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000B2C0 File Offset: 0x000094C0
		private void SaveAs()
		{
			if (this.macheType2 == Form1ShiBoQi.MachineType2.DMM)
			{
				switch (this.dmmMode)
				{
				case Form1ShiBoQi.DMM_Mode.DMM_realTimeData:
					this.saveFileDialog1.Filter = "DMM RealTime Data(*.txt)|*.txt";
					this.saveFileDialog1.Title = "DMM RealTime Data Save";
					break;
				case Form1ShiBoQi.DMM_Mode.AutoRecoreData:
					this.saveFileDialog1.Filter = "DMM AutoRecord Data(*.txt)|*.txt";
					this.saveFileDialog1.Title = "DMM AutoRecored Data Save";
					break;
				case Form1ShiBoQi.DMM_Mode.HoldData:
					this.saveFileDialog1.Filter = "DMM Hold Data(*.txt)|*.txt";
					this.saveFileDialog1.Title = "DMM Hold Data Save";
					break;
				case Form1ShiBoQi.DMM_Mode.Calibation:
					this.saveFileDialog1.Filter = "DMM Calibration Data(*.txt)|*.txt";
					this.saveFileDialog1.Title = "DMM Calibration Data Save";
					break;
				case Form1ShiBoQi.DMM_Mode.NULL:
					if (this.oldMode == Form1ShiBoQi.DMM_Mode.AutoRecoreData)
					{
						this.saveFileDialog1.Filter = "DMM AutoRecord Data(*.txt)|*.txt";
						this.saveFileDialog1.Title = "DMM AutoRecored Data Save";
					}
					break;
				}
			}
			else
			{
				switch (this.dsoMode)
				{
				case Form1ShiBoQi.DSO_MODE.OldDrawingData:
					this.saveFileDialog1.Filter = "DSO Old Drawing Data(*.dat)|*.dat";
					this.saveFileDialog1.Title = "DSO OLD Drawing Data Save";
					break;
				case Form1ShiBoQi.DSO_MODE.RealtimeData:
					this.saveFileDialog1.Filter = "DSO RealTime Data(*.dat)|*.dat";
					this.saveFileDialog1.Title = "DSO RealTime Data Save";
					break;
				case Form1ShiBoQi.DSO_MODE.RealTimeDataForAnalyse:
					this.saveFileDialog1.Filter = "DSO RealTime Data For Analyse(*.dat)|*.dat";
					this.saveFileDialog1.Title = "DSO RealTime Data For Analyse Data Save";
					break;
				}
			}
			if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				this.FileFath = this.saveFileDialog1.FileName;
				this.toolStripButton1_Click(new object(), new EventArgs());
			}
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000022D4 File Offset: 0x000004D4
		private void pictureBox13_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00002525 File Offset: 0x00000725
		private void pictureBox13_MouseUp(object sender, MouseEventArgs e)
		{
			this.mouseIsDown = false;
			this.clickButtonID = -1;
			this.panel4.Invalidate();
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000257C File Offset: 0x0000077C
		private void hScrollBar1_ValueChanged(object sender, EventArgs e)
		{
			this.panel4.Invalidate();
		}

		// Token: 0x06000094 RID: 148 RVA: 0x0000B474 File Offset: 0x00009674
		private void kbfft(double[] pr, double[] pi, int n, int k, double[] fr, double[] fi, int l, int il)
		{
			int i;
			for (int it = 0; it <= n - 1; it++)
			{
				i = it;
				int isa = 0;
				for (int j = 0; j <= k - 1; j++)
				{
					int m = i / 2;
					isa = 2 * isa + (i - 2 * m);
					i = m;
				}
				fr[it] = pr[isa];
				fi[it] = pi[isa];
			}
			pr[0] = 1.0;
			pi[0] = 0.0;
			double p = 6.283185306 / (1.0 * (double)n);
			pr[1] = Math.Cos(p);
			pi[1] = -Math.Sin(p);
			if (l != 0)
			{
				pi[1] = -pi[1];
			}
			for (int j = 2; j < n - 1; j++)
			{
				p = pr[j - 1] * pr[1];
				double q = pi[j - 1] * pi[1];
				double s = (pr[j - 1] + pi[j - 1]) * (pr[1] + pi[1]);
				pr[j] = p - q;
				pi[j] = s - p - q;
			}
			for (int it = 0; it <= n - 2; it += 2)
			{
				double vr = fr[it];
				double vi = fi[it];
				fr[it] = vr + fr[it + 1];
				fi[it] = vi + fi[it + 1];
				fr[it + 1] = vr - fr[it + 1];
				fi[it + 1] = vi - fi[it + 1];
			}
			i = n / 2;
			int nv = 2;
			for (int l2 = k - 2; l2 >= 0; l2--)
			{
				i /= 2;
				nv = 2 * nv;
				for (int it = 0; it <= (i - 1) * nv; it += nv)
				{
					for (int m = 0; m <= nv / 2 - 1; m++)
					{
						p = pr[i * m] * fr[it + m + nv / 2];
						double q = pi[i * m] * fi[it + m + nv / 2];
						double s = pr[i * m] + pi[i * m];
						s *= fr[it + m + nv / 2] + fi[it + m + nv / 2];
						double poddr = p - q;
						double poddi = s - p - q;
						fr[it + m + nv / 2] = fr[it + m] - poddr;
						fi[it + m + nv / 2] = fi[it + m] - poddi;
						fr[it + m] = fr[it + m] + poddr;
						fi[it + m] = fi[it + m] + poddi;
					}
				}
			}
			if (l != 0)
			{
				for (int j = 0; j <= n - 1; j++)
				{
					fr[j] /= 1.0 * (double)n;
					fi[j] /= 1.0 * (double)n;
				}
			}
			if (il != 0)
			{
				for (int j = 0; j <= n - 1; j++)
				{
					pr[j] = Math.Sqrt(fr[j] * fr[j] + fi[j] * fi[j]);
					if (Math.Abs(fr[j]) < 1E-06 * Math.Abs(fi[j]))
					{
						if (fi[j] * fr[j] > 0.0)
						{
							pi[j] = 90.0;
						}
						else
						{
							pi[j] = -90.0;
						}
					}
					else
					{
						pi[j] = Math.Atan(fi[j] / fr[j]) * 360.0 / 6.283185306;
					}
				}
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000025B1 File Offset: 0x000007B1
		private void 帮助主题ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Process
			{
				StartInfo = 
				{
					FileName = "PC521 HELP.mht",
					Arguments = "PC521 HELP.mht",
					UseShellExecute = true
				}
			}.Start();
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000025EA File Offset: 0x000007EA
		private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new AboutBox1().Show();
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000025F6 File Offset: 0x000007F6
		private void splitContainer2_Panel1_Resize(object sender, EventArgs e)
		{
			this.GraphControl1.Invalidate();
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000B7AC File Offset: 0x000099AC
		private void SaveAsJPG(object sender, EventArgs e)
		{
			this.saveFileDialog1.Filter = "Bitmap(*.bmp)|*.bmp";
			this.saveFileDialog1.Title = "Bitmap Save";
			if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				string filename = this.saveFileDialog1.FileName;
				Bitmap bitmap = new Bitmap(1, 1);
				if (this.GraphControl1.Visible)
				{
					bitmap = new Bitmap(this.GraphControl1.Bounds.Width, this.GraphControl1.Bounds.Height);
					this.GraphControl1.DrawToBitmap(bitmap, this.GraphControl1.Bounds);
				}
				else if (this.panel4.Visible)
				{
					bitmap = new Bitmap(this.panel4.Width, this.panel4.Height);
					this.panel4.DrawToBitmap(bitmap, this.panel4.Bounds);
				}
				bitmap.Save(filename);
			}
		}

		// Token: 0x0400000C RID: 12
		private string[] timeBaseS = new string[]
		{
			"",
			"5nS",
			"10nS",
			"25nS",
			"50nS",
			"100ns",
			"200ns",
			"500nS",
			"1μS",
			"2μS",
			"5μS",
			"10μS",
			"20μS",
			"50μS",
			"100μS",
			"200μS",
			"500μS",
			"1mS",
			"2mS",
			"5mS",
			"10mS",
			"20mS",
			"50mS",
			"100mS",
			" 200mS",
			"500mS",
			"1S",
			"2S",
			"5S",
			"10S",
			"20S",
			"50S"
		};

		// Token: 0x0400000D RID: 13
		private double[] timeBaseF = new double[]
		{
			0.0,
			5E-09,
			1E-08,
			2.5E-08,
			5E-08,
			1E-07,
			2E-07,
			5E-07,
			1E-06,
			2E-06,
			5E-06,
			1E-05,
			2E-05,
			5E-05,
			0.0001,
			0.0002,
			0.0005,
			0.001,
			0.002,
			0.005,
			0.01,
			0.02,
			0.05,
			0.1,
			0.2,
			0.5,
			1.0,
			2.0,
			5.0,
			10.0,
			20.0,
			50.0
		};

		// Token: 0x0400000E RID: 14
		private string[] volateBaseS = new string[]
		{
			"10mV",
			"20mV",
			"50mV",
			"100mV",
			"200mV",
			"500mV",
			"1V",
			"2V",
			"5V",
			"10V",
			"100mV",
			"200mV",
			"500mV",
			"1V",
			"2V",
			"5V",
			"10V",
			"20V",
			"50V",
			"100V",
			"1V",
			"2V",
			"5V",
			"10V",
			"20V",
			"50V",
			"100V",
			"200V",
			"500V",
			"1000V"
		};

		// Token: 0x0400000F RID: 15
		private double[] voltaBaseF = new double[]
		{
			0.01,
			0.02,
			0.05,
			0.1,
			0.2,
			0.5,
			1.0,
			2.0,
			5.0,
			10.0,
			0.1,
			0.2,
			0.5,
			1.0,
			2.0,
			5.0,
			10.0,
			20.0,
			50.0,
			100.0,
			1.0,
			2.0,
			5.0,
			10.0,
			20.0,
			50.0,
			100.0,
			200.0,
			500.0,
			1000.0
		};

		// Token: 0x04000010 RID: 16
		private int tempx;

		// Token: 0x04000011 RID: 17
		private bool bValid;

		// Token: 0x04000012 RID: 18
		private bool bReturn;

		// Token: 0x04000013 RID: 19
		private double minNumber;

		// Token: 0x04000014 RID: 20
		private double maxNumber;

		// Token: 0x04000015 RID: 21
		private double currentNumber;

		// Token: 0x04000016 RID: 22
		private string[] spaceNum = new string[]
		{
			" ",
			"  ",
			"   "
		};

		// Token: 0x04000017 RID: 23
		private LED[] ledMin = new LED[4];

		// Token: 0x04000018 RID: 24
		private PictureBox[] ledMinPoint = new PictureBox[4];

		// Token: 0x04000019 RID: 25
		private LED[] ledCurrent = new LED[4];

		// Token: 0x0400001A RID: 26
		private PictureBox[] ledCurrentPoint = new PictureBox[4];

		// Token: 0x0400001B RID: 27
		private LED[] ledMax = new LED[4];

		// Token: 0x0400001C RID: 28
		private PictureBox[] ledMaxPoint = new PictureBox[4];

		// Token: 0x0400001D RID: 29
		private Color backColor;

		// Token: 0x0400001E RID: 30
		private string textbox2string = "";

		// Token: 0x0400001F RID: 31
		public Form1ShiBoQi.MultimeterData minMultimeterData = new Form1ShiBoQi.MultimeterData();

		// Token: 0x04000020 RID: 32
		public Form1ShiBoQi.MultimeterData maxMultimeterData = new Form1ShiBoQi.MultimeterData();

		// Token: 0x04000021 RID: 33
		public Form1ShiBoQi.MultimeterData currentMultimeterData = new Form1ShiBoQi.MultimeterData();

		// Token: 0x04000022 RID: 34
		public Form1ShiBoQi.MultimeterData oldMultimeterData = new Form1ShiBoQi.MultimeterData();

		// Token: 0x04000023 RID: 35
		private byte newData = 1;

		// Token: 0x04000024 RID: 36
		private bool dataSaved = true;

		// Token: 0x04000025 RID: 37
		private bool over_pub;

		// Token: 0x04000026 RID: 38
		private byte currentMode;

		// Token: 0x04000027 RID: 39
		private int oldScaleYmin;

		// Token: 0x04000028 RID: 40
		private float oldScaleYmax = 100f;

		// Token: 0x04000029 RID: 41
		private Form1ShiBoQi.MultimeterData oldMultiMachineData = new Form1ShiBoQi.MultimeterData();

		// Token: 0x0400002A RID: 42
		private Form1ShiBoQi.FrontType oldMultiMachintType = Form1ShiBoQi.FrontType.nullType;

		// Token: 0x0400002B RID: 43
		private string oldTail = "";

		// Token: 0x0400002C RID: 44
		private float maxscale = 10f;

		// Token: 0x0400002D RID: 45
		private float minLedVale;

		// Token: 0x0400002E RID: 46
		private float maxLedValue;

		// Token: 0x0400002F RID: 47
		private float currentLedValue;

		// Token: 0x04000030 RID: 48
		private Form1ShiBoQi.FUC oldFunc;

		// Token: 0x04000031 RID: 49
		private int[] pointNumArray = new int[]
		{
			1,
			10,
			100,
			1000,
			10000,
			100000,
			1000000
		};

		// Token: 0x04000032 RID: 50
		private Form1ShiBoQi.DSOWindowMode currentWindowMode;

		// Token: 0x04000033 RID: 51
		private float calibration_ratio_x = 15f;

		// Token: 0x04000034 RID: 52
		private float calibration_ratio_y = 0.0001f;

		// Token: 0x04000035 RID: 53
		private USBClass USBPort = new USBClass();

		// Token: 0x04000036 RID: 54
		private List<USBClass.DeviceProperties> ListOfUSBDeviceProperties;

		// Token: 0x04000037 RID: 55
		private const uint MyDeviceVID = 49745u;

		// Token: 0x04000038 RID: 56
		private const uint MyDevicePID = 61452u;

		// Token: 0x04000039 RID: 57
		private System.Timers.Timer timer定时测量 = new System.Timers.Timer();

		// Token: 0x0400003A RID: 58
		private LineItem myCurve = new LineItem("");

		// Token: 0x0400003B RID: 59
		private LineItem myCurveYellow = new LineItem("");

		// Token: 0x0400003C RID: 60
		private Form1ShiBoQi.DSO_DATAS dsoData1 = new Form1ShiBoQi.DSO_DATAS();

		// Token: 0x0400003D RID: 61
		private string path;

		// Token: 0x0400003E RID: 62
		private Point[] controlPoints;

		// Token: 0x0400003F RID: 63
		private Size[] controlSizes;

		// Token: 0x04000040 RID: 64
		private float[] fontSize;

		// Token: 0x04000041 RID: 65
		private Point panel1Point;

		// Token: 0x04000042 RID: 66
		private Size panel1Size;

		// Token: 0x04000043 RID: 67
		private Form1ShiBoQi.MachineType2 macheType2;

		// Token: 0x04000044 RID: 68
		private Form1ShiBoQi.DSO_MODE dsoMode;

		// Token: 0x04000045 RID: 69
		private Form1ShiBoQi.DSOWindowMode dsoWindowMode;

		// Token: 0x04000046 RID: 70
		private Form1ShiBoQi.DSOXLinearMode dsoXLinearMode;

		// Token: 0x04000047 RID: 71
		private Form1ShiBoQi.DMM_Mode dmmMode;

		// Token: 0x04000048 RID: 72
		private static bool b;

		// Token: 0x04000049 RID: 73
		private string machineType;

		// Token: 0x0400004A RID: 74
		private Font textFont = new Font("宋体", 0.2f, FontStyle.Regular, GraphicsUnit.Point, 134);

		// Token: 0x0400004B RID: 75
		private Brush blackBrush = Brushes.White;

		// Token: 0x0400004C RID: 76
		private Brush blueBrush = Brushes.Blue;

		// Token: 0x0400004D RID: 77
		private Pen peBlack = new Pen(Brushes.White, 0.001f);

		// Token: 0x0400004E RID: 78
		private Pen peGray = new Pen(Brushes.DimGray, 0.001f);

		// Token: 0x0400004F RID: 79
		private Pen peGrayN = new Pen(Brushes.Red, 0.001f);

		// Token: 0x04000050 RID: 80
		private Pen peRed = new Pen(Brushes.Red, 0.001f);

		// Token: 0x04000051 RID: 81
		private string 平移 = "向上平移";

		// Token: 0x04000052 RID: 82
		private float 平移V;

		// Token: 0x04000053 RID: 83
		private int timeBase = 2;

		// Token: 0x04000054 RID: 84
		private int widthBase = 2;

		// Token: 0x04000055 RID: 85
		private PointF[] multimeterdata = new PointF[40];

		// Token: 0x04000056 RID: 86
		private Pen p = new Pen(Brushes.White, 1f);

		// Token: 0x04000057 RID: 87
		private string degree = "nμm kM";

		// Token: 0x04000058 RID: 88
		private Pen peGreen = new Pen(Brushes.Red);

		// Token: 0x04000059 RID: 89
		private Pen peYellow = new Pen(Brushes.Yellow, 0.001f);

		// Token: 0x0400005A RID: 90
		private int currentCOMID = -1;

		// Token: 0x0400005B RID: 91
		private object serialportObject = new object();

		// Token: 0x0400005C RID: 92
		private byte commandParamter;

		// Token: 0x0400005D RID: 93
		private Queue<byte[]> packageQueue = new Queue<byte[]>();

		// Token: 0x0400005E RID: 94
		private Thread getDataThread = new Thread(new ThreadStart(Form1ShiBoQi.emptyFun));

		// Token: 0x0400005F RID: 95
		private Thread daemonThread = new Thread(new ThreadStart(Form1ShiBoQi.emptyFun));

		// Token: 0x04000060 RID: 96
		private byte vacancyTimes;

		// Token: 0x04000061 RID: 97
		private string communicationState;

		// Token: 0x04000062 RID: 98
		private bool bSerialConnected;

		// Token: 0x04000063 RID: 99
		private int intervalTime = 1;

		// Token: 0x04000064 RID: 100
		private Form1ShiBoQi.ThreadType threadType;

		// Token: 0x04000065 RID: 101
		private bool threadRunResult;

		// Token: 0x04000066 RID: 102
		private byte[] saveData = new byte[0];

		// Token: 0x04000067 RID: 103
		private Thread packageHandleThread = new Thread(new ThreadStart(Form1ShiBoQi.emptyFun));

		// Token: 0x04000068 RID: 104
		private Thread demonSaveThread = new Thread(new ThreadStart(Form1ShiBoQi.emptyFun));

		// Token: 0x04000069 RID: 105
		private Thread messageProcessThread = new Thread(new ThreadStart(Form1ShiBoQi.emptyFun));

		// Token: 0x0400006A RID: 106
		private Thread currentThread = new Thread(new ThreadStart(Form1ShiBoQi.emptyFun));

		// Token: 0x0400006B RID: 107
		private int listNum;

		// Token: 0x0400006C RID: 108
		private int numx;

		// Token: 0x0400006D RID: 109
		private float oldMaxScale;

		// Token: 0x0400006E RID: 110
		private bool addElement;

		// Token: 0x0400006F RID: 111
		private float oldScale;

		// Token: 0x04000070 RID: 112
		private int num2;

		// Token: 0x04000071 RID: 113
		private float XAxisIncData = 1f;

		// Token: 0x04000072 RID: 114
		private Form1ShiBoQi.DMM_Mode oldMode = Form1ShiBoQi.DMM_Mode.NULL;

		// Token: 0x04000073 RID: 115
		private Form1ShiBoQi.MultimeterDisplayData multimeterDisplayData = new Form1ShiBoQi.MultimeterDisplayData();

		// Token: 0x04000074 RID: 116
		private string[] caliString = new string[]
		{
			"DC660mV",
			"DC6V6",
			"DC66V",
			"DC660V",
			"DC2000V",
			"AC660mV",
			"AC6V6",
			"AC66V",
			"AC660V",
			"AC2000V",
			"DC66mA",
			"DC660mA",
			"AC66mA",
			"AC660mA",
			"R660",
			"R6K",
			"R66K",
			"R660K",
			"R6M6",
			"R66M",
			"C6n6",
			"C66n",
			"C660n",
			"c6u6",
			"C66u",
			"C66u",
			"C6m6",
			"C66m",
			"Res1",
			"Res2",
			"H660",
			"H6K6",
			"H66K",
			"H660K",
			"H6M6",
			"H66M",
			"Res3",
			"Res4",
			"Res5",
			"HLxVrms"
		};

		// Token: 0x04000075 RID: 117
		private bool addToright;

		// Token: 0x04000076 RID: 118
		private int splitterDistance1;

		// Token: 0x04000077 RID: 119
		private int splitterDistance2;

		// Token: 0x04000078 RID: 120
		private bool maxScreen;

		// Token: 0x04000079 RID: 121
		private bool mouseIsDown;

		// Token: 0x0400007A RID: 122
		private Point mousePoint;

		// Token: 0x0400007B RID: 123
		private int clickButtonID = -1;

		// Token: 0x0400007C RID: 124
		private Button[] buttons = new Button[4];

		// Token: 0x0400007D RID: 125
		private bool holdScreen;

		// Token: 0x0400007E RID: 126
		private string FileFath = string.Empty;

		// Token: 0x02000006 RID: 6
		public enum MultimeterType
		{
			// Token: 0x04000128 RID: 296
			ACvoltage,
			// Token: 0x04000129 RID: 297
			ACcurrent,
			// Token: 0x0400012A RID: 298
			dinode,
			// Token: 0x0400012B RID: 299
			frequency,
			// Token: 0x0400012C RID: 300
			DCvoltage,
			// Token: 0x0400012D RID: 301
			DCcurrent,
			// Token: 0x0400012E RID: 302
			resistance,
			// Token: 0x0400012F RID: 303
			Lx,
			// Token: 0x04000130 RID: 304
			Cx,
			// Token: 0x04000131 RID: 305
			OSC,
			// Token: 0x04000132 RID: 306
			Percent,
			// Token: 0x04000133 RID: 307
			FengMen,
			// Token: 0x04000134 RID: 308
			Null
		}

		// Token: 0x02000007 RID: 7
		public enum FrontType
		{
			// Token: 0x04000136 RID: 310
			dinode,
			// Token: 0x04000137 RID: 311
			fengmi,
			// Token: 0x04000138 RID: 312
			nullType,
			// Token: 0x04000139 RID: 313
			percent,
			// Token: 0x0400013A RID: 314
			ac,
			// Token: 0x0400013B RID: 315
			DC,
			// Token: 0x0400013C RID: 316
			duty,
			// Token: 0x0400013D RID: 317
			capcity,
			// Token: 0x0400013E RID: 318
			CONST,
			// Token: 0x0400013F RID: 319
			RES
		}

		// Token: 0x02000008 RID: 8
		public enum compareResult
		{
			// Token: 0x04000141 RID: 321
			bigger,
			// Token: 0x04000142 RID: 322
			smaller,
			// Token: 0x04000143 RID: 323
			equal,
			// Token: 0x04000144 RID: 324
			notCompareable
		}

		// Token: 0x02000009 RID: 9
		public class MultimeterData
		{
			// Token: 0x0600009C RID: 156 RVA: 0x00002622 File Offset: 0x00000822
			public bool getSameType(Form1ShiBoQi.MultimeterData cd)
			{
				return this.m_type == cd.m_type;
			}

			// Token: 0x0600009D RID: 157 RVA: 0x0000F754 File Offset: 0x0000D954
			public Form1ShiBoQi.MultimeterData toEqual(Form1ShiBoQi.MultimeterData cd)
			{
				this.auto_mode = cd.auto_mode;
				this.data = cd.data;
				this.dots = (bool[])cd.dots.Clone();
				this.frontType = cd.frontType;
				this.plus = cd.plus;
				this.tail = (string)cd.tail.Clone();
				this.bValid = cd.bValid;
				this.floatValue = cd.floatValue;
				this.pointNum = cd.pointNum;
				return this;
			}

			// Token: 0x0600009E RID: 158 RVA: 0x0000F7E4 File Offset: 0x0000D9E4
			public MultimeterData()
			{
				this.m_type = Form1ShiBoQi.MultimeterType.Null;
				this.frontType = Form1ShiBoQi.FrontType.nullType;
				this.plus = true;
				this.data = 0;
				this.dots = new bool[3];
				this.dots[0] = (this.dots[1] = (this.dots[2] = false));
				this.tail = null;
				this.auto_mode = true;
			}

			// Token: 0x0600009F RID: 159 RVA: 0x0000F850 File Offset: 0x0000DA50
			public float getUnitPara()
			{
				char rw = this.tail[0];
				if (rw <= 'M')
				{
					if (rw != 'K')
					{
						if (rw != 'M')
						{
							goto IL_5D;
						}
						return 1000000f;
					}
				}
				else
				{
					switch (rw)
					{
					case 'k':
						break;
					case 'l':
						goto IL_5D;
					case 'm':
						return 0.001f;
					case 'n':
						return 1E-09f;
					default:
						if (rw == 'μ')
						{
							return 1E-06f;
						}
						goto IL_5D;
					}
				}
				return 1000f;
				IL_5D:
				return 1f;
			}

			// Token: 0x060000A0 RID: 160 RVA: 0x0000F8C0 File Offset: 0x0000DAC0
			public Form1ShiBoQi.compareResult Comparer(Form1ShiBoQi.MultimeterData data)
			{
				if (!this.bValid || !data.bValid)
				{
					return Form1ShiBoQi.compareResult.notCompareable;
				}
				if (this.floatValue < data.floatValue)
				{
					return Form1ShiBoQi.compareResult.smaller;
				}
				if (this.floatValue > data.floatValue)
				{
					return Form1ShiBoQi.compareResult.bigger;
				}
				return Form1ShiBoQi.compareResult.equal;
			}

			// Token: 0x060000A1 RID: 161 RVA: 0x0000F900 File Offset: 0x0000DB00
			public MultimeterData(Form1ShiBoQi.FrontType frontType_, bool plus_, int data_, byte dotPos, bool auto_, string tail_)
			{
				this.frontType = frontType_;
				this.plus = plus_;
				this.data = data_;
				this.dots = new bool[3];
				this.dots[(int)dotPos] = true;
				this.tail = (string)tail_.Clone();
				this.auto_mode = auto_;
			}

			// Token: 0x060000A2 RID: 162 RVA: 0x0000F958 File Offset: 0x0000DB58
			public float getfloatDataWithoutUnit()
			{
				return this.floatValue;
			}

			// Token: 0x060000A3 RID: 163 RVA: 0x00002632 File Offset: 0x00000832
			public float getfloatData()
			{
				return this.getfloatDatax((float)this.data);
			}

			// Token: 0x060000A4 RID: 164 RVA: 0x0000F96C File Offset: 0x0000DB6C
			private float getfloatDatax(float da1)
			{
				float pName = da1;
				int id = 0;
				while (id < 3 && !this.dots[id])
				{
					id++;
				}
				switch (id)
				{
				case 0:
					pName /= 1000f;
					break;
				case 1:
					pName /= 100f;
					break;
				case 2:
					pName /= 10f;
					break;
				}
				if (this.tail != null && this.tail.Length > 0)
				{
					char c = this.tail[0];
					if (c <= 'M')
					{
						if (c != 'K')
						{
							if (c != 'M')
							{
								goto IL_CF;
							}
							pName *= 1000000f;
							goto IL_CF;
						}
					}
					else
					{
						switch (c)
						{
						case 'k':
							break;
						case 'l':
							goto IL_CF;
						case 'm':
							pName *= 0.001f;
							goto IL_CF;
						case 'n':
							pName *= 1E-09f;
							goto IL_CF;
						default:
							if (c != 'μ')
							{
								goto IL_CF;
							}
							pName *= 1E-06f;
							goto IL_CF;
						}
					}
					pName *= 1000f;
				}
				IL_CF:
				if (!this.plus)
				{
					return -pName;
				}
				return pName;
			}

			// Token: 0x060000A5 RID: 165 RVA: 0x0000FA54 File Offset: 0x0000DC54
			public float getMaxScaleWithoutUnit()
			{
				float num = 0f;
				int num2 = 0;
				while (num2 < 3 && !this.dots[num2])
				{
					num2++;
				}
				switch (num2)
				{
				case 0:
					num /= 1000f;
					break;
				case 1:
					num /= 100f;
					break;
				case 2:
					num /= 10f;
					break;
				}
				return num;
			}

			// Token: 0x060000A6 RID: 166 RVA: 0x0000FAB0 File Offset: 0x0000DCB0
			public float getMaxScale()
			{
				float da = 10000f;
				return Math.Abs(this.getfloatDatax(da));
			}

			// Token: 0x060000A7 RID: 167 RVA: 0x0000FAD0 File Offset: 0x0000DCD0
			public string toString()
			{
				if (!this.bValid)
				{
					return "0L";
				}
				return this.floatValue.ToString("F" + (4 - this.pointNum).ToString()) + this.tail;
			}

			// Token: 0x060000A8 RID: 168 RVA: 0x0000FB1C File Offset: 0x0000DD1C
			public void showLED(LED[] led, PictureBox[] box, System.Windows.Forms.Label tailLable)
			{
				if (!this.plus)
				{
					box[3].Show();
				}
				else
				{
					box[3].Hide();
				}
				tailLable.Text = this.tail;
				if (!this.bValid)
				{
					led[0].setNum(12);
					led[1].setNum(0);
					led[2].setNum(11);
					led[3].setNum(12);
					for (int i = 0; i < box.Length; i++)
					{
						box[i].Hide();
					}
					return;
				}
				string text = Math.Abs(this.floatValue).ToString("F4");
				int j;
				for (j = text.IndexOf('.'); j < this.pointNum; j++)
				{
					text = "0" + text;
				}
				if (j > 0)
				{
					text = text.Remove(j, 1);
				}
				for (int k = 0; k < text.Length; k++)
				{
					try
					{
						led[k].setNum((short)(text[k] - '0'));
					}
					catch
					{
					}
				}
				for (int l = 0; l < 3; l++)
				{
					if (l + 1 == j)
					{
						box[l].Show();
					}
					else
					{
						box[l].Hide();
					}
				}
			}

			// Token: 0x04000145 RID: 325
			public Form1ShiBoQi.FrontType frontType;

			// Token: 0x04000146 RID: 326
			public bool plus;

			// Token: 0x04000147 RID: 327
			public int data;

			// Token: 0x04000148 RID: 328
			public bool[] dots;

			// Token: 0x04000149 RID: 329
			public bool auto_mode;

			// Token: 0x0400014A RID: 330
			public string tail;

			// Token: 0x0400014B RID: 331
			public bool bValid;

			// Token: 0x0400014C RID: 332
			public Form1ShiBoQi.MultimeterType m_type;

			// Token: 0x0400014D RID: 333
			public float floatValue;

			// Token: 0x0400014E RID: 334
			public float VBase;

			// Token: 0x0400014F RID: 335
			public int pointNum;
		}

		// Token: 0x0200000A RID: 10
		public enum FUC : byte
		{
			// Token: 0x04000151 RID: 337
			NULL,
			// Token: 0x04000152 RID: 338
			dcvoltage = 5,
			// Token: 0x04000153 RID: 339
			acVoltage,
			// Token: 0x04000154 RID: 340
			Res,
			// Token: 0x04000155 RID: 341
			cont = 9,
			// Token: 0x04000156 RID: 342
			diode,
			// Token: 0x04000157 RID: 343
			capacity,
			// Token: 0x04000158 RID: 344
			dcMa = 14,
			// Token: 0x04000159 RID: 345
			acma,
			// Token: 0x0400015A RID: 346
			DCA,
			// Token: 0x0400015B RID: 347
			ACA,
			// Token: 0x0400015C RID: 348
			HZ,
			// Token: 0x0400015D RID: 349
			duty
		}

		// Token: 0x0200000B RID: 11
		private class DMM_RECORD
		{
			// Token: 0x060000A9 RID: 169 RVA: 0x00002641 File Offset: 0x00000841
			public float getValue()
			{
				return this.fvalue;
			}

			// Token: 0x060000AA RID: 170 RVA: 0x0000FC54 File Offset: 0x0000DE54
			public DMM_RECORD(byte[] bys, int pos = 0)
			{
				this.value = BitConverter.ToInt32(bys, pos);
				this.range = bys[4 + pos];
				this.Function = (Form1ShiBoQi.FUC)bys[5 + pos];
				this.PointNum = bys[6 + pos];
				this.StatusBar = bys[7 + pos];
				this.rangeOUt = bys[8 + pos];
				this.fvalue = (float)((double)this.value / Math.Pow(10.0, (double)this.PointNum));
				this.Vbase = 1f;
				switch (this.Function)
				{
				case Form1ShiBoQi.FUC.dcvoltage:
				case Form1ShiBoQi.FUC.acVoltage:
					if (this.range == 0)
					{
						this.Vbase = 0.001f;
						return;
					}
					this.Vbase = 1f;
					return;
				case Form1ShiBoQi.FUC.Res:
					if (this.range == 0)
					{
						this.Vbase = 1f;
						return;
					}
					if (this.range <= 3)
					{
						this.Vbase = 1000f;
						return;
					}
					this.Vbase = 1000000f;
					return;
				case (Form1ShiBoQi.FUC)8:
				case Form1ShiBoQi.FUC.cont:
				case Form1ShiBoQi.FUC.diode:
				case (Form1ShiBoQi.FUC)12:
				case (Form1ShiBoQi.FUC)13:
				case Form1ShiBoQi.FUC.DCA:
				case Form1ShiBoQi.FUC.ACA:
				case Form1ShiBoQi.FUC.duty:
					break;
				case Form1ShiBoQi.FUC.capacity:
					switch (this.range)
					{
					case 0:
						this.Vbase = 1E-09f;
						return;
					case 1:
						this.Vbase = 1E-06f;
						return;
					case 2:
						this.Vbase = 1E-06f;
						return;
					case 3:
						this.Vbase = 0.001f;
						return;
					default:
						return;
					}
					break;
				case Form1ShiBoQi.FUC.dcMa:
				case Form1ShiBoQi.FUC.acma:
					this.Vbase = 0.001f;
					return;
				case Form1ShiBoQi.FUC.HZ:
					this.Vbase = 1f;
					while (this.fvalue > 1000f)
					{
						this.fvalue /= 1000f;
						this.Vbase *= 1000f;
					}
					break;
				default:
					return;
				}
			}

			// Token: 0x060000AB RID: 171 RVA: 0x0000FE14 File Offset: 0x0000E014
			private string getTail()
			{
				string text = "";
				float vbase = this.Vbase;
				if (vbase != 1E-09f)
				{
					if (vbase != 1E-06f)
					{
						if (vbase != 0.001f)
						{
							if (vbase != 1000f)
							{
								if (vbase == 1000000f)
								{
									text = "M";
								}
							}
							else
							{
								text = "K";
							}
						}
						else
						{
							text = "m";
						}
					}
					else
					{
						text = "u";
					}
				}
				else
				{
					text = "n";
				}
				switch (this.Function)
				{
				case Form1ShiBoQi.FUC.dcvoltage:
				case Form1ShiBoQi.FUC.acVoltage:
				case Form1ShiBoQi.FUC.diode:
					return text + "V";
				case Form1ShiBoQi.FUC.Res:
				case Form1ShiBoQi.FUC.cont:
					return text + "Ω";
				case Form1ShiBoQi.FUC.capacity:
					return text + "F";
				case Form1ShiBoQi.FUC.dcMa:
				case Form1ShiBoQi.FUC.acma:
				case Form1ShiBoQi.FUC.DCA:
				case Form1ShiBoQi.FUC.ACA:
					return text + "A";
				case Form1ShiBoQi.FUC.HZ:
					return text + "Hz";
				}
				return text;
			}

			// Token: 0x060000AC RID: 172 RVA: 0x0000FF04 File Offset: 0x0000E104
			public string toString()
			{
				string arg;
				if (this.rangeOUt == 0)
				{
					arg = this.fvalue.ToString() + this.getTail();
				}
				else if (this.rangeOUt == 1)
				{
					arg = "0L";
				}
				else
				{
					arg = "-0L";
				}
				return string.Format("{0} :{1} ", Form1ShiBoQi.FunToString(this.Function), arg);
			}

			// Token: 0x0400015E RID: 350
			public int value;

			// Token: 0x0400015F RID: 351
			public byte range;

			// Token: 0x04000160 RID: 352
			public Form1ShiBoQi.FUC Function;

			// Token: 0x04000161 RID: 353
			public byte PointNum;

			// Token: 0x04000162 RID: 354
			public byte StatusBar;

			// Token: 0x04000163 RID: 355
			public byte rangeOUt;

			// Token: 0x04000164 RID: 356
			public float fvalue;

			// Token: 0x04000165 RID: 357
			public float Vbase;
		}

		// Token: 0x0200000C RID: 12
		private class DSO_DATA
		{
			// Token: 0x060000AD RID: 173 RVA: 0x0000FF60 File Offset: 0x0000E160
			public DSO_DATA()
			{
				this.dso_Data = new Point[600];
				this.VRms = (this.vp0 = (this.vp1 = (this.vpp = (this.Freq = (this.Period = (float)(this.ch = (this.M = 0)))))));
			}

			// Token: 0x04000166 RID: 358
			public bool bValid;

			// Token: 0x04000167 RID: 359
			public Point[] dso_Data;

			// Token: 0x04000168 RID: 360
			public float VRms;

			// Token: 0x04000169 RID: 361
			public float vp0;

			// Token: 0x0400016A RID: 362
			public float vp1;

			// Token: 0x0400016B RID: 363
			public float vpp;

			// Token: 0x0400016C RID: 364
			public int ch;

			// Token: 0x0400016D RID: 365
			public double ch_value;

			// Token: 0x0400016E RID: 366
			public int M;

			// Token: 0x0400016F RID: 367
			public float Freq;

			// Token: 0x04000170 RID: 368
			public float Period;

			// Token: 0x04000171 RID: 369
			public COMPLEX[] TD;
		}

		// Token: 0x0200000D RID: 13
		private enum Channel
		{
			// Token: 0x04000173 RID: 371
			NULL,
			// Token: 0x04000174 RID: 372
			channel0,
			// Token: 0x04000175 RID: 373
			channel1,
			// Token: 0x04000176 RID: 374
			channel01
		}

		// Token: 0x0200000E RID: 14
		private class DSO_DATAS
		{
			// Token: 0x060000AE RID: 174 RVA: 0x00002649 File Offset: 0x00000849
			public DSO_DATAS()
			{
				this.dao = new Form1ShiBoQi.DSO_DATA[2];
				this.dao[0] = new Form1ShiBoQi.DSO_DATA();
				this.dao[1] = new Form1ShiBoQi.DSO_DATA();
			}

			// Token: 0x04000177 RID: 375
			public Form1ShiBoQi.DSO_DATA[] dao;

			// Token: 0x04000178 RID: 376
			public Form1ShiBoQi.Channel channel;

			// Token: 0x04000179 RID: 377
			public int Channel_processing;

			// Token: 0x0400017A RID: 378
			public int timBase;
		}

		// Token: 0x0200000F RID: 15
		private class DSO_DATA1
		{
			// Token: 0x060000AF RID: 175 RVA: 0x0000FFCC File Offset: 0x0000E1CC
			public DSO_DATA1()
			{
				this.hightBase = 1f;
				this.timeBase = 1f;
				this.frequency = 1f;
				this.VPH = (this.VPH = 1f);
				this.VrmsVal = 1f;
				this.dso_Data0 = new Point[600];
				this.dso_Data1 = new Point[600];
			}

			// Token: 0x0400017B RID: 379
			public Point[] dso_Data0;

			// Token: 0x0400017C RID: 380
			public Point[] dso_Data1;

			// Token: 0x0400017D RID: 381
			public bool bA0;

			// Token: 0x0400017E RID: 382
			public bool bA1;

			// Token: 0x0400017F RID: 383
			public float hightBase;

			// Token: 0x04000180 RID: 384
			public float timeBase;

			// Token: 0x04000181 RID: 385
			public float VPH;

			// Token: 0x04000182 RID: 386
			public float VPL;

			// Token: 0x04000183 RID: 387
			public float frequency;

			// Token: 0x04000184 RID: 388
			public float VrmsVal;

			// Token: 0x04000185 RID: 389
			public double[] FFTResult;

			// Token: 0x04000186 RID: 390
			public double FFTResultMax;
		}

		// Token: 0x02000010 RID: 16
		private enum MachineType2
		{
			// Token: 0x04000188 RID: 392
			DSO,
			// Token: 0x04000189 RID: 393
			DMM,
			// Token: 0x0400018A RID: 394
			NULL
		}

		// Token: 0x02000011 RID: 17
		private enum DSO_MODE
		{
			// Token: 0x0400018C RID: 396
			OldDrawingData,
			// Token: 0x0400018D RID: 397
			RealtimeData,
			// Token: 0x0400018E RID: 398
			RealTimeDataForAnalyse,
			// Token: 0x0400018F RID: 399
			NULL
		}

		// Token: 0x02000012 RID: 18
		private enum DSOWindowMode
		{
			// Token: 0x04000191 RID: 401
			Rectanglular,
			// Token: 0x04000192 RID: 402
			Hanning,
			// Token: 0x04000193 RID: 403
			Hamming,
			// Token: 0x04000194 RID: 404
			Blackman,
			// Token: 0x04000195 RID: 405
			Flattop,
			// Token: 0x04000196 RID: 406
			Bartlett,
			// Token: 0x04000197 RID: 407
			Null
		}

		// Token: 0x02000013 RID: 19
		private enum DSOXLinearMode
		{
			// Token: 0x04000199 RID: 409
			Linear,
			// Token: 0x0400019A RID: 410
			logarithm
		}

		// Token: 0x02000014 RID: 20
		private enum DMM_Mode
		{
			// Token: 0x0400019C RID: 412
			DMM_realTimeData,
			// Token: 0x0400019D RID: 413
			AutoRecoreData,
			// Token: 0x0400019E RID: 414
			HoldData,
			// Token: 0x0400019F RID: 415
			Calibation,
			// Token: 0x040001A0 RID: 416
			GET_PRODUCT_MESSAGE,
			// Token: 0x040001A1 RID: 417
			NULL,
			// Token: 0x040001A2 RID: 418
			GET_PRODUCT_MESSAGE_Finished
		}

		// Token: 0x02000015 RID: 21
		private struct processDataType
		{
			// Token: 0x040001A3 RID: 419
			public char ch;

			// Token: 0x040001A4 RID: 420
			public double data;
		}

		// Token: 0x02000016 RID: 22
		private struct ret_data
		{
			// Token: 0x040001A5 RID: 421
			public byte ret_data1;

			// Token: 0x040001A6 RID: 422
			public bool overtime;
		}

		// Token: 0x02000017 RID: 23
		private enum ThreadType
		{
			// Token: 0x040001A8 RID: 424
			Zero,
			// Token: 0x040001A9 RID: 425
			CommucationTest,
			// Token: 0x040001AA RID: 426
			DrawhistoricalData,
			// Token: 0x040001AB RID: 427
			ForRealTimeData,
			// Token: 0x040001AC RID: 428
			forDMM_Wave,
			// Token: 0x040001AD RID: 429
			DMM_realTimeData,
			// Token: 0x040001AE RID: 430
			DMM_AutoRecord,
			// Token: 0x040001AF RID: 431
			DMM_HOLD,
			// Token: 0x040001B0 RID: 432
			DMM_Calibration,
			// Token: 0x040001B1 RID: 433
			GetProductionMessage,
			// Token: 0x040001B2 RID: 434
			NOT_CLOSE_Machine
		}

		// Token: 0x02000018 RID: 24
		private class MultimeterDisplayData
		{
			// Token: 0x060000B0 RID: 176 RVA: 0x00002677 File Offset: 0x00000877
			public void Init()
			{
				this.multimeterData = new PointF[0];
				this.num = 0;
			}

			// Token: 0x060000B1 RID: 177 RVA: 0x00010040 File Offset: 0x0000E240
			public bool addNumber(float data)
			{
				PointF[] array = new PointF[this.num + 1];
				for (int i = 0; i < this.num; i++)
				{
					array[i].X = (float)i;
					array[i].Y = this.multimeterData[i].Y;
				}
				array[this.num].X = (float)this.num;
				array[this.num].Y = data;
				this.multimeterData = array;
				if (this.num > 0)
				{
					if (this.minY > data)
					{
						this.minY = data;
					}
					if (this.maxY < data)
					{
						this.maxY = data;
					}
				}
				else
				{
					this.minY = data;
					this.maxY = data + 10f;
				}
				this.num++;
				return true;
			}

			// Token: 0x040001B3 RID: 435
			public PointF[] multimeterData;

			// Token: 0x040001B4 RID: 436
			public int num;

			// Token: 0x040001B5 RID: 437
			public float maxY;

			// Token: 0x040001B6 RID: 438
			public float minY;
		}

		// Token: 0x02000019 RID: 25
		private enum UpdateState
		{
			// Token: 0x040001B8 RID: 440
			changeMenuState,
			// Token: 0x040001B9 RID: 441
			changeTitle,
			// Token: 0x040001BA RID: 442
			changeCommunicationState
		}
	}
}
