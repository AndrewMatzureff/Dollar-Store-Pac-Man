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
        public Tile tile;
        public int row, column;

        public bool isAlternate = false;
        public ConduitNode a, b;

        public ConduitNode(Tile tile, int row, int column) {
            this.tile = tile;
            this.row = row;
            this.column = column;
        }

        int key() { return key(row, column); }

        static int key(int row, int column) {
            int hash = column == 0 ? 10 : 1;
            for (int c = column; c > 0; c /= 10) hash *= 10;
            return row * hash + column; // Not available in C# 4: HashCode.Combine(row, column);
        }

        public static ConduitNode build(bool isAlternate, int row, int column, Dictionary<int, ConduitNode> visited, List<string> mazeGrid) { return null; }
    }
}
