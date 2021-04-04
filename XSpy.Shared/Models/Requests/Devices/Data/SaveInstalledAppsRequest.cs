using System.Collections.Generic;

namespace XSpy.Shared.Models.Requests.Devices
{
    public class SaveInstalledAppsRequest
    {
        public List<InstalledApp> Apps { get; set; }
    }

    public class InstalledApp
    {
        public string AppName { get; set; }
        public string PackageName { get; set; }
        public string VersionName { get; set; }
        public int VersionCode { get; set; }

        public long InstallDate { get; set; }
    }
}