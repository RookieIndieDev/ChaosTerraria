﻿using ChaosTerraria.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace ChaosTerraria.AI
{
    //TODO: Replace Current system for BlockInputs with X & Y + direction similar to PlaceBlock and MineBlock?
    public class NNet
    {
        public List<Neuron> neurons;

        public int GetOutput(Point center, List<Item> inventory, out int direction, out string itemToCraft, out string blockToPlace, out int x, out int y)
        {
            int output = -100;
            double outputValue = -100;
            int tempDirection = -1;
            string tempitemToCraft = "";
            string tempBlockToPlace = "";
            Point tilePos = new Point();
            int tileType = 0;
            int blockX = 0;
            int blockY = 0;
            Type coordType = typeof(Coord);
            foreach (Neuron neuron in neurons)
            {
                switch (neuron.type)
                {
                    case "BlockInput":
                        switch ((int)Enum.Parse(typeof(Direction), neuron.direction))
                        {

                            case (int)Direction.Top:
                                tilePos.X = center.X;
                                tilePos.Y = center.Y - (int)Enum.Parse(coordType, neuron.range) - 1;
                                break;
                            case (int)Direction.TopLeft:
                                tilePos.X = center.X - (int)Enum.Parse(coordType, neuron.range) - 1;
                                tilePos.Y = center.Y - (int)Enum.Parse(coordType, neuron.range) - 1;
                                break;
                            case (int)Direction.TopRight:
                                tilePos.X = center.X + (int)Enum.Parse(coordType, neuron.range) + 1;
                                tilePos.Y = center.Y - (int)Enum.Parse(coordType, neuron.range) - 1;
                                break;
                            case (int)Direction.Bottom:
                                tilePos.X = center.X;
                                tilePos.Y = center.Y + (int)Enum.Parse(coordType, neuron.range) + 1;
                                break;
                            case (int)Direction.BottomLeft:
                                tilePos.X = center.X - (int)Enum.Parse(coordType, neuron.range) - 1;
                                tilePos.Y = center.Y + (int)Enum.Parse(coordType, neuron.range) + 1;
                                break;
                            case (int)Direction.BottomRight:
                                tilePos.X = center.X + (int)Enum.Parse(coordType, neuron.range) + 1;
                                tilePos.Y = center.Y + (int)Enum.Parse(coordType, neuron.range) + 1;
                                break;
                            case (int)Direction.Left:
                                tilePos.X = center.X - (int)Enum.Parse(coordType, neuron.range) - 1;
                                tilePos.Y = center.Y;
                                break;
                            case (int)Direction.Right:
                                tilePos.X = center.X + (int)Enum.Parse(coordType, neuron.range) + 1;
                                tilePos.Y = center.Y;
                                break;
                        }
                        Point pos = tilePos;
                        Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Red, null);
                        if (pos.X >= 0 && pos.Y >= 0 && pos.X < Main.maxTilesX && pos.Y < Main.maxTilesY)
                        {
                            Tile tile = Framing.GetTileSafely(pos);
                            if (tile.active())
                            {
                                tileType = tile.type == 0 ? 1 : tile.type;
                            }

                        }

                        neuron.value = tileType;
                        neuron.evaluated = true;
                        break;
                    case "BiasInput":
                        neuron.value = neuron.weight;
                        neuron.evaluated = true;
                        break;
                    case "HasInInventory":
                        if (inventory != null)
                        {
                            foreach (Item item in inventory)
                            {
                                if (neuron.attributeValue.Contains("Door") && item.Name.Contains("Door"))
                                {
                                    neuron.value = 1;
                                    neuron.evaluated = true;
                                    break;
                                }
                                else if (neuron.attributeValue == item.Name)
                                {
                                    neuron.value = 1;
                                    neuron.evaluated = true;
                                    break;
                                }
                            }
                        }
                        break;
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
                                blockX = (int)Enum.Parse(coordType, neuron.x);
                                blockY = (int)Enum.Parse(coordType, neuron.y);
                                break;
                            case "PlaceBlock":
                                output = (int)OutputType.PlaceBlock;
                                tempDirection = (int)Enum.Parse(typeof(Direction), neuron.direction);
                                tempBlockToPlace = neuron.attributeValue;
                                blockX = (int)Enum.Parse(coordType, neuron.x);
                                blockY = (int)Enum.Parse(coordType, neuron.y);
                                break;
                            case "Move":
                                output = (int)OutputType.Move;
                                tempDirection = (int)Enum.Parse(typeof(MoveDirection), neuron.direction);
                                break;
                            case "CraftItem":
                                output = (int)OutputType.CraftItem;
                                tempitemToCraft = neuron.attributeValue;
                                break;
                        }
                    }
                }
            }
            direction = tempDirection;
            itemToCraft = tempitemToCraft;
            blockToPlace = tempBlockToPlace;
            x = blockX;
            y = blockY;
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
