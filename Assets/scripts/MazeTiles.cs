using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MazeTiles {
    public enum Tile { }

    public static class Util {
        public static string ToEnumMemberAttrValue(this Enum @enum) {
            var member = @enum
                .GetType()
                .GetMember(@enum.ToString())
                .FirstOrDefault();

            var attr = member == null ? null : member
                .GetCustomAttributes(false)
                .OfType<EnumMemberAttribute>()
                .FirstOrDefault();

            if (attr != null) return attr.Value;
            throw new ArgumentException(string.Format("Enum({0}).Member({1}).Attribute({2}) does not produce a value.", @enum, member, attr));
        }

        public static Tile[] Tiles() { return null; }
        public static Tile Tile(char symbol) { return 0; }
        public static char GetSymbol(Tile tile) { return (char)0; }
        public static bool IsJunction(Tile tile) { return false; }
        public static bool IsNull(Tile tile) { return false; }
        public static bool IsActor(Tile tile) { return false; }
        public static T GetMember<T>(Tile tile, Regex member, Func<Match, T> mapper) { return mapper(null); }
    }
}
