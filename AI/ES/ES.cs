using ChaosTerraria.Classes;
using ChaosTerraria.Managers;
using System;

namespace ChaosTerraria.AI
{
    internal static class ES
    {
        private static readonly double learningRate = 0.001;
        private static Random rand = new(DateTime.Now.Millisecond);
        internal static void UpdateWeights()
        {
            for (int i = 0; i < ChaosTerraria.weights.Count; i++)
            {
                for (int j = 0; j < ChaosTerraria.weights[i].values.Count; j++)
                {
                    ChaosTerraria.weights[i].values[j] *= SessionManager.Scores[i].Item2;
                    ChaosTerraria.sum.values[j] += ChaosTerraria.weights[i].values[j];
                }
            }
            for (int i = 0; i < ChaosTerraria.weight.values.Count; i++)
            {
                ChaosTerraria.weight.values[i] += (ChaosTerraria.sum.values[i] * learningRate);
                ChaosTerraria.sum.values[i] = 0;
            }

            foreach (Weight weight in ChaosTerraria.weights)
            {
                for (int j = 0; j < weight.values.Count; j++)
                {
                    var random = rand.NextDouble();
                    weight.values[j] = ChaosTerraria.weight.values[j] + random;
                }
            }

            for (int i = 0; i < SessionManager.Organisms.Count; i++)
            {
                SessionManager.Organisms[i].assigned = false;
                SessionManager.Organisms[i].nNet.AssignWeight(ChaosTerraria.weights[i]);
            }
        }
    }
}
