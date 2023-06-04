using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Devices;

namespace PassXYZLib.xunit.PassXYZ
{
    public class MockDeviceInfo : IDeviceInfo
    {
        public string Model => "Lib";
        public string Manufacturer => "PassXYZ";
        public string Name => "PassXYZLib";
        public Version Version => new(1, 0, 0, 0);
        public string VersionString => Version.ToString();
        public DevicePlatform Platform => new();
        public DeviceIdiom Idiom => new();
        public DeviceType DeviceType => new();
    }
}
