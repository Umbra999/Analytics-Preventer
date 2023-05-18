using System.Diagnostics;
using System.Net;
using System.Security.Principal;

namespace AnalyticsPreventer
{
    internal class Boot
    {
        private static readonly string[] Blocklist = new string[]
        {
            // VRChat
            "*.amplitude.com",
            "api.amplitude.com",
            "api2.amplitude.com",
            "cdn.amplitude.com",
            "api.lab.amplitude.com",
            "api3.amplitude.com",
            "api.eu.amplitude.com",
            // Unity
            "api.uca.cloud.unity3d.com",
            "config.uca.cloud.unity3d.com",
            "cdp.cloud.unity3d.com",
            "data-optout-service.uca.cloud.unity3d.com",
            "perf-events.cloud.unity3d.com",
            "public.cloud.unity3d.com",
            "ecommerce.iap.unity3d.com",
            "remote-config-proxy-prd.uca.cloud.unity3d.com",
            "thind-gke-euw.prd.data.corp.unity3d.com",
            "thind-gke-usc.prd.data.corp.unity3d.com",
            "thind-gke-ape.prd.data.corp.unity3d.com",
            // DBD
            "gamelogs.live.bhvrdbd.com",
            "rtm.live.dbd.bhvronline.com",
            // Genshin Impact
            "log-upload-os.mihoyo.com",
            // Redshell 
            "api.gameanalytics.com",
            "files.facepunch.com",
            "in.treasuredata.com",
            "api.redshell.io",
            "rubick.gameanalytics.com",
            // Just Cause
            "nelo2-col.nhncorp.jp",
            // GoG Galaxy
            "galaxy-client-reports.gog.com",
            "insights-collector.gog.com",
            "gwent-bi-collector.gog.com",
            // Google
            "stats.g.doubleclick.net",
            // Steam
            "crash.steampowered.com",
            // Solarwinds
            "logs-01.loggly.com",
            // Vermintide
            "5fs-crashify.s3-accelerate.amazonaws.com",
            // China Analytics
            "crashlogs.woniu.com",
            // Microsoft
            "vortex.data.microsoft.com",
            // Labymod 
            "issue.labymod.net",
            // Rec Room
            "gamelogs.rec.net",
            "datacollection.rec.net",
            "bugreporting.rec.net",
            "commerce.rec.net",
            "notify.bugsnag.com",
        };

        public static void Main()
        {
            Console.Title = "Analytics Prevention | By Umbra";

            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {

                ProcessStartInfo startInfo = new()
                {
                    FileName = Environment.GetCommandLineArgs()[0],
                    UseShellExecute = true,
                    Verb = "runas",
                    Arguments = "/runas"
                };

                Process.Start(startInfo);
                return;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Press Enter to Block analytics permanent");
            Console.ReadLine();

            BlockAnalytics();
        }

        private static void BlockAnalytics()
        {
            string HostsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts");

            List<string> AllHostLines = File.ReadAllLines(HostsFile).ToList();

            foreach (string url in Blocklist)
            {
                bool IsExisting = false;
                foreach (string line in AllHostLines)
                {
                    if (line.Contains(url))
                    {
                        IsExisting = true;
                        break;
                    }
                }
                if (!IsExisting) AllHostLines.Add($"0.0.0.0 {url}");
            }

            File.WriteAllLines(HostsFile, AllHostLines);

            foreach (string url in Blocklist)
            {
                try
                {
                    Uri uri = new("http://" + url + "/");
                    var ip = Dns.GetHostAddresses(uri.Host)[0];
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"failed to block {url}");
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{url} is succesfully blocked");
                }
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("");
            Console.WriteLine($"Press Enter to Exit", ConsoleColor.Blue);

            Console.ReadLine();
        }
    }
}
