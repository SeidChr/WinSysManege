namespace WinSysManege.PowerProfilesPlugin
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;
    using WinSysManege.PowerProfiles;

    /// <summary>
    /// Interaction logic for PowerProfileList.xaml
    /// </summary>
    public partial class PowerProfileList : UserControl
    {
        private readonly IPowerProfileService profileService;

        public PowerProfileList()
        {
            this.InitializeComponent();

            this.profileService = new PowerProfileService();

            var powerProfilesCollection = this.profileService.ListPowerProfiles();
            var powerProfileList = powerProfilesCollection as IList<IPowerProfile>
                                   ?? powerProfilesCollection.ToList();

            this.PowerProfileListBox.ItemsSource = powerProfileList;
            this.PowerProfileListBox.SelectedItem = powerProfileList.FirstOrDefault(i => i.Active);
            this.PowerProfileListBox.SelectionChanged += this.PowerProfileListBox_SelectionChanged;
        }

        private void PowerProfileListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.PowerProfileListBox.SelectedItem is PowerProfile profile)
            {
                this.profileService.SetActivePowerProfile(profile.Guid);
            }
        }
    }
}
