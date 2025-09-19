using MazeTiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MazeNodes {

    public class ConduitNode {
        public const int LOG_10_MAX_LENGTH_PER_GRID_DIMENSION = 2; // Used to determine max length per grid dimension: log_10(10 ^ 2 == 100) == 2.
        public static readonly int MAX_LENGTH_PER_GRID_DIMENSION = ( // Grid dimensions must not exceed: [100, 100].
          from n in new int[LOG_10_MAX_LENGTH_PER_GRID_DIMENSION] select 10
        ).Aggregate(1, (a, b) => a * b);

        public Tile tile;
        public int row, column;

        public Flip flip;
        public ConduitNode a, b;

        public ConduitNode(Tile tile, int row, int column) {
            this.tile = tile;
            this.row = row;
            this.column = column;
        }

        public int key() { return key(row, column); }
        
        public static int key(int row, int column) {
            if (!isValid(row, column)) throw new ArgumentException(string.Format(
                "Invalid argument(s): expected values in range [0, {0}), but got key(row={1}, column={2}).", MAX_LENGTH_PER_GRID_DIMENSION, row, column
            ));

            int hash = 1;
            int magnitude = MAX_LENGTH_PER_GRID_DIMENSION / 10; // Corresponds to the minimum number of digits required to represent a valid key in the key set.
            for (int digit = ++row | ++column; digit > 0; digit /= magnitude) hash *= magnitude; // Increment the inputs to avoid the 0 case and then accumulate on the hash based on the combined input digits.
            return row * hash + column; // This is all to compensate for the absence of 'HashCode.Combine(row, column)' in C# 4 as well as to avoid having to pass the grid width into the node builder.
        }

        public static bool isValid(int row, int column) {
            return (row | column) >= 0 && Math.Max(row, column) < MAX_LENGTH_PER_GRID_DIMENSION; // row && column => [0, MAX_LENGTH_PER_GRID_DIMENSION)
        }

        public static ConduitNode build(Flip flip, int row, int column, Dictionary<int, ConduitNode> visited, List<string> mazeGrid) {
            if ((row | column) < 0     ||
                 row >= mazeGrid.Count ||
                 column >= mazeGrid[row].Length) {
                return null;
            }

            int key = ConduitNode.key(row, column);
            if (visited.ContainsKey(key)) return null;

            char symbol = mazeGrid[row][column];
            ConduitNode node = new ConduitNode(Util.Tile(symbol), row, column);

            if (!Util.IsJunction(node.tile)) node.flip = flip;
            //node.isAlternate = allowAlternate && !Util.IsJunction(node.tile);
            visited[key] = node;

            int rowA, columnA;
            int rowB, columnB;
            //bool altA, altB;
            Flip altA, altB;
            switch (node.tile) {
                // Bends
                case Tile.DOUBLE_ELBOW_TOP_LEFT:
                case Tile.SINGLE_ELBOW_TOP_LEFT:
                case Tile.GHOST_HOME_TOP_LEFT:
                    rowA = row; columnA = column + 1; altA = node.tile != Tile.DOUBLE_ELBOW_TOP_LEFT ? Flip.Y : Flip.NONE; // →
                    rowB = row + 1; columnB = column; altB = node.tile != Tile.DOUBLE_ELBOW_TOP_LEFT ? Flip.X : Flip.NONE; // ↓
                    break;

                case Tile.DOUBLE_ELBOW_TOP_RIGHT:
                case Tile.SINGLE_ELBOW_TOP_RIGHT:
                case Tile.GHOST_HOME_TOP_RIGHT:
                    rowA = row; columnA = column - 1; altA = node.tile != Tile.DOUBLE_ELBOW_TOP_RIGHT ? Flip.Y : Flip.NONE; // ←
                    rowB = row + 1; columnB = column; altB = node.tile == Tile.DOUBLE_ELBOW_TOP_RIGHT ? Flip.X : Flip.NONE; // ↓
                    break;

                case Tile.DOUBLE_ELBOW_BOTTOM_LEFT:
                case Tile.SINGLE_ELBOW_BOTTOM_LEFT:
                case Tile.GHOST_HOME_BOTTOM_LEFT:
                    rowA = row - 1; columnA = column; altA = node.tile != Tile.DOUBLE_ELBOW_BOTTOM_LEFT ? Flip.X : Flip.NONE; // ↑
                    rowB = row; columnB = column + 1; altB = node.tile == Tile.DOUBLE_ELBOW_BOTTOM_LEFT ? Flip.Y : Flip.NONE; // →
                    break;

                case Tile.DOUBLE_ELBOW_BOTTOM_RIGHT:
                case Tile.SINGLE_ELBOW_BOTTOM_RIGHT:
                case Tile.GHOST_HOME_BOTTOM_RIGHT:
                    rowA = row - 1; columnA = column; altA = node.tile == Tile.DOUBLE_ELBOW_BOTTOM_RIGHT ? Flip.X : Flip.NONE; // ↑
                    rowB = row; columnB = column - 1; altB = node.tile == Tile.DOUBLE_ELBOW_BOTTOM_RIGHT ? Flip.Y : Flip.NONE; // ←
                    break;

                // Pipes
                case Tile.DOUBLE_PIPE_HORIZONTAL:
                case Tile.SINGLE_PIPE_HORIZONTAL:
                case Tile.GHOST_DOOR_LEFT:
                case Tile.GHOST_DOORWAY:
                case Tile.GHOST_DOOR_RIGHT:
                    rowA = row; columnA = column - 1; altA = flip;// node.tile == Tile.DOUBLE_PIPE_HORIZONTAL ? allowAlternate;
                    rowB = row; columnB = column + 1; altB = flip;// node.tile == Tile.DOUBLE_PIPE_HORIZONTAL ? allowAlternate;
                    break;

                case Tile.DOUBLE_PIPE_VERTICAL:
                case Tile.SINGLE_PIPE_VERTICAL:
                    rowA = row - 1; columnA = column; altA = flip;// allowAlternate && node.tile == Tile.DOUBLE_PIPE_VERTICAL;
                    rowB = row + 1; columnB = column; altB = flip;// allowAlternate && node.tile == Tile.DOUBLE_PIPE_VERTICAL;
                    break;

                default: throw new ArgumentException(string.Format("Invalid argument: expected a conduit tile at '([row={0}, column={1}])', but got '{2}'.", row, column, node.tile));
            }

            ConduitNode nodeA = build(altA, rowA, columnA, visited, mazeGrid);
            ConduitNode nodeB = build(altB, rowB, columnB, visited, mazeGrid);

            node.a = nodeA == null ? (ConduitNode.isValid(rowA, columnA) && visited.ContainsKey(ConduitNode.key(rowA, columnA)) ? visited[ConduitNode.key(rowA, columnA)] : null) : nodeA;
            node.b = nodeB == null ? (ConduitNode.isValid(rowB, columnB) && visited.ContainsKey(ConduitNode.key(rowB, columnB)) ? visited[ConduitNode.key(rowB, columnB)] : null) : nodeB;

            return node;
        }
    }
}
