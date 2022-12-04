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
        public bool IsGameplay { get => _isGameplay == 0x01; }
        internal byte _isGameplay;
        public bool IsPaused { get => _isPaused == 0x01; }
        internal byte _isPaused;
        public int Money { get => _money; set => _money = value; }
        internal int _money;
        public int Kudos { get => _kudos; set => _kudos = value; }
        internal int _kudos;
        public int Liberty { get => _liberty; set => _liberty = value; }
        internal int _liberty;
        public int Utility { get => _utility; set => _utility = value; }
        internal int _utility;
        public int Morality { get => _morality; set => _morality = value; }
        internal int _morality;
    }
}
