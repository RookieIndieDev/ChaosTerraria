using ChaosTerraria.Enums;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace ChaosTerraria.AI
{
    public class NNet
    {
        public List<Neuron> neurons;

        public int GetOutput(int[] input)
        {
            int inputIndex = 0;
            int output = 0;
            float outputValue = 0;
            foreach (Neuron neuron in neurons)
            {
/*                switch (neuron.type)
                {
                    case "BlockInput":
                        neuron.value = input[inputIndex++];
                        break;
                    case "BiasInput":
                        neuron.value = neuron.weight;
                        break;
                    default:
                        if(neuron.baseType == "output")
                            neuron.value = SetValue(neuron);
                        break;
                }

                if (neuron.baseType == "output")
                {
                    if (neuron.value > outputValue)
                    {
                        outputValue = neuron.value;
                        switch (neuron.type)
                        {
                            case "MoveLeft":
                                output = (int)OutputType.MoveLeft;
                                break;
                            case "MoveRight":
                                output = (int)OutputType.MoveRight;
                                break;
                            case "Jump":
                                output = (int)OutputType.Jump;
                                break;
                        }
                    }
                }

                if (neuron.baseType == "middle")
                {
                    switch (neuron.activator)
                    {
                        case "Gaussian":
                            neuron.value = GetGaussianActivation(neuron);
                            break;
                        case "Sigmoid":
                            neuron.value = GetSigmoidActivation(neuron);
                            break;
                        case "BinaryStep":
                            neuron.value = GetBinaryStepActivation(neuron);
                            break;
                        case "Relu":
                            neuron.value = GetReluActivation(neuron);
                            break;
                    }
                }*/
                if(neuron.type == "BlockInput")
                {
                    neuron.value = input[inputIndex++];
                }
                else if(neuron.type == "BiasInput")
                {
                    neuron.value = neuron.weight;
                }
            }

            foreach(Neuron neuron in neurons)
            {
                if(neuron.baseType == "middle")
                {
                    switch (neuron.activator)
                    {
                        case "Gaussian":
                            neuron.value = GetGaussianActivation(neuron);
                            break;
                        case "Sigmoid":
                            neuron.value = GetSigmoidActivation(neuron);
                            break;
                        case "BinaryStep":
                            neuron.value = GetBinaryStepActivation(neuron);
                            break;
                        case "Relu":
                            neuron.value = GetReluActivation(neuron);
                            break;
                    }
                }
            }

            foreach(Neuron neuron in neurons)
            {
                if(neuron.baseType == "output")
                {
                    neuron.value = SetValue(neuron);
                    if (neuron.value > outputValue)
                    {
                        outputValue = neuron.value;
                        switch (neuron.type)
                        {
                            case "MoveLeft":
                                output = (int)OutputType.MoveLeft;
                                break;
                            case "MoveRight":
                                output = (int)OutputType.MoveRight;
                                break;
                            case "Jump":
                                output = (int)OutputType.Jump;
                                break;
                        }
                    }
                }
            }

            return output;
        }

        private float SetValue(Neuron neuron)
        {
            float value = 0;
            foreach (Dependency dependency in neuron.dependencies)
            {
                Neuron dependencyNeuron = GetDependencyNeuron(dependency);
                if (dependencyNeuron != null)
                {
                    value += dependencyNeuron.value * dependency.weight;
                }
                else
                {
                    value += dependency.weight;
                }
            }
            return value;
        }

/*        private float GetGaussianActivation(Dependency[] dependencies)
        {
            float weight = GetWeight(dependencies);
            return (float)Math.Exp(-(weight * weight));
        }

        private float GetSigmoidActivation(Dependency[] dependencies)
        {
            float weight = GetWeight(dependencies);
            return (float)(1 / (1 + Math.Exp(-weight)));
        }

        private float GetBinaryStepActivation(Dependency[] dependencies)
        {
            float weight = GetWeight(dependencies);
            if (weight < 0)
            {
                return 0;
            }
            return 1;
        }

        private float GetReluActivation(Dependency[] dependencies)
        {
            float weight = GetWeight(dependencies);
            if (weight <= 0)
            {
                return 0;
            }
            return weight;
        }*/

        private float GetGaussianActivation(Neuron neuron)
        {
            float value = SetValue(neuron);
            return (float)Math.Exp(-(value * value));
        }

        private float GetSigmoidActivation(Neuron neuron)
        {
            float value = SetValue(neuron);
            return (float)(1 / (1 + Math.Exp(-value)));
        }

        private float GetBinaryStepActivation(Neuron neuron)
        {
            float value = SetValue(neuron);
            if (value < 0)
            {
                return 0;
            }
            return 1;
        }

        private float GetReluActivation(Neuron neuron)
        {
            float value = SetValue(neuron);
            if (value <= 0)
            {
                return 0;
            }
            return value;
        }

        private float GetWeight(Dependency[] dependencies)
        {
            float weight = 0;
            foreach (Dependency dependency in dependencies)
            {
                weight += dependency.weight;
            }

            return weight;
        }

        private Neuron GetDependencyNeuron(Dependency dependency)
        {
            foreach(Neuron neuron in neurons)
            {
                if(neuron.id == dependency.neuronId)
                {
                    return neuron;
                }
            }
            return null;
        }
    }
}
