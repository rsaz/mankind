using UnityEngine;
using UnityEngine.Tilemaps;

public enum SortingMethod {
    Random,
    Sequential,
}

[CreateAssetMenu(fileName = "TerrainLayerData", menuName = "ScriptableObjects/TerrainLayer", order = 1)]
public class TerrainLayer : ScriptableObject
{
    #region Variables
    [Tooltip("This layer's tile")] public TileBase[] tiles;
    [Tooltip("Specify how often it has to draw a tile"), Range(0,1)] public float densityFactor = 0.3f;

    public TileBase[] tilesOffBoundary;
    #endregion

    #region Methods
    public TileBase RandomTile(TileBase[] tiles = null)
    {
        TileBase[] tilesNotNull = tiles != null ? tiles : this.tiles;
        int randomIndex = Random.Range(0, tilesNotNull.Length);

        return tilesNotNull[randomIndex];
    }

    public TileBase SequentialTile(ref int tileIndex, TileBase[] tiles = null)
    {
        TileBase[] tilesNotNull = tiles != null ? tiles : this.tiles;
        TileBase nextTile = tilesNotNull[tileIndex];
        tileIndex = tileIndex == tilesNotNull.Length-1 ? 0 : tileIndex+1;

        return nextTile;
    }

    public void ProceduralGeneration(Tilemap tilemap, int width, int height, SortingMethod method = SortingMethod.Random, Tilemap[] disabledArea = null)
    {
        int tempIndex = 0;
        for (int x = 0; x <= width; x++) 
        {
            for (int y = 0; y <= height; y++) 
            {
                Vector3Int coordinate = new Vector3Int(Mathf.FloorToInt(tilemap.transform.position.x) - ((width+1)/2) + x, 
                        Mathf.FloorToInt(tilemap.transform.position.y) - ((height+1)/2) + y, 0);

                if(disabledArea != null) {
                    TileBase placedTile = tilemap.GetTile(coordinate);
                    TileBase disabledTile = null;

                    foreach (Tilemap map in disabledArea)
                    {
                        disabledTile = map.GetTile(coordinate);
                        if(disabledTile != null) break;
                    }
                    
                    if(placedTile || disabledTile) break;
                }

                TileBase nextTile;
                switch (method)
                {
                    case SortingMethod.Random:
                        nextTile = RandomTile();
                        break;
                    case SortingMethod.Sequential:
                        nextTile = SequentialTile(ref tempIndex);
                        break;
                    default:
                        nextTile = RandomTile();
                        break;
                }

                if(densityFactor == 1 || Random.Range(0f, 1f) < densityFactor) 
                    tilemap.SetTile(coordinate, nextTile); 
            }
        }
    }

    public void DrawOutsideBoundaries(Tilemap tilemap, int width, int height, int offset, SortingMethod method = SortingMethod.Random)
    {
        int tempIndex = 0;
        int newWidth = width+(offset*2);
        int newHeight = height+(offset*2);

        for (int y = 0; y <= newWidth; y++) 
        {
            for (int x = 0; x <= newHeight; x++) 
            {
                if(y > offset && y < height + offset && x == offset) x = width+offset;
                    
                Vector3Int coordinate = new Vector3Int(Mathf.FloorToInt(tilemap.transform.position.x) - ((newWidth+1)/2) + x, 
                        Mathf.FloorToInt(tilemap.transform.position.y) - ((newHeight)/2) + y, 0);

                TileBase nextTile;
                switch (method)
                {
                    case SortingMethod.Random:
                        nextTile = RandomTile(tilesOffBoundary);
                        break;
                    case SortingMethod.Sequential:
                        nextTile = SequentialTile(ref tempIndex, tilesOffBoundary);
                        break;
                    default:
                        nextTile = RandomTile(tilesOffBoundary);
                        break;
                }

                tilemap.SetTile(coordinate, nextTile);
            }
        }
    }
    #endregion
}