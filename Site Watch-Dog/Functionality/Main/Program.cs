using Site_Watch_Dog;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;
Site_Watch_Dog.Functionality.Main.Globals globals = new Site_Watch_Dog.Functionality.Main.Globals();
Site_Watch_Dog.Functionality.JSON.SWDConfig config = new Site_Watch_Dog.Functionality.JSON.SWDConfig();
if (System.IO.File.Exists("config.cfg"))
{
    using (StreamReader r = new StreamReader("config.cfg"))
    {
        string json = r.ReadToEnd();
        config = JsonSerializer.Deserialize<Site_Watch_Dog.Functionality.JSON.SWDConfig>(json);
    }
   
}
else
{
    config.site_name = "10";
    config.scan_cycle_time = 600;
    config.poe_scan_after_x = 5;
    config.ping_scan_after_x = 1;
    config.temp_scan_after_x = 2;
    config.poe_scan_every_cycle = false;
    config.ping_scan_every_cycle = true;
    config.temp_scan_every_cycle = false;
   
    var data = JsonSerializer.Serialize<Site_Watch_Dog.Functionality.JSON.SWDConfig>(config, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText("config.cfg", data);
}
WebClient client = new WebClient();

string switch_json = client.DownloadString("http://testwebapp.xyz/index.php?p=getallknownswitches&subnet=" + config.site_name);
if (switch_json != null)
    globals.Switches = JsonSerializer.Deserialize<Site_Watch_Dog.Functionality.JSON.SwitchList>(switch_json);


while (true)
{

    globals.Task_Handler.RegisterTasks(config);
    var task_list = globals.Task_Handler.GetTasks();

    foreach (var equiptment in globals.Switches.Switches)
    {
        foreach (var task_handle in task_list)
        {
            try
            {
                task_handle.Invoke(equiptment.ip);
            }
            catch (Exception ex) { }
        }
    }

    globals.Task_Handler.IterateTaskCounts();
    globals.Task_Handler.DestroyTasks();






    Thread.Sleep(config.scan_cycle_time * 10000);//change to 1000
}
