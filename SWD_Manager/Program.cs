using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;
using System.Diagnostics;

SWDConfig config = new SWDConfig();
LocationList Locations = new LocationList();

  

WebClient client = new WebClient();

string json = client.DownloadString("http://testwebapp.xyz/index.php?p=smgetinv");
if (json != null)
    Locations = JsonSerializer.Deserialize<LocationList>(json);

string localdir = System.AppDomain.CurrentDomain.BaseDirectory;
string zipdir = System.AppDomain.CurrentDomain.BaseDirectory + "swd.zip";

foreach( var location in Locations.Locations )
{
   if (Directory.Exists(localdir+location.LocationName))
    {
        //start
        string command = "sh";
        string argss = "\""+localdir + location.LocationName + "/start.sh\"";
        string verb = " ";

        ProcessStartInfo procInfo = new ProcessStartInfo();
        procInfo.WindowStyle = ProcessWindowStyle.Normal;
        procInfo.UseShellExecute = false;
        procInfo.FileName = command;    
        procInfo.Arguments = argss;         
        procInfo.Verb = verb;                  
        Process.Start(procInfo);              
    }
    else
    {
        System.IO.Compression.ZipFile.ExtractToDirectory(zipdir, localdir + location.LocationName);
        config = new SWDConfig();
        config.site_name = location.LocationTemplate;
        config.scan_cycle_time = 600;
        config.poe_scan_after_x = 5;
        config.ping_scan_after_x = 1;
        config.temp_scan_after_x = 2;
        config.poe_scan_every_cycle = false;
        config.ping_scan_every_cycle = true;
        config.temp_scan_every_cycle = false;

        var data = JsonSerializer.Serialize<SWDConfig>(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(localdir + location.LocationName+"/config.cfg", data);
        File.WriteAllText(localdir + location.LocationName + "/start.sh", "#!/bin/bash\ntmux new-session -d -s '"+location.LocationName+ " SWD'  './Site\\ Watch-Dog'");
        //start
        string command = "sh";
        string argss = localdir + location.LocationName + "/start.sh";
        string verb = " ";

        ProcessStartInfo procInfo = new ProcessStartInfo();
        procInfo.WindowStyle = ProcessWindowStyle.Normal;
        procInfo.UseShellExecute = false;
        procInfo.FileName = command;   
        procInfo.Arguments = argss;        
        procInfo.Verb = verb;                 
        Process.Start(procInfo);              
    }
}

public class LocationInformation
{
    public string LocationName { get; set; }
    public string LocationTemplate { get; set; }
}
public class LocationList
{
    public List<LocationInformation> Locations { get; set; }
}
public class SWDConfig
{
    public string site_name { get; set; }
    public int scan_cycle_time { get; set; }
    public bool poe_scan_every_cycle { get; set; }
    public bool ping_scan_every_cycle { get; set; }
    public bool temp_scan_every_cycle { get; set; }
    public int poe_scan_after_x { get; set; }
    public int ping_scan_after_x { get; set; }
    public int temp_scan_after_x { get; set; }


}