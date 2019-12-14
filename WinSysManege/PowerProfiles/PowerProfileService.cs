namespace WinSysManege.PowerProfiles
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class PowerProfileService : IPowerProfileService
    {
        private const string PowerConfigExecutable = "powercfg";

        private static readonly Regex LineRegex = new Regex(
            @":\s(?<Guid>[0-9a-f]{8}\-[0-9a-f]{4}\-[0-9a-f]{4}\-[0-9a-f]{4}\-[0-9a-f]{12})\s+\((?<Name>\w+)\)\s?(?<Active>\*?)?",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public IEnumerable<IPowerProfile> ListPowerProfiles()
            => ParsePowerConfig(GetPowerConfig());

        public bool SetActivePowerProfile(Guid activePowerProfileGuid)
            => string.IsNullOrWhiteSpace(ExecuteShellCommand(PowerConfigExecutable, $"/s {activePowerProfileGuid:D}"));

        private static ProcessStartInfo GetHiddenProcessStartInfo()
            => new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardInput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

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

        private static IEnumerable<PowerProfile> ParsePowerConfig(string powerConfigOutput)
            => powerConfigOutput
                .Split(Environment.NewLine)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => LineRegex.Match(l))
                .Where(m => m.Success)
                .Select(m => new PowerProfile
                {
                    Active = m.Groups["Active"].Value.Equals("*"),
                    Name = m.Groups["Name"].Value,
                    Guid = Guid.Parse(m.Groups["Guid"].Value),
                });

        private static string GetPowerConfig()
            => ExecuteShellCommand(PowerConfigExecutable, "/list");
    }
}
