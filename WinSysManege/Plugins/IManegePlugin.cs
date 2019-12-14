namespace WinSysManege.Plugins
{
    using System.Windows;

    public interface IManegePlugin
    {
        IManegePluginViewModel ViewModel { get; }

        ResourceDictionary Resources { get; }
    }
}
