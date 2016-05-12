using ProcessTools;
using PSView2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace MemHack
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class MemHackMain : Form
	{
		private TextBox procLocation;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;
		private Button procSelect;
		private Label label1;
		private ListBox nameList;
		private Label label2;
		SortedList<uint, ProcessInformation> m_plist = new SortedList<uint, ProcessInformation>();
		uint m_lastid = 0xFFFFFFFF;

		private System.Threading.Timer m_timer = null;
		private Label lblVisible;
		private Label lblDate;
		private ListView procList;
		private TimerCallback m_cb = new TimerCallback(UpdateList);
		private ColumnHeader clmID;
		private ColumnHeader clmName;
		private ColumnHeader clmTitle;
		private ColumnHeader clmFake;

		private delegate void UpdateListDelegate();

		UpdateListDelegate updList = null;
		public MemHackMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Set the ListViewItemSorter property to a new ListViewItemComparer object.
			procList.ListViewItemSorter = new ListViewItemComparer(1, SortOrder.Ascending);

			UpdateBackgrounds();

			procSelect.Enabled = false;

			updList = new UpdateListDelegate(UpdateList);

			UpdateList();
			UpdateList();	// The second call is for debugging any problems in this function.
			m_timer = new System.Threading.Timer(m_cb, this, 1000, 1000);
			this.procLocation.Text = Resources.Unknown;
			this.procSelect.Text = Resources.Select;
			this.label1.Text = Resources.RunningProgramsLabel;
			this.label2.Text = Resources.ProcessNameLabel;
			this.clmID.Text = Resources.ID;
			this.clmName.Text = Resources.Name;
			this.clmTitle.Text = Resources.Title;
			this.Text = Resources.MemoryHacker;
			(new ToolTip()).SetToolTip(this, Resources.ToolTip_MemHackDlg);
			(new ToolTip()).SetToolTip(this.procList, Resources.ToolTip_ProcessList);
			(new ToolTip()).SetToolTip(this.procSelect, Resources.ToolTip_SelectProcess);
			(new ToolTip()).SetToolTip(this.procLocation, Resources.ToolTip_ApplicationPath);
			(new ToolTip()).SetToolTip(this.nameList, Resources.ToolTip_FriendlyNames);

			this.procList.DoubleClick += new EventHandler(procList_DoubleClick);
		}

		// I've overloaded Show and Hide to disable and reenable the timer when the UI goes away because
		// process updates don't need to occur while the UI isn't visible.  Less work on CPU=good
		new void Show()
		{
			base.Show();
			UpdateList();
			m_timer.Change(1000, 1000);
		}
		new void Hide()
		{
			base.Hide();
			m_timer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		// This is the static method called by the timer which gets the memhack main object
		// and calls invoke on it to update the list (since UI updates have to be on the UI thread)
		static public void UpdateList(object obj)
		{
			MemHackMain m = (MemHackMain) obj;
			m.Invoke(m.updList);
		}

		// UpdateList updates the list of processes that are currently running
		object m_lockobj = new object();
		bool m_fRunning = false;
		public void UpdateList()
		{
			// If I'm already updating just skip doing it again since it is already working.
			lock(m_lockobj)
			{
				if(m_fRunning)
					return;
				m_fRunning = true;
			}
			procList.BeginUpdate();
			ProcessViewer pviewer = new ProcessViewer();
			m_plist = pviewer.GetProcessList(Resources.IdleProcessName, Resources.SystemName);

			var sl = new SortedList<uint, ListViewItem>();
			// Remove old items
			for(int i = 0; i < procList.Items.Count;)
			{
				uint id = IDFromPos(i);
                ProcessInformation info;
				if(!m_plist.TryGetValue(id, out info))
				{
					procList.Items.RemoveAt(i);
					continue;
				}
				sl[id] = procList.Items[i];
				++i;
			}
			// Add new items
			ListViewItem lvi;
			foreach(ProcessInformation s in m_plist.Values)
			{
				if(sl.TryGetValue(s.ID, out lvi))
				{
					// this sort of construct is to help reduce the number of updates that occur to reduce flicker
					if(lvi.SubItems[3].Text != s.DefaultFriendlyName())  
						lvi.SubItems[3].Text = s.DefaultFriendlyName();
					continue;
				}

				lvi = new ListViewItem();
				lvi.Text = "";
				lvi.SubItems.Add(s.ID.ToString().PadLeft(5, ' '));
				lvi.SubItems.Add(s.Name);
				lvi.SubItems.Add(s.DefaultFriendlyName());
				procList.Items.Add(lvi);
			}

			UpdateBackgrounds();

			procList.Sort();

			// Update dependent fields if their is no longer an item selected
			if(procList.SelectedIndices.Count == 0)
			{
				procLocation.Text = "";
				lblVisible.Text = "";
				lblDate.Text = "";
				nameList.Items.Clear();
				procSelect.Enabled = false;
			}

			procList.EndUpdate();
			lock(m_lockobj)
			{
				m_fRunning = false;
			}
		}

		protected uint IDFromPos(int pos)
		{
			return IDFromLVI(procList.Items[pos]);
		}
		protected uint IDFromLVI(ListViewItem lvi)
		{
			return Convert.ToUInt32(lvi.SubItems[1].Text.Trim());
		}

		protected string rpt(char c, int count)
		{
			string s = "";
			for(int i = 0; i < count; ++i)
				s += c;
			return s;
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			m_timer.Change(Timeout.Infinite, Timeout.Infinite);
			if( disposing )
			{
				if(m_timer != null)
					m_timer.Dispose();
				m_timer = null;
				if (components != null) 
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
			ResourceManager resources = new ResourceManager(typeof(MemHackMain));
			this.procLocation = new TextBox();
			this.procSelect = new Button();
			this.label1 = new Label();
			this.nameList = new ListBox();
			this.label2 = new Label();
			this.lblVisible = new Label();
			this.lblDate = new Label();
			this.procList = new ListView();
			this.clmFake = new ColumnHeader();
			this.clmID = new ColumnHeader();
			this.clmName = new ColumnHeader();
			this.clmTitle = new ColumnHeader();
			this.SuspendLayout();
			// 
			// procLocation
			// 
			this.procLocation.AccessibleDescription = resources.GetString("procLocation.AccessibleDescription");
			this.procLocation.AccessibleName = resources.GetString("procLocation.AccessibleName");
			this.procLocation.Anchor = ((AnchorStyles)(resources.GetObject("procLocation.Anchor")));
			this.procLocation.AutoSize = ((bool)(resources.GetObject("procLocation.AutoSize")));
			this.procLocation.BackgroundImage = ((Image)(resources.GetObject("procLocation.BackgroundImage")));
			this.procLocation.Dock = ((DockStyle)(resources.GetObject("procLocation.Dock")));
			this.procLocation.Enabled = ((bool)(resources.GetObject("procLocation.Enabled")));
			this.procLocation.Font = ((Font)(resources.GetObject("procLocation.Font")));
			this.procLocation.ImeMode = ((ImeMode)(resources.GetObject("procLocation.ImeMode")));
			this.procLocation.Location = ((Point)(resources.GetObject("procLocation.Location")));
			this.procLocation.MaxLength = ((int)(resources.GetObject("procLocation.MaxLength")));
			this.procLocation.Multiline = ((bool)(resources.GetObject("procLocation.Multiline")));
			this.procLocation.Name = "procLocation";
			this.procLocation.PasswordChar = ((char)(resources.GetObject("procLocation.PasswordChar")));
			this.procLocation.ReadOnly = true;
			this.procLocation.RightToLeft = ((RightToLeft)(resources.GetObject("procLocation.RightToLeft")));
			this.procLocation.ScrollBars = ((ScrollBars)(resources.GetObject("procLocation.ScrollBars")));
			this.procLocation.Size = ((Size)(resources.GetObject("procLocation.Size")));
			this.procLocation.TabIndex = ((int)(resources.GetObject("procLocation.TabIndex")));
			this.procLocation.TabStop = false;
			this.procLocation.Text = resources.GetString("procLocation.Text");
			this.procLocation.TextAlign = ((HorizontalAlignment)(resources.GetObject("procLocation.TextAlign")));
			this.procLocation.Visible = ((bool)(resources.GetObject("procLocation.Visible")));
			this.procLocation.WordWrap = ((bool)(resources.GetObject("procLocation.WordWrap")));
			// 
			// procSelect
			// 
			this.procSelect.AccessibleDescription = resources.GetString("procSelect.AccessibleDescription");
			this.procSelect.AccessibleName = resources.GetString("procSelect.AccessibleName");
			this.procSelect.Anchor = ((AnchorStyles)(resources.GetObject("procSelect.Anchor")));
			this.procSelect.BackgroundImage = ((Image)(resources.GetObject("procSelect.BackgroundImage")));
			this.procSelect.Dock = ((DockStyle)(resources.GetObject("procSelect.Dock")));
			this.procSelect.Enabled = ((bool)(resources.GetObject("procSelect.Enabled")));
			this.procSelect.FlatStyle = ((FlatStyle)(resources.GetObject("procSelect.FlatStyle")));
			this.procSelect.Font = ((Font)(resources.GetObject("procSelect.Font")));
			this.procSelect.Image = ((Image)(resources.GetObject("procSelect.Image")));
			this.procSelect.ImageAlign = ((ContentAlignment)(resources.GetObject("procSelect.ImageAlign")));
			this.procSelect.ImageIndex = ((int)(resources.GetObject("procSelect.ImageIndex")));
			this.procSelect.ImeMode = ((ImeMode)(resources.GetObject("procSelect.ImeMode")));
			this.procSelect.Location = ((Point)(resources.GetObject("procSelect.Location")));
			this.procSelect.Name = "procSelect";
			this.procSelect.RightToLeft = ((RightToLeft)(resources.GetObject("procSelect.RightToLeft")));
			this.procSelect.Size = ((Size)(resources.GetObject("procSelect.Size")));
			this.procSelect.TabIndex = ((int)(resources.GetObject("procSelect.TabIndex")));
			this.procSelect.Text = resources.GetString("procSelect.Text");
			this.procSelect.TextAlign = ((ContentAlignment)(resources.GetObject("procSelect.TextAlign")));
			this.procSelect.Visible = ((bool)(resources.GetObject("procSelect.Visible")));
			this.procSelect.Click += new EventHandler(this.procSelect_Click);
			// 
			// label1
			// 
			this.label1.AccessibleDescription = resources.GetString("label1.AccessibleDescription");
			this.label1.AccessibleName = resources.GetString("label1.AccessibleName");
			this.label1.Anchor = ((AnchorStyles)(resources.GetObject("label1.Anchor")));
			this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
			this.label1.Dock = ((DockStyle)(resources.GetObject("label1.Dock")));
			this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
			this.label1.Font = ((Font)(resources.GetObject("label1.Font")));
			this.label1.Image = ((Image)(resources.GetObject("label1.Image")));
			this.label1.ImageAlign = ((ContentAlignment)(resources.GetObject("label1.ImageAlign")));
			this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
			this.label1.ImeMode = ((ImeMode)(resources.GetObject("label1.ImeMode")));
			this.label1.Location = ((Point)(resources.GetObject("label1.Location")));
			this.label1.Name = "label1";
			this.label1.RightToLeft = ((RightToLeft)(resources.GetObject("label1.RightToLeft")));
			this.label1.Size = ((Size)(resources.GetObject("label1.Size")));
			this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = ((ContentAlignment)(resources.GetObject("label1.TextAlign")));
			this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
			// 
			// nameList
			// 
			this.nameList.AccessibleDescription = resources.GetString("nameList.AccessibleDescription");
			this.nameList.AccessibleName = resources.GetString("nameList.AccessibleName");
			this.nameList.Anchor = ((AnchorStyles)(resources.GetObject("nameList.Anchor")));
			this.nameList.BackgroundImage = ((Image)(resources.GetObject("nameList.BackgroundImage")));
			this.nameList.ColumnWidth = ((int)(resources.GetObject("nameList.ColumnWidth")));
			this.nameList.Dock = ((DockStyle)(resources.GetObject("nameList.Dock")));
			this.nameList.Enabled = ((bool)(resources.GetObject("nameList.Enabled")));
			this.nameList.Font = ((Font)(resources.GetObject("nameList.Font")));
			this.nameList.HorizontalExtent = ((int)(resources.GetObject("nameList.HorizontalExtent")));
			this.nameList.HorizontalScrollbar = ((bool)(resources.GetObject("nameList.HorizontalScrollbar")));
			this.nameList.ImeMode = ((ImeMode)(resources.GetObject("nameList.ImeMode")));
			this.nameList.IntegralHeight = ((bool)(resources.GetObject("nameList.IntegralHeight")));
			this.nameList.ItemHeight = ((int)(resources.GetObject("nameList.ItemHeight")));
			this.nameList.Location = ((Point)(resources.GetObject("nameList.Location")));
			this.nameList.Name = "nameList";
			this.nameList.RightToLeft = ((RightToLeft)(resources.GetObject("nameList.RightToLeft")));
			this.nameList.ScrollAlwaysVisible = ((bool)(resources.GetObject("nameList.ScrollAlwaysVisible")));
			this.nameList.Size = ((Size)(resources.GetObject("nameList.Size")));
			this.nameList.TabIndex = ((int)(resources.GetObject("nameList.TabIndex")));
			this.nameList.Visible = ((bool)(resources.GetObject("nameList.Visible")));
			this.nameList.SelectedIndexChanged += new EventHandler(this.nameList_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AccessibleDescription = resources.GetString("label2.AccessibleDescription");
			this.label2.AccessibleName = resources.GetString("label2.AccessibleName");
			this.label2.Anchor = ((AnchorStyles)(resources.GetObject("label2.Anchor")));
			this.label2.AutoSize = ((bool)(resources.GetObject("label2.AutoSize")));
			this.label2.Dock = ((DockStyle)(resources.GetObject("label2.Dock")));
			this.label2.Enabled = ((bool)(resources.GetObject("label2.Enabled")));
			this.label2.Font = ((Font)(resources.GetObject("label2.Font")));
			this.label2.Image = ((Image)(resources.GetObject("label2.Image")));
			this.label2.ImageAlign = ((ContentAlignment)(resources.GetObject("label2.ImageAlign")));
			this.label2.ImageIndex = ((int)(resources.GetObject("label2.ImageIndex")));
			this.label2.ImeMode = ((ImeMode)(resources.GetObject("label2.ImeMode")));
			this.label2.Location = ((Point)(resources.GetObject("label2.Location")));
			this.label2.Name = "label2";
			this.label2.RightToLeft = ((RightToLeft)(resources.GetObject("label2.RightToLeft")));
			this.label2.Size = ((Size)(resources.GetObject("label2.Size")));
			this.label2.TabIndex = ((int)(resources.GetObject("label2.TabIndex")));
			this.label2.Text = resources.GetString("label2.Text");
			this.label2.TextAlign = ((ContentAlignment)(resources.GetObject("label2.TextAlign")));
			this.label2.Visible = ((bool)(resources.GetObject("label2.Visible")));
			// 
			// lblVisible
			// 
			this.lblVisible.AccessibleDescription = resources.GetString("lblVisible.AccessibleDescription");
			this.lblVisible.AccessibleName = resources.GetString("lblVisible.AccessibleName");
			this.lblVisible.Anchor = ((AnchorStyles)(resources.GetObject("lblVisible.Anchor")));
			this.lblVisible.AutoSize = ((bool)(resources.GetObject("lblVisible.AutoSize")));
			this.lblVisible.Dock = ((DockStyle)(resources.GetObject("lblVisible.Dock")));
			this.lblVisible.Enabled = ((bool)(resources.GetObject("lblVisible.Enabled")));
			this.lblVisible.Font = ((Font)(resources.GetObject("lblVisible.Font")));
			this.lblVisible.Image = ((Image)(resources.GetObject("lblVisible.Image")));
			this.lblVisible.ImageAlign = ((ContentAlignment)(resources.GetObject("lblVisible.ImageAlign")));
			this.lblVisible.ImageIndex = ((int)(resources.GetObject("lblVisible.ImageIndex")));
			this.lblVisible.ImeMode = ((ImeMode)(resources.GetObject("lblVisible.ImeMode")));
			this.lblVisible.Location = ((Point)(resources.GetObject("lblVisible.Location")));
			this.lblVisible.Name = "lblVisible";
			this.lblVisible.RightToLeft = ((RightToLeft)(resources.GetObject("lblVisible.RightToLeft")));
			this.lblVisible.Size = ((Size)(resources.GetObject("lblVisible.Size")));
			this.lblVisible.TabIndex = ((int)(resources.GetObject("lblVisible.TabIndex")));
			this.lblVisible.Text = resources.GetString("lblVisible.Text");
			this.lblVisible.TextAlign = ((ContentAlignment)(resources.GetObject("lblVisible.TextAlign")));
			this.lblVisible.Visible = ((bool)(resources.GetObject("lblVisible.Visible")));
			// 
			// lblDate
			// 
			this.lblDate.AccessibleDescription = resources.GetString("lblDate.AccessibleDescription");
			this.lblDate.AccessibleName = resources.GetString("lblDate.AccessibleName");
			this.lblDate.Anchor = ((AnchorStyles)(resources.GetObject("lblDate.Anchor")));
			this.lblDate.AutoSize = ((bool)(resources.GetObject("lblDate.AutoSize")));
			this.lblDate.Dock = ((DockStyle)(resources.GetObject("lblDate.Dock")));
			this.lblDate.Enabled = ((bool)(resources.GetObject("lblDate.Enabled")));
			this.lblDate.Font = ((Font)(resources.GetObject("lblDate.Font")));
			this.lblDate.Image = ((Image)(resources.GetObject("lblDate.Image")));
			this.lblDate.ImageAlign = ((ContentAlignment)(resources.GetObject("lblDate.ImageAlign")));
			this.lblDate.ImageIndex = ((int)(resources.GetObject("lblDate.ImageIndex")));
			this.lblDate.ImeMode = ((ImeMode)(resources.GetObject("lblDate.ImeMode")));
			this.lblDate.Location = ((Point)(resources.GetObject("lblDate.Location")));
			this.lblDate.Name = "lblDate";
			this.lblDate.RightToLeft = ((RightToLeft)(resources.GetObject("lblDate.RightToLeft")));
			this.lblDate.Size = ((Size)(resources.GetObject("lblDate.Size")));
			this.lblDate.TabIndex = ((int)(resources.GetObject("lblDate.TabIndex")));
			this.lblDate.Text = resources.GetString("lblDate.Text");
			this.lblDate.TextAlign = ((ContentAlignment)(resources.GetObject("lblDate.TextAlign")));
			this.lblDate.Visible = ((bool)(resources.GetObject("lblDate.Visible")));
			// 
			// procList
			// 
			this.procList.AccessibleDescription = resources.GetString("procList.AccessibleDescription");
			this.procList.AccessibleName = resources.GetString("procList.AccessibleName");
			this.procList.Alignment = ((ListViewAlignment)(resources.GetObject("procList.Alignment")));
			this.procList.Anchor = ((AnchorStyles)(resources.GetObject("procList.Anchor")));
			this.procList.BackgroundImage = ((Image)(resources.GetObject("procList.BackgroundImage")));
			this.procList.Columns.AddRange(new ColumnHeader[] {
																					   this.clmFake,
																					   this.clmID,
																					   this.clmName,
																					   this.clmTitle});
			this.procList.Dock = ((DockStyle)(resources.GetObject("procList.Dock")));
			this.procList.Enabled = ((bool)(resources.GetObject("procList.Enabled")));
			this.procList.Font = ((Font)(resources.GetObject("procList.Font")));
			this.procList.FullRowSelect = true;
			this.procList.HideSelection = false;
			this.procList.ImeMode = ((ImeMode)(resources.GetObject("procList.ImeMode")));
			this.procList.LabelWrap = ((bool)(resources.GetObject("procList.LabelWrap")));
			this.procList.Location = ((Point)(resources.GetObject("procList.Location")));
			this.procList.MultiSelect = false;
			this.procList.Name = "procList";
			this.procList.RightToLeft = ((RightToLeft)(resources.GetObject("procList.RightToLeft")));
			this.procList.Size = ((Size)(resources.GetObject("procList.Size")));
			this.procList.TabIndex = ((int)(resources.GetObject("procList.TabIndex")));
			this.procList.Text = resources.GetString("procList.Text");
			this.procList.View = View.Details;
			this.procList.Visible = ((bool)(resources.GetObject("procList.Visible")));
			this.procList.ColumnClick += new ColumnClickEventHandler(this.procList_ColumnClick);
			this.procList.SelectedIndexChanged += new EventHandler(this.procList_SelectedIndexChanged);
			// 
			// clmFake
			// 
			this.clmFake.Text = resources.GetString("clmFake.Text");
			this.clmFake.TextAlign = ((HorizontalAlignment)(resources.GetObject("clmFake.TextAlign")));
			this.clmFake.Width = ((int)(resources.GetObject("clmFake.Width")));
			// 
			// clmID
			// 
			this.clmID.Text = resources.GetString("clmID.Text");
			this.clmID.TextAlign = ((HorizontalAlignment)(resources.GetObject("clmID.TextAlign")));
			this.clmID.Width = ((int)(resources.GetObject("clmID.Width")));
			// 
			// clmName
			// 
			this.clmName.Text = resources.GetString("clmName.Text");
			this.clmName.TextAlign = ((HorizontalAlignment)(resources.GetObject("clmName.TextAlign")));
			this.clmName.Width = ((int)(resources.GetObject("clmName.Width")));
			// 
			// clmTitle
			// 
			this.clmTitle.Text = resources.GetString("clmTitle.Text");
			this.clmTitle.TextAlign = ((HorizontalAlignment)(resources.GetObject("clmTitle.TextAlign")));
			this.clmTitle.Width = ((int)(resources.GetObject("clmTitle.Width")));
			// 
			// MemHackMain
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleMode = AutoScaleMode.None;
			this.AutoScaleBaseSize = ((Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.procList);
			this.Controls.Add(this.lblDate);
			this.Controls.Add(this.lblVisible);
			this.Controls.Add(this.nameList);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.procSelect);
			this.Controls.Add(this.procLocation);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((Font)(resources.GetObject("$this.Font")));
			this.HelpButton = false;
			this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "MemHackMain";
			this.RightToLeft = ((RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.SizeGripStyle = SizeGripStyle.Show;
			this.StartPosition = ((FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			string [] args = Environment.GetCommandLineArgs();
			foreach(string s in args)
			{
				if(s.ToLower() == "/verifystrings")
					Resources.ValidateResources = true;
			}
			Resources.Test();
			Application.Run(new MemHackMain());
		}

		private void procList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(procList.SelectedIndices.Count == 0)
			{
				procSelect.Enabled = false;
				return;
			}

			// If there is a selected item, we want to populate the nameList field intelligently
			// which means removing previously non-existent entries and adding new ones
			int pos = procList.SelectedIndices[0];
			uint id = IDFromPos(pos);
			if(id != m_lastid)
			{
				// If there was a change in the id of the running app, then clear it and add
				nameList.Items.Clear();
				lblVisible.Text = "";
				lblDate.Text = "";
			}
			m_lastid = id;

			ProcessInformation proc = (ProcessInformation) m_plist[id];
			var sortedNames = new SortedList<string, ProcessFriendlyName>();
			// I'm setting up some sorted names to make this quite a bit more clean
			for(int i = 0; i < proc.Count; ++i)
			{
				ProcessFriendlyName f = proc.FriendlyName(i);
				sortedNames[f.Name] = f;
			}
			procLocation.Text = ((ProcessInformation) m_plist[id]).FullPath;
			procSelect.Enabled = proc.Modifiable;	// Whether the process is modifiable should determine whether the button is enabled

			if(pos < 0 || pos >= m_plist.Count)
				return;

			// Remove old items
			for(int i = 0; i < nameList.Items.Count;)
			{
				string s = (string)nameList.Items[i];
				if(null == sortedNames[s])
				{
					nameList.Items.RemoveAt(i);
					continue;
				}
				++i;
			}
			// Sort the current list if it has changed (and there are entries to sort)
			var sl = new SortedList<ProcessFriendlyName, int>();
			foreach(string s in nameList.Items)
				sl[sortedNames[s]] = 1;

			// For all remaining items in pNames add them to nameList.Items inserting as appropriate.
			for(int i = 0; i < proc.Count; ++i)
			{
				ProcessFriendlyName f = proc.FriendlyName(i);
                int value;
				if(!sl.TryGetValue(f, out value))
					continue;
				sl[f] = 2;
				nameList.Items.Insert(sl.IndexOfKey(f), f.Name);
			}
		}

		private void procList_DoubleClick(object sender, EventArgs e)
		{
			procSelect_Click(null, null);
		}

		private void procSelect_Click(object sender, EventArgs e)
		{
			// We hide the old window and bring up the memory display window instead assuming the user
			// didn't select something absurd.
			this.Hide();
			MemDisplay display = new MemDisplay();
			// Absurd 1:  Really shouldn't be possible, but just in case
			if(procList.SelectedIndices[0] < 0 || procList.SelectedIndices[0] >= procList.Items.Count)
			{
				MessageBox.Show(Resources.ErrorProcessSelect);
				goto Cleanup;
			}

			// Absurd 2:  Really shouldn't be modifying myself
			uint id = IDFromPos(procList.SelectedIndices[0]);
			if(id == Process.GetCurrentProcess().Id)
			{
				MessageBox.Show(Resources.ErrorProcessThisOne);
				goto Cleanup;
			}

			ProcessInformation s = ((ProcessInformation) m_plist[id]);

			// Don't attempt to modify a process that can't be modified
			if(!s.Modifiable)
			{
				MessageBox.Show(Resources.ErrorProcessUnmodifiable);
				goto Cleanup;
			}

			if(!display.Configure(s))
			{
				display.Dispose();
				MessageBox.Show(Resources.ErrorProcessOpenFailed);
				goto Cleanup;
			}
			display.ShowDialog();
			Cleanup:
			UpdateList();
			m_timer.Change(Timeout.Infinite, Timeout.Infinite);
			this.Show();
		}

		private void nameList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(procList.SelectedIndices.Count > 0)
			{
				// This function essentially indicates some of the characteristics of the windows or services
				// owned by the process, such as whether the window is visible and the time of creation
				// This was more of a debug feature to ensure I was sorting the service and window lists correctly
				uint id = IDFromPos(procList.SelectedIndices[0]);
				if(m_plist[id] != null)
				{
					ProcessFriendlyName f = ((ProcessInformation) m_plist[id]).FriendlyName(nameList.SelectedIndices[0]);
					lblVisible.Text = (f.Visible) ? "Visible" : "";
					if(f.Date != DateTime.FromFileTimeUtc(0))
						lblDate.Text = f.Date.ToString();
					else
						lblDate.Text = "";
				}
				else
				{
					lblDate.Text = "";
					lblVisible.Text = "";
				}
			}
			else
			{
				lblDate.Text = "";
				lblVisible.Text = "";
			}
		}

		private void procList_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// Set the ListViewItemSorter property to a new ListViewItemComparer object.
			ListViewItemComparer lvic = (ListViewItemComparer) procList.ListViewItemSorter;
			if(e.Column == lvic.Column)
			{
				lvic.Order = ((lvic.Order == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending);
			}
			else
			{
				lvic.Order = SortOrder.Ascending;
				lvic.Column = e.Column;
				UpdateBackgrounds();
			}
			// Call the sort method to manually sort the column based on the ListViewItemComparer implementation.
			procList.Sort();
		}

		// This handy little function actually goes through the entire list and changes the 
		// column back colour to indicate which column is being sorted on.  Kinda a nice feature really
		private void UpdateBackgrounds()
		{
			ListViewItemComparer lvic = (ListViewItemComparer) procList.ListViewItemSorter;
			Color c = Color.FromArgb(0xF0, 0xF0, 0xF0);
			foreach(ListViewItem lvi in procList.Items)
			{
				lvi.UseItemStyleForSubItems = false;
				for(int i = 0; i < lvi.SubItems.Count; ++i)
				{
					// Check to see if the colour needs to change--reduce number of writes that require updates
					if(i == lvic.Column && lvi.SubItems[i].BackColor != c)
						lvi.SubItems[i].BackColor = c;
					else if(i != lvic.Column && lvi.SubItems[i].BackColor != Color.White)
						lvi.SubItems[i].BackColor = Color.White;
				}
			}
		}

        // Implements the manual sorting of items by columns.
        #region class ListViewItemComparer : System.Collections.IComparer
        class ListViewItemComparer : System.Collections.IComparer
		{
			private int col;
			private SortOrder so;

			// I can set the column remotely instead of constantly creating a new class
			public int Column
			{
				get
				{
					return col;
				}
				set
				{
					col = value;
				}
			}
			// Ditto for sort order
			public SortOrder Order
			{
				get
				{
					return so;
				}
				set
				{
					so = value;
				}
			}
			public ListViewItemComparer() 
			{
				col = 0;
				so = SortOrder.Ascending;
			}
			public ListViewItemComparer(int column, SortOrder order) 
			{
				col = column;
				so = order; 
			}
			public int Compare(object x, object y) 
			{
				if(so == SortOrder.Ascending)
                    return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
				else
					return -String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
			}
		}
		#endregion
	}
}
