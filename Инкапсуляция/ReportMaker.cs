using System;
using System.Collections.Generic;

namespace Incapsulation.Failures
{
    public enum FailureType
    {
        UnexpectedShutdown,
        ShortNonResponding,
        HardwareFailures,
        ConnectionProblems
    }

    public struct Device
    {
        public string DeviceId;
        public string DeviceName;
        public FailureType Failure;
        public Device(string id, string name, FailureType failureType = FailureType.ShortNonResponding)
        {
            DeviceName = name;
            DeviceId = id;
            Failure = failureType;
        }
    }

    public class ReportMaker
    {
        public static bool GetEarlier(DateTime now, DateTime myTimes) => now > myTimes;
        public static bool IsFailureSerious(int failureType) => failureType % 2 == 0;
        public static DateTime[] GetMyTimes(object[][] v)
        {
            int length = v.GetLength(0);
            DateTime[] myTimes = new DateTime[length];
            for (int i = 0; i < length; i++)
                myTimes[i] = new DateTime(year: (int)v[i][2], month: (int)v[i][1], day: (int)v[i][0]);
            return myTimes;
        }

        public static HashSet<Device> GetMyDevices(List<Dictionary<string, object>> devices)
        {
            Device oneDevice = new Device("", "");
            HashSet<Device> myDevices = new HashSet<Device>();
            foreach (var device in devices)
            {
                foreach (var element in device)
                {
                    if (element.Key == "DeviceId") oneDevice.DeviceId = element.Value.ToString();
                    if (element.Key == "Name") oneDevice.DeviceName = element.Value.ToString();
                }
                myDevices.Add(oneDevice);
            }
            return myDevices;
        }

        public static HashSet<Device> GetProblematicDevice(HashSet<Device> devices,
            int[] failures,
            int[] deviceId,
            DateTime dateTime,
            DateTime[] myTimes)
        {
            HashSet<Device> problematicDevices = new HashSet<Device>();
            for (int i = 0; i < failures.Length; i++)
                if (IsFailureSerious(failures[i]) && GetEarlier(dateTime, myTimes[i]))
                    foreach (var device in devices)
                        if (deviceId[i] == int.Parse(device.DeviceId))
                            problematicDevices.Add(new Device(device.DeviceId, 
                                device.DeviceName, 
                                (FailureType)failures[i]));
            return problematicDevices;
        }

        public static List<string> FindDevicesFailedBeforeDateObsolete(
            int day,
            int month,
            int year,
            int[] failureTypes, 
            int[] deviceId, 
            object[][] times,
            List<Dictionary<string, object>> devices)
        {
            HashSet<Device> myDevices = GetMyDevices(devices);
            DateTime dateTime = new DateTime(day: day, month: month, year: year);
            DateTime[] myTimes = GetMyTimes(times);
            HashSet<Device> problematicDevices = GetProblematicDevice(myDevices, failureTypes, deviceId, dateTime, myTimes);
            return FindDevicesFailedBeforeDate(problematicDevices, dateTime);
        }

        public static List<string> FindDevicesFailedBeforeDate(HashSet<Device> problematicDevices, DateTime dateTime)
        {
            List<string> result = new List<string>();
            foreach (var device in problematicDevices)
                result.Add(device.DeviceName);
            return result;
        }
    }
}
