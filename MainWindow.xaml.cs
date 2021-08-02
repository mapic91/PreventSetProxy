using Microsoft.Win32;
using RegistryUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PreventSetProxy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RegistryMonitor monitor;
        public MainWindow()
        {
            InitializeComponent();

            var registry = OpenUserRegKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);
            if (registry != null && (int)registry.GetValue("ProxyEnable") != 0)
            {
                registry.SetValue("ProxyEnable", 0);
            }

            monitor = new
            RegistryMonitor(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Internet Settings");
            monitor.RegChanged += new EventHandler(OnRegChanged);
            monitor.Start();
        }

        public static RegistryKey OpenUserRegKey(string name, bool writable)
        {
            // we are building x86 binary for both x86 and x64, which will
            // cause problem when opening registry key
            // detect operating system instead of CPU
            RegistryKey userKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentUser, "",
                Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32)
                .OpenSubKey(name, writable);
            return userKey;
        }

        private void OnRegChanged(object sender, EventArgs e)
        {
            var registry = OpenUserRegKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);
            if(registry != null && (int)registry.GetValue("ProxyEnable") != 0)
            {
                registry.SetValue("ProxyEnable", 0);
            }
        }
    }
}
