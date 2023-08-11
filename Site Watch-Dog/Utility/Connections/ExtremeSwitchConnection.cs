using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.Net;
using System.Threading;
using System.Net.NetworkInformation;

namespace Site_Watch_Dog.Utility.Connections
{
    public enum ExtremeSwitchType
    {
        None = 0,
        X440_48p,
        X460G2_48p_10G4,
        X440_24p,
        X430_8p,
        X460_24x,
        X690_48x_2q_4c,
        X440_8p,
        X670G2_48x_4q,
        X460_24t,
        X5420M_16MW_32P_4YE,
        X5420M_48W_4YE,
        X5420M_16MW_32P_4YE_SwitchEngine,
    }
    public class TemperatureReading
    {
        public int slot { get; set; }
        public float temp { get; set; }
        public int warn_temp { get; set; }
        public int max_temp { get; set; }
    }
    public enum ExtremeStackNumbers
    {
        Single = 0,
        Slot_1,
        Slot_2,
        Slot_3,
        Slot_4,
        WHOLE_STACK

    }
    public class ExtremeSwitchConnection
    {
        public ExtremeSwitchConnection()
        {
        }

        public ExtremeSwitchConnection(string ip)
        {
            SwitchIP = ip;
        }

        ~ExtremeSwitchConnection()
        {
            Disconnect();
        }

        public bool Connect()
        {
            if (CheckReachable())
            {
                SSH = new SshClient(SwitchIP, Username, Password);

                if (SSH == null)
                    return false;
                try
                {
                    SSH.Connect();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public void Disconnect()
        {
            if (SSH != null)
                SSH.Disconnect();
        }

        public bool IsConnected()
        {
            if (SSH == null)
                return false;

            return SSH.IsConnected;
        }

        public string Run(string command)
        {
            if (SSH != null && SSH.IsConnected)
            {
                var sshCommand = SSH.CreateCommand(command);

                sshCommand.Execute();

                return sshCommand.Result;
            }

            return "";
        }

        public bool CheckReachable()
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(SwitchIP, 100);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        public long Ping()
        {
            long latency = -1;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(SwitchIP);

                if (reply.Status == IPStatus.Success)
                    latency = reply.RoundtripTime;

            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return latency;
        }
        public string GetSwitchTypeString()
        {
            return SwitchType;
        }
        public ExtremeSwitchType GetSwitchType()
        {
            if (!IsConnected())
                return ExtremeSwitchType.None;

            var ret = "";

            if (SwitchType == "")
            {
                ret = Run("sh sw | i \"System Type:\"");

                ret = ret.Split(":")[1];
                ret = ret.Replace("(Stack)", "");
                ret = ret.Trim();

                SwitchType = ret;
            }
            else
            {
                ret = SwitchType;
            }

            switch (ret)
            {
                case "X440-48p":
                    return ExtremeSwitchType.X440_48p;
                case "X460G2-48p-10G4":
                    return ExtremeSwitchType.X460G2_48p_10G4;
                case "X440-24p":
                    return ExtremeSwitchType.X440_24p;
                case "X430-8p":
                    return ExtremeSwitchType.X430_8p;
                case "X460-24x":
                    return ExtremeSwitchType.X460_24x;
                case "X690-48x-2q-4c":
                    return ExtremeSwitchType.X690_48x_2q_4c;
                case "X440-8p":
                    return ExtremeSwitchType.X440_8p;
                case "X670G2-48x-4q":
                    return ExtremeSwitchType.X670G2_48x_4q;
                case "X460-24t":
                    return ExtremeSwitchType.X460_24t;
                case "5420M-16MW-32P-4YE":
                    return ExtremeSwitchType.X5420M_16MW_32P_4YE;
                case "5420M-48W-4YE":
                    return ExtremeSwitchType.X5420M_48W_4YE;
                case "5420M-16MW-32P-4YE-SwitchEngine":
                    return ExtremeSwitchType.X5420M_16MW_32P_4YE_SwitchEngine;
                default:
                    return ExtremeSwitchType.None;
            }


        }

        public int GetPortCount()
        {
            switch (this.GetSwitchType())
            {
                case ExtremeSwitchType.None:
                    return 0;
                case ExtremeSwitchType.X440_48p:
                    return 48;
                case ExtremeSwitchType.X460G2_48p_10G4:
                    return 48;
                case ExtremeSwitchType.X440_24p:
                    return 24;
                case ExtremeSwitchType.X430_8p:
                    return 8;
                case ExtremeSwitchType.X460_24x:
                    return 24;
                case ExtremeSwitchType.X690_48x_2q_4c:
                    return 48;
                case ExtremeSwitchType.X440_8p:
                    return 8;
                case ExtremeSwitchType.X670G2_48x_4q:
                    return 48;
                case ExtremeSwitchType.X460_24t:
                    return 24;
                case ExtremeSwitchType.X5420M_16MW_32P_4YE:
                    return 48;
                case ExtremeSwitchType.X5420M_48W_4YE:
                    return 48;
                case ExtremeSwitchType.X5420M_16MW_32P_4YE_SwitchEngine:
                    return 48;
                default:
                    return 0;
            }
        }
        public int GetStackPortCount()
        {
            return this.GetPortCount() * this.GetStackSize();
        }
        public bool HasPOE()
        {
            switch (this.GetSwitchType())
            {
                case ExtremeSwitchType.None:
                    return false;
                case ExtremeSwitchType.X440_48p:
                    return true;
                case ExtremeSwitchType.X460G2_48p_10G4:
                    return true;
                case ExtremeSwitchType.X440_24p:
                    return true;
                case ExtremeSwitchType.X430_8p:
                    return true;
                case ExtremeSwitchType.X460_24x:
                    return false;
                case ExtremeSwitchType.X690_48x_2q_4c:
                    return false;
                case ExtremeSwitchType.X440_8p:
                    return true;
                case ExtremeSwitchType.X670G2_48x_4q:
                    return false;
                case ExtremeSwitchType.X460_24t:
                    return false;
                case ExtremeSwitchType.X5420M_16MW_32P_4YE:
                    return true;
                case ExtremeSwitchType.X5420M_48W_4YE:
                    return true;
                case ExtremeSwitchType.X5420M_16MW_32P_4YE_SwitchEngine:
                    return true;
                default:
                    return false;
            }
        }
        public int GetStackSize()
        {
            var res = this.Run("sh stacking | i CA");
            if (res == "" || res.Contains("%% Invalid") || res.Contains("%% Unrecognized command:"))
                return 1;

            var parse = res.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            int count = 0;

            foreach (var item in parse)
            {
                count += 1;
            }

            return count;

        }
        public List<TemperatureReading> GetTemperature()
        {
            var temp_readings = new List<TemperatureReading>();

            var res = this.Run("sh temperature");        

            if (res == "" || res.Contains("%% Invalid") || res.Contains("%% Unrecognized command:"))
                return temp_readings;

            var parse = res.Split('\n', StringSplitOptions.RemoveEmptyEntries).Skip(2).ToArray();
            foreach (var item in parse)
            {
                var line = item.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (line.Length > 4)
                {
                    if (line[0].Contains("SS-")) continue;
                    TemperatureReading temp = new TemperatureReading();
                    if (line[0].Contains("Switch")) temp.slot = 1;                 
                    else temp.slot = Int32.Parse(line[0].Split('-')[1]);
                    temp.warn_temp = Int32.Parse(line[6].Split('-')[1]);
                    temp.temp = float.Parse(line[3]);
                    temp.max_temp = Int32.Parse(line[7]);
                    temp_readings.Add(temp);
                }
            }

            return temp_readings;

        }
        public List<ExtremeStackNumbers> GetPOEStatus()
        {
            List<ExtremeStackNumbers> res = new List<ExtremeStackNumbers>();
            var size = this.GetStackSize();
            var ret = "";//this.Run("sh inline-power | i Operational");
            switch (size)
            {
                case 1:
                    ret = this.Run("sh inline-power | i Operational");
                    break;
                case 2:
                    ret += this.Run("sh inline-power slot 1 | i Operational");
                    ret += this.Run("sh inline-power slot 2 | i Operational");
                 
                    break;
                case 3:
                    ret += this.Run("sh inline-power slot 1 | i Operational");
                    ret += this.Run("sh inline-power slot 2 | i Operational");
                    ret += this.Run("sh inline-power slot 3 | i Operational");
                 
                    break;
                case 4:
                    ret += this.Run("sh inline-power slot 1 | i Operational");
                    ret += this.Run("sh inline-power slot 2 | i Operational");
                    ret += this.Run("sh inline-power slot 3 | i Operational");
                    ret += this.Run("sh inline-power slot 4 | i Operational");
                    break;

            }
           
            if (ret != "")
            {
                var parse = ret.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                if (parse[0][0] == 'O')
                {
                    res.Add(ExtremeStackNumbers.Single);
                    return res;
                }
                foreach (var item in parse)
                {                 
                    switch (item[0])
                    {
                        case '1':
                            res.Add(ExtremeStackNumbers.Slot_1);
                            break;
                        case '2':
                            res.Add(ExtremeStackNumbers.Slot_2);
                            break;
                        case '3':
                            res.Add(ExtremeStackNumbers.Slot_3);
                            break;
                        case '4':
                            res.Add(ExtremeStackNumbers.Slot_4);
                            break;                     

                    }
                }
                return res;
            } else if (this.Run("sh inline-power | i Disabled") != "")
            {
                res.Add(ExtremeStackNumbers.Single);
            }
           

            return res;
        }
        public string SwitchIP { get; set; }
        public string Username = "";
        public string Password = "";
        private string SwitchType = "";
        private SshClient SSH = null;
    }


}
