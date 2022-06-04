using ChaosTerraria.AI;
using ChaosTerraria.ChaosUtils;
using ChaosTerraria.Classes;
using ChaosTerraria.Config;
using ChaosTerraria.Managers;
using ChaosTerraria.UI;
using ChaosTerraria.World;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosTerraria
{
    public class ChaosTerraria : Mod
    {
        internal static ModKeybind loginHotkey;
        internal static ModKeybind sessionHotkey;
        internal static ModKeybind observerModeHotkey;
        internal static ModKeybind cycleOrgs;
        internal static NNet nNet;
        internal static Weight weight;
        internal static List<Weight> weights;
        readonly static Random rand = new(DateTime.Now.Millisecond);
        internal static Weight sum;
        public override void Load()
        {
            loginHotkey = KeybindLoader.RegisterKeybind(this, "Login", "P");
            sessionHotkey = KeybindLoader.RegisterKeybind(this, "Session", "O");
            observerModeHotkey = KeybindLoader.RegisterKeybind(this, "Observe Mode", "N");
            cycleOrgs = KeybindLoader.RegisterKeybind(this, "Cycle Orgs", "]");
            nNet = JsonConvert.DeserializeObject<NNet>(Encoding.UTF8.GetString(GetFileBytes("testNNet.json")));
            ChaosTerrariaConfig config = (ChaosTerrariaConfig)GetConfig("ChaosTerrariaConfig");
            SessionManager.InitializeSession();
            weights = new List<Weight>();
            sum = new() { values = new() };
            if (!File.Exists("weight.json"))
            {
                weight = new() { values = new() };
                foreach (Neuron neuron in nNet.neurons)
                {
                    if (neuron.baseType != "input")
                    {
                        foreach (Dependency dependency in neuron.dependencies)
                        {
                            sum.values.Add(0);
                            weight.values.Add(dependency.weight);
                        }
                    }
                }
                if (config.roles != null)
                {
                    foreach (Role role in config.roles)
                        CreatePop(weight.values, role);
                    weight.roleName = "main";
                    File.WriteAllText("weight.json", JsonConvert.SerializeObject(weight));
                }
            }
            else
            {
                weight = JsonConvert.DeserializeObject<Weight>(File.ReadAllText("weight.json"));
                if (config.roles != null)
                {
                    foreach (Role role in config.roles)
                        CreatePop(weight.values, role);
                    for(int i = 0; i < weight.values.Count; i++)
                    {
                        sum.values.Add(0);
                    }
                }
            }
            SessionManager.InitScores();
            if (Main.netMode != NetmodeID.Server)
            {
                ChaosSystem.loginScreen.Activate();
                ChaosSystem.sessionScreen.Activate();
                ChaosSystem.spawnBlockScreen.Activate();
                ChaosSystem.progressBar.Activate();

                if (!ChaosNetConfig.CheckForConfig())
                {
                    ChaosSystem.mainInterface.SetState(ChaosSystem.loginScreen);
                    UIHandler.IsLoginUiVisible = true;
                }
                else
                {
                    ChaosNetConfig.ReadConfig();
                    SessionManager.SetCurrentSessionNamespace();
                }
            }
        }

        private static void CreatePop(List<double> tempWeights, Role role)
        {
            List<double> vals = new(tempWeights);
            for (int i = 0; i < role.count; i++)
            {
                for (int j = 0; j < tempWeights.Count; j++)
                {
                    //var val = rand.NextDouble();
                    //vals[j] *= ((val * 10) - 5);
                    vals[j] += rand.NextDouble();
                }
                Weight temp = new()
                {
                    values = new(vals),
                    roleName = role.name
                };
                weights.Add(temp);
            }
            AddOrgs(role);
        }

        private static void AddOrgs(Role role)
        {
            for (int i = 0; i < role.count; i++)
            {
                Organism organism = new()
                {
                    nNet = nNet,
                    trainingRoomRoleNamespace = role.name,
                    name = "ChaosTerrarian " + i,
                    assigned = false
                };
                organism.nNet.AssignWeight(weights[i]);
                SessionManager.Organisms.Add(organism);
            }
        }

        public override void Unload()
        {
            loginHotkey = null;
            sessionHotkey = null;
            observerModeHotkey = null;
            cycleOrgs = null;
            nNet = null;
            weights = null;
            SessionManager.Organisms = null;
            SessionManager.ObservableNPCs = null;
            weight = null;
        }
    }
}