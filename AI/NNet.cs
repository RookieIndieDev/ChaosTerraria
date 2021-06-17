using ChaosTerraria.Enums;
using ChaosTerraria.Managers;
using ChaosTerraria.Structs;
using System;
using System.Collections.Generic;
using Terraria.ID;

namespace ChaosTerraria.AI
{
    public class NNet
    {
        public List<Neuron> neurons;

        public int GetOutput(int[] input, string speciesNamespace)
        {
            int inputIndex = 0;
            int output = -100;
            double outputValue = 0;
            foreach (Neuron neuron in neurons)
            {
                if(neuron.type == "BlockInput")
                {
                    neuron.value = input[inputIndex++];
                    neuron.evaluated = true;
                    ObservedAttributes observedAttr;
                    observedAttr.attributeId = "BLOCK_ID";
                    observedAttr.attributeValue = Enum.GetName(typeof(TerrariaTileTypes), (int)neuron.value);
                    observedAttr.species = speciesNamespace;
                    if(!SessionManager.ObservedAttributes.Contains(observedAttr))
                        SessionManager.ObservedAttributes.Add(observedAttr);
                }
                else if(neuron.type == "BiasInput")
                {
                    neuron.value = neuron.weight;
                    neuron.evaluated = true;
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
                            case "PlaceBlockTop":
                                output = (int)OutputType.PlaceBlockTop;
                                break;
                            case "PlaceBlockTopLeft":
                                output = (int)OutputType.PlaceBlockTopLeft;
                                break;
                            case "PlaceBlockTopRight":
                                output = (int)OutputType.PlaceBlockTopRight;
                                break;
                            case "PlaceBlockBottom":
                                output = (int)OutputType.PlaceBlockBottom;
                                break;
                            case "PlaceBlockBottomLeft":
                                output = (int)OutputType.PlaceBlockBottomLeft;
                                break;
                            case "PlaceBlockBottomRight":
                                output = (int)OutputType.PlaceBlockBottomRight;
                                break;
                            case "PlaceBlockRight":
                                output = (int)OutputType.PlaceBlockRight;
                                break;
                            case "PlaceBlockLeft":
                                output = (int)OutputType.PlaceBlockLeft;
                                break;
                            case "MineBlockTop":
                                output = (int)OutputType.MineBlockTop;
                                break;
                            case "MineBlockTopLeft":
                                output = (int)OutputType.MineBlockTopLeft;
                                break;
                            case "MineBlockTopRight":
                                output = (int)OutputType.MineBlockTopRight;
                                break;
                            case "MineBlockBottom":
                                output = (int)OutputType.MineBlockBottom;
                                break;
                            case "MineBlockBottomLeft":
                                output = (int)OutputType.MineBlockBottomLeft;
                                break;
                            case "MineBlockBottomRight":
                                output = (int)OutputType.MineBlockBottomRight;
                                break;
                            case "MineBlockRight":
                                output = (int)OutputType.MineBlockRight;
                                break;
                            case "MineBlockLeft":
                                output = (int)OutputType.MineBlockLeft;
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
