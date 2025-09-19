using MazeNodes;
using MazeTiles;
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

    public GameObject Maze;

    // Empty
    public GameObject p_NullSpace;
    public GameObject p_NullShiftLeft;
    public GameObject p_NullShiftRight;

    // Actors
    public GameObject p_PacMan;
    public GameObject p_Ghost;
    public GameObject p_PowerPellet;
    public GameObject p_Pellet;

    // Single Pipes
    public GameObject p_SinglePipeHorizontal;
    public GameObject p_SinglePipeVertical;

    // Single Elbows
    public GameObject p_SingleElbowTopLeft;
    public GameObject p_SingleElbowTopRight;
    public GameObject p_SingleElbowBottomLeft;
    public GameObject p_SingleElbowBottomRight;

    // Double Pipes
    public GameObject p_DoublePipeHorizontal;
    public GameObject p_DoublePipeVertical;

    // Double Elbows
    public GameObject p_DoubleElbowTopLeft;
    public GameObject p_DoubleElbowTopRight;
    public GameObject p_DoubleElbowBottomLeft;
    public GameObject p_DoubleElbowBottomRight;

    // Ghost Home
    public GameObject p_GhostHomeTopLeft;
    public GameObject p_GhostHomeTopRight;
    public GameObject p_GhostHomeBottomLeft;
    public GameObject p_GhostHomeBottomRight;
    public GameObject p_GhostDoorLeft;
    public GameObject p_GhostDoorway;
    public GameObject p_GhostDoorRight;

    private Dictionary<Tile, GameObject> prefabsByTile;

    // Use this for initialization
    void Start () {
        prefabsByTile = new Dictionary<Tile, GameObject> {
            { Tile.NULL_SPACE,                  p_NullSpace },
            { Tile.NULL_SHIFT_LEFT,             p_NullShiftLeft },
            { Tile.NULL_SHIFT_RIGHT,            p_NullShiftRight },

            { Tile.PAC_MAN,                     p_PacMan },
            { Tile.GHOST,                       p_Ghost },
            { Tile.POWER_PELLET,                p_PowerPellet },
            { Tile.PELLET,                      p_Pellet },

            { Tile.SINGLE_PIPE_HORIZONTAL,      p_SinglePipeHorizontal },
            { Tile.SINGLE_PIPE_VERTICAL,        p_SinglePipeVertical },

            { Tile.SINGLE_ELBOW_TOP_LEFT,       p_SingleElbowTopLeft },
            { Tile.SINGLE_ELBOW_TOP_RIGHT,      p_SingleElbowTopRight },
            { Tile.SINGLE_ELBOW_BOTTOM_LEFT,    p_SingleElbowBottomLeft },
            { Tile.SINGLE_ELBOW_BOTTOM_RIGHT,   p_SingleElbowBottomRight },

            { Tile.DOUBLE_PIPE_HORIZONTAL,      p_DoublePipeHorizontal },
            { Tile.DOUBLE_PIPE_VERTICAL,        p_DoublePipeVertical },

            { Tile.DOUBLE_ELBOW_TOP_LEFT,       p_DoubleElbowTopLeft },
            { Tile.DOUBLE_ELBOW_TOP_RIGHT,      p_DoubleElbowTopRight },
            { Tile.DOUBLE_ELBOW_BOTTOM_LEFT,    p_DoubleElbowBottomLeft },
            { Tile.DOUBLE_ELBOW_BOTTOM_RIGHT,   p_DoubleElbowBottomRight },

            { Tile.GHOST_HOME_TOP_LEFT,         p_GhostHomeTopLeft },
            { Tile.GHOST_HOME_TOP_RIGHT,        p_GhostHomeTopRight },
            { Tile.GHOST_HOME_BOTTOM_LEFT,      p_GhostHomeBottomLeft },
            { Tile.GHOST_HOME_BOTTOM_RIGHT,     p_GhostHomeBottomRight },
            { Tile.GHOST_DOOR_LEFT,             p_GhostDoorLeft },
            { Tile.GHOST_DOORWAY,               p_GhostDoorway },
            { Tile.GHOST_DOOR_RIGHT,            p_GhostDoorRight },
        };
        
        List<string> lines = new List<string>(MazeString.Trim().Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries));
        ParseActors(gameObject, lines);
        ParseConduits(gameObject, lines);
    }

    // Update is called once per frame
    void Update() {}

    private void ParseActors(GameObject maze, List<String> mazeGrid) {
        int rows = mazeGrid.Count;
        Dictionary<GameObject, Tile> tiles = new Dictionary<GameObject, Tile>();
        GameObject gameObject = null;

        for (int r = 0; r < rows; r++) {
            int columns = mazeGrid[r].Length;

            for (int c = 0; c < columns; c++) {
                char actor = mazeGrid[r][c];

                if (gameObject == null) {
                    gameObject = new GameObject();
                    gameObject.transform.SetParent(maze.transform);
                    tiles.Add(gameObject, Util.Tile(actor));
                } else tiles[gameObject] = Util.Tile(actor);

                float subcellOffset
                  = ( c > 0
                    ? mazeGrid[r][c - 1] == '<' ? -0.5f : 0
                    : 0 )
                  + ( c < columns - 1
                    ? mazeGrid[r][c + 1] == '>' ? +0.5f : 0
                    : 0 );

                if (Util.IsActor(tiles[gameObject])) {
                    // spawn actor
                    // TODO: find a way to centralize the pixels per unit value between the spritesheet and the position helper in a constant.
                    Instantiate(prefabsByTile[tiles[gameObject]], maze.transform.position + PositionAt(c, r, columns, rows, 16, 100f, subcellOffset), Quaternion.identity);
                    gameObject = null;
                }
            }
        }

        if (gameObject != null && !Util.IsActor(tiles[gameObject])) {
            tiles.Remove(gameObject);
            Destroy(gameObject);
        }

        tiles.Clear();
    }

    private Vector3 PositionAt(int c, int r, int columns, int rows, int tilePixelDimensions, float pixelsPerUnit, float subcellOffset) {
        return new Vector3(
            ((c + subcellOffset)    * tilePixelDimensions) / pixelsPerUnit - columns    * tilePixelDimensions / (pixelsPerUnit * 2) + ((columns & 1) == 1 ? 0 : 0.5f * tilePixelDimensions / pixelsPerUnit),
            ((rows - r - 1)         * tilePixelDimensions) / pixelsPerUnit - rows       * tilePixelDimensions / (pixelsPerUnit * 2) + ((rows    & 1) == 1 ? 0 : 0.5f * tilePixelDimensions / pixelsPerUnit)
        );
    }

    List<ConduitNode> BuildConduitGraph(GameObject gameObject, List<string> mazeGrid) {
        var visited = new Dictionary<int, ConduitNode>();
        var conduitTrees = new List<ConduitNode>();
        var tileWrapper = new Tile[1];
        int rows = mazeGrid.Count;

        for (int r = 0; r < rows; r++) {
            int columns = mazeGrid[r].Length;

            for (int c = 0; c < columns; c++) {
                int row = r, column = c;
                tileWrapper[0] = Util.Tile(mazeGrid[r][c]);

                var result = (
                  from tile in tileWrapper
                  where Util.IsJunction(tile)
                  select ConduitNode.build(Flip.NONE, row, column, visited, mazeGrid)
                ).DefaultIfEmpty(null).Single();
                if (result != null) conduitTrees.Add(result);
            }
        }

        return conduitTrees;
    }

    void ParseConduits(GameObject maze, List<String> mazeGrid) {
        List<ConduitNode> conduitGraph = BuildConduitGraph(null, mazeGrid);
        char[,] chars = new char[mazeGrid.Count, mazeGrid.Select(row => row.Length).Aggregate(0, Math.Max)];

        (from root in conduitGraph where root != null select root).ToList().ForEach(
          root => ParseConduits(root, node => {
              chars[node.row, node.column] = Util.GetSymbol(node.tile);
              Debug.Log(String.Format("chars[{0}, {1}]: {2}", node.row, node.column, chars[node.row, node.column]));
              // TODO: find a way to centralize the pixels per unit value between the spritesheet and the position helper in a constant.
              GameObject cell = Instantiate(prefabsByTile[node.tile], maze.transform.position + PositionAt(node.column, node.row, chars.GetLength(1), chars.GetLength(0), 16, 100f, 0), Quaternion.identity, maze.transform);
              cell.name = String.Format("{0} @({1},{2}): ~{3}", node.tile, node.column, node.row, node.flip);
              cell.GetComponent<SpriteRenderer>().flipX = (node.flip & Flip.X) != 0;
              cell.GetComponent<SpriteRenderer>().flipY = (node.flip & Flip.Y) != 0;
          })
        );

        (from row in Enumerable.Range(0, chars.GetLength(0))
         select new string((
           from column in Enumerable.Range(0, chars.GetLength(1))
           select chars[row, column]
         ).ToArray())
        ).ToList()
          .ForEach(Console.WriteLine);
    }

    void ParseConduits(ConduitNode root, Action<ConduitNode> consumer) {
        var visited = new HashSet<ConduitNode>(new ConduitNode[] { root });
        visited.Add(null);

        ConduitNode ap = root;
        ConduitNode bp = root;
        ConduitNode ac = root.a;
        ConduitNode bc = root.b;

        while ((!visited.Contains(ac) || !visited.Contains(bc)) && (ac != null || bc != null)) {
            if (ac != null) {
                ConduitNode temp = ac;
                ac = ac.a != ap ? ac.a : ac.b != ap ? ac.b : null;
                visited.Add(temp);
                ap = temp;
            }

            if (bc != null) {
                ConduitNode temp = bc;
                bc = bc.a != bp ? bc.a : bc.b != bp ? bc.b : null;
                visited.Add(temp);
                bp = temp;
            }
        }

        visited.Remove(null);
        foreach (ConduitNode node in visited) consumer.Invoke(node);
    }
}
