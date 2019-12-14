namespace WinSysManege.PowerProfiles
{
    using System;
    using System.Collections.Generic;

    public interface IPowerProfileService
    {
        IEnumerable<IPowerProfile> ListPowerProfiles();

        bool SetActivePowerProfile(Guid powerProfileId);
    }
}
