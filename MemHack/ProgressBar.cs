using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using PSView;

namespace MemHack
{

	/// <summary>
	/// Summary description for ProgressBar.
	/// </summary>
	public class ProgressBar : System.Windows.Forms.Form, IProgressIndicator
	{
		private System.Windows.Forms.ProgressBar pBar;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// Delegates for updating the UI
		protected delegate void SetValueDelegate(int val);
		protected delegate void SetMaximumDelegate(int val);


		SetValueDelegate setVal = null;
		SetMaximumDelegate setMax = null;
		public ProgressBar()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			setVal = new SetValueDelegate(SetVal);
			setMax = new SetMaximumDelegate(SetMax);
			this.Text = Resources.ProgressTitle;
		}

		// The below set of functions is for setting the value and maximum in a "safe" way.
		// The UI requires that such updates be done on the thread that owns the UI so 
		// I ensure that if necessary the update is marshalled to the thread for running
		// through the implemented methods from IProgressIndicator
		protected void SetVal(int val)
		{
			pBar.Value = val;
		}

		void SetMax(int max)
		{
			pBar.Maximum = max;
		}

		public void SetCurrent(int val)
		{
			if(InvokeRequired)	// Check to see if I really need to invoke (in case I ever use this UI elsewhere)
			{
				object [] args = new object[1];
				args[0] = val;
				Invoke(setVal, args);
			}
			else
			{
				SetVal(val);
			}
		}

		public void SetMaximum(int max)
		{
			if(InvokeRequired)	// Check to see if I really need to invoke (in case I ever use this UI elsewhere)
			{
				object [] args = new object[1];
				args[0] = max;
				Invoke(setMax, args);
			}
			else
			{
				SetMax(max);
			}
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ProgressBar));
			this.pBar = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// pBar
			// 
			this.pBar.AccessibleDescription = resources.GetString("pBar.AccessibleDescription");
			this.pBar.AccessibleName = resources.GetString("pBar.AccessibleName");
			this.pBar.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("pBar.Anchor")));
			this.pBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pBar.BackgroundImage")));
			this.pBar.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("pBar.Dock")));
			this.pBar.Enabled = ((bool)(resources.GetObject("pBar.Enabled")));
			this.pBar.Font = ((System.Drawing.Font)(resources.GetObject("pBar.Font")));
			this.pBar.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("pBar.ImeMode")));
			this.pBar.Location = ((System.Drawing.Point)(resources.GetObject("pBar.Location")));
			this.pBar.Name = "pBar";
			this.pBar.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("pBar.RightToLeft")));
			this.pBar.Size = ((System.Drawing.Size)(resources.GetObject("pBar.Size")));
			this.pBar.TabIndex = ((int)(resources.GetObject("pBar.TabIndex")));
			this.pBar.Text = resources.GetString("pBar.Text");
			this.pBar.Visible = ((bool)(resources.GetObject("pBar.Visible")));
			// 
			// ProgressBar
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.ControlBox = false;
			this.Controls.Add(this.pBar);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimizeBox = false;
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "ProgressBar";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.ResumeLayout(false);

		}
		#endregion
	}
}
