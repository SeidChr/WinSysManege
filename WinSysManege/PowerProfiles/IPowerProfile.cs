namespace WinSysManege.PowerProfiles
{
    using System;

    public interface IPowerProfile
    {
        string Name { get; }

        bool Active { get; }

        Guid Guid { get; }
    }
}