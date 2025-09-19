using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MazeTiles {
    [Flags]
    public enum Flip { NONE, X, Y, BOTH }
    public enum Tile {
        // Empty
        [EnumMember(Value = ":symbol= :isNull:")] NULL_SPACE,
        [EnumMember(Value = ":symbol=<:isNull:")] NULL_SHIFT_LEFT,
        [EnumMember(Value = ":symbol=>:isNull:")] NULL_SHIFT_RIGHT,

        // Actors
        [EnumMember(Value = ":symbol=☻:isActor:")] PAC_MAN,
        [EnumMember(Value = ":symbol=☺:isActor:")] GHOST,
        [EnumMember(Value = ":symbol=○:isActor:")] POWER_PELLET,
        [EnumMember(Value = ":symbol=•:isActor:")] PELLET,

        // Single Pipes
        [EnumMember(Value = ":symbol=─:")] SINGLE_PIPE_HORIZONTAL,
        [EnumMember(Value = ":symbol=│:")] SINGLE_PIPE_VERTICAL,

        // Single Elbows
        [EnumMember(Value = ":symbol=┌:isJunction:")] SINGLE_ELBOW_TOP_LEFT,
        [EnumMember(Value = ":symbol=┐:isJunction:")] SINGLE_ELBOW_TOP_RIGHT,
        [EnumMember(Value = ":symbol=└:isJunction:")] SINGLE_ELBOW_BOTTOM_LEFT,
        [EnumMember(Value = ":symbol=┘:isJunction:")] SINGLE_ELBOW_BOTTOM_RIGHT,

        // Double Pipes
        [EnumMember(Value = ":symbol=═:")] DOUBLE_PIPE_HORIZONTAL,
        [EnumMember(Value = ":symbol=║:")] DOUBLE_PIPE_VERTICAL,

        // Double Elbows
        [EnumMember(Value = ":symbol=╔:isJunction:")] DOUBLE_ELBOW_TOP_LEFT,
        [EnumMember(Value = ":symbol=╗:isJunction:")] DOUBLE_ELBOW_TOP_RIGHT,
        [EnumMember(Value = ":symbol=╚:isJunction:")] DOUBLE_ELBOW_BOTTOM_LEFT,
        [EnumMember(Value = ":symbol=╝:isJunction:")] DOUBLE_ELBOW_BOTTOM_RIGHT,

        // Ghost Home
        [EnumMember(Value = ":symbol=╓:isJunction:")] GHOST_HOME_TOP_LEFT,
        [EnumMember(Value = ":symbol=╖:isJunction:")] GHOST_HOME_TOP_RIGHT,
        [EnumMember(Value = ":symbol=╙:isJunction:")] GHOST_HOME_BOTTOM_LEFT,
        [EnumMember(Value = ":symbol=╜:isJunction:")] GHOST_HOME_BOTTOM_RIGHT,
        [EnumMember(Value = ":symbol=]:")] GHOST_DOOR_LEFT,
        [EnumMember(Value = ":symbol=_:")] GHOST_DOORWAY,
        [EnumMember(Value = ":symbol=[:")] GHOST_DOOR_RIGHT,
    }

    public static class Util {
        private static string MATCH_GROUP_KEY = "key";
        private static string MATCH_GROUP_VALUE = "value";
        private static string REGEX_PATTERN_VAR = string.Format(@":(?<{0}>{{0}})=(?<{1}>{{1}}):", MATCH_GROUP_KEY, MATCH_GROUP_VALUE);
        private static string REGEX_PATTERN_FLAG = string.Format(@":(?<{0}>{{0}}):", MATCH_GROUP_KEY);
        private static Regex REGEX_ENTIRE_STRING = new Regex(@"^.*$");
        private static Regex REGEX_SYMBOL = new Regex(string.Format(REGEX_PATTERN_VAR, "symbol", "."));
        private static Regex REGEX_IS_JUNCTION = new Regex(string.Format(REGEX_PATTERN_FLAG, "isJunction"));
        private static Regex REGEX_IS_NULL = new Regex(string.Format(REGEX_PATTERN_FLAG, "isNull"));
        private static Regex REGEX_IS_ACTOR = new Regex(string.Format(REGEX_PATTERN_FLAG, "isActor"));
        private static Dictionary<char, Tile> TILES_BY_SYMBOL = Tiles().ToDictionary(GetSymbol, tile => tile);

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

        public static Tile[] Tiles() {
            return Enum
                .GetValues(typeof(Tile))
                .Cast<Tile>()
                .ToArray();
        }

        public static Tile Tile(char symbol) {
            if (TILES_BY_SYMBOL.ContainsKey(symbol)) return TILES_BY_SYMBOL[symbol];
            throw new ArgumentException(string.Format("No Tile exists for symbol: Tile('{0}')", symbol));
        }

        public static char GetSymbol(Tile tile) {
            return GetMember(tile, REGEX_SYMBOL, match => {
                if (match.Success) return match.Groups[MATCH_GROUP_VALUE].Value;
                throw new ArgumentException(string.Format(
                    "No symbol matching '{0}' exists for Tile: GetSymbol({1}:'{2}')", REGEX_SYMBOL, tile, GetMember(tile, REGEX_ENTIRE_STRING, m => m.Value)
                ));
            })[0];
        }

        public static bool IsJunction(Tile tile) {
            return GetMember(tile, REGEX_IS_JUNCTION, match => match.Success);
        }

        public static bool IsNull(Tile tile) {
            return GetMember(tile, REGEX_IS_NULL, match => match.Success);
        }

        public static bool IsActor(Tile tile) {
            return GetMember(tile, REGEX_IS_ACTOR, match => match.Success);
        }

        public static T GetMember<T>(Tile tile, Regex member, Func<Match, T> mapper) {
            return mapper(member.Match(tile.ToEnumMemberAttrValue()));
        }
    }
}
