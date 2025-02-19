﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldReduxServer
{
    [Serializable]
    public class ConfigFile
    {
        public string LocalAddress { get; set; } = "0.0.0.0";

        public int ServerPort { get; set; } = 25555;

        public int MaxPlayers { get; set; } = 10;

        public string PlayerVersion { get; set; } = "";

        public bool EnforceMods { get; set; } = false;

        public bool UseModBlacklist { get; set; } = false;

        public bool UseCustomDifficulty { get; set; } = false;

        public bool UseWhitelist { get; set; } = false;

        public bool AllowCustomScenarios { get; set; } = false;

        [NonSerialized] public List<string> enforcedMods = new List<string>();
        [NonSerialized] public List<string> whitelistedMods = new List<string>();
        [NonSerialized] public List<string> blacklistedMods = new List<string>();
    }
}
