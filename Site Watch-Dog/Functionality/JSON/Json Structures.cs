using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site_Watch_Dog.Functionality.JSON
{
    public class SwitchInformation
    {
        public int id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string mac { get; set; }
        public string type { get; set; }
        public string ip { get; set; }

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
    public class SwitchList
    {
        public List<SwitchInformation> Switches { get; set; }
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
}
