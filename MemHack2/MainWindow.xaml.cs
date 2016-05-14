using Microsoft.Win32;
using ProcessTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace MemHack2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SortState mProcessSortState = new SortState();
        private Timer mUpdateProcessListTimer = null;

        public ObservableCollection<ProcessInformation> ActiveProcesses { get; set; }
        
        public MainWindow()
        {
            ActiveProcesses = new ObservableCollection<ProcessInformation>();
            DataContext = this;

            InitializeComponent();

            var theme = GetThemeFromRegistry();
            foreach(var item in cmbThemes.Items)
            {
                var cmbItem = item as ComboBoxItem;
                if (cmbItem != null && (cmbItem.Tag as string) == theme)
                {
                    cmbThemes.SelectedItem = cmbItem;
                    break;
                }
            }

            mUpdateProcessListTimer = new Timer(UpdateProcessList, null, 0, 60 * 1000);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mUpdateProcessListTimer.Change(Timeout.Infinite, Timeout.Infinite);

            while (IsUpdateRunning)
            {
                Thread.Sleep(15);
            }
        }

        private bool mIsUpdateRunning = false;
        private object mUpdateLock = new object();
        private bool IsUpdateRunning { get { lock (mUpdateLock) { return mIsUpdateRunning; } } }

        private void UpdateProcessList(object context)
        {
            lock(mUpdateLock)
            {
                if (mIsUpdateRunning)
                {
                    return;
                }
                mIsUpdateRunning = true;
            }
            Dispatcher.Invoke(() => Refresh.IsEnabled = false);

            try
            {
                var processes = Processes.GetProcessList(Properties.Resources.IdleProcessName, Properties.Resources.SystemName);
                Dispatcher.Invoke(() => UpdateProcessListUI(processes));
            }
            finally
            {
                Dispatcher.Invoke(() => Refresh.IsEnabled = true);
                lock (mUpdateLock)
                {
                    mIsUpdateRunning = false;
                }
            }
        }

        private void UpdateProcessListUI(SortedList<int, ProcessInformation> processes)
        {
            var selectedProcess = RunningProcesses.SelectedItem as ProcessInformation;
            bool findNewProcessToSelect = false;

            for(int i = ActiveProcesses.Count - 1; i >= 0; --i)
            {
                ProcessInformation info = ActiveProcesses[i];
                if (!processes.ContainsKey(info.ID))
                {
                    ActiveProcesses.RemoveAt(i);
                    if (info == selectedProcess)
                    {
                        findNewProcessToSelect = true;
                    }
                }
                else
                {
                    // Replace the instance to get any window changes.
                    ActiveProcesses[i] = processes[info.ID];
                    processes.Remove(info.ID);
                    if (selectedProcess == info)
                    {
                        RunningProcesses.SelectedItem = ActiveProcesses[i];
                    }
                }
            }

            if (processes.Count > 0)
            {
                int currentProcessIndex = 0;
                ProcessInformation infoToInsert = processes.Values[currentProcessIndex];

                for (int i = 0; i < ActiveProcesses.Count; ++i)
                {
                    ProcessInformation current = ActiveProcesses[i];
                    if (infoToInsert.ID < current.ID)
                    {
                        ActiveProcesses.Insert(i, infoToInsert);
                        ++currentProcessIndex;
                        if (currentProcessIndex >= processes.Count)
                        {
                            break;
                        }
                        infoToInsert = processes.Values[currentProcessIndex];
                    }
                }

                for (; currentProcessIndex < processes.Count; ++currentProcessIndex)
                {
                    infoToInsert = processes.Values[currentProcessIndex];
                    ActiveProcesses.Add(infoToInsert);
                }
            }

            if (findNewProcessToSelect && ActiveProcesses.Count > 0)
            {
                ProcessInformation toSelect = ActiveProcesses[ActiveProcesses.Count - 1];
                foreach (var proc in ActiveProcesses)
                {
                    if (proc.ID > selectedProcess.ID)
                    {
                        toSelect = proc;
                        break;
                    }
                }
                RunningProcesses.SelectedItem = toSelect;
            }
        }

        private void cmbThemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = cmbThemes.SelectedItem as ComboBoxItem;
            var theme = selectedItem != null
                ? selectedItem.Tag as string
                : null;
            var lastTheme = GetThemeFromRegistry();
            if (theme != lastTheme)
            {
                SetThemeInRegistry(theme);
            }
        }

        private RegistryKey GetRegistryKey()
        {
            return Registry.CurrentUser.CreateSubKey("Software\\OneOddSock\\MemHack2");
        }

        private string GetThemeFromRegistry()
        {
            using (var key = GetRegistryKey())
            {
                return key.GetValue("Theme", null) as string;
            }
        }

        private void SetThemeInRegistry(string themeTag)
        {
            using (var key = GetRegistryKey())
            {
                if (string.IsNullOrEmpty(themeTag))
                {
                    try
                    {
                        key.DeleteValue("Theme");
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    key.SetValue("Theme", themeTag);
                }
            }
        }

        private void RunningProcessesColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            mProcessSortState.OnColumnHeader_Click(RunningProcesses, sender, e);
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            HackSelectedProcess();
        }

        private void HackSelectedProcess()
        {
            var procInfo = RunningProcesses.SelectedItem as ProcessInformation;
            if (procInfo == null)
            {
                return;
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(UpdateProcessList);
        }

        private void RunningProcesses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var procInfo = RunningProcesses.SelectedItem as ProcessInformation;
            Select.IsEnabled = procInfo != null ? procInfo.Modifiable : false;
        }

        private void RunningProcesses_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HackSelectedProcess();
        }
    }

}
