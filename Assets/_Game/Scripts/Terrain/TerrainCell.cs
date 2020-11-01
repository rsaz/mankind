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

    private Tilemap tilemap;
	private TileBase tileBuy;
	private TileBase tileSell;

    public TerrainCell(int id, Vector3 center, int cellSize, Tilemap tilemap, TileBase tileBuy, bool available = true, int ownerId = -1)
    {
        this.id = id;
        this.center = center;
        this.map = GenerateArray(cellSize, cellSize);
        this.tilemap = tilemap;
        this.tileBuy = tileBuy;
        this.available = available;
        this.ownerId = ownerId;
    }

    public bool BuyTerrain(int ownerId, Tilemap tilemap = null, TileBase tile = null)
    {
        if(!available) return false;

        this.ownerId = ownerId;
        RenderMap(tilemap ? tilemap : this.tilemap, tile ? tile : this.tileBuy);
        return true;
    }

    public void SellTerrain(Tilemap tilemap = null, TileBase tile = null)
    {
        this.ownerId = -1;
        available = true;
        RenderMap(tilemap ? tilemap : this.tilemap, tile ? tile : this.tileSell);
    }

    private int[,] GenerateArray(int width, int height)
    {
        int[,] map = new int[width, height];
        return map;
    }

    public void RenderMap(Tilemap tilemap, TileBase tile)
    {
        //Loop through the width of the map
        for (int x = 0; x <= map.GetUpperBound(0) ; x++) 
        {
            //Loop through the height of the map
            for (int y = 0; y <= map.GetUpperBound(1); y++) 
            {
                map[x, y] = 1;
                tilemap.SetTile(new Vector3Int(Mathf.FloorToInt(center.x) - ((map.GetUpperBound(0)+1)/2) + x, 
                                        Mathf.FloorToInt(center.y) - ((map.GetUpperBound(0)+1)/2) + y, 0), tile); 
            }
        }
    }
}
