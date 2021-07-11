using ChaosTerraria.ChaosUtils;
using ChaosTerraria.Classes;
using ChaosTerraria.Structs;
using System.Collections.Generic;

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

        public static bool SessionStarted { get => sessionStarted; set => sessionStarted = value; }
        public static List<Report> Reports { get => reports; set => reports = value; }
        public static List<Species> Species { get => species; set => species = value; }
        public static List<ObservedAttributes> ObservedAttributes { get => observedAttributes; set => observedAttributes = value; }
        public static Stats CurrentStats { get => currentStats; set => currentStats = value; }
        public static List<Organism> Organisms { get => organisms; set => organisms = value; }
        public static Session CurrentSession { get => currentSession; set => currentSession = value; }

        public static Package Package { get => package; set => package = value; }

        public static void InitializeSession()
        {
            if (!sessionStarted)
            {
                reports = new List<Report>();
                species = new List<Species>();
                organisms = new List<Organism>();
                observedAttributes = new List<ObservedAttributes>();
            }
        }

        public static void SetCurrentSessionNamespace()
        {
            if (ChaosNetConfig.data.sessionNamespace != null)
            {
                currentSession.nameSpace = ChaosNetConfig.data.sessionNamespace;
            }
        }

        private static void ResetOrgs()
        {
            if (Organisms != null)
            {
                foreach (Organism org in Organisms)
                {
                    org.assigned = !org.assigned;
                }
            }
        }

        public static Organism GetOrganism()
        {
            foreach (Organism org in organisms)
            {
                if (org.assigned == false)
                {
                    org.assigned = true;
                    return org;
                }
            }
            return null;
        }
    }
}
