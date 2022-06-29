using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour {
    public GameObject background;
    public GameObject foreground;
    public Tile hexagonTile;
    public Tile squareTile;

    public int xLevelSize = 100;
    public int yLevelSize = 100;
    public int minRoomSize = 10;
    public int doorMinWidth = 4;
    public int doorMaxWidth = 10;

    public float xPerlinSize = 20;
    public float yPerlinSize = 20;

    public PlayerController playerScript;
    public GameObject zombiePrefab;
    public GameObject fastZombiePrefab;

    public void Awake() {

        Room root = new(Vector2Int.zero, new Vector2Int(xLevelSize - 1, yLevelSize - 1));
        ICollection<Room> levelRooms = SplitRoomsRandom(root);

        GenerateForegroundTileMap(root);
        GenerateBackgroundTileMap();

        PopulateRooms(levelRooms);
    }

    // recursively split the rooms via binary space partitioning
    private ICollection<Room> SplitRoomsRandom(Room room) {
        bool canSplitHorizontal = room.Height >= minRoomSize * 2 + 1;
        bool canSplitVertical = room.Width >= minRoomSize * 2 + 1;

        if (!canSplitHorizontal && !canSplitVertical) {
            return new List<Room>() { room };
        }

        ICollection<Room> levelRooms = new List<Room>();

        // randomly pick which way to split
        if (!canSplitVertical || (canSplitHorizontal && Random.Range(0, 2) == 0)) {
            // horizontal split
            int splitLocation = Random.Range(room.bottomLeft.y + minRoomSize, room.topRight.y - minRoomSize + 1);
            int doorStart = Random.Range(room.bottomLeft.x, room.topRight.x - doorMinWidth + 2);
            int doorEnd = Random.Range(doorStart + doorMinWidth - 1, Mathf.Min(doorStart + doorMaxWidth, room.topRight.x + 1));
            room.SplitHorizontal(splitLocation, doorStart, doorEnd);
            levelRooms.AddRange(SplitRoomsRandom(room.subRoom1));
            levelRooms.AddRange(SplitRoomsRandom(room.subRoom2));
        } else {
            // vertical split
            int splitLocation = Random.Range(room.bottomLeft.x + minRoomSize, room.topRight.x - minRoomSize + 1);
            int doorStart = Random.Range(room.bottomLeft.y, room.topRight.y - doorMinWidth + 2);
            int doorEnd = Random.Range(doorStart + doorMinWidth - 1, Mathf.Min(doorStart + doorMaxWidth, room.topRight.y + 1));
            room.SplitVertical(splitLocation, doorStart, doorEnd);
            levelRooms.AddRange(SplitRoomsRandom(room.subRoom1));
            levelRooms.AddRange(SplitRoomsRandom(room.subRoom2));
        }

        return levelRooms;
    }

    private void GenerateBackgroundTileMap() {
        const float HEXAGON_WIDTH = 1f;
        const float HEXAGON_EDGE_LENGTH = HEXAGON_WIDTH / 1.73205f; // 1.73205f = sqrt(3)
        Grid grid = background.GetComponent<Grid>();
        Tilemap tilemap = background.GetComponent<Tilemap>();
        grid.cellLayout = GridLayout.CellLayout.Hexagon;
        grid.cellSwizzle = GridLayout.CellSwizzle.YXZ;
        grid.cellSize = new Vector3(HEXAGON_WIDTH, 2 * HEXAGON_EDGE_LENGTH, 1); // add to hexagon height because unit hexagons do not have the same width and height
        float xMax = xLevelSize * grid.cellSize.x / background.transform.localScale.x;
        float yMax = yLevelSize * grid.cellSize.y / background.transform.localScale.y;
        for (int x = 0; x <= xMax; x++) {
            for (int y = 0; y <= yMax + 1; y++) {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                float perlin = Mathf.PerlinNoise(x / xMax * xPerlinSize, y / yMax * yPerlinSize);
                tilemap.SetTile(tilePos, hexagonTile);
                tilemap.SetTileFlags(tilePos, TileFlags.None);
                tilemap.SetColor(tilePos, Color.Lerp(Color.grey, Color.cyan, perlin * perlin));
            }
        }
    }

    private void GenerateForegroundTileMap(Room root) {
        ICollection<Vector2Int> tileCoordinates = GetForegroundTileCoordinates(root);

        Grid grid = foreground.GetComponent<Grid>();
        Tilemap tilemap = foreground.GetComponent<Tilemap>();
        grid.cellLayout = GridLayout.CellLayout.Rectangle;
        grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;
        grid.cellSize = Vector3.one;

        // create the wall tiles
        foreach (Vector2Int tileCoordinate in tileCoordinates) {
            tilemap.SetTile((Vector3Int)tileCoordinate, squareTile);
        }

        // create the level boundary tiles
        const int borderSize = 3;
        for (int x = -borderSize; x < xLevelSize + borderSize; x++) {
            for (int i = 0; i < borderSize; i++) {
                Vector3Int tilePos = new Vector3Int(x, -1 - i, 0);
                tilemap.SetTile(tilePos, squareTile);
                tilemap.SetTileFlags(tilePos, TileFlags.None);
                tilemap.SetColor(tilePos, Color.black);

                tilePos = new Vector3Int(x, yLevelSize + i, 0);
                tilemap.SetTile(tilePos, squareTile);
                tilemap.SetTileFlags(tilePos, TileFlags.None);
                tilemap.SetColor(tilePos, Color.black);
            }
        }
        for (int y = 0; y < yLevelSize; y++) {
            for (int i = 0; i < borderSize; i++) {
                Vector3Int tilePos = new Vector3Int(-1 - i, y, 0);
                tilemap.SetTile(tilePos, squareTile);
                tilemap.SetTileFlags(tilePos, TileFlags.None);
                tilemap.SetColor(tilePos, Color.black);

                tilePos = new Vector3Int(xLevelSize + i, y, 0);
                tilemap.SetTile(tilePos, squareTile);
                tilemap.SetTileFlags(tilePos, TileFlags.None);
                tilemap.SetColor(tilePos, Color.black);
            }
        }
    }

    // recursively get the coordinates of where the wall tiles should be
    private ICollection<Vector2Int> GetForegroundTileCoordinates(Room room) {
        if (room.splitType == RoomSplit.None) {
            return new List<Vector2Int>();
        }

        List<Vector2Int> coordinates = new();

        if (room.splitType == RoomSplit.Horizontal) {
            for (int x = room.bottomLeft.x; x <= room.topRight.x; x++) {
                if (x < room.doorStart || x > room.doorEnd) {
                    coordinates.Add(new Vector2Int(x, room.splitLocation));
                }
            }
        } else if (room.splitType == RoomSplit.Vertical) {
            for (int y = room.bottomLeft.y; y <= room.topRight.y; y++) {
                if (y < room.doorStart || y > room.doorEnd) {
                    coordinates.Add(new Vector2Int(room.splitLocation, y));
                }
            }
        }

        coordinates.AddRange(GetForegroundTileCoordinates(room.subRoom1));
        coordinates.AddRange(GetForegroundTileCoordinates(room.subRoom2));

        return coordinates;
    }

    private void PopulateRooms(ICollection<Room> rooms) {
        /*
        int numZombies = 0;
        for (int x = 0; x < xLevelSize; x++) {
            for (int y = 0; y < yLevelSize; y++) {
                if (Mathf.PerlinNoise((float)x / xLevelSize * xPerlinSize, (float)y / yLevelSize * yPerlinSize) < Random.value * Random.value * Random.value) {
                    if (enemyType < 0.7) {
                        GameObject newZombie = Instantiate(zombiePrefab, new Vector3(x, y, 0), Quaternion.identity, Globals.enemies);
                        newZombie.GetComponent<EnemyMovementController>().playerScript = playerScript;
                    } else {
                        GameObject newFastZombie = Instantiate(fastZombiePrefab, new Vector3(x, y, 0), Quaternion.identity, Globals.enemies);
                        newFastZombie.GetComponent<EnemyMovementController>().playerScript = playerScript;
                    }
                }
            }
        }

        Debug.Log(numZombies);
        */

        foreach (Room room in rooms) {
            Vector3 spawnLocation = new Vector3((room.bottomLeft.x + room.topRight.x) / 2, (room.bottomLeft.y + room.topRight.y) / 2, 0);
            if (Random.value < 0.5) {
                GameObject newZombie = Instantiate(zombiePrefab, spawnLocation, Quaternion.identity, Globals.enemies);
                newZombie.GetComponent<EnemyMovementController>().playerScript = playerScript;
            } else {
                GameObject newFastZombie = Instantiate(fastZombiePrefab, spawnLocation, Quaternion.identity, Globals.enemies);
                newFastZombie.GetComponent<EnemyMovementController>().playerScript = playerScript;
            }
            /*
            GameObject newZombie = Instantiate(zombiePrefab, spawnLocation, Quaternion.identity, Globals.enemies);
            newZombie.GetComponent<EnemyMovementController>().playerScript = playerScript;
            */
        }
    }

    private class Room {
        public Vector2Int bottomLeft;
        public Vector2Int topRight;
        public Room subRoom1;
        public Room subRoom2;
        public RoomSplit splitType = RoomSplit.None;
        public int splitLocation;
        public int doorStart;
        public int doorEnd;

        public int Width => topRight.x - bottomLeft.x + 1;
        public int Height => topRight.y - bottomLeft.y + 1;

        public Room(Vector2Int bottomLeft, Vector2Int topRight) {
            this.bottomLeft = bottomLeft;
            this.topRight = topRight;
        }

        public (Room lower, Room upper) SplitHorizontal(int location, int doorStart, int doorEnd) {
            if (location <= bottomLeft.y || location >= topRight.y) {
                throw new System.Exception($"Horizontal split is outside of bounds. Bound: ({bottomLeft.y}, {topRight.y}). Split location: {location}");
            }

            subRoom1 = new(new Vector2Int(bottomLeft.x, bottomLeft.y), new Vector2Int(topRight.x, location - 1));
            subRoom2 = new(new Vector2Int(bottomLeft.x, location + 1), new Vector2Int(topRight.x, topRight.y));

            splitType = RoomSplit.Horizontal;
            splitLocation = location;
            this.doorStart = doorStart;
            this.doorEnd = doorEnd;

            return (subRoom1, subRoom2);
        }

        public (Room left, Room right) SplitVertical(int location, int doorStart, int doorEnd) {
            if (location <= bottomLeft.x || location >= topRight.x) {
                throw new System.Exception($"Vertical split is outside of bounds. Bound: ({bottomLeft.x}, {topRight.x}). Split location: {location}");
            }

            subRoom1 = new(new Vector2Int(bottomLeft.x, bottomLeft.y), new Vector2Int(location - 1, topRight.y));
            subRoom2 = new(new Vector2Int(location + 1, bottomLeft.y), new Vector2Int(topRight.x, topRight.y));

            splitType = RoomSplit.Vertical;
            splitLocation = location;
            this.doorStart = doorStart;
            this.doorEnd = doorEnd;

            return (subRoom1, subRoom2);
        }
    }

    private enum RoomSplit {
        None, Horizontal, Vertical
    }
}
