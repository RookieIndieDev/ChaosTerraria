using ChaosTerraria.Enums;
using ChaosTerraria.Managers;
using ChaosTerraria.Structs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ChaosTerraria.AI
{
    public class NNet
    {
        public List<Neuron> neurons;

        public int GetOutput(Vector2 center, string speciesNamespace, out int direction)
        {
            int output = -100;
            double outputValue = 0;
            int tempDirection = -1;
            Vector2 tilePos = new Vector2();
            int tileType = 0;
            Type rangeType = typeof(Range);
            foreach (Neuron neuron in neurons)
            {
                if (neuron.type == "BlockInput")
                {
                    switch((int)Enum.Parse(typeof(Direction), neuron.direction))
                    {

                        case (int)Direction.Top:
                            tilePos.X = center.X;
                            tilePos.Y = center.Y - (int)Enum.Parse(rangeType, neuron.range);
                            break;
                        case (int)Direction.TopLeft:
                            tilePos.X = center.X - (int)Enum.Parse(rangeType, neuron.range);
                            tilePos.Y = center.Y - (int)Enum.Parse(rangeType, neuron.range);
                            break;
                        case (int)Direction.TopRight:
                            tilePos.X = center.X + (int)Enum.Parse(rangeType, neuron.range);
                            tilePos.Y = center.Y - (int)Enum.Parse(rangeType, neuron.range);
                            break;
                        case (int)Direction.Bottom:
                            tilePos.X = center.X;
                            tilePos.Y = center.Y + (int)Enum.Parse(rangeType, neuron.range);
                            break;
                        case (int)Direction.BottomLeft:
                            tilePos.X = center.X - (int)Enum.Parse(rangeType, neuron.range);
                            tilePos.Y = center.Y + (int)Enum.Parse(rangeType, neuron.range);
                            break;
                        case (int)Direction.BottomRight:
                            tilePos.X = center.X + (int)Enum.Parse(rangeType, neuron.range);
                            tilePos.Y = center.Y + (int)Enum.Parse(rangeType, neuron.range);
                            break;
                        case (int)Direction.Left:
                            tilePos.X = center.X - (int)Enum.Parse(rangeType, neuron.range);
                            tilePos.Y = center.Y;
                            break;
                        case (int)Direction.Right:
                            tilePos.X = center.X + (int)Enum.Parse(rangeType, neuron.range);
                            tilePos.Y = center.Y;
                            break;
                    }
                    Point pos = tilePos.ToTileCoordinates();
                    if(pos.X >= 0 && pos.Y >=0 && pos.X < Main.maxTilesX && pos.Y < Main.maxTilesY)
                    {
                        Tile tile = Framing.GetTileSafely(pos);
                        if (tile.active())
                        {
                            tileType = tile.type == 0 ? 1 : tile.type;
                        }
                            
                    }

                    neuron.value = tileType;
                    neuron.evaluated = true;
                    ObservedAttributes observedAttr;
                    observedAttr.attributeId = "BLOCK_ID";
                    string uniqueKey = TileID.GetUniqueKey((int)(neuron.value));
                    observedAttr.attributeValue = uniqueKey.Split(' ')[1];
                    observedAttr.species = speciesNamespace;
                    if (!SessionManager.ObservedAttributes.Contains(observedAttr))
                        SessionManager.ObservedAttributes.Add(observedAttr);
                    observedAttr.attributeValue = "Dirt";
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
