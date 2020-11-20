using ChaosTerraria.Structs;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ChaosTerraria.ChaosUtils
{
    public static class ChaosNetConfig
    {
        public static ChaosNetConfigData data;
        private readonly static string configPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games\Terraria\ModLoader\Mod Configs\chaosnet.json";

        public static bool CheckForConfig()
        {
            return File.Exists(configPath);
        }

        public static void ReadConfig()
        {
            data = JsonConvert.DeserializeObject<ChaosNetConfigData>(File.ReadAllText(configPath));
        }

        public static void Save()
        {
            if(!CheckForConfig())
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games\Terraria\ModLoader\Mod Configs\");
            File.WriteAllText(configPath, JsonConvert.SerializeObject(data));
        }
    }
}
