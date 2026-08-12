using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HandleINI;

namespace 示波器
{
	// Token: 0x0200001C RID: 28
	public class LoadDrawingIDDialog : Form
	{
		// Token: 0x060000B6 RID: 182 RVA: 0x00010198 File Offset: 0x0000E398
		public LoadDrawingIDDialog()
		{
			this.InitializeComponent();
			this.numericUpDown1.Maximum = 200m;
			IniRW ini = ClassINI.getIni();
			this.selectedNumber = ini.ReadInt("1", "NUM");
			this.numericUpDown1.Text = this.selectedNumber.ToString();
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000101F8 File Offset: 0x0000E3F8
		private void button1_Click(object sender, EventArgs e)
		{
			this.selectedNumber = int.Parse(this.numericUpDown1.Text);
			base.DialogResult = DialogResult.OK;
			base.Close();
			ClassINI.getIni().WriteValue("1", "NUM", this.selectedNumber);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000022D4 File Offset: 0x000004D4
		private void Form2_Load(object sender, EventArgs e)
		{
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x0000269C File Offset: 0x0000089C
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00010248 File Offset: 0x0000E448
		private void InitializeComponent()
		{
			this.button1 = new Button();
			this.图号 = new Label();
			this.numericUpDown1 = new NumericUpDown();
			((ISupportInitialize)this.numericUpDown1).BeginInit();
			base.SuspendLayout();
			this.button1.Location = new Point(176, 47);
			this.button1.Name = "button1";
			this.button1.Size = new Size(94, 38);
			this.button1.TabIndex = 0;
			this.button1.Text = "Load";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += this.button1_Click;
			this.图号.AutoSize = true;
			this.图号.Location = new Point(31, 22);
			this.图号.Name = "图号";
			this.图号.Size = new Size(131, 12);
			this.图号.TabIndex = 2;
			this.图号.Text = "ID of Drawing to Load";
			this.numericUpDown1.Font = new Font("宋体", 21.75f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.numericUpDown1.Location = new Point(63, 47);
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new Size(60, 41);
			this.numericUpDown1.TabIndex = 5;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(292, 117);
			base.Controls.Add(this.numericUpDown1);
			base.Controls.Add(this.图号);
			base.Controls.Add(this.button1);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "LoadDrawingIDDialog";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Form2";
			base.Load += this.Form2_Load;
			((ISupportInitialize)this.numericUpDown1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x040001BB RID: 443
		public int selectedNumber;

		// Token: 0x040001BC RID: 444
		private IContainer components;

		// Token: 0x040001BD RID: 445
		private Button button1;

		// Token: 0x040001BE RID: 446
		private Label 图号;

		// Token: 0x040001BF RID: 447
		private NumericUpDown numericUpDown1;
	}
}
