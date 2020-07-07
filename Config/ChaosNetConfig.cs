using ChaosTerraria.Structs;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ChaosTerraria.ChaosUtils
{
    public static class ChaosNetConfig
    {
        internal static ChaosNetConfigData data;
        private readonly static string configPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games\Terraria\ModLoader\Mod Configs\chaosnet.json";

        public static bool CheckForConfig()
        {
            return File.Exists(configPath);
        }

        public static ChaosNetConfigData ReadConfig()
        {
            return JsonConvert.DeserializeObject<ChaosNetConfigData>(File.ReadAllText(configPath));
        }

        public static void Save()
        {
            File.WriteAllText(configPath, JsonConvert.SerializeObject(data));
        }
    }
}
