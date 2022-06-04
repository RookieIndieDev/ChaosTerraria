using ChaosTerraria.ChaosUtils;
using ChaosTerraria.Classes;
using ChaosTerraria.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace ChaosTerraria.Managers
{
    public static class SessionManager
    {
        private static Session currentSession;
        private static List<Organism> organisms;
        private static Stats currentStats;
        private static List<Species> species;
        private static List<Report> reports;
        private static List<ObservedAttributes> observedAttributes;
        private static bool sessionStarted = false;
        private static Package package;
        private static List<ModNPC> observableNPCs;
        private static List<(string, int)> scores;
        public static bool SessionStarted { get => sessionStarted; set => sessionStarted = value; }
        public static List<Report> Reports { get => reports; set => reports = value; }
        public static List<(string, int)> Scores { get => scores; set => scores = value; }
        public static List<Species> Species { get => species; set => species = value; }
        public static List<ObservedAttributes> ObservedAttributes { get => observedAttributes; set => observedAttributes = value; }
        public static Stats CurrentStats { get => currentStats; set => currentStats = value; }
        public static List<Organism> Organisms { get => organisms; set => organisms = value; }
        public static Session CurrentSession { get => currentSession; set => currentSession = value; }

        public static Package Package { get => package; set => package = value; }
        public static List<ModNPC> ObservableNPCs { get => observableNPCs; set => observableNPCs = value; }

        public static void InitializeSession()
        {
            if (!sessionStarted)
            {
                reports = new();
                species = new();
                organisms = new();
                observedAttributes = new();
                observableNPCs = new();
                scores = new();
            }
        }

        public static void InitScores()
        {
            foreach(Organism org in organisms)
            {
                scores.Add((org.trainingRoomRoleNamespace + " " + org.name, 0));
            }
        }

        public static void SetCurrentSessionNamespace()
        {
            if (ChaosNetConfig.data.sessionNamespace != null)
            {
                currentSession.nameSpace = ChaosNetConfig.data.sessionNamespace;
            }
        }

        public static Organism GetOrganism(string roleName)
        {
            foreach (Organism org in organisms)
            {
                if (org.assigned == false && roleName == org.trainingRoomRoleNamespace)
                {
                    org.assigned = true;
                    return org;
                }
            }
            return null;
        }
    }
}
