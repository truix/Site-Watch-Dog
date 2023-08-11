using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Site_Watch_Dog.Utility.Task_Handler
{

    internal class Task_Handler
    {
        public void RegisterTasks(Functionality.JSON.SWDConfig config)
        {
            if (poe_task_count >= config.poe_scan_after_x || config.poe_scan_every_cycle)
            {
                poe_task_count = 0;
                Tasks.Add(Functionality.Tasks.Tasks.POEScan);
            }
            if (ping_task_count >= config.ping_scan_after_x || config.ping_scan_every_cycle)
            {
                ping_task_count = 0;
                Tasks.Add(Functionality.Tasks.Tasks.PingScan);
            }
            if (temp_task_count >= config.temp_scan_after_x || config.temp_scan_every_cycle)
            {
                temp_task_count = 0;
                Tasks.Add(Functionality.Tasks.Tasks.TempScan);
            }
        }
        public List<Func<string,bool>> GetTasks()
        {
            return Tasks;
        }
        public void IterateTaskCounts()
        {
            poe_task_count  += 1;
            ping_task_count += 1;
            temp_task_count += 1;
        }
        public void DestroyTasks()
        {
            Tasks.Clear();
        }
        public List<Func<string,bool>> Tasks = new List<Func<string,bool>>();
        public int poe_task_count = 0;
        public int ping_task_count = 0;
        public int temp_task_count = 0;

    }
}
