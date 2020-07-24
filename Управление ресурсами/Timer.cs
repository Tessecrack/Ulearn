using System;
using System.Diagnostics;
using System.Text;

namespace Memory.Timers
{
    public class Timer : IDisposable
    {
        private static int valueOfTime = 0;

        private readonly Stopwatch tikTok = new Stopwatch();

        private string nameTime;

        private bool disposedValue;

        public static string Report => stringReportBuilder.ToString();
        private static StringBuilder stringReportBuilder = new StringBuilder();
        private static int beforeThis = -1;
        public static Timer Start(string patronymicTime = "") => new Timer(patronymicTime);

        public Timer(string timeName = "")
        {
            nameTime = timeName == "" ? "*" : timeName;
            if (beforeThis == -1) stringReportBuilder = new StringBuilder();
            tikTok.Start();
            beforeThis++;
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                tikTok.Stop();
                valueOfTime += GetAroundValue((int)tikTok.ElapsedMilliseconds);
                var beforeNameTime = new string(' ', beforeThis * 4);
                var afterNameTime = new string(' ', 20 - nameTime.Length - beforeThis * 4);

                var comdom = GetAroundValue((int)tikTok.ElapsedMilliseconds);

                var resultName = beforeNameTime
                    + nameTime
                    + afterNameTime + ": "
                    + comdom + "\n";

                if (IsRestTime(resultName)) return;
                stringReportBuilder.Append(resultName);
                beforeThis--;
                disposedValue = true;
            }
        }
        private int GetAroundValue(int value)
        {
            int num = value % 10;
            while(num != 0)
            {
                var ten = (value / 10) % 10;
                if (ten == 0) value--;
                else value++;
                num = value % 10;
            }
            return value;
        }
        private bool IsRestTime(string res)
        {
            if (stringReportBuilder.Length != 0 && stringReportBuilder[beforeThis * 4] == ' ')
            {
                var restTime = GetAroundValue((int)(valueOfTime - tikTok.ElapsedMilliseconds));
                stringReportBuilder.Insert(0, res);
                stringReportBuilder.Append(
                    new string(' ', beforeThis * 4 + 4)
                    + "Rest"
                    + new string(' ', 20 - 8 - beforeThis * 4)
                    + ": "
                    + restTime.ToString() + "\n");
                beforeThis--;
                return true;
            }
            return false;
        }
        ~Timer() { }
    }
}
