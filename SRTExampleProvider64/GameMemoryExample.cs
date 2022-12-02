using System;
using System.Globalization;
using System.Collections.Generic;
using SRTExampleProvider64.Structs;
using System.Diagnostics;
using System.Reflection;
using SRTExampleProvider64.Structs.GameStructs;

namespace SRTExampleProvider64
{
    public class GameMemoryExample : IGameMemoryExample
    {
        public string GameName => "Triangle Strategy";
        public string VersionInfo => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        public int Money { get => _money; set => _money = value; }
        internal int _money;
    }
}
