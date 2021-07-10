using ChaosTerraria.Enums;
using ChaosTerraria.Managers;
using ChaosTerraria.Structs;
using System;
using System.Collections.Generic;

namespace ChaosTerraria.AI
{
    public class NNet
    {
        public List<Neuron> neurons;
        public int GetOutput(int[] input, string speciesNamespace, out int direction)
        {
            int inputIndex = 0;
            int output = -100;
            double outputValue = 0;
            int tempDirection = -1;
            foreach (Neuron neuron in neurons)
            {
                if (neuron.type == "BlockInput")
                {
                    neuron.value = input[inputIndex++];
                    neuron.evaluated = true;
                    ObservedAttributes observedAttr;
                    observedAttr.attributeId = "BLOCK_ID";
                    observedAttr.attributeValue = Enum.GetName(typeof(TerrariaTileType), (int)neuron.value);
                    observedAttr.species = speciesNamespace;
                    if (!SessionManager.ObservedAttributes.Contains(observedAttr))
                        SessionManager.ObservedAttributes.Add(observedAttr);
                }
                else if (neuron.type == "BiasInput")
                {
                    neuron.value = neuron.weight;
                    neuron.evaluated = true;
                }
            }

            foreach (Neuron neuron in neurons)
            {
                if (neuron.baseType == "output")
                {
                    neuron.value = SetValue(neuron);
                    if (neuron.value > outputValue)
                    {
                        outputValue = neuron.value;
                        switch (neuron.type)
                        {
                            case "Jump":
                                output = (int)OutputType.Jump;
                                break;
                            case "MineBlock":
                                output = (int)OutputType.MineBlock;
                                tempDirection = (int)Enum.Parse(typeof(Direction), neuron.direction);
                                break;
                            case "PlaceBlock":
                                output = (int)OutputType.PlaceBlock;
                                tempDirection = (int)Enum.Parse(typeof(Direction), neuron.direction);
                                break;
                            case "Move":
                                output = (int)OutputType.Move;
                                tempDirection = (int)Enum.Parse(typeof(MoveDirection), neuron.direction);
                                break;
                        }
                    }
                }
            }
            direction = tempDirection;
            return output;
        }

        private double SetValue(Neuron neuron)
        {
            double value = 0;
            foreach (Dependency dependency in neuron.dependencies)
            {
                GetDependencyNeuron(dependency, out Neuron dependencyNeuron);
                if (dependencyNeuron != null)
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
            if (neuron.baseType == "middle")
            {
                switch (neuron.activator)
                {
                    case "Gaussian":
                        value = GetGaussianActivation(value);
                        break;
                    case "Sigmoid":
                        value = GetSigmoidActivation(value);
                        break;
                    case "BinaryStep":
                        value = GetBinaryStepActivation(value);
                        break;
                    case "Relu":
                        value = GetReluActivation(value);
                        break;
                }
            }
            return value;
        }

        private double GetGaussianActivation(double value)
        {
            return Math.Exp(-(value * value));
        }

        private double GetSigmoidActivation(double value)
        {
            return (1 / (1 + Math.Exp(-value)));
        }

        private double GetBinaryStepActivation(double value)
        {
            if (value < 0)
            {
                return 0;
            }
            return 1;
        }

        private double GetReluActivation(double value)
        {
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
            foreach (Neuron neuron in neurons)
            {
                if (neuron.id == dependency.neuronId)
                {
                    outputNeuron = neuron;
                    return;
                }
            }
            outputNeuron = null;
        }
    }
}
