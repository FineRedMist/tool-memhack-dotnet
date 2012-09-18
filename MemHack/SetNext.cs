using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MemHack
{
	/// <summary>
	/// Summary description for SetNext.
	/// </summary>
	public class SetNext : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblHeader;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtValue;
		private System.Windows.Forms.Button btnJob;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SetNext(bool FindNext, uint addr, ulong val)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.label2.Text = Resources.ValueLabel;
			this.btnJob.Text = Resources.FindNext;

			if(FindNext)
			{
				Text = Resources.FindNext;
				lblHeader.Text = Resources.SelectNext;
			}
			else
			{
				Text = Resources.SetValue;
				btnJob.Text = Resources.Set;
				lblHeader.Text = String.Format(Resources.ValueAtAddressFormatString, addr.ToString("X8"), val);
			}
			txtValue.Text = val.ToString();
		}

		public string Value
		{
			get{return txtValue.Text;}
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SetNext));
			this.lblHeader = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtValue = new System.Windows.Forms.TextBox();
			this.btnJob = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblHeader
			// 
			this.lblHeader.AccessibleDescription = resources.GetString("lblHeader.AccessibleDescription");
			this.lblHeader.AccessibleName = resources.GetString("lblHeader.AccessibleName");
			this.lblHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblHeader.Anchor")));
			this.lblHeader.AutoSize = ((bool)(resources.GetObject("lblHeader.AutoSize")));
			this.lblHeader.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblHeader.Dock")));
			this.lblHeader.Enabled = ((bool)(resources.GetObject("lblHeader.Enabled")));
			this.lblHeader.Font = ((System.Drawing.Font)(resources.GetObject("lblHeader.Font")));
			this.lblHeader.Image = ((System.Drawing.Image)(resources.GetObject("lblHeader.Image")));
			this.lblHeader.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblHeader.ImageAlign")));
			this.lblHeader.ImageIndex = ((int)(resources.GetObject("lblHeader.ImageIndex")));
			this.lblHeader.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblHeader.ImeMode")));
			this.lblHeader.Location = ((System.Drawing.Point)(resources.GetObject("lblHeader.Location")));
			this.lblHeader.Name = "lblHeader";
			this.lblHeader.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblHeader.RightToLeft")));
			this.lblHeader.Size = ((System.Drawing.Size)(resources.GetObject("lblHeader.Size")));
			this.lblHeader.TabIndex = ((int)(resources.GetObject("lblHeader.TabIndex")));
			this.lblHeader.Text = resources.GetString("lblHeader.Text");
			this.lblHeader.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblHeader.TextAlign")));
			this.lblHeader.Visible = ((bool)(resources.GetObject("lblHeader.Visible")));
			// 
			// label2
			// 
			this.label2.AccessibleDescription = resources.GetString("label2.AccessibleDescription");
			this.label2.AccessibleName = resources.GetString("label2.AccessibleName");
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label2.Anchor")));
			this.label2.AutoSize = ((bool)(resources.GetObject("label2.AutoSize")));
			this.label2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label2.Dock")));
			this.label2.Enabled = ((bool)(resources.GetObject("label2.Enabled")));
			this.label2.Font = ((System.Drawing.Font)(resources.GetObject("label2.Font")));
			this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
			this.label2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.ImageAlign")));
			this.label2.ImageIndex = ((int)(resources.GetObject("label2.ImageIndex")));
			this.label2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label2.ImeMode")));
			this.label2.Location = ((System.Drawing.Point)(resources.GetObject("label2.Location")));
			this.label2.Name = "label2";
			this.label2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label2.RightToLeft")));
			this.label2.Size = ((System.Drawing.Size)(resources.GetObject("label2.Size")));
			this.label2.TabIndex = ((int)(resources.GetObject("label2.TabIndex")));
			this.label2.Text = resources.GetString("label2.Text");
			this.label2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.TextAlign")));
			this.label2.Visible = ((bool)(resources.GetObject("label2.Visible")));
			// 
			// txtValue
			// 
			this.txtValue.AccessibleDescription = resources.GetString("txtValue.AccessibleDescription");
			this.txtValue.AccessibleName = resources.GetString("txtValue.AccessibleName");
			this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtValue.Anchor")));
			this.txtValue.AutoSize = ((bool)(resources.GetObject("txtValue.AutoSize")));
			this.txtValue.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtValue.BackgroundImage")));
			this.txtValue.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtValue.Dock")));
			this.txtValue.Enabled = ((bool)(resources.GetObject("txtValue.Enabled")));
			this.txtValue.Font = ((System.Drawing.Font)(resources.GetObject("txtValue.Font")));
			this.txtValue.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtValue.ImeMode")));
			this.txtValue.Location = ((System.Drawing.Point)(resources.GetObject("txtValue.Location")));
			this.txtValue.MaxLength = ((int)(resources.GetObject("txtValue.MaxLength")));
			this.txtValue.Multiline = ((bool)(resources.GetObject("txtValue.Multiline")));
			this.txtValue.Name = "txtValue";
			this.txtValue.PasswordChar = ((char)(resources.GetObject("txtValue.PasswordChar")));
			this.txtValue.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtValue.RightToLeft")));
			this.txtValue.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtValue.ScrollBars")));
			this.txtValue.Size = ((System.Drawing.Size)(resources.GetObject("txtValue.Size")));
			this.txtValue.TabIndex = ((int)(resources.GetObject("txtValue.TabIndex")));
			this.txtValue.Text = resources.GetString("txtValue.Text");
			this.txtValue.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtValue.TextAlign")));
			this.txtValue.Visible = ((bool)(resources.GetObject("txtValue.Visible")));
			this.txtValue.WordWrap = ((bool)(resources.GetObject("txtValue.WordWrap")));
			// 
			// btnJob
			// 
			this.btnJob.AccessibleDescription = resources.GetString("btnJob.AccessibleDescription");
			this.btnJob.AccessibleName = resources.GetString("btnJob.AccessibleName");
			this.btnJob.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnJob.Anchor")));
			this.btnJob.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnJob.BackgroundImage")));
			this.btnJob.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnJob.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnJob.Dock")));
			this.btnJob.Enabled = ((bool)(resources.GetObject("btnJob.Enabled")));
			this.btnJob.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnJob.FlatStyle")));
			this.btnJob.Font = ((System.Drawing.Font)(resources.GetObject("btnJob.Font")));
			this.btnJob.Image = ((System.Drawing.Image)(resources.GetObject("btnJob.Image")));
			this.btnJob.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnJob.ImageAlign")));
			this.btnJob.ImageIndex = ((int)(resources.GetObject("btnJob.ImageIndex")));
			this.btnJob.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnJob.ImeMode")));
			this.btnJob.Location = ((System.Drawing.Point)(resources.GetObject("btnJob.Location")));
			this.btnJob.Name = "btnJob";
			this.btnJob.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnJob.RightToLeft")));
			this.btnJob.Size = ((System.Drawing.Size)(resources.GetObject("btnJob.Size")));
			this.btnJob.TabIndex = ((int)(resources.GetObject("btnJob.TabIndex")));
			this.btnJob.Text = resources.GetString("btnJob.Text");
			this.btnJob.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnJob.TextAlign")));
			this.btnJob.Visible = ((bool)(resources.GetObject("btnJob.Visible")));
			// 
			// SetNext
			// 
			this.AcceptButton = this.btnJob;
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.btnJob);
			this.Controls.Add(this.txtValue);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lblHeader);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "SetNext";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.ResumeLayout(false);

		}
		#endregion
	}
}
