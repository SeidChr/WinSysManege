namespace WinSysManege
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms;
    using WinSysManege.PowerProfiles;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly NotifyIcon trayIcon;

        private readonly IPowerProfileService profileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.profileService = new PowerProfileService();

            var powerProfilesCollection = this.profileService.ListPowerProfiles();
            var powerProfileList = powerProfilesCollection as IList<IPowerProfile>
                ?? powerProfilesCollection.ToList();

            var contextMenu = new ContextMenuStrip();
            foreach (var profile in powerProfileList)
            {
                var menuItem = contextMenu.Items.Add(profile.Name);
                menuItem.Click += (sender, args) 
                    => this.profileService.SetActivePowerProfile(profile.Guid);
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
    }
}
