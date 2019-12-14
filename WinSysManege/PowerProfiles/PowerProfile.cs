namespace WinSysManege.PowerProfiles
{
    using System;

    public class PowerProfile : IPowerProfile
    {
        public string Name { get; set; }

        public bool Active { get; set; }

        public Guid Guid { get; set; }
    }
}
