namespace 示波器
{
	// Token: 0x02000005 RID: 5
	public partial class Form1ShiBoQi : global::System.Windows.Forms.Form
	{
		// Token: 0x06000099 RID: 153 RVA: 0x00002603 File Offset: 0x00000803
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000B898 File Offset: 0x00009A98
		private void InitializeComponent()
		{
			this.components = new global::System.ComponentModel.Container();
			global::System.ComponentModel.ComponentResourceManager resources = new global::System.ComponentModel.ComponentResourceManager(typeof(global::示波器.Form1ShiBoQi));
			this.splitContainer2 = new global::System.Windows.Forms.SplitContainer();
			this.GraphControl1 = new global::ZedGraph.ZedGraphControl();
			this.panel4 = new global::示波器.MyPanel();
			this.panel3 = new global::System.Windows.Forms.Panel();
			this.pictureBox16 = new global::System.Windows.Forms.PictureBox();
			this.pictureBox15 = new global::System.Windows.Forms.PictureBox();
			this.button5 = new global::System.Windows.Forms.Button();
			this.button4 = new global::System.Windows.Forms.Button();
			this.panel2 = new global::System.Windows.Forms.Panel();
			this.button6 = new global::System.Windows.Forms.Button();
			this.pictureBox14 = new global::System.Windows.Forms.PictureBox();
			this.pictureBox13 = new global::System.Windows.Forms.PictureBox();
			this.button3 = new global::System.Windows.Forms.Button();
			this.button2 = new global::System.Windows.Forms.Button();
			this.hScrollBar1 = new global::System.Windows.Forms.HScrollBar();
			this.panel1 = new global::System.Windows.Forms.Panel();
			this.AutoMode = new global::System.Windows.Forms.Label();
			this.axLED13 = new global::LEDConTroler.LED();
			this.pictureBox8 = new global::System.Windows.Forms.PictureBox();
			this.pictureBox5 = new global::System.Windows.Forms.PictureBox();
			this.pictureBox1 = new global::System.Windows.Forms.PictureBox();
			this.label6 = new global::System.Windows.Forms.Label();
			this.label5 = new global::System.Windows.Forms.Label();
			this.label4 = new global::System.Windows.Forms.Label();
			this.label3 = new global::System.Windows.Forms.Label();
			this.label2 = new global::System.Windows.Forms.Label();
			this.axLED9 = new global::LEDConTroler.LED();
			this.pictureBox9 = new global::System.Windows.Forms.PictureBox();
			this.axLED10 = new global::LEDConTroler.LED();
			this.pictureBox10 = new global::System.Windows.Forms.PictureBox();
			this.axLED11 = new global::LEDConTroler.LED();
			this.pictureBox11 = new global::System.Windows.Forms.PictureBox();
			this.axLED12 = new global::LEDConTroler.LED();
			this.axLED6 = new global::LEDConTroler.LED();
			this.pictureBox7 = new global::System.Windows.Forms.PictureBox();
			this.axLED7 = new global::LEDConTroler.LED();
			this.pictureBox4 = new global::System.Windows.Forms.PictureBox();
			this.axLED5 = new global::LEDConTroler.LED();
			this.pictureBox6 = new global::System.Windows.Forms.PictureBox();
			this.axLED8 = new global::LEDConTroler.LED();
			this.pictureBox3 = new global::System.Windows.Forms.PictureBox();
			this.pictureBox2 = new global::System.Windows.Forms.PictureBox();
			this.pictureBox12 = new global::System.Windows.Forms.PictureBox();
			this.axLED4 = new global::LEDConTroler.LED();
			this.axLED3 = new global::LEDConTroler.LED();
			this.axLED2 = new global::LEDConTroler.LED();
			this.axLED1 = new global::LEDConTroler.LED();
			this.splitContainer1 = new global::System.Windows.Forms.SplitContainer();
			this.textBox2 = new global::System.Windows.Forms.TextBox();
			this.menuStrip1 = new global::System.Windows.Forms.MenuStrip();
			this.文件ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.打开ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem23 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new global::System.Windows.Forms.ToolStripSeparator();
			this.printPreviewToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.打印ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new global::System.Windows.Forms.ToolStripSeparator();
			this.关闭ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.编辑ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.复制波形ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator9 = new global::System.Windows.Forms.ToolStripSeparator();
			this.全屏显示ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.通信ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.连接ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.断开ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new global::System.Windows.Forms.ToolStripSeparator();
			this.communicationTestToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.运行ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.startMenu = new global::System.Windows.Forms.ToolStripMenuItem();
			this.stopMenu = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new global::System.Windows.Forms.ToolStripSeparator();
			this.dMMToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.dSOToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.dSOToolStripMenuItem1 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.WaveToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.fTTToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new global::System.Windows.Forms.ToolStripSeparator();
			this.fTTToolStripMenuItem1 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.rectangleToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.hanningToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.hammingToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.blackmanToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.flattopToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.britlittToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.频谱显示ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.线性ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.对数ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.dMM记录ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.开始记录ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.结束ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.帮助ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.帮助主题ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.关于ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new global::System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new global::System.Windows.Forms.ToolStripStatusLabel();
			this.仪表 = new global::System.Windows.Forms.ToolStripStatusLabel();
			this.通讯状态 = new global::System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel3 = new global::System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel2 = new global::System.Windows.Forms.ToolStripStatusLabel();
			this.webBrowser1 = new global::System.Windows.Forms.WebBrowser();
			this.TimerContextMenuStrip1 = new global::System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem4 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem6 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem7 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem8 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem9 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem10 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem11 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem12 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem13 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1 = new global::System.Windows.Forms.ToolStrip();
			this.NewButton1 = new global::System.Windows.Forms.ToolStripButton();
			this.打开 = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripButton1 = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripButton5 = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripButton3 = new global::System.Windows.Forms.ToolStripButton();
			this.串口 = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripButton7 = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripButton9 = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripButton8 = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripButton4 = new global::System.Windows.Forms.ToolStripButton();
			this.toolStrip实时波形 = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripButton6 = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripButton14 = new global::System.Windows.Forms.ToolStripButton();
			this.startButton = new global::System.Windows.Forms.ToolStripButton();
			this.endButton = new global::System.Windows.Forms.ToolStripButton();
			this.伊万Button = new global::System.Windows.Forms.ToolStripButton();
			this.serialPort1 = new global::System.IO.Ports.SerialPort(this.components);
			this.SerialPortContextMenuStrip2 = new global::System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem14 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem15 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem16 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem17 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem18 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem19 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem20 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem21 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem22 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem28 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.cOM11ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.cOM12ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.cOM13ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.cOM14ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.cOM15ToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MultimeterContextMenuStrip1 = new global::System.Windows.Forms.ContextMenuStrip(this.components);
			this.realtimeDataToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.autoRecordDataToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.ProdctionMessage = new global::System.Windows.Forms.ToolStripMenuItem();
			this.OscillographContextMenuStrip2 = new global::System.Windows.Forms.ContextMenuStrip(this.components);
			this.historicalDataToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.realtimeDataToolStripMenuItem1 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.realTimeDataForAnalyseToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.printDocument1 = new global::System.Drawing.Printing.PrintDocument();
			this.printPreviewDialog1 = new global::System.Windows.Forms.PrintPreviewDialog();
			this.printDialog1 = new global::System.Windows.Forms.PrintDialog();
			this.button1 = new global::System.Windows.Forms.Button();
			this.serviceController1 = new global::System.ServiceProcess.ServiceController();
			this.DmmAutorecordTextBox = new global::System.Windows.Forms.TextBox();
			this.ChangeHoldStatecontextMenuStrip1 = new global::System.Windows.Forms.ContextMenuStrip(this.components);
			this.changeHoldStateToolstripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.leaveScreentoolStripMenuItem23 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog1 = new global::System.Windows.Forms.SaveFileDialog();
			this.openFileDialog1 = new global::System.Windows.Forms.OpenFileDialog();
			((global::System.ComponentModel.ISupportInitialize)this.splitContainer2).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.panel3.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox16).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox15).BeginInit();
			this.panel2.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox14).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox13).BeginInit();
			this.panel1.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox8).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox5).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox9).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox10).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox11).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox7).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox4).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox6).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox3).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox2).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox12).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.TimerContextMenuStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SerialPortContextMenuStrip2.SuspendLayout();
			this.MultimeterContextMenuStrip1.SuspendLayout();
			this.OscillographContextMenuStrip2.SuspendLayout();
			this.ChangeHoldStatecontextMenuStrip1.SuspendLayout();
			base.SuspendLayout();
			resources.ApplyResources(this.splitContainer2, "splitContainer2");
			this.splitContainer2.BorderStyle = global::System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitContainer2.Name = "splitContainer2";
			resources.ApplyResources(this.splitContainer2.Panel1, "splitContainer2.Panel1");
			this.splitContainer2.Panel1.BackColor = global::System.Drawing.Color.White;
			this.splitContainer2.Panel1.Controls.Add(this.GraphControl1);
			this.splitContainer2.Panel1.Controls.Add(this.panel4);
			this.splitContainer2.Panel1.Controls.Add(this.panel3);
			this.splitContainer2.Panel1.Controls.Add(this.panel2);
			this.splitContainer2.Panel1.Controls.Add(this.hScrollBar1);
			this.splitContainer2.Panel1.SizeChanged += new global::System.EventHandler(this.splitContainer2_Panel1_SizeChanged);
			this.splitContainer2.Panel1.MouseDoubleClick += new global::System.Windows.Forms.MouseEventHandler(this.showHoldMenu);
			this.splitContainer2.Panel1.Resize += new global::System.EventHandler(this.splitContainer2_Panel1_Resize);
			resources.ApplyResources(this.splitContainer2.Panel2, "splitContainer2.Panel2");
			this.splitContainer2.Panel2.Controls.Add(this.panel1);
			this.splitContainer2.Panel2.Move += new global::System.EventHandler(this.MoveLeds);
			resources.ApplyResources(this.GraphControl1, "GraphControl1");
			this.GraphControl1.IsEnableHZoom = false;
			this.GraphControl1.IsEnableVZoom = false;
			this.GraphControl1.Name = "GraphControl1";
			this.GraphControl1.ScrollGrace = 0.0;
			this.GraphControl1.ScrollMaxX = 0.0;
			this.GraphControl1.ScrollMaxY = 0.0;
			this.GraphControl1.ScrollMaxY2 = 0.0;
			this.GraphControl1.ScrollMinX = 0.0;
			this.GraphControl1.ScrollMinY = 0.0;
			this.GraphControl1.ScrollMinY2 = 0.0;
			this.GraphControl1.ZoomStepFraction = 1.0;
			resources.ApplyResources(this.panel4, "panel4");
			this.panel4.BackColor = global::System.Drawing.Color.Black;
			this.panel4.CausesValidation = false;
			this.panel4.Name = "panel4";
			this.panel4.Paint += new global::System.Windows.Forms.PaintEventHandler(this.示波器Paint);
			this.panel4.MouseDoubleClick += new global::System.Windows.Forms.MouseEventHandler(this.showHoldMenu);
			resources.ApplyResources(this.panel3, "panel3");
			this.panel3.BackColor = global::System.Drawing.Color.White;
			this.panel3.Controls.Add(this.pictureBox16);
			this.panel3.Controls.Add(this.pictureBox15);
			this.panel3.Controls.Add(this.button5);
			this.panel3.Controls.Add(this.button4);
			this.panel3.Name = "panel3";
			resources.ApplyResources(this.pictureBox16, "pictureBox16");
			this.pictureBox16.Cursor = global::System.Windows.Forms.Cursors.Hand;
			this.pictureBox16.Name = "pictureBox16";
			this.pictureBox16.TabStop = false;
			this.pictureBox16.MouseDown += new global::System.Windows.Forms.MouseEventHandler(this.OnmouseDown);
			this.pictureBox16.MouseMove += new global::System.Windows.Forms.MouseEventHandler(this.OnMouseMoveButton2);
			this.pictureBox16.MouseUp += new global::System.Windows.Forms.MouseEventHandler(this.pictureBox13_MouseUp);
			resources.ApplyResources(this.pictureBox15, "pictureBox15");
			this.pictureBox15.Cursor = global::System.Windows.Forms.Cursors.Hand;
			this.pictureBox15.Name = "pictureBox15";
			this.pictureBox15.TabStop = false;
			this.pictureBox15.MouseDown += new global::System.Windows.Forms.MouseEventHandler(this.OnmouseDown);
			this.pictureBox15.MouseMove += new global::System.Windows.Forms.MouseEventHandler(this.OnMouseMoveButton2);
			this.pictureBox15.MouseUp += new global::System.Windows.Forms.MouseEventHandler(this.pictureBox13_MouseUp);
			resources.ApplyResources(this.button5, "button5");
			this.button5.BackColor = global::System.Drawing.Color.LightSkyBlue;
			this.button5.FlatAppearance.BorderSize = 0;
			this.button5.Name = "button5";
			this.button5.Tag = "Hor";
			this.button5.UseVisualStyleBackColor = false;
			resources.ApplyResources(this.button4, "button4");
			this.button4.BackColor = global::System.Drawing.Color.LightSkyBlue;
			this.button4.FlatAppearance.BorderSize = 0;
			this.button4.Name = "button4";
			this.button4.Tag = "Hor";
			this.button4.UseVisualStyleBackColor = false;
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.BackColor = global::System.Drawing.Color.White;
			this.panel2.Controls.Add(this.button6);
			this.panel2.Controls.Add(this.pictureBox14);
			this.panel2.Controls.Add(this.pictureBox13);
			this.panel2.Controls.Add(this.button3);
			this.panel2.Controls.Add(this.button2);
			this.panel2.Name = "panel2";
			resources.ApplyResources(this.button6, "button6");
			this.button6.BackColor = global::System.Drawing.Color.YellowGreen;
			this.button6.FlatAppearance.BorderSize = 0;
			this.button6.Name = "button6";
			this.button6.Tag = "Ver";
			this.button6.UseVisualStyleBackColor = false;
			resources.ApplyResources(this.pictureBox14, "pictureBox14");
			this.pictureBox14.Cursor = global::System.Windows.Forms.Cursors.Hand;
			this.pictureBox14.Name = "pictureBox14";
			this.pictureBox14.TabStop = false;
			this.pictureBox14.MouseDown += new global::System.Windows.Forms.MouseEventHandler(this.OnmouseDown);
			this.pictureBox14.MouseMove += new global::System.Windows.Forms.MouseEventHandler(this.OnMouseMoveButton2);
			this.pictureBox14.MouseUp += new global::System.Windows.Forms.MouseEventHandler(this.pictureBox13_MouseUp);
			resources.ApplyResources(this.pictureBox13, "pictureBox13");
			this.pictureBox13.Cursor = global::System.Windows.Forms.Cursors.Hand;
			this.pictureBox13.Name = "pictureBox13";
			this.pictureBox13.TabStop = false;
			this.pictureBox13.MouseDown += new global::System.Windows.Forms.MouseEventHandler(this.OnmouseDown);
			this.pictureBox13.MouseMove += new global::System.Windows.Forms.MouseEventHandler(this.OnMouseMoveButton2);
			this.pictureBox13.MouseUp += new global::System.Windows.Forms.MouseEventHandler(this.pictureBox13_MouseUp);
			resources.ApplyResources(this.button3, "button3");
			this.button3.BackColor = global::System.Drawing.Color.YellowGreen;
			this.button3.FlatAppearance.BorderSize = 0;
			this.button3.Name = "button3";
			this.button3.Tag = "Ver";
			this.button3.UseVisualStyleBackColor = false;
			resources.ApplyResources(this.button2, "button2");
			this.button2.BackColor = global::System.Drawing.Color.YellowGreen;
			this.button2.CausesValidation = false;
			this.button2.FlatAppearance.BorderSize = 0;
			this.button2.Name = "button2";
			this.button2.Tag = "Ver";
			this.button2.UseVisualStyleBackColor = false;
			resources.ApplyResources(this.hScrollBar1, "hScrollBar1");
			this.hScrollBar1.Name = "hScrollBar1";
			this.hScrollBar1.Value = 10;
			this.hScrollBar1.ValueChanged += new global::System.EventHandler(this.hScrollBar1_ValueChanged);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.BackColor = global::System.Drawing.Color.LightSkyBlue;
			this.panel1.Controls.Add(this.AutoMode);
			this.panel1.Controls.Add(this.axLED13);
			this.panel1.Controls.Add(this.pictureBox8);
			this.panel1.Controls.Add(this.pictureBox5);
			this.panel1.Controls.Add(this.pictureBox1);
			this.panel1.Controls.Add(this.label6);
			this.panel1.Controls.Add(this.label5);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.axLED9);
			this.panel1.Controls.Add(this.pictureBox9);
			this.panel1.Controls.Add(this.axLED10);
			this.panel1.Controls.Add(this.pictureBox10);
			this.panel1.Controls.Add(this.axLED11);
			this.panel1.Controls.Add(this.pictureBox11);
			this.panel1.Controls.Add(this.axLED12);
			this.panel1.Controls.Add(this.axLED6);
			this.panel1.Controls.Add(this.pictureBox7);
			this.panel1.Controls.Add(this.axLED7);
			this.panel1.Controls.Add(this.pictureBox4);
			this.panel1.Controls.Add(this.axLED5);
			this.panel1.Controls.Add(this.pictureBox6);
			this.panel1.Controls.Add(this.axLED8);
			this.panel1.Controls.Add(this.pictureBox3);
			this.panel1.Controls.Add(this.pictureBox2);
			this.panel1.Controls.Add(this.pictureBox12);
			this.panel1.Controls.Add(this.axLED4);
			this.panel1.Controls.Add(this.axLED3);
			this.panel1.Controls.Add(this.axLED2);
			this.panel1.Controls.Add(this.axLED1);
			this.panel1.Name = "panel1";
			this.panel1.Paint += new global::System.Windows.Forms.PaintEventHandler(this.PaintLEDLines);
			this.panel1.Resize += new global::System.EventHandler(this.resize);
			resources.ApplyResources(this.AutoMode, "AutoMode");
			this.AutoMode.ForeColor = global::System.Drawing.Color.Black;
			this.AutoMode.Name = "AutoMode";
			resources.ApplyResources(this.axLED13, "axLED13");
			this.axLED13.Name = "axLED13";
			resources.ApplyResources(this.pictureBox8, "pictureBox8");
			this.pictureBox8.Image = global::示波器.Properties.Resources._;
			this.pictureBox8.Name = "pictureBox8";
			this.pictureBox8.TabStop = false;
			resources.ApplyResources(this.pictureBox5, "pictureBox5");
			this.pictureBox5.Image = global::示波器.Properties.Resources._;
			this.pictureBox5.Name = "pictureBox5";
			this.pictureBox5.TabStop = false;
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = global::示波器.Properties.Resources._;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			resources.ApplyResources(this.label6, "label6");
			this.label6.ForeColor = global::System.Drawing.Color.Black;
			this.label6.Name = "label6";
			resources.ApplyResources(this.label5, "label5");
			this.label5.ForeColor = global::System.Drawing.Color.Black;
			this.label5.Name = "label5";
			resources.ApplyResources(this.label4, "label4");
			this.label4.ForeColor = global::System.Drawing.Color.Black;
			this.label4.Name = "label4";
			resources.ApplyResources(this.label3, "label3");
			this.label3.ForeColor = global::System.Drawing.Color.Black;
			this.label3.Name = "label3";
			resources.ApplyResources(this.label2, "label2");
			this.label2.ForeColor = global::System.Drawing.Color.Black;
			this.label2.Name = "label2";
			resources.ApplyResources(this.axLED9, "axLED9");
			this.axLED9.Name = "axLED9";
			resources.ApplyResources(this.pictureBox9, "pictureBox9");
			this.pictureBox9.BackColor = global::System.Drawing.Color.Black;
			this.pictureBox9.Name = "pictureBox9";
			this.pictureBox9.TabStop = false;
			resources.ApplyResources(this.axLED10, "axLED10");
			this.axLED10.Name = "axLED10";
			resources.ApplyResources(this.pictureBox10, "pictureBox10");
			this.pictureBox10.BackColor = global::System.Drawing.Color.Black;
			this.pictureBox10.Name = "pictureBox10";
			this.pictureBox10.TabStop = false;
			resources.ApplyResources(this.axLED11, "axLED11");
			this.axLED11.Name = "axLED11";
			resources.ApplyResources(this.pictureBox11, "pictureBox11");
			this.pictureBox11.BackColor = global::System.Drawing.Color.Black;
			this.pictureBox11.Name = "pictureBox11";
			this.pictureBox11.TabStop = false;
			resources.ApplyResources(this.axLED12, "axLED12");
			this.axLED12.Name = "axLED12";
			resources.ApplyResources(this.axLED6, "axLED6");
			this.axLED6.Name = "axLED6";
			resources.ApplyResources(this.pictureBox7, "pictureBox7");
			this.pictureBox7.BackColor = global::System.Drawing.Color.Black;
			this.pictureBox7.Name = "pictureBox7";
			this.pictureBox7.TabStop = false;
			resources.ApplyResources(this.axLED7, "axLED7");
			this.axLED7.Name = "axLED7";
			resources.ApplyResources(this.pictureBox4, "pictureBox4");
			this.pictureBox4.BackColor = global::System.Drawing.Color.Black;
			this.pictureBox4.Name = "pictureBox4";
			this.pictureBox4.TabStop = false;
			resources.ApplyResources(this.axLED5, "axLED5");
			this.axLED5.Name = "axLED5";
			resources.ApplyResources(this.pictureBox6, "pictureBox6");
			this.pictureBox6.BackColor = global::System.Drawing.Color.Black;
			this.pictureBox6.Name = "pictureBox6";
			this.pictureBox6.TabStop = false;
			resources.ApplyResources(this.axLED8, "axLED8");
			this.axLED8.Name = "axLED8";
			resources.ApplyResources(this.pictureBox3, "pictureBox3");
			this.pictureBox3.BackColor = global::System.Drawing.Color.Black;
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.TabStop = false;
			resources.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.BackColor = global::System.Drawing.Color.Black;
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			resources.ApplyResources(this.pictureBox12, "pictureBox12");
			this.pictureBox12.BackColor = global::System.Drawing.Color.Black;
			this.pictureBox12.Name = "pictureBox12";
			this.pictureBox12.TabStop = false;
			resources.ApplyResources(this.axLED4, "axLED4");
			this.axLED4.Name = "axLED4";
			resources.ApplyResources(this.axLED3, "axLED3");
			this.axLED3.Name = "axLED3";
			resources.ApplyResources(this.axLED2, "axLED2");
			this.axLED2.Name = "axLED2";
			resources.ApplyResources(this.axLED1, "axLED1");
			this.axLED1.Name = "axLED1";
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.BorderStyle = global::System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitContainer1.Name = "splitContainer1";
			resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
			this.splitContainer1.Panel2.Controls.Add(this.textBox2);
			this.splitContainer1.Panel2.Move += new global::System.EventHandler(this.MoveLeds);
			resources.ApplyResources(this.textBox2, "textBox2");
			this.textBox2.BackColor = global::System.Drawing.Color.FromArgb(223, 223, 221);
			this.textBox2.ForeColor = global::System.Drawing.Color.Black;
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			resources.ApplyResources(this.menuStrip1, "menuStrip1");
			this.menuStrip1.BackColor = global::System.Drawing.Color.RoyalBlue;
			this.menuStrip1.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.文件ToolStripMenuItem,
				this.编辑ToolStripMenuItem,
				this.通信ToolStripMenuItem,
				this.运行ToolStripMenuItem,
				this.dSOToolStripMenuItem1,
				this.dMM记录ToolStripMenuItem,
				this.帮助ToolStripMenuItem
			});
			this.menuStrip1.LayoutStyle = global::System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.DragOver += new global::System.Windows.Forms.DragEventHandler(this.menuStrip1_DragOver);
			resources.ApplyResources(this.文件ToolStripMenuItem, "文件ToolStripMenuItem");
			this.文件ToolStripMenuItem.Checked = true;
			this.文件ToolStripMenuItem.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.文件ToolStripMenuItem.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.toolStripMenuItem2,
				this.打开ToolStripMenuItem,
				this.toolStripMenuItem1,
				this.toolStripMenuItem3,
				this.toolStripMenuItem23,
				this.toolStripSeparator6,
				this.printPreviewToolStripMenuItem,
				this.打印ToolStripMenuItem,
				this.toolStripSeparator5,
				this.关闭ToolStripMenuItem
			});
			this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
			this.文件ToolStripMenuItem.Click += new global::System.EventHandler(this.invalideMenu);
			resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Click += new global::System.EventHandler(this.newFile);
			resources.ApplyResources(this.打开ToolStripMenuItem, "打开ToolStripMenuItem");
			this.打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
			this.打开ToolStripMenuItem.Click += new global::System.EventHandler(this.打开_Click);
			resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Click += new global::System.EventHandler(this.toolStripButton1_Click);
			resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Click += new global::System.EventHandler(this.toolStripMenuItem3_Click);
			resources.ApplyResources(this.toolStripMenuItem23, "toolStripMenuItem23");
			this.toolStripMenuItem23.Name = "toolStripMenuItem23";
			this.toolStripMenuItem23.Click += new global::System.EventHandler(this.SaveAsJPG);
			resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			resources.ApplyResources(this.printPreviewToolStripMenuItem, "printPreviewToolStripMenuItem");
			this.printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
			this.printPreviewToolStripMenuItem.Click += new global::System.EventHandler(this.printPreviewToolStripMenuItem_Click);
			resources.ApplyResources(this.打印ToolStripMenuItem, "打印ToolStripMenuItem");
			this.打印ToolStripMenuItem.Name = "打印ToolStripMenuItem";
			this.打印ToolStripMenuItem.Click += new global::System.EventHandler(this.打印ToolStripMenuItem_Click);
			resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			resources.ApplyResources(this.关闭ToolStripMenuItem, "关闭ToolStripMenuItem");
			this.关闭ToolStripMenuItem.Name = "关闭ToolStripMenuItem";
			this.关闭ToolStripMenuItem.Click += new global::System.EventHandler(this.exitWindow);
			resources.ApplyResources(this.编辑ToolStripMenuItem, "编辑ToolStripMenuItem");
			this.编辑ToolStripMenuItem.CheckOnClick = true;
			this.编辑ToolStripMenuItem.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.复制波形ToolStripMenuItem,
				this.toolStripSeparator9,
				this.全屏显示ToolStripMenuItem
			});
			this.编辑ToolStripMenuItem.Name = "编辑ToolStripMenuItem";
			this.编辑ToolStripMenuItem.Tag = "编辑";
			this.编辑ToolStripMenuItem.Click += new global::System.EventHandler(this.invalideMenu);
			resources.ApplyResources(this.复制波形ToolStripMenuItem, "复制波形ToolStripMenuItem");
			this.复制波形ToolStripMenuItem.Name = "复制波形ToolStripMenuItem";
			this.复制波形ToolStripMenuItem.Tag = "拷贝";
			this.复制波形ToolStripMenuItem.Click += new global::System.EventHandler(this.ToClipboard);
			resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			resources.ApplyResources(this.全屏显示ToolStripMenuItem, "全屏显示ToolStripMenuItem");
			this.全屏显示ToolStripMenuItem.Name = "全屏显示ToolStripMenuItem";
			this.全屏显示ToolStripMenuItem.Click += new global::System.EventHandler(this.toolStripButton8_Click);
			resources.ApplyResources(this.通信ToolStripMenuItem, "通信ToolStripMenuItem");
			this.通信ToolStripMenuItem.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.连接ToolStripMenuItem,
				this.断开ToolStripMenuItem,
				this.toolStripSeparator1,
				this.communicationTestToolStripMenuItem
			});
			this.通信ToolStripMenuItem.Name = "通信ToolStripMenuItem";
			this.通信ToolStripMenuItem.Tag = "通信";
			this.通信ToolStripMenuItem.Click += new global::System.EventHandler(this.invalideMenu);
			resources.ApplyResources(this.连接ToolStripMenuItem, "连接ToolStripMenuItem");
			this.连接ToolStripMenuItem.Name = "连接ToolStripMenuItem";
			this.连接ToolStripMenuItem.Click += new global::System.EventHandler(this.onConnectSerialPort);
			resources.ApplyResources(this.断开ToolStripMenuItem, "断开ToolStripMenuItem");
			this.断开ToolStripMenuItem.Name = "断开ToolStripMenuItem";
			this.断开ToolStripMenuItem.Click += new global::System.EventHandler(this.onCloseSerialPort);
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.communicationTestToolStripMenuItem, "communicationTestToolStripMenuItem");
			this.communicationTestToolStripMenuItem.Name = "communicationTestToolStripMenuItem";
			this.communicationTestToolStripMenuItem.Click += new global::System.EventHandler(this.communicationTestToolStripMenuItem_Click);
			resources.ApplyResources(this.运行ToolStripMenuItem, "运行ToolStripMenuItem");
			this.运行ToolStripMenuItem.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.startMenu,
				this.stopMenu,
				this.toolStripSeparator2,
				this.dMMToolStripMenuItem,
				this.dSOToolStripMenuItem
			});
			this.运行ToolStripMenuItem.Name = "运行ToolStripMenuItem";
			this.运行ToolStripMenuItem.Tag = "Running";
			this.运行ToolStripMenuItem.Click += new global::System.EventHandler(this.invalideMenu);
			resources.ApplyResources(this.startMenu, "startMenu");
			this.startMenu.Name = "startMenu";
			this.startMenu.Click += new global::System.EventHandler(this.comMenu2_Click);
			resources.ApplyResources(this.stopMenu, "stopMenu");
			this.stopMenu.Name = "stopMenu";
			this.stopMenu.Click += new global::System.EventHandler(this.toolStripButton13_Click);
			resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources(this.dMMToolStripMenuItem, "dMMToolStripMenuItem");
			this.dMMToolStripMenuItem.Name = "dMMToolStripMenuItem";
			this.dMMToolStripMenuItem.Click += new global::System.EventHandler(this.changeDMM_DSO_Mode);
			resources.ApplyResources(this.dSOToolStripMenuItem, "dSOToolStripMenuItem");
			this.dSOToolStripMenuItem.Name = "dSOToolStripMenuItem";
			this.dSOToolStripMenuItem.Click += new global::System.EventHandler(this.changeDMM_DSO_Mode);
			resources.ApplyResources(this.dSOToolStripMenuItem1, "dSOToolStripMenuItem1");
			this.dSOToolStripMenuItem1.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.WaveToolStripMenuItem,
				this.fTTToolStripMenuItem,
				this.toolStripSeparator4,
				this.fTTToolStripMenuItem1,
				this.频谱显示ToolStripMenuItem
			});
			this.dSOToolStripMenuItem1.Name = "dSOToolStripMenuItem1";
			this.dSOToolStripMenuItem1.Tag = "DSO";
			this.dSOToolStripMenuItem1.DropDownClosed += new global::System.EventHandler(this.dSOToolStripMenuItem1_DropDownClosed);
			this.dSOToolStripMenuItem1.Click += new global::System.EventHandler(this.invalideMenu);
			resources.ApplyResources(this.WaveToolStripMenuItem, "WaveToolStripMenuItem");
			this.WaveToolStripMenuItem.Name = "WaveToolStripMenuItem";
			this.WaveToolStripMenuItem.Tag = "WAVE";
			this.WaveToolStripMenuItem.Click += new global::System.EventHandler(this.onChangeWaveMode);
			resources.ApplyResources(this.fTTToolStripMenuItem, "fTTToolStripMenuItem");
			this.fTTToolStripMenuItem.Name = "fTTToolStripMenuItem";
			this.fTTToolStripMenuItem.Tag = "FFT";
			this.fTTToolStripMenuItem.Click += new global::System.EventHandler(this.onChangeWaveMode);
			resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			resources.ApplyResources(this.fTTToolStripMenuItem1, "fTTToolStripMenuItem1");
			this.fTTToolStripMenuItem1.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.rectangleToolStripMenuItem,
				this.hanningToolStripMenuItem,
				this.hammingToolStripMenuItem,
				this.blackmanToolStripMenuItem,
				this.flattopToolStripMenuItem,
				this.britlittToolStripMenuItem
			});
			this.fTTToolStripMenuItem1.Name = "fTTToolStripMenuItem1";
			this.fTTToolStripMenuItem1.MouseHover += new global::System.EventHandler(this.onClickFttWindowMenu);
			resources.ApplyResources(this.rectangleToolStripMenuItem, "rectangleToolStripMenuItem");
			this.rectangleToolStripMenuItem.Name = "rectangleToolStripMenuItem";
			this.rectangleToolStripMenuItem.Click += new global::System.EventHandler(this.changeWindowMode);
			resources.ApplyResources(this.hanningToolStripMenuItem, "hanningToolStripMenuItem");
			this.hanningToolStripMenuItem.Name = "hanningToolStripMenuItem";
			this.hanningToolStripMenuItem.Click += new global::System.EventHandler(this.changeWindowMode);
			resources.ApplyResources(this.hammingToolStripMenuItem, "hammingToolStripMenuItem");
			this.hammingToolStripMenuItem.Name = "hammingToolStripMenuItem";
			this.hammingToolStripMenuItem.Click += new global::System.EventHandler(this.changeWindowMode);
			resources.ApplyResources(this.blackmanToolStripMenuItem, "blackmanToolStripMenuItem");
			this.blackmanToolStripMenuItem.Name = "blackmanToolStripMenuItem";
			this.blackmanToolStripMenuItem.Click += new global::System.EventHandler(this.changeWindowMode);
			resources.ApplyResources(this.flattopToolStripMenuItem, "flattopToolStripMenuItem");
			this.flattopToolStripMenuItem.Name = "flattopToolStripMenuItem";
			this.flattopToolStripMenuItem.Click += new global::System.EventHandler(this.changeWindowMode);
			resources.ApplyResources(this.britlittToolStripMenuItem, "britlittToolStripMenuItem");
			this.britlittToolStripMenuItem.Name = "britlittToolStripMenuItem";
			this.britlittToolStripMenuItem.Click += new global::System.EventHandler(this.changeWindowMode);
			resources.ApplyResources(this.频谱显示ToolStripMenuItem, "频谱显示ToolStripMenuItem");
			this.频谱显示ToolStripMenuItem.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.线性ToolStripMenuItem,
				this.对数ToolStripMenuItem
			});
			this.频谱显示ToolStripMenuItem.Name = "频谱显示ToolStripMenuItem";
			this.频谱显示ToolStripMenuItem.Click += new global::System.EventHandler(this.onChangeDSOLineralogicalMode);
			this.频谱显示ToolStripMenuItem.MouseHover += new global::System.EventHandler(this.onChangeDSOLineralogicalMode);
			resources.ApplyResources(this.线性ToolStripMenuItem, "线性ToolStripMenuItem");
			this.线性ToolStripMenuItem.Name = "线性ToolStripMenuItem";
			this.线性ToolStripMenuItem.Click += new global::System.EventHandler(this.线性ToolStripMenuItem_Click);
			resources.ApplyResources(this.对数ToolStripMenuItem, "对数ToolStripMenuItem");
			this.对数ToolStripMenuItem.Name = "对数ToolStripMenuItem";
			this.对数ToolStripMenuItem.Click += new global::System.EventHandler(this.对数ToolStripMenuItem_Click);
			resources.ApplyResources(this.dMM记录ToolStripMenuItem, "dMM记录ToolStripMenuItem");
			this.dMM记录ToolStripMenuItem.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.开始记录ToolStripMenuItem,
				this.结束ToolStripMenuItem
			});
			this.dMM记录ToolStripMenuItem.Name = "dMM记录ToolStripMenuItem";
			this.dMM记录ToolStripMenuItem.Click += new global::System.EventHandler(this.invalideMenu);
			resources.ApplyResources(this.开始记录ToolStripMenuItem, "开始记录ToolStripMenuItem");
			this.开始记录ToolStripMenuItem.Name = "开始记录ToolStripMenuItem";
			this.开始记录ToolStripMenuItem.Click += new global::System.EventHandler(this.comMenu2_Click);
			resources.ApplyResources(this.结束ToolStripMenuItem, "结束ToolStripMenuItem");
			this.结束ToolStripMenuItem.Name = "结束ToolStripMenuItem";
			this.结束ToolStripMenuItem.Click += new global::System.EventHandler(this.toolStripButton13_Click);
			resources.ApplyResources(this.帮助ToolStripMenuItem, "帮助ToolStripMenuItem");
			this.帮助ToolStripMenuItem.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.帮助主题ToolStripMenuItem,
				this.关于ToolStripMenuItem
			});
			this.帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
			resources.ApplyResources(this.帮助主题ToolStripMenuItem, "帮助主题ToolStripMenuItem");
			this.帮助主题ToolStripMenuItem.Name = "帮助主题ToolStripMenuItem";
			this.帮助主题ToolStripMenuItem.Click += new global::System.EventHandler(this.帮助主题ToolStripMenuItem_Click);
			resources.ApplyResources(this.关于ToolStripMenuItem, "关于ToolStripMenuItem");
			this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
			this.关于ToolStripMenuItem.Click += new global::System.EventHandler(this.关于ToolStripMenuItem_Click);
			resources.ApplyResources(this.statusStrip1, "statusStrip1");
			this.statusStrip1.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.toolStripStatusLabel1,
				this.仪表,
				this.通讯状态,
				this.toolStripStatusLabel3,
				this.toolStripStatusLabel2
			});
			this.statusStrip1.Name = "statusStrip1";
			resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
			this.toolStripStatusLabel1.ForeColor = global::System.Drawing.Color.Blue;
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			resources.ApplyResources(this.仪表, "仪表");
			this.仪表.Name = "仪表";
			resources.ApplyResources(this.通讯状态, "通讯状态");
			this.通讯状态.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.通讯状态.ForeColor = global::System.Drawing.Color.Red;
			this.通讯状态.Name = "通讯状态";
			resources.ApplyResources(this.toolStripStatusLabel3, "toolStripStatusLabel3");
			this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
			resources.ApplyResources(this.toolStripStatusLabel2, "toolStripStatusLabel2");
			this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
			resources.ApplyResources(this.webBrowser1, "webBrowser1");
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.Url = new global::System.Uri("c:\\E-ONE仪表网站.mht", global::System.UriKind.Absolute);
			resources.ApplyResources(this.TimerContextMenuStrip1, "TimerContextMenuStrip1");
			this.TimerContextMenuStrip1.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.toolStripMenuItem4,
				this.toolStripMenuItem5,
				this.toolStripMenuItem6,
				this.toolStripMenuItem7,
				this.toolStripMenuItem8,
				this.toolStripMenuItem9,
				this.toolStripMenuItem10,
				this.toolStripMenuItem11,
				this.toolStripMenuItem12,
				this.toolStripMenuItem13
			});
			this.TimerContextMenuStrip1.Name = "contextMenuStrip1";
			this.TimerContextMenuStrip1.ShowCheckMargin = true;
			this.TimerContextMenuStrip1.ShowImageMargin = false;
			resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Click += new global::System.EventHandler(this.changeIntervalTime);
			resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
			this.toolStripMenuItem5.Checked = true;
			this.toolStripMenuItem5.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Click += new global::System.EventHandler(this.changeIntervalTime);
			resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
			this.toolStripMenuItem6.Checked = true;
			this.toolStripMenuItem6.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Click += new global::System.EventHandler(this.changeIntervalTime);
			resources.ApplyResources(this.toolStripMenuItem7, "toolStripMenuItem7");
			this.toolStripMenuItem7.Checked = true;
			this.toolStripMenuItem7.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Click += new global::System.EventHandler(this.changeIntervalTime);
			resources.ApplyResources(this.toolStripMenuItem8, "toolStripMenuItem8");
			this.toolStripMenuItem8.Checked = true;
			this.toolStripMenuItem8.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem8.Name = "toolStripMenuItem8";
			this.toolStripMenuItem8.Click += new global::System.EventHandler(this.changeIntervalTime);
			resources.ApplyResources(this.toolStripMenuItem9, "toolStripMenuItem9");
			this.toolStripMenuItem9.Checked = true;
			this.toolStripMenuItem9.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem9.Name = "toolStripMenuItem9";
			this.toolStripMenuItem9.Click += new global::System.EventHandler(this.changeIntervalTime);
			resources.ApplyResources(this.toolStripMenuItem10, "toolStripMenuItem10");
			this.toolStripMenuItem10.Checked = true;
			this.toolStripMenuItem10.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem10.Name = "toolStripMenuItem10";
			this.toolStripMenuItem10.Click += new global::System.EventHandler(this.changeIntervalTime);
			resources.ApplyResources(this.toolStripMenuItem11, "toolStripMenuItem11");
			this.toolStripMenuItem11.Checked = true;
			this.toolStripMenuItem11.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem11.Name = "toolStripMenuItem11";
			this.toolStripMenuItem11.Click += new global::System.EventHandler(this.changeIntervalTime);
			resources.ApplyResources(this.toolStripMenuItem12, "toolStripMenuItem12");
			this.toolStripMenuItem12.Checked = true;
			this.toolStripMenuItem12.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem12.Name = "toolStripMenuItem12";
			this.toolStripMenuItem12.Click += new global::System.EventHandler(this.changeIntervalTime);
			resources.ApplyResources(this.toolStripMenuItem13, "toolStripMenuItem13");
			this.toolStripMenuItem13.Checked = true;
			this.toolStripMenuItem13.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem13.Name = "toolStripMenuItem13";
			this.toolStripMenuItem13.Click += new global::System.EventHandler(this.changeIntervalTime);
			resources.ApplyResources(this.toolStrip1, "toolStrip1");
			this.toolStrip1.BackColor = global::System.Drawing.Color.LightSkyBlue;
			this.toolStrip1.GripMargin = new global::System.Windows.Forms.Padding(0);
			this.toolStrip1.ImageScalingSize = new global::System.Drawing.Size(32, 32);
			this.toolStrip1.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.NewButton1,
				this.打开,
				this.toolStripButton1,
				this.toolStripButton5,
				this.toolStripButton3,
				this.串口,
				this.toolStripButton7,
				this.toolStripButton9,
				this.toolStripButton8,
				this.toolStripButton4,
				this.toolStrip实时波形,
				this.toolStripButton6,
				this.toolStripButton14,
				this.startButton,
				this.endButton,
				this.伊万Button
			});
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = global::System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip1.Stretch = true;
			resources.ApplyResources(this.NewButton1, "NewButton1");
			this.NewButton1.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.NewButton1.Name = "NewButton1";
			this.NewButton1.Click += new global::System.EventHandler(this.newFile);
			resources.ApplyResources(this.打开, "打开");
			this.打开.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.打开.Name = "打开";
			this.打开.Click += new global::System.EventHandler(this.打开_Click);
			resources.ApplyResources(this.toolStripButton1, "toolStripButton1");
			this.toolStripButton1.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Click += new global::System.EventHandler(this.toolStripButton1_Click);
			resources.ApplyResources(this.toolStripButton5, "toolStripButton5");
			this.toolStripButton5.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton5.Name = "toolStripButton5";
			this.toolStripButton5.Click += new global::System.EventHandler(this.ToClipboard);
			resources.ApplyResources(this.toolStripButton3, "toolStripButton3");
			this.toolStripButton3.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Click += new global::System.EventHandler(this.打印ToolStripMenuItem_Click);
			resources.ApplyResources(this.串口, "串口");
			this.串口.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.串口.Name = "串口";
			this.串口.Tag = "串口";
			this.串口.Click += new global::System.EventHandler(this.串口_Click);
			resources.ApplyResources(this.toolStripButton7, "toolStripButton7");
			this.toolStripButton7.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton7.Name = "toolStripButton7";
			this.toolStripButton7.Tag = "示波器";
			this.toolStripButton7.Click += new global::System.EventHandler(this.toolStripButton7_Click);
			resources.ApplyResources(this.toolStripButton9, "toolStripButton9");
			this.toolStripButton9.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton9.Name = "toolStripButton9";
			this.toolStripButton9.Tag = "万用表";
			this.toolStripButton9.Click += new global::System.EventHandler(this.toolStripButton9_Click);
			resources.ApplyResources(this.toolStripButton8, "toolStripButton8");
			this.toolStripButton8.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton8.Name = "toolStripButton8";
			this.toolStripButton8.Click += new global::System.EventHandler(this.toolStripButton8_Click);
			resources.ApplyResources(this.toolStripButton4, "toolStripButton4");
			this.toolStripButton4.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton4.Name = "toolStripButton4";
			this.toolStripButton4.Click += new global::System.EventHandler(this.toolStripButton4_Click);
			resources.ApplyResources(this.toolStrip实时波形, "toolStrip实时波形");
			this.toolStrip实时波形.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStrip实时波形.Name = "toolStrip实时波形";
			this.toolStrip实时波形.Click += new global::System.EventHandler(this.toolStrip实时波形_Click);
			resources.ApplyResources(this.toolStripButton6, "toolStripButton6");
			this.toolStripButton6.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton6.Name = "toolStripButton6";
			this.toolStripButton6.Click += new global::System.EventHandler(this.toolStripButton6_Click);
			resources.ApplyResources(this.toolStripButton14, "toolStripButton14");
			this.toolStripButton14.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton14.Name = "toolStripButton14";
			this.toolStripButton14.Tag = "定时";
			this.toolStripButton14.Click += new global::System.EventHandler(this.toolStripButton14_Click);
			resources.ApplyResources(this.startButton, "startButton");
			this.startButton.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.startButton.Name = "startButton";
			this.startButton.Click += new global::System.EventHandler(this.comMenu2_Click);
			resources.ApplyResources(this.endButton, "endButton");
			this.endButton.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.endButton.Name = "endButton";
			this.endButton.Click += new global::System.EventHandler(this.toolStripButton13_Click);
			resources.ApplyResources(this.伊万Button, "伊万Button");
			this.伊万Button.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.伊万Button.Name = "伊万Button";
			this.伊万Button.Click += new global::System.EventHandler(this.伊万_Click);
			this.serialPort1.BaudRate = 57600;
			this.serialPort1.PortName = "COM12";
			this.serialPort1.ReadBufferSize = 100000;
			resources.ApplyResources(this.SerialPortContextMenuStrip2, "SerialPortContextMenuStrip2");
			this.SerialPortContextMenuStrip2.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.toolStripMenuItem14,
				this.toolStripMenuItem15,
				this.toolStripMenuItem16,
				this.toolStripMenuItem17,
				this.toolStripMenuItem18,
				this.toolStripMenuItem19,
				this.toolStripMenuItem20,
				this.toolStripMenuItem21,
				this.toolStripMenuItem22,
				this.toolStripMenuItem28,
				this.cOM11ToolStripMenuItem,
				this.cOM12ToolStripMenuItem,
				this.cOM13ToolStripMenuItem,
				this.cOM14ToolStripMenuItem,
				this.cOM15ToolStripMenuItem
			});
			this.SerialPortContextMenuStrip2.Name = "contextMenuStrip2";
			this.SerialPortContextMenuStrip2.Closed += new global::System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.onClosedContextMenuStrip);
			resources.ApplyResources(this.toolStripMenuItem14, "toolStripMenuItem14");
			this.toolStripMenuItem14.Name = "toolStripMenuItem14";
			this.toolStripMenuItem14.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.toolStripMenuItem15, "toolStripMenuItem15");
			this.toolStripMenuItem15.Name = "toolStripMenuItem15";
			this.toolStripMenuItem15.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.toolStripMenuItem16, "toolStripMenuItem16");
			this.toolStripMenuItem16.Name = "toolStripMenuItem16";
			this.toolStripMenuItem16.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.toolStripMenuItem17, "toolStripMenuItem17");
			this.toolStripMenuItem17.Name = "toolStripMenuItem17";
			this.toolStripMenuItem17.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.toolStripMenuItem18, "toolStripMenuItem18");
			this.toolStripMenuItem18.Name = "toolStripMenuItem18";
			this.toolStripMenuItem18.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.toolStripMenuItem19, "toolStripMenuItem19");
			this.toolStripMenuItem19.Name = "toolStripMenuItem19";
			this.toolStripMenuItem19.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.toolStripMenuItem20, "toolStripMenuItem20");
			this.toolStripMenuItem20.Name = "toolStripMenuItem20";
			this.toolStripMenuItem20.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.toolStripMenuItem21, "toolStripMenuItem21");
			this.toolStripMenuItem21.Name = "toolStripMenuItem21";
			this.toolStripMenuItem21.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.toolStripMenuItem22, "toolStripMenuItem22");
			this.toolStripMenuItem22.Name = "toolStripMenuItem22";
			this.toolStripMenuItem22.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.toolStripMenuItem28, "toolStripMenuItem28");
			this.toolStripMenuItem28.Name = "toolStripMenuItem28";
			this.toolStripMenuItem28.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.cOM11ToolStripMenuItem, "cOM11ToolStripMenuItem");
			this.cOM11ToolStripMenuItem.Name = "cOM11ToolStripMenuItem";
			this.cOM11ToolStripMenuItem.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.cOM12ToolStripMenuItem, "cOM12ToolStripMenuItem");
			this.cOM12ToolStripMenuItem.Name = "cOM12ToolStripMenuItem";
			this.cOM12ToolStripMenuItem.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.cOM13ToolStripMenuItem, "cOM13ToolStripMenuItem");
			this.cOM13ToolStripMenuItem.Name = "cOM13ToolStripMenuItem";
			this.cOM13ToolStripMenuItem.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.cOM14ToolStripMenuItem, "cOM14ToolStripMenuItem");
			this.cOM14ToolStripMenuItem.Name = "cOM14ToolStripMenuItem";
			this.cOM14ToolStripMenuItem.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.cOM15ToolStripMenuItem, "cOM15ToolStripMenuItem");
			this.cOM15ToolStripMenuItem.Name = "cOM15ToolStripMenuItem";
			this.cOM15ToolStripMenuItem.Click += new global::System.EventHandler(this.selectComMenu);
			resources.ApplyResources(this.MultimeterContextMenuStrip1, "MultimeterContextMenuStrip1");
			this.MultimeterContextMenuStrip1.AccessibleRole = global::System.Windows.Forms.AccessibleRole.None;
			this.MultimeterContextMenuStrip1.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.realtimeDataToolStripMenuItem,
				this.autoRecordDataToolStripMenuItem,
				this.ProdctionMessage
			});
			this.MultimeterContextMenuStrip1.Name = "MultimeterContextMenuStrip1";
			this.MultimeterContextMenuStrip1.Closed += new global::System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.onClosedContextMenuStrip);
			resources.ApplyResources(this.realtimeDataToolStripMenuItem, "realtimeDataToolStripMenuItem");
			this.realtimeDataToolStripMenuItem.Name = "realtimeDataToolStripMenuItem";
			this.realtimeDataToolStripMenuItem.Click += new global::System.EventHandler(this.changeDMM_mode);
			resources.ApplyResources(this.autoRecordDataToolStripMenuItem, "autoRecordDataToolStripMenuItem");
			this.autoRecordDataToolStripMenuItem.Name = "autoRecordDataToolStripMenuItem";
			this.autoRecordDataToolStripMenuItem.Click += new global::System.EventHandler(this.changeDMM_mode);
			resources.ApplyResources(this.ProdctionMessage, "ProdctionMessage");
			this.ProdctionMessage.Name = "ProdctionMessage";
			this.ProdctionMessage.Click += new global::System.EventHandler(this.GetProductionMEessage);
			resources.ApplyResources(this.OscillographContextMenuStrip2, "OscillographContextMenuStrip2");
			this.OscillographContextMenuStrip2.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.historicalDataToolStripMenuItem,
				this.realtimeDataToolStripMenuItem1,
				this.realTimeDataForAnalyseToolStripMenuItem
			});
			this.OscillographContextMenuStrip2.Name = "OscillographContextMenuStrip2";
			this.OscillographContextMenuStrip2.Closed += new global::System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.onClosedContextMenuStrip);
			resources.ApplyResources(this.historicalDataToolStripMenuItem, "historicalDataToolStripMenuItem");
			this.historicalDataToolStripMenuItem.Name = "historicalDataToolStripMenuItem";
			this.historicalDataToolStripMenuItem.Click += new global::System.EventHandler(this.OscillographContextMenuStrip2_Click);
			resources.ApplyResources(this.realtimeDataToolStripMenuItem1, "realtimeDataToolStripMenuItem1");
			this.realtimeDataToolStripMenuItem1.Name = "realtimeDataToolStripMenuItem1";
			this.realtimeDataToolStripMenuItem1.Click += new global::System.EventHandler(this.OscillographContextMenuStrip2_Click);
			resources.ApplyResources(this.realTimeDataForAnalyseToolStripMenuItem, "realTimeDataForAnalyseToolStripMenuItem");
			this.realTimeDataForAnalyseToolStripMenuItem.Name = "realTimeDataForAnalyseToolStripMenuItem";
			this.realTimeDataForAnalyseToolStripMenuItem.Click += new global::System.EventHandler(this.OscillographContextMenuStrip2_Click);
			this.printDocument1.PrintPage += new global::System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
			resources.ApplyResources(this.printPreviewDialog1, "printPreviewDialog1");
			this.printPreviewDialog1.Name = "printPreviewDialog1";
			this.printDialog1.UseEXDialog = true;
			resources.ApplyResources(this.button1, "button1");
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new global::System.EventHandler(this.button1_Click);
			resources.ApplyResources(this.DmmAutorecordTextBox, "DmmAutorecordTextBox");
			this.DmmAutorecordTextBox.Name = "DmmAutorecordTextBox";
			this.DmmAutorecordTextBox.ReadOnly = true;
			resources.ApplyResources(this.ChangeHoldStatecontextMenuStrip1, "ChangeHoldStatecontextMenuStrip1");
			this.ChangeHoldStatecontextMenuStrip1.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.changeHoldStateToolstripMenuItem,
				this.leaveScreentoolStripMenuItem23
			});
			this.ChangeHoldStatecontextMenuStrip1.Name = "ChangeHoldStatecontextMenuStrip1";
			this.ChangeHoldStatecontextMenuStrip1.RenderMode = global::System.Windows.Forms.ToolStripRenderMode.System;
			this.ChangeHoldStatecontextMenuStrip1.Closed += new global::System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.onClosedContextMenuStrip);
			resources.ApplyResources(this.changeHoldStateToolstripMenuItem, "changeHoldStateToolstripMenuItem");
			this.changeHoldStateToolstripMenuItem.Name = "changeHoldStateToolstripMenuItem";
			this.changeHoldStateToolstripMenuItem.Click += new global::System.EventHandler(this.changeHoldState);
			resources.ApplyResources(this.leaveScreentoolStripMenuItem23, "leaveScreentoolStripMenuItem23");
			this.leaveScreentoolStripMenuItem23.Name = "leaveScreentoolStripMenuItem23";
			this.leaveScreentoolStripMenuItem23.Click += new global::System.EventHandler(this.changeHoldState);
			resources.ApplyResources(this.saveFileDialog1, "saveFileDialog1");
			this.openFileDialog1.FileName = "openFileDialog1";
			resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.webBrowser1);
			base.Controls.Add(this.DmmAutorecordTextBox);
			base.Controls.Add(this.toolStrip1);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.splitContainer1);
			base.Controls.Add(this.statusStrip1);
			base.Controls.Add(this.menuStrip1);
			base.MainMenuStrip = this.menuStrip1;
			base.Name = "Form1ShiBoQi";
			base.FormClosing += new global::System.Windows.Forms.FormClosingEventHandler(this.FormCloseing);
			base.Load += new global::System.EventHandler(this.Form1ShiBoQi_Load);
			base.Resize += new global::System.EventHandler(this.OnResize);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((global::System.ComponentModel.ISupportInitialize)this.splitContainer2).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox16).EndInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox15).EndInit();
			this.panel2.ResumeLayout(false);
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox14).EndInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox13).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox8).EndInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox5).EndInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox9).EndInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox10).EndInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox11).EndInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox7).EndInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox4).EndInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox6).EndInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox3).EndInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox2).EndInit();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox12).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((global::System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.TimerContextMenuStrip1.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.SerialPortContextMenuStrip2.ResumeLayout(false);
			this.MultimeterContextMenuStrip1.ResumeLayout(false);
			this.OscillographContextMenuStrip2.ResumeLayout(false);
			this.ChangeHoldStatecontextMenuStrip1.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x0400007F RID: 127
		private global::System.ComponentModel.IContainer components;

		// Token: 0x04000080 RID: 128
		private global::System.Windows.Forms.MenuStrip menuStrip1;

		// Token: 0x04000081 RID: 129
		private global::System.Windows.Forms.StatusStrip statusStrip1;

		// Token: 0x04000082 RID: 130
		private global::System.Windows.Forms.SplitContainer splitContainer1;

		// Token: 0x04000083 RID: 131
		private global::System.Windows.Forms.TextBox textBox2;

		// Token: 0x04000084 RID: 132
		private global::System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;

		// Token: 0x04000085 RID: 133
		private global::System.Windows.Forms.ToolStripMenuItem 编辑ToolStripMenuItem;

		// Token: 0x04000086 RID: 134
		private global::System.Windows.Forms.ContextMenuStrip TimerContextMenuStrip1;

		// Token: 0x04000087 RID: 135
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;

		// Token: 0x04000088 RID: 136
		private global::System.Windows.Forms.ToolStripMenuItem 打开ToolStripMenuItem;

		// Token: 0x04000089 RID: 137
		private global::System.Windows.Forms.ToolStripMenuItem 关闭ToolStripMenuItem;

		// Token: 0x0400008A RID: 138
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;

		// Token: 0x0400008B RID: 139
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;

		// Token: 0x0400008C RID: 140
		private global::System.Windows.Forms.ToolStripMenuItem 复制波形ToolStripMenuItem;

		// Token: 0x0400008D RID: 141
		private global::System.Windows.Forms.ToolStripMenuItem 通信ToolStripMenuItem;

		// Token: 0x0400008E RID: 142
		private global::System.Windows.Forms.ToolStripMenuItem 连接ToolStripMenuItem;

		// Token: 0x0400008F RID: 143
		private global::System.Windows.Forms.ToolStripMenuItem 断开ToolStripMenuItem;

		// Token: 0x04000090 RID: 144
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

		// Token: 0x04000091 RID: 145
		private global::System.Windows.Forms.ToolStripMenuItem 运行ToolStripMenuItem;

		// Token: 0x04000092 RID: 146
		private global::System.Windows.Forms.ToolStripMenuItem startMenu;

		// Token: 0x04000093 RID: 147
		private global::System.Windows.Forms.ToolStripMenuItem stopMenu;

		// Token: 0x04000094 RID: 148
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator2;

		// Token: 0x04000095 RID: 149
		private global::System.Windows.Forms.ToolStripMenuItem dMMToolStripMenuItem;

		// Token: 0x04000096 RID: 150
		private global::System.Windows.Forms.ToolStripMenuItem dSOToolStripMenuItem;

		// Token: 0x04000097 RID: 151
		private global::System.Windows.Forms.ToolStripMenuItem dMM记录ToolStripMenuItem;

		// Token: 0x04000098 RID: 152
		private global::System.Windows.Forms.ToolStripMenuItem 开始记录ToolStripMenuItem;

		// Token: 0x04000099 RID: 153
		private global::System.Windows.Forms.ToolStripMenuItem 结束ToolStripMenuItem;

		// Token: 0x0400009A RID: 154
		private global::System.Windows.Forms.ToolStripMenuItem dSOToolStripMenuItem1;

		// Token: 0x0400009B RID: 155
		private global::System.Windows.Forms.ToolStripMenuItem WaveToolStripMenuItem;

		// Token: 0x0400009C RID: 156
		private global::System.Windows.Forms.ToolStripMenuItem fTTToolStripMenuItem;

		// Token: 0x0400009D RID: 157
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator4;

		// Token: 0x0400009E RID: 158
		private global::System.Windows.Forms.ToolStripMenuItem fTTToolStripMenuItem1;

		// Token: 0x0400009F RID: 159
		private global::System.Windows.Forms.ToolStripMenuItem rectangleToolStripMenuItem;

		// Token: 0x040000A0 RID: 160
		private global::System.Windows.Forms.ToolStripMenuItem 频谱显示ToolStripMenuItem;

		// Token: 0x040000A1 RID: 161
		private global::System.Windows.Forms.ToolStripMenuItem 线性ToolStripMenuItem;

		// Token: 0x040000A2 RID: 162
		private global::System.Windows.Forms.ToolStripMenuItem 对数ToolStripMenuItem;

		// Token: 0x040000A3 RID: 163
		private global::System.Windows.Forms.ToolStripMenuItem hanningToolStripMenuItem;

		// Token: 0x040000A4 RID: 164
		private global::System.Windows.Forms.ToolStripMenuItem hammingToolStripMenuItem;

		// Token: 0x040000A5 RID: 165
		private global::System.Windows.Forms.ToolStripMenuItem blackmanToolStripMenuItem;

		// Token: 0x040000A6 RID: 166
		private global::System.Windows.Forms.ToolStripMenuItem flattopToolStripMenuItem;

		// Token: 0x040000A7 RID: 167
		private global::System.Windows.Forms.ToolStripMenuItem britlittToolStripMenuItem;

		// Token: 0x040000A8 RID: 168
		private global::System.Windows.Forms.WebBrowser webBrowser1;

		// Token: 0x040000A9 RID: 169
		private global::System.Windows.Forms.ToolStripButton NewButton1;

		// Token: 0x040000AA RID: 170
		private global::System.Windows.Forms.ToolStrip toolStrip1;

		// Token: 0x040000AB RID: 171
		private global::System.Windows.Forms.ToolStripButton 打开;

		// Token: 0x040000AC RID: 172
		private global::System.Windows.Forms.ToolStripButton toolStripButton3;

		// Token: 0x040000AD RID: 173
		private global::System.Windows.Forms.ToolStripButton toolStripButton4;

		// Token: 0x040000AE RID: 174
		private global::System.Windows.Forms.ToolStripButton 伊万Button;

		// Token: 0x040000AF RID: 175
		private global::System.Windows.Forms.ToolStripButton 串口;

		// Token: 0x040000B0 RID: 176
		private global::System.Windows.Forms.ToolStripButton toolStripButton7;

		// Token: 0x040000B1 RID: 177
		private global::System.Windows.Forms.ToolStripButton toolStripButton6;

		// Token: 0x040000B2 RID: 178
		private global::System.Windows.Forms.ToolStripButton toolStripButton8;

		// Token: 0x040000B3 RID: 179
		private global::System.Windows.Forms.SplitContainer splitContainer2;

		// Token: 0x040000B4 RID: 180
		private global::System.Windows.Forms.Panel panel1;

		// Token: 0x040000B5 RID: 181
		private global::System.Windows.Forms.Label label6;

		// Token: 0x040000B6 RID: 182
		private global::System.Windows.Forms.Label label5;

		// Token: 0x040000B7 RID: 183
		private global::System.Windows.Forms.Label label4;

		// Token: 0x040000B8 RID: 184
		private global::System.Windows.Forms.Label label3;

		// Token: 0x040000B9 RID: 185
		private global::System.Windows.Forms.Label label2;

		// Token: 0x040000BA RID: 186
		private global::LEDConTroler.LED axLED9;

		// Token: 0x040000BB RID: 187
		private global::System.Windows.Forms.PictureBox pictureBox9;

		// Token: 0x040000BC RID: 188
		private global::LEDConTroler.LED axLED10;

		// Token: 0x040000BD RID: 189
		private global::System.Windows.Forms.PictureBox pictureBox10;

		// Token: 0x040000BE RID: 190
		private global::LEDConTroler.LED axLED11;

		// Token: 0x040000BF RID: 191
		private global::System.Windows.Forms.PictureBox pictureBox11;

		// Token: 0x040000C0 RID: 192
		private global::LEDConTroler.LED axLED12;

		// Token: 0x040000C1 RID: 193
		private global::LEDConTroler.LED axLED6;

		// Token: 0x040000C2 RID: 194
		private global::System.Windows.Forms.PictureBox pictureBox7;

		// Token: 0x040000C3 RID: 195
		private global::LEDConTroler.LED axLED7;

		// Token: 0x040000C4 RID: 196
		private global::System.Windows.Forms.PictureBox pictureBox4;

		// Token: 0x040000C5 RID: 197
		private global::LEDConTroler.LED axLED5;

		// Token: 0x040000C6 RID: 198
		private global::System.Windows.Forms.PictureBox pictureBox6;

		// Token: 0x040000C7 RID: 199
		private global::LEDConTroler.LED axLED8;

		// Token: 0x040000C8 RID: 200
		private global::System.Windows.Forms.PictureBox pictureBox3;

		// Token: 0x040000C9 RID: 201
		private global::System.Windows.Forms.PictureBox pictureBox2;

		// Token: 0x040000CA RID: 202
		private global::System.Windows.Forms.PictureBox pictureBox12;

		// Token: 0x040000CB RID: 203
		private global::LEDConTroler.LED axLED4;

		// Token: 0x040000CC RID: 204
		private global::LEDConTroler.LED axLED3;

		// Token: 0x040000CD RID: 205
		private global::LEDConTroler.LED axLED2;

		// Token: 0x040000CE RID: 206
		private global::LEDConTroler.LED axLED1;

		// Token: 0x040000CF RID: 207
		private global::System.IO.Ports.SerialPort serialPort1;

		// Token: 0x040000D0 RID: 208
		private global::System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;

		// Token: 0x040000D1 RID: 209
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator6;

		// Token: 0x040000D2 RID: 210
		private global::System.Windows.Forms.ToolStripMenuItem 打印ToolStripMenuItem;

		// Token: 0x040000D3 RID: 211
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator5;

		// Token: 0x040000D4 RID: 212
		private global::System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;

		// Token: 0x040000D5 RID: 213
		private global::System.Windows.Forms.ToolStripMenuItem 帮助主题ToolStripMenuItem;

		// Token: 0x040000D6 RID: 214
		private global::System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem;

		// Token: 0x040000D7 RID: 215
		private global::System.Windows.Forms.ToolStripButton toolStripButton1;

		// Token: 0x040000D8 RID: 216
		private global::System.Windows.Forms.ToolStripButton toolStripButton5;

		// Token: 0x040000D9 RID: 217
		private global::System.Windows.Forms.ToolStripButton toolStripButton9;

		// Token: 0x040000DA RID: 218
		private global::System.Windows.Forms.ToolStripButton startButton;

		// Token: 0x040000DB RID: 219
		private global::System.Windows.Forms.ToolStripButton endButton;

		// Token: 0x040000DC RID: 220
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;

		// Token: 0x040000DD RID: 221
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;

		// Token: 0x040000DE RID: 222
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;

		// Token: 0x040000DF RID: 223
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;

		// Token: 0x040000E0 RID: 224
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;

		// Token: 0x040000E1 RID: 225
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;

		// Token: 0x040000E2 RID: 226
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;

		// Token: 0x040000E3 RID: 227
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;

		// Token: 0x040000E4 RID: 228
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12;

		// Token: 0x040000E5 RID: 229
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem13;

		// Token: 0x040000E6 RID: 230
		private global::System.Windows.Forms.ToolStripButton toolStripButton14;

		// Token: 0x040000E7 RID: 231
		private global::System.Windows.Forms.ContextMenuStrip SerialPortContextMenuStrip2;

		// Token: 0x040000E8 RID: 232
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem14;

		// Token: 0x040000E9 RID: 233
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem15;

		// Token: 0x040000EA RID: 234
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem16;

		// Token: 0x040000EB RID: 235
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem17;

		// Token: 0x040000EC RID: 236
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem18;

		// Token: 0x040000ED RID: 237
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem19;

		// Token: 0x040000EE RID: 238
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem20;

		// Token: 0x040000EF RID: 239
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem21;

		// Token: 0x040000F0 RID: 240
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem22;

		// Token: 0x040000F1 RID: 241
		private global::System.Windows.Forms.ToolStripStatusLabel 通讯状态;

		// Token: 0x040000F2 RID: 242
		private global::System.Windows.Forms.ToolStripStatusLabel 仪表;

		// Token: 0x040000F3 RID: 243
		private global::System.Windows.Forms.ContextMenuStrip MultimeterContextMenuStrip1;

		// Token: 0x040000F4 RID: 244
		private global::System.Windows.Forms.ContextMenuStrip OscillographContextMenuStrip2;

		// Token: 0x040000F5 RID: 245
		private global::System.Windows.Forms.ToolStripMenuItem realtimeDataToolStripMenuItem;

		// Token: 0x040000F6 RID: 246
		private global::System.Windows.Forms.ToolStripMenuItem autoRecordDataToolStripMenuItem;

		// Token: 0x040000F7 RID: 247
		private global::System.Windows.Forms.ToolStripMenuItem historicalDataToolStripMenuItem;

		// Token: 0x040000F8 RID: 248
		private global::System.Windows.Forms.ToolStripMenuItem realtimeDataToolStripMenuItem1;

		// Token: 0x040000F9 RID: 249
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator9;

		// Token: 0x040000FA RID: 250
		private global::System.Windows.Forms.ToolStripMenuItem 全屏显示ToolStripMenuItem;

		// Token: 0x040000FB RID: 251
		private global::System.Drawing.Printing.PrintDocument printDocument1;

		// Token: 0x040000FC RID: 252
		private global::System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;

		// Token: 0x040000FD RID: 253
		private global::System.Windows.Forms.PrintDialog printDialog1;

		// Token: 0x040000FE RID: 254
		private global::System.Windows.Forms.ToolStripMenuItem printPreviewToolStripMenuItem;

		// Token: 0x040000FF RID: 255
		private global::System.Windows.Forms.Button button1;

		// Token: 0x04000100 RID: 256
		private global::System.ServiceProcess.ServiceController serviceController1;

		// Token: 0x04000101 RID: 257
		private global::System.Windows.Forms.HScrollBar hScrollBar1;

		// Token: 0x04000102 RID: 258
		private global::System.Windows.Forms.ToolStripMenuItem communicationTestToolStripMenuItem;

		// Token: 0x04000103 RID: 259
		private global::System.Windows.Forms.Panel panel2;

		// Token: 0x04000104 RID: 260
		private global::System.Windows.Forms.Panel panel3;

		// Token: 0x04000105 RID: 261
		private global::System.Windows.Forms.Button button3;

		// Token: 0x04000106 RID: 262
		private global::System.Windows.Forms.Button button2;

		// Token: 0x04000107 RID: 263
		private global::System.Windows.Forms.Button button5;

		// Token: 0x04000108 RID: 264
		private global::System.Windows.Forms.Button button4;

		// Token: 0x04000109 RID: 265
		private global::System.Windows.Forms.ToolStripMenuItem realTimeDataForAnalyseToolStripMenuItem;

		// Token: 0x0400010A RID: 266
		private global::System.Windows.Forms.PictureBox pictureBox1;

		// Token: 0x0400010B RID: 267
		private global::System.Windows.Forms.PictureBox pictureBox8;

		// Token: 0x0400010C RID: 268
		private global::System.Windows.Forms.PictureBox pictureBox5;

		// Token: 0x0400010D RID: 269
		private global::LEDConTroler.LED axLED13;

		// Token: 0x0400010E RID: 270
		private global::System.Windows.Forms.TextBox DmmAutorecordTextBox;

		// Token: 0x0400010F RID: 271
		private global::System.Windows.Forms.ToolStripMenuItem ProdctionMessage;

		// Token: 0x04000110 RID: 272
		private global::System.Windows.Forms.ContextMenuStrip ChangeHoldStatecontextMenuStrip1;

		// Token: 0x04000111 RID: 273
		private global::System.Windows.Forms.ToolStripMenuItem changeHoldStateToolstripMenuItem;

		// Token: 0x04000112 RID: 274
		private global::System.Windows.Forms.ToolStripMenuItem leaveScreentoolStripMenuItem23;

		// Token: 0x04000113 RID: 275
		private global::System.Windows.Forms.ToolStripButton toolStrip实时波形;

		// Token: 0x04000114 RID: 276
		private global::System.Windows.Forms.SaveFileDialog saveFileDialog1;

		// Token: 0x04000115 RID: 277
		private global::System.Windows.Forms.OpenFileDialog openFileDialog1;

		// Token: 0x04000116 RID: 278
		private global::System.Windows.Forms.PictureBox pictureBox13;

		// Token: 0x04000117 RID: 279
		private global::System.Windows.Forms.PictureBox pictureBox16;

		// Token: 0x04000118 RID: 280
		private global::System.Windows.Forms.PictureBox pictureBox15;

		// Token: 0x04000119 RID: 281
		private global::System.Windows.Forms.PictureBox pictureBox14;

		// Token: 0x0400011A RID: 282
		private global::System.Windows.Forms.Button button6;

		// Token: 0x0400011B RID: 283
		private global::System.Windows.Forms.Label AutoMode;

		// Token: 0x0400011C RID: 284
		private global::示波器.MyPanel panel4;

		// Token: 0x0400011D RID: 285
		private global::ZedGraph.ZedGraphControl GraphControl1;

		// Token: 0x0400011E RID: 286
		private global::System.Windows.Forms.ToolStripMenuItem cOM11ToolStripMenuItem;

		// Token: 0x0400011F RID: 287
		private global::System.Windows.Forms.ToolStripMenuItem cOM12ToolStripMenuItem;

		// Token: 0x04000120 RID: 288
		private global::System.Windows.Forms.ToolStripMenuItem cOM13ToolStripMenuItem;

		// Token: 0x04000121 RID: 289
		private global::System.Windows.Forms.ToolStripMenuItem cOM14ToolStripMenuItem;

		// Token: 0x04000122 RID: 290
		private global::System.Windows.Forms.ToolStripMenuItem cOM15ToolStripMenuItem;

		// Token: 0x04000123 RID: 291
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem28;

		// Token: 0x04000124 RID: 292
		private global::System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;

		// Token: 0x04000125 RID: 293
		private global::System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;

		// Token: 0x04000126 RID: 294
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem23;
	}
}
