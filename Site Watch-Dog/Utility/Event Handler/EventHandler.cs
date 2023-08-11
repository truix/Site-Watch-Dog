using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Site_Watch_Dog.Utility.Event_Handler
{
    internal class EventHandler
    {
        public void ReportEvent(Functionality.Events.Event reported_event, string ip,Connections.ExtremeStackNumbers slot_number = Connections.ExtremeStackNumbers.WHOLE_STACK)
        {
            WebClient client = new WebClient();

            
            client.QueryString.Add("switchip", ip.ToString());
            client.QueryString.Add("switchslot", slot_number.ToString());          
            
            switch (reported_event)
            {
                case Functionality.Events.Event.EVENT_SCAN_PING_PROBLEM:
                    client.QueryString.Add("p", "swdping");
                    break;
                case Functionality.Events.Event.EVENT_SCAN_POE_PROBLEM:
                    client.QueryString.Add("p", "swdpoe");
                    break;
                case Functionality.Events.Event.EVENT_SCAN_TEMP_PROBLEM:
                    client.QueryString.Add("p", "swdtemp");
                    break;
            }

            client.DownloadString("http://testwebplatform.xyz/index.php");
        }
    }
}
