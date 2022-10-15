using ChaosTerraria.Config;
using ChaosTerraria.Managers;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace ChaosTerraria.AI
{
    internal static class ES
    {
        private static Random rand = new();
        private static double totalScore = 0;
        private static List<float> noise = new();

        internal static float GenerateGaussianNoise()
        {
            float u1 = 1 - rand.NextSingle();
            float u2 = 1 - rand.NextSingle();
            float num = (float)Math.Sqrt(-2.0 * (float)Math.Log(u1)) * (float)Math.Sin(2 * (float)Math.PI * u2);
            return num * ModContent.GetInstance<ChaosTerrariaConfig>().std + ModContent.GetInstance<ChaosTerrariaConfig>().mean;
        }

        internal static void UpdateWeights()
        {
            //for (int i = 0; i < ChaosTerraria.weights.Count; i++)
            //{
            //    for (int j = 0; j < ChaosTerraria.weights[i].values.Count; j++)
            //    {
            //        ChaosTerraria.weights[i].values[j] *= SessionManager.Scores[i].Item2;
            //        ChaosTerraria.sum.values[j] += ChaosTerraria.weights[i].values[j];
            //    }
            //}
            //for (int i = 0; i < ChaosTerraria.weight.values.Count; i++)
            //{
            //    ChaosTerraria.weight.values[i] += ChaosTerraria.sum.values[i] * learningRate;
            //    ChaosTerraria.sum.values[i] = 0;
            //}

            //foreach (Weight weight in ChaosTerraria.weights)
            //{
            //    for (int j = 0; j < weight.values.Count; j++)
            //    {
            //        weight.values[j] = ChaosTerraria.weight.values[j] + rand.NextDouble() * learningRate;
            //    }
            //}

            //for (int i = 0; i < SessionManager.Organisms.Count; i++)
            //{
            //    SessionManager.Organisms[i].assigned = false;
            //    SessionManager.Organisms[i].nNet.AssignWeight(ChaosTerraria.weights[i]);
            //}
            foreach((string, int) score in SessionManager.Scores)
            {
                totalScore += score.Item2;
            }

            for (int i = 0; i < ChaosTerraria.weight.values.Count; i++)
            {
                //ChaosTerraria.weight.values[i] *= score.Item2;
                ChaosTerraria.weight.values[i] += ModContent.GetInstance<ChaosTerrariaConfig>().learningRate * totalScore;
            }

            for (int i = 0; i < SessionManager.Organisms.Count; i++)
            {
                SessionManager.Organisms[i].assigned = false;
                SessionManager.Organisms[i].nNet.AssignWeight();
            }
            ChaosTerraria.weight.epoch++;
            noise.Clear();
            totalScore = 0;
        }
    }
}
