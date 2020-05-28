using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory.Timers
{
    public class Timer : IDisposable
    {
        private static int responseTimer = 0;
        public static string Report { get => reportBuilder.ToString(); set { } }
        private Stopwatch tick = new Stopwatch();

        private string nameTime;
        private bool disposedValue;

        private static StringBuilder reportBuilder = new StringBuilder();
        private static int parents = -1;


        public Timer(string name = "")
        {
            _ = name == "" ? nameTime = "*" : nameTime = name;
            if (parents == -1) reportBuilder = new StringBuilder();
            tick.Start();
            parents++;
        }
        public static Timer Start(string name = "") => new Timer(name);
        public void Dispose()
        {
            if (!disposedValue)
            {
                tick.Stop();
                responseTimer += (int)tick.ElapsedMilliseconds;
                var spacesBeforeName = new string(' ', parents * 4);
                var spacesAfterName = new string(' ', 20 - nameTime.Length - parents * 4);

                var result = spacesBeforeName + nameTime + spacesAfterName + ": "
                    + tick.ElapsedMilliseconds.ToString() + "\n";

                if (CheckRest(result)) return;
                reportBuilder.Append(result);
                parents--;
                disposedValue = true;
            }
        }
        private bool CheckRest(string result)
        {
            if (reportBuilder.Length != 0 && reportBuilder[parents * 4] == ' ')
            {
                var res = responseTimer - tick.ElapsedMilliseconds;
                reportBuilder.Insert(0, result);
                reportBuilder.Append(new string(' ', parents * 4 + 4) + "Rest"
                    + new string(' ', 20 - 8 - parents * 4) + ": " +
                        res.ToString() + "\n");
                parents--;
                return true;
            }
            return false;
        }
        ~Timer()
        {

        }
    }
}
