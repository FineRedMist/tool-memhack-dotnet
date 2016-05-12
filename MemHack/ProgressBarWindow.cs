using ProcessTools;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MemHack
{

    /// <summary>
    /// Summary description for ProgressBar.
    /// </summary>
    public class ProgressBarWindow : Form, IProgressIndicator
    {
        private const int InternalMax = 10000;

        private ProgressBar pBar;
        UInt64 mCurrentValue;
        UInt64 mMaximumValue;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        // Delegates for updating the UI
        protected delegate void SetValueDelegate(UInt64 val);
        protected delegate void SetMaximumDelegate(UInt64 val);


        SetValueDelegate setVal = null;
        SetMaximumDelegate setMax = null;
        public ProgressBarWindow()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            pBar.Maximum = InternalMax;

            SetVal(0);
            SetMax(InternalMax);

            setVal = new SetValueDelegate(SetVal);
            setMax = new SetMaximumDelegate(SetMax);
            this.Text = Resources.ProgressTitle;
        }

        private void UpdateProgress()
        {
            double newVal = mCurrentValue;
            newVal /= (mMaximumValue == 0 ? 1 : mMaximumValue);
            newVal *= InternalMax;
            pBar.Value = (int)(newVal);
        }

        // The below set of functions is for setting the value and maximum in a "safe" way.
        // The UI requires that such updates be done on the thread that owns the UI so 
        // I ensure that if necessary the update is marshalled to the thread for running
        // through the implemented methods from IProgressIndicator
        private void SetVal(UInt64 val)
		{
            mCurrentValue = val;
            UpdateProgress();
        }

        private void SetMax(UInt64 max)
		{
            mMaximumValue = max;
            UpdateProgress();
		}

		public void SetCurrent(UInt64 val)
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

		public void SetMaximum(UInt64 max)
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ProgressBarWindow));
			this.pBar = new ProgressBar();
			this.SuspendLayout();
			// 
			// pBar
			// 
			this.pBar.AccessibleDescription = resources.GetString("pBar.AccessibleDescription");
			this.pBar.AccessibleName = resources.GetString("pBar.AccessibleName");
			this.pBar.Anchor = ((AnchorStyles)(resources.GetObject("pBar.Anchor")));
			this.pBar.BackgroundImage = ((Image)(resources.GetObject("pBar.BackgroundImage")));
			this.pBar.Dock = ((DockStyle)(resources.GetObject("pBar.Dock")));
			this.pBar.Enabled = ((bool)(resources.GetObject("pBar.Enabled")));
			this.pBar.Font = ((Font)(resources.GetObject("pBar.Font")));
			this.pBar.ImeMode = ((ImeMode)(resources.GetObject("pBar.ImeMode")));
			this.pBar.Location = ((Point)(resources.GetObject("pBar.Location")));
			this.pBar.Name = "pBar";
			this.pBar.RightToLeft = ((RightToLeft)(resources.GetObject("pBar.RightToLeft")));
			this.pBar.Size = ((Size)(resources.GetObject("pBar.Size")));
			this.pBar.TabIndex = ((int)(resources.GetObject("pBar.TabIndex")));
			this.pBar.Text = resources.GetString("pBar.Text");
			this.pBar.Visible = ((bool)(resources.GetObject("pBar.Visible")));
			// 
			// ProgressBar
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((Size)(resources.GetObject("$this.ClientSize")));
			this.ControlBox = false;
			this.Controls.Add(this.pBar);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = FormBorderStyle.Fixed3D;
			this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimizeBox = false;
			this.MinimumSize = ((Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "ProgressBar";
			this.RightToLeft = ((RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.SizeGripStyle = SizeGripStyle.Hide;
			this.StartPosition = ((FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.ResumeLayout(false);

		}
		#endregion
	}
}
