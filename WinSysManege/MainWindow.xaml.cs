using System.Drawing;

namespace WinSysManege
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Forms;
    using WinSysManege.ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string PowerConfigExecutable = "powercfg";

        private static readonly Regex LineRegex = new Regex(
            @":\s(?<Guid>[0-9a-f]{8}\-[0-9a-f]{4}\-[0-9a-f]{4}\-[0-9a-f]{4}\-[0-9a-f]{12})\s+\((?<Name>\w+)\)\s?(?<Active>\*?)?",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private readonly NotifyIcon trayIcon;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            var powerProfilesCollection = ParsePowerConfig(GetPowerConfig());
            var powerProfileList = powerProfilesCollection as IList<PowerProfileViewModel>
                ?? powerProfilesCollection.ToList();

            this.PowerProfileListBox.ItemsSource = powerProfileList;
            this.PowerProfileListBox.SelectedItem = powerProfileList.FirstOrDefault(i => i.Active);
            this.PowerProfileListBox.SelectionChanged += this.PowerProfileListBox_SelectionChanged;

            var contextMenu = new ContextMenuStrip();
            foreach (var profile in powerProfileList)
            {
                var menuItem = contextMenu.Items.Add(profile.Name);
                menuItem.Click += (sender, args) => SetActivePowerProfile(profile.Guid);
            }

            this.trayIcon = new NotifyIcon
            {
                Visible = true,
                Icon = new Icon("Resources/colorfan.ico"),
                ContextMenuStrip = contextMenu,
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            this.trayIcon.Dispose();
            base.OnClosed(e);
        }

        private static ProcessStartInfo GetHiddenProcessStartInfo()
            => new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardInput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

        private static void SetActivePowerProfile(Guid activePowerProfileGuid)
            => ExecuteShellCommand(PowerConfigExecutable, $"/s {activePowerProfileGuid:D}");

        private static IEnumerable<PowerProfileViewModel> ParsePowerConfig(string powerConfigOutput)
            => powerConfigOutput
                .Split(Environment.NewLine)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => LineRegex.Match(l))
                .Where(m => m.Success)
                .Select(m => new PowerProfileViewModel
                {
                    Active = m.Groups["Active"].Value.Equals("*"),
                    Name = m.Groups["Name"].Value,
                    Guid = Guid.Parse(m.Groups["Guid"].Value),
                });

        private static string GetPowerConfig()
            => ExecuteShellCommand(PowerConfigExecutable, "/list");

        private static string ExecuteShellCommand(string executable, string arguments)
        {
            var process = new Process();
            var startInfo = GetHiddenProcessStartInfo();

            startInfo.FileName = executable;
            startInfo.Arguments = arguments;

            process.StartInfo = startInfo;
            process.Start();

            var result = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return result;
        }

        private void PowerProfileListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.PowerProfileListBox.SelectedItem is PowerProfileViewModel profile)
            {
                SetActivePowerProfile(profile.Guid);
            }
        }
    }
}
