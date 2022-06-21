using ChaosTerraria.Managers;
using System;
using System.Collections.Generic;

namespace ChaosTerraria.AI
{
    internal static class ES
    {
        private static readonly double learningRate = 0.01;
        private static readonly double noiseStd = 0.1;
        private static Random rand = new();
        private static double totalScore = 0;
        private static List<double> noise = new();
        internal static double GenerateGaussianNoise()
        {
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
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
                var random = GenerateGaussianNoise();
                totalScore += score.Item2 * random;
                noise.Add(random);
            }

            for (int i = 0; i < ChaosTerraria.weight.values.Count; i++)
            {
                //ChaosTerraria.weight.values[i] *= score.Item2;
                ChaosTerraria.weight.values[i] += learningRate * totalScore/(SessionManager.Organisms.Count * noiseStd);
            }

            for (int i = 0; i < SessionManager.Organisms.Count; i++)
            {
                SessionManager.Organisms[i].assigned = false;
                SessionManager.Organisms[i].nNet.AssignWeight(ChaosTerraria.weight, noise[i]);
            }
            ChaosTerraria.weight.epoch++;
            noise.Clear();
            totalScore = 0;
        }
    }
}
