using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Site_Watch_Dog.Functionality.Tasks
{
    public static class Tasks
    {
        private static Utility.Event_Handler.EventHandler Event_Handler = new Utility.Event_Handler.EventHandler();
        public static bool PingScan(string ip)
        {            
            Utility.Connections.ExtremeSwitchConnection sw = new Utility.Connections.ExtremeSwitchConnection(ip);
            var latency = sw.Ping();

            Console.WriteLine("{0}:IP:{1} Latency:{2} ", DateTime.Now.ToString("(h:mm:ss[tt])"),ip,latency);

            if (latency != -1)
                return true; 
            Event_Handler.ReportEvent(Events.Event.EVENT_SCAN_PING_PROBLEM, ip);
            return false;
        }
        public static bool POEScan(string ip)
        {
            Console.WriteLine("{0}-POE Scan:{1} ", DateTime.Now.ToString("(h:mm:ss[tt])"), ip);
            Utility.Connections.ExtremeSwitchConnection sw = new Utility.Connections.ExtremeSwitchConnection(ip);
            sw.Connect();
            var stacksize =  sw.GetStackSize();
            List<Utility.Connections.ExtremeStackNumbers> compare = new List<Utility.Connections.ExtremeStackNumbers> {
                Utility.Connections.ExtremeStackNumbers.Slot_1,
                Utility.Connections.ExtremeStackNumbers.Slot_2,
                Utility.Connections.ExtremeStackNumbers.Slot_3,
                Utility.Connections.ExtremeStackNumbers.Slot_4 };
            if (sw.HasPOE())
            {
                var list = sw.GetPOEStatus();
                if (list.Count() == stacksize)
                    return true;
                if (stacksize == 1)
                {
                    if (list.Contains(Utility.Connections.ExtremeStackNumbers.Single))
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("{0}-Bad POE:{1} Slot:{2}", DateTime.Now.ToString("(h:mm:ss[tt])"), ip, Utility.Connections.ExtremeStackNumbers.Single);
                        Event_Handler.ReportEvent(Events.Event.EVENT_SCAN_POE_PROBLEM, ip,Utility.Connections.ExtremeStackNumbers.Single);
                    }
                    
                }
                
                for (var i = 0; i < stacksize; i++)
                {
                    if (list.Contains(compare[i]))
                        continue;
                    //throw bad poe event
                    Console.WriteLine("{0}-Bad POE:{1} Slot:{2}", DateTime.Now.ToString("(h:mm:ss[tt])"), ip, compare[i]);
                    Event_Handler.ReportEvent(Events.Event.EVENT_SCAN_POE_PROBLEM, ip, compare[i]);
                }

                return false;
            }
            else return true;
        }
        public static bool TempScan(string ip)
        {
           
            Utility.Connections.ExtremeSwitchConnection sw = new Utility.Connections.ExtremeSwitchConnection(ip);
            sw.Connect();
            var temperatureReadings = sw.GetTemperature();
            sw.Disconnect();
            bool bad_temp = true;
            foreach (var item in temperatureReadings)
            {
                if (item.temp >= item.warn_temp)
                {
                    Console.WriteLine("-TEMP ALARM:{0} -> {1}: Temperature:{2} Warn Temp:{3}",ip,item.slot,item.temp,item.warn_temp);
                    Event_Handler.ReportEvent(Events.Event.EVENT_SCAN_TEMP_PROBLEM, ip, (Utility.Connections.ExtremeStackNumbers)item.slot);
                    bad_temp = false;
                }
                Console.WriteLine("-TEMP Scan:{0} -> {1}: Temperature:{2} Warn Temp:{3}", ip, item.slot, item.temp, item.warn_temp);
            }
            return bad_temp;
        }
    }
}
