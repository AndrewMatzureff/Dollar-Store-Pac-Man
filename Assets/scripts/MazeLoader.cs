using MazeNodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using UnityEngine;

public class MazeLoader : MonoBehaviour {
    private const string MazeString = @"
╔════════════╗╔════════════╗
║••••••••••••││••••••••••••║
║•┌──┐•┌───┐•││•┌───┐•┌──┐•║
║○│  │•│   │•││•│   │•│  │○║
║•└──┘•└───┘•└┘•└───┘•└──┘•║
║••••••••••••••••••••••••••║
║•┌──┐•┌┐•┌──────┐•┌┐•┌──┐•║
║•└──┘•││•└──┐┌──┘•││•└──┘•║
║••••••││••••││••••││••••••║
╚════┐•│└──┐ ││ ┌──┘│•┌════╝
     ║•│┌──┘ └┘ └──┐│•║     
     ║•││    <☺    ││•║     
     ║•││ ╓═]__[═╖ ││•║     
═════┘•└┘ ║      ║ └┘•└═════
      •   ║<☺<☺<☺║   •      
═════┐•┌┐ ║      ║ ┌┐•┌═════
     ║•││ ╙══════╜ ││•║     
     ║•││          ││•║     
     ║•││ ┌──────┐ ││•║     
╔════┘•└┘ └──┐┌──┘ └┘•└════╗
║••••••••••••││••••••••••••║
║•┌──┐•┌───┐•││•┌───┐•┌──┐•║
║•└─┐│•└───┘•└┘•└───┘•│┌─┘•║
║○••││•••••••<☻•••••••││••○║
╚─┐•││•┌┐•┌──────┐•┌┐•││•┌─╝
╔─┘•└┘•││•└──┐┌──┘•││•└┘•└─╗
║••••••││••••││••••││••••••║
║•┌────┘└──┐•││•┌──┘└────┐•║
║•└────────┘•└┘•└────────┘•║
║••••••••••••••••••••••••••║
╚══════════════════════════╝";

    // Use this for initialization
    void Start () {
        List<string> lines = new List<string>(MazeString.Split('\n'));
        ParseActors(gameObject, lines);
        ParseConduits(lines);
    }

    // Update is called once per frame
    void Update() {}

    void ParseActors(GameObject maze, List<String> mazeGrid) {}

    List<ConduitNode> BuildConduitGraph(GameObject gameObject, List<string> mazeGrid) { return null; }

    void ParseConduits(List<String> mazeGrid) {}

    void ParseConduits(ConduitNode root, Action<ConduitNode> consumer) {}
}
