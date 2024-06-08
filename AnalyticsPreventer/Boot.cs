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
            "amplitude.com",
            "api.amplitude.com",
            "api2.amplitude.com",
            "cdn.amplitude.com",
            "api.lab.amplitude.com",
            "api3.amplitude.com",
            "api.eu.amplitude.com",
            "analytics.amplitude.com",
            "analytics.eu.amplitude.com",
            "info.amplitude.com",
            "static.amplitude.com",
            "regionconfig.amplitude.com",
            "regionconfig.eu.amplitude.com",
            "o1125869.ingest.sentry.io",
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
            "stats.unity3d.com",
            "ads-game-configuration-master.ads.prd.ie.internal.unity3d.com",
            "ads-game-configuration.unityads.unity3d.com",
            "ads-privacy-api.prd.mz.internal.unity3d.com",
            "adserver.unityads.unity3d.com",
            "analytics.cloud.unity3d.com",
            "analytics.social.unity.com",
            "analytics.uca.cloud.unity3d.com",
            "analytics.unity3d.com",
            "api.uca.cloud.unity3d.com",
            "auction.unityads.unity3d.com",
            "config.unityads.unity3d.com",
            "events.iap.unity3d.com",
            "events.mz.unity3d.com",
            "geocdn.unityads.unity3d.com",
            "httpkafka.unityads.unity3d.com",
            "mediation-tracking.prd.mz.internal.unity3d.com",
            "publisher-event.ads.prd.ie.internal.unity3d.com",
            "tracking.prd.mz.internal.unity3d.com",
            "unityads.unity3d.com",
            "userreporting.cloud.unity3d.com",
            "webview.unityads.unity3d.com",
            // DBD
            "gamelogs.live.bhvrdbd.com",
            "rtm.live.dbd.bhvronline.com",
            "engage14352dbdmb.deltadna.net",
            "collect14352dbdmb.deltadna.net",
            // Genshin Impact
            "log-upload-os.mihoyo.com",
            // Redshell 
            "api.gameanalytics.com",
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
            "adservice.google.com",
            // Steam
            "crash.steampowered.com",
            // Solarwinds
            "logs-01.loggly.com",
            // Vermintide
            "5fs-crashify.s3-accelerate.amazonaws.com",
            // China Analytics
            "crashlogs.woniu.com",
            "crashlytics.com",
            "down.anticheatexpert.com",
            "log-upload-os.hoyoverse.com",
            "webstatic.hoyoverse.com",
            "minor-api-os.hoyoverse.com",
            "abtest-api-data-sg.hoyoverse.com",
            "sg-public-data-api.hoyoverse.com",
            // Microsoft
            "vortex.data.microsoft.com",
            "v10.events.data.microsoft.com",
            "settings-win.data.microsoft.com",
            "v10.vortex-win.data.microsoft.com",
            "v20.events.data.microsoft.com",
            "watson.telemetry.microsoft.com",
            "watson.events.data.microsoft.com",
            "web.vortex.data.microsoft.com",
            "mobile.events.data.microsoft.com",
            "cortana.ai",
            "api.cortana.ai",
            // Labymod 
            "issue.labymod.net",
            // Rec Room
            "gamelogs.rec.net",
            "datacollection.rec.net",
            "bugreporting.rec.net",
            "commerce.rec.net",
            "notify.bugsnag.com",
            // Facebook
            "oculuscdn.com",
            "fbcdn.com",
            "fbsbx.com",
            "facebook-hardware.com",
            // Xbox
            "cdn.optimizely.com",
            "analytics.xboxlive.com",
            "cdf-anon.xboxlive.com",
            "settings-ssl.xboxlive.com",
            // Sea of Thieves
            "athenaprod.maelstrom.gameservices.xboxlive.com",
            //"e5ed.playfabapi.com", breaks voice chat, only use if voice isnt needed
            //"playfabapi.com", breaks voice chat, only use if voice isnt needed
            // Epic Games
            "et.epicgames.com",
            "et2.epicgames.com",
            "udn.epicgames.com",
            "etsource.epicgames.com",
            "metrics.ol.epicgames.com",
            "datarouter.ol.epicgames.com",
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
