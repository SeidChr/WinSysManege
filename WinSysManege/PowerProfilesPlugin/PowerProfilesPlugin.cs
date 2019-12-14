namespace WinSysManege.PowerProfilesPlugin
{
    using System;
    using System.Windows;
    using WinSysManege.Plugins;

    public class PowerProfilesPlugin : IManegePlugin
    {
        public IManegePluginViewModel ViewModel { get; }

        public ResourceDictionary Resources { get; } = new ResourceDictionary();

        public PowerProfilesPlugin()
        {
            Resources.Source =
                new Uri("/Extension.MyPlugin;component/View.xaml",
                    UriKind.RelativeOrAbsolute);
        }
    }
}
