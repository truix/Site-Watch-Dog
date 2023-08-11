using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site_Watch_Dog.Functionality.Events
{
    public enum Event
    {
        EVENT_UNKNOWN = 0,//critical
        EVENT_START,//info
        EVENT_SCAN_PING_START,//info
        EVENT_SCAN_PING_PROBLEM,//error
        EVENT_SCAN_PING_END,//info
        EVENT_SCAN_POE_START,//info
        EVENT_SCAN_POE_PROBLEM,//error
        EVENT_SCAN_POE_END,//info
        EVENT_SCAN_TEMP_START,//info
        EVENT_SCAN_TEMP_PROBLEM,//error
        EVENT_SCAN_TEMP_END,//info
    }
    
}
