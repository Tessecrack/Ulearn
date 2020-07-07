using System;
using System.Collections.Generic;
using System.Linq;
namespace Incapsulation.Failures
{
    public enum FailureType
    {
        UnexpectedShutdown,
        ShortNonResponding,
        HardwateFailure,
        ConnectionProblem
    }

    public class Device
    {
        public int DeviceId;
        public string DeviceName;

        public Device(int id, string name)
        {
            DeviceId = id;
            DeviceName = name;
        }
    }

    public class ReportMaker
    {
        public static List<string> FindDevicesFailedBeforeDateObsolete(
            int day,
            int month,
            int year,
            int[] failureTypes, 
            int[] deviceId, 
            object[][] times,
            List<Dictionary<string, object>> devices)
        {
            var dateTimeSpec = new DateTime(year, month, day);
            var dateTimesFail = times.Select(time => new DateTime((int)time[2], (int)time[1], (int)time[0])).ToArray();
            var failEnum = failureTypes.Select(fail => (FailureType) fail).ToArray();
            var listOfDevices = devices
                .Select(device => new Device((int)device["DeviceId"], device["Name"] as string))
                .ToList();
            var problematicDevices = new HashSet<int>();
            for (int i = 0; i < failureTypes.Length; i++)
            {
                if (IsFailureSerious(failEnum[i]) && IsEarlier(dateTimeSpec, dateTimesFail[i]))
                {
                    problematicDevices.Add(deviceId[i]);
                }
            }
            return FindDevicesFailedBeforeDate(problematicDevices, listOfDevices, dateTimeSpec);
        }

        public static List<string> FindDevicesFailedBeforeDate(HashSet<int> problematicDevices,
            List<Device> devices, DateTime dateTime)
        {
            var result = new List<string>();
            foreach (var device in devices)
            {
                if (problematicDevices.Contains(device.DeviceId))
                {
                    result.Add(device.DeviceName);
                }
            }
            return result;
        }

        public static bool IsFailureSerious(FailureType failureType) => (int)failureType % 2 == 0;

        public static bool IsEarlier(DateTime specifiedTime, DateTime failTime) => failTime < specifiedTime;
    }
}
