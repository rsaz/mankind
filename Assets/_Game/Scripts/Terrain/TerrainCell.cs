using UnityEngine.Tilemaps;
using UnityEngine;
public class TerrainCell
{
    private int id;
    public int ID { get => id; }

    private Vector3 center;
    public Vector3 Center { get => center; }

    private int[,] map;
    public int[,] Map { get => map; }

    private bool available;
    public bool Available { get => available; }

    private int ownerId;
    public int OwnerId 
    { 
        get => ownerId; 
        set { available = false; ownerId = value; }
    }

    public TerrainCell(int id, Vector3 center, int cellSize, bool available = true, int ownerId = -1)
    {
        this.id = id;
        this.center = center;
        this.map = GenerateArray(cellSize, cellSize, false);
        this.available = available;
        this.ownerId = ownerId;
    }

    public bool BuyTerrain(int ownerId)
    {
        if(!available) return false;

        this.ownerId = ownerId;
        return true;
    }

    public void SellTerrain()
    {
        this.ownerId = -1;
        available = true;
    }

    private int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (empty)
                {
                    map[x, y] = 0;
                }
                else
                {
                    map[x, y] = 1;
                }
            }
        }
        return map;
    }

    public void RenderMap(Tilemap tilemap, TileBase tile)
    {
        Debug.Log(map.GetUpperBound(0) + " | " + map.GetUpperBound(1));
        //Loop through the width of the map
        for (int x = 0; x < map.GetUpperBound(0) ; x++) 
        {
            //Loop through the height of the map
            for (int y = 0; y < map.GetUpperBound(1); y++) 
            {
                // 1 = tile, 0 = no tile
                if (map[x, y] == 1) 
                {
                    tilemap.SetTile(new Vector3Int(Mathf.FloorToInt(center.x) - (10/2) + x, 
                                            Mathf.FloorToInt(center.y) - (10/2) + y, 0), tile); 
                }
            }
        }
    }
}
