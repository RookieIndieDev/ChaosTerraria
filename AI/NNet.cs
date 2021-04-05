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
            double outputValue = 0;
            foreach (Neuron neuron in neurons)
            {
                if(neuron.type == "BlockInput")
                {
                    neuron.value = input[inputIndex++];
                    neuron.evaluated = true;
                }
                else if(neuron.type == "BiasInput")
                {
                    neuron.value = neuron.weight;
                    neuron.evaluated = true;
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

        private double SetValue(Neuron neuron)
        {
            double value = 0;
            foreach (Dependency dependency in neuron.dependencies)
            {
                Neuron dependencyNeuron;
                GetDependencyNeuron(dependency, out dependencyNeuron);
                if(dependencyNeuron != null)
                {
                    if (dependencyNeuron.evaluated == true)
                    {
                        value += dependencyNeuron.value * dependency.weight;
                    }
                    else
                    {
                        value += SetValue(dependencyNeuron) * dependency.weight;
                        dependencyNeuron.evaluated = true;
                    }
                }
            }
            return value;
        }

        private double GetGaussianActivation(Neuron neuron)
        {
            double value = SetValue(neuron);
            return Math.Exp(-(value * value));
        }

        private double GetSigmoidActivation(Neuron neuron)
        {
            double value = SetValue(neuron);
            return (1 / (1 + Math.Exp(-value)));
        }

        private double GetBinaryStepActivation(Neuron neuron)
        {
            double value = SetValue(neuron);
            if (value < 0)
            {
                return 0;
            }
            return 1;
        }

        private double GetReluActivation(Neuron neuron)
        {
            double value = SetValue(neuron);
            if (value <= 0)
            {
                return 0;
            }
            return value;
        }

        private double GetWeight(Dependency[] dependencies)
        {
            double weight = 0;
            foreach (Dependency dependency in dependencies)
            {
                weight += dependency.weight;
            }

            return weight;
        }

        private void GetDependencyNeuron(Dependency dependency, out Neuron outputNeuron)
        {
            foreach(Neuron neuron in neurons)
            {
                if(neuron.id == dependency.neuronId)
                {
                    outputNeuron = neuron;
                    return;
                }
            }
            outputNeuron = null;
        }
    }
}
