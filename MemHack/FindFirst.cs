using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;

namespace MemHack
{
	/// <summary>
	/// Summary description for FindFirst.
	/// </summary>
	public class FindFirst : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnFindFirst;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.TextBox textValue;
		public System.Windows.Forms.RadioButton radioLong;
		public System.Windows.Forms.RadioButton radioInt;
		public System.Windows.Forms.RadioButton radioShort;
		public System.Windows.Forms.RadioButton radioByte;
		public System.Windows.Forms.GroupBox grpRadio;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FindFirst()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.btnFindFirst.Text = Resources.FindFirst;
			this.label1.Text = Resources.ValueLabel;
			this.grpRadio.Text = Resources.SearchSizeLabel;
			this.radioLong.Text = Resources.Bytes8;
			this.radioInt.Text = Resources.Bytes4;
			this.radioShort.Text = Resources.Bytes2;
			this.radioByte.Text = Resources.Bytes1;
			this.Text = Resources.FindFirstValue;
			(new ToolTip()).SetToolTip(this, Resources.ToolTip_FindFirstDlg);
			(new ToolTip()).SetToolTip(this.btnFindFirst, Resources.ToolTip_FindFirstButton);
			(new ToolTip()).SetToolTip(this.radioByte, Resources.ToolTip_FindFirst1Byte);
			(new ToolTip()).SetToolTip(this.radioShort, Resources.ToolTip_FindFirst2Bytes);
			(new ToolTip()).SetToolTip(this.radioInt, Resources.ToolTip_FindFirst4Bytes);
			(new ToolTip()).SetToolTip(this.radioLong, Resources.ToolTip_FindFirst8Bytes);
			(new ToolTip()).SetToolTip(this.textValue, Resources.ToolTip_FindFirstValue);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FindFirst));
			this.btnFindFirst = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textValue = new System.Windows.Forms.TextBox();
			this.grpRadio = new System.Windows.Forms.GroupBox();
			this.radioLong = new System.Windows.Forms.RadioButton();
			this.radioInt = new System.Windows.Forms.RadioButton();
			this.radioShort = new System.Windows.Forms.RadioButton();
			this.radioByte = new System.Windows.Forms.RadioButton();
			this.grpRadio.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnFindFirst
			// 
			this.btnFindFirst.AccessibleDescription = resources.GetString("btnFindFirst.AccessibleDescription");
			this.btnFindFirst.AccessibleName = resources.GetString("btnFindFirst.AccessibleName");
			this.btnFindFirst.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnFindFirst.Anchor")));
			this.btnFindFirst.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFindFirst.BackgroundImage")));
			this.btnFindFirst.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnFindFirst.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnFindFirst.Dock")));
			this.btnFindFirst.Enabled = ((bool)(resources.GetObject("btnFindFirst.Enabled")));
			this.btnFindFirst.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnFindFirst.FlatStyle")));
			this.btnFindFirst.Font = ((System.Drawing.Font)(resources.GetObject("btnFindFirst.Font")));
			this.btnFindFirst.Image = ((System.Drawing.Image)(resources.GetObject("btnFindFirst.Image")));
			this.btnFindFirst.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnFindFirst.ImageAlign")));
			this.btnFindFirst.ImageIndex = ((int)(resources.GetObject("btnFindFirst.ImageIndex")));
			this.btnFindFirst.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnFindFirst.ImeMode")));
			this.btnFindFirst.Location = ((System.Drawing.Point)(resources.GetObject("btnFindFirst.Location")));
			this.btnFindFirst.Name = "btnFindFirst";
			this.btnFindFirst.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnFindFirst.RightToLeft")));
			this.btnFindFirst.Size = ((System.Drawing.Size)(resources.GetObject("btnFindFirst.Size")));
			this.btnFindFirst.TabIndex = ((int)(resources.GetObject("btnFindFirst.TabIndex")));
			this.btnFindFirst.Text = resources.GetString("btnFindFirst.Text");
			this.btnFindFirst.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnFindFirst.TextAlign")));
			this.btnFindFirst.Visible = ((bool)(resources.GetObject("btnFindFirst.Visible")));
			// 
			// label1
			// 
			this.label1.AccessibleDescription = resources.GetString("label1.AccessibleDescription");
			this.label1.AccessibleName = resources.GetString("label1.AccessibleName");
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
			this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
			this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
			this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
			this.label1.Font = ((System.Drawing.Font)(resources.GetObject("label1.Font")));
			this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
			this.label1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.ImageAlign")));
			this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
			this.label1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label1.ImeMode")));
			this.label1.Location = ((System.Drawing.Point)(resources.GetObject("label1.Location")));
			this.label1.Name = "label1";
			this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label1.RightToLeft")));
			this.label1.Size = ((System.Drawing.Size)(resources.GetObject("label1.Size")));
			this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.TextAlign")));
			this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
			// 
			// textValue
			// 
			this.textValue.AccessibleDescription = resources.GetString("textValue.AccessibleDescription");
			this.textValue.AccessibleName = resources.GetString("textValue.AccessibleName");
			this.textValue.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("textValue.Anchor")));
			this.textValue.AutoSize = ((bool)(resources.GetObject("textValue.AutoSize")));
			this.textValue.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("textValue.BackgroundImage")));
			this.textValue.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("textValue.Dock")));
			this.textValue.Enabled = ((bool)(resources.GetObject("textValue.Enabled")));
			this.textValue.Font = ((System.Drawing.Font)(resources.GetObject("textValue.Font")));
			this.textValue.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("textValue.ImeMode")));
			this.textValue.Location = ((System.Drawing.Point)(resources.GetObject("textValue.Location")));
			this.textValue.MaxLength = ((int)(resources.GetObject("textValue.MaxLength")));
			this.textValue.Multiline = ((bool)(resources.GetObject("textValue.Multiline")));
			this.textValue.Name = "textValue";
			this.textValue.PasswordChar = ((char)(resources.GetObject("textValue.PasswordChar")));
			this.textValue.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("textValue.RightToLeft")));
			this.textValue.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("textValue.ScrollBars")));
			this.textValue.Size = ((System.Drawing.Size)(resources.GetObject("textValue.Size")));
			this.textValue.TabIndex = ((int)(resources.GetObject("textValue.TabIndex")));
			this.textValue.Text = resources.GetString("textValue.Text");
			this.textValue.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("textValue.TextAlign")));
			this.textValue.Visible = ((bool)(resources.GetObject("textValue.Visible")));
			this.textValue.WordWrap = ((bool)(resources.GetObject("textValue.WordWrap")));
			// 
			// grpRadio
			// 
			this.grpRadio.AccessibleDescription = resources.GetString("grpRadio.AccessibleDescription");
			this.grpRadio.AccessibleName = resources.GetString("grpRadio.AccessibleName");
			this.grpRadio.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("grpRadio.Anchor")));
			this.grpRadio.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("grpRadio.BackgroundImage")));
			this.grpRadio.Controls.Add(this.radioLong);
			this.grpRadio.Controls.Add(this.radioInt);
			this.grpRadio.Controls.Add(this.radioShort);
			this.grpRadio.Controls.Add(this.radioByte);
			this.grpRadio.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("grpRadio.Dock")));
			this.grpRadio.Enabled = ((bool)(resources.GetObject("grpRadio.Enabled")));
			this.grpRadio.Font = ((System.Drawing.Font)(resources.GetObject("grpRadio.Font")));
			this.grpRadio.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("grpRadio.ImeMode")));
			this.grpRadio.Location = ((System.Drawing.Point)(resources.GetObject("grpRadio.Location")));
			this.grpRadio.Name = "grpRadio";
			this.grpRadio.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("grpRadio.RightToLeft")));
			this.grpRadio.Size = ((System.Drawing.Size)(resources.GetObject("grpRadio.Size")));
			this.grpRadio.TabIndex = ((int)(resources.GetObject("grpRadio.TabIndex")));
			this.grpRadio.TabStop = false;
			this.grpRadio.Text = resources.GetString("grpRadio.Text");
			this.grpRadio.Visible = ((bool)(resources.GetObject("grpRadio.Visible")));
			// 
			// radioLong
			// 
			this.radioLong.AccessibleDescription = resources.GetString("radioLong.AccessibleDescription");
			this.radioLong.AccessibleName = resources.GetString("radioLong.AccessibleName");
			this.radioLong.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("radioLong.Anchor")));
			this.radioLong.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("radioLong.Appearance")));
			this.radioLong.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("radioLong.BackgroundImage")));
			this.radioLong.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radioLong.CheckAlign")));
			this.radioLong.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("radioLong.Dock")));
			this.radioLong.Enabled = ((bool)(resources.GetObject("radioLong.Enabled")));
			this.radioLong.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("radioLong.FlatStyle")));
			this.radioLong.Font = ((System.Drawing.Font)(resources.GetObject("radioLong.Font")));
			this.radioLong.Image = ((System.Drawing.Image)(resources.GetObject("radioLong.Image")));
			this.radioLong.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radioLong.ImageAlign")));
			this.radioLong.ImageIndex = ((int)(resources.GetObject("radioLong.ImageIndex")));
			this.radioLong.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("radioLong.ImeMode")));
			this.radioLong.Location = ((System.Drawing.Point)(resources.GetObject("radioLong.Location")));
			this.radioLong.Name = "radioLong";
			this.radioLong.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("radioLong.RightToLeft")));
			this.radioLong.Size = ((System.Drawing.Size)(resources.GetObject("radioLong.Size")));
			this.radioLong.TabIndex = ((int)(resources.GetObject("radioLong.TabIndex")));
			this.radioLong.Text = resources.GetString("radioLong.Text");
			this.radioLong.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radioLong.TextAlign")));
			this.radioLong.Visible = ((bool)(resources.GetObject("radioLong.Visible")));
			// 
			// radioInt
			// 
			this.radioInt.AccessibleDescription = resources.GetString("radioInt.AccessibleDescription");
			this.radioInt.AccessibleName = resources.GetString("radioInt.AccessibleName");
			this.radioInt.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("radioInt.Anchor")));
			this.radioInt.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("radioInt.Appearance")));
			this.radioInt.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("radioInt.BackgroundImage")));
			this.radioInt.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radioInt.CheckAlign")));
			this.radioInt.Checked = true;
			this.radioInt.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("radioInt.Dock")));
			this.radioInt.Enabled = ((bool)(resources.GetObject("radioInt.Enabled")));
			this.radioInt.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("radioInt.FlatStyle")));
			this.radioInt.Font = ((System.Drawing.Font)(resources.GetObject("radioInt.Font")));
			this.radioInt.Image = ((System.Drawing.Image)(resources.GetObject("radioInt.Image")));
			this.radioInt.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radioInt.ImageAlign")));
			this.radioInt.ImageIndex = ((int)(resources.GetObject("radioInt.ImageIndex")));
			this.radioInt.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("radioInt.ImeMode")));
			this.radioInt.Location = ((System.Drawing.Point)(resources.GetObject("radioInt.Location")));
			this.radioInt.Name = "radioInt";
			this.radioInt.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("radioInt.RightToLeft")));
			this.radioInt.Size = ((System.Drawing.Size)(resources.GetObject("radioInt.Size")));
			this.radioInt.TabIndex = ((int)(resources.GetObject("radioInt.TabIndex")));
			this.radioInt.TabStop = true;
			this.radioInt.Text = resources.GetString("radioInt.Text");
			this.radioInt.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radioInt.TextAlign")));
			this.radioInt.Visible = ((bool)(resources.GetObject("radioInt.Visible")));
			// 
			// radioShort
			// 
			this.radioShort.AccessibleDescription = resources.GetString("radioShort.AccessibleDescription");
			this.radioShort.AccessibleName = resources.GetString("radioShort.AccessibleName");
			this.radioShort.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("radioShort.Anchor")));
			this.radioShort.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("radioShort.Appearance")));
			this.radioShort.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("radioShort.BackgroundImage")));
			this.radioShort.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radioShort.CheckAlign")));
			this.radioShort.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("radioShort.Dock")));
			this.radioShort.Enabled = ((bool)(resources.GetObject("radioShort.Enabled")));
			this.radioShort.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("radioShort.FlatStyle")));
			this.radioShort.Font = ((System.Drawing.Font)(resources.GetObject("radioShort.Font")));
			this.radioShort.Image = ((System.Drawing.Image)(resources.GetObject("radioShort.Image")));
			this.radioShort.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radioShort.ImageAlign")));
			this.radioShort.ImageIndex = ((int)(resources.GetObject("radioShort.ImageIndex")));
			this.radioShort.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("radioShort.ImeMode")));
			this.radioShort.Location = ((System.Drawing.Point)(resources.GetObject("radioShort.Location")));
			this.radioShort.Name = "radioShort";
			this.radioShort.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("radioShort.RightToLeft")));
			this.radioShort.Size = ((System.Drawing.Size)(resources.GetObject("radioShort.Size")));
			this.radioShort.TabIndex = ((int)(resources.GetObject("radioShort.TabIndex")));
			this.radioShort.Text = resources.GetString("radioShort.Text");
			this.radioShort.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radioShort.TextAlign")));
			this.radioShort.Visible = ((bool)(resources.GetObject("radioShort.Visible")));
			// 
			// radioByte
			// 
			this.radioByte.AccessibleDescription = resources.GetString("radioByte.AccessibleDescription");
			this.radioByte.AccessibleName = resources.GetString("radioByte.AccessibleName");
			this.radioByte.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("radioByte.Anchor")));
			this.radioByte.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("radioByte.Appearance")));
			this.radioByte.BackColor = System.Drawing.SystemColors.Control;
			this.radioByte.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("radioByte.BackgroundImage")));
			this.radioByte.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radioByte.CheckAlign")));
			this.radioByte.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("radioByte.Dock")));
			this.radioByte.Enabled = ((bool)(resources.GetObject("radioByte.Enabled")));
			this.radioByte.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("radioByte.FlatStyle")));
			this.radioByte.Font = ((System.Drawing.Font)(resources.GetObject("radioByte.Font")));
			this.radioByte.Image = ((System.Drawing.Image)(resources.GetObject("radioByte.Image")));
			this.radioByte.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radioByte.ImageAlign")));
			this.radioByte.ImageIndex = ((int)(resources.GetObject("radioByte.ImageIndex")));
			this.radioByte.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("radioByte.ImeMode")));
			this.radioByte.Location = ((System.Drawing.Point)(resources.GetObject("radioByte.Location")));
			this.radioByte.Name = "radioByte";
			this.radioByte.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("radioByte.RightToLeft")));
			this.radioByte.Size = ((System.Drawing.Size)(resources.GetObject("radioByte.Size")));
			this.radioByte.TabIndex = ((int)(resources.GetObject("radioByte.TabIndex")));
			this.radioByte.Text = resources.GetString("radioByte.Text");
			this.radioByte.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radioByte.TextAlign")));
			this.radioByte.Visible = ((bool)(resources.GetObject("radioByte.Visible")));
			// 
			// FindFirst
			// 
			this.AcceptButton = this.btnFindFirst;
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.grpRadio);
			this.Controls.Add(this.btnFindFirst);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textValue);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "FindFirst";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.grpRadio.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
