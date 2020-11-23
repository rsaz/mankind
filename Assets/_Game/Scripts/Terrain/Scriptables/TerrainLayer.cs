using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TerrainLayerData", menuName = "ScriptableObjects/TerrainLayer", order = 1)]
public class TerrainLayer : ScriptableObject
{
    #region Variables
    [Tooltip("This layer's tile")] public TileBase[] tiles;
    [Tooltip("Specify how often it has to draw a tile"), Range(0,1)] public float densityFactor = 0.3f;

    private TileSorting sortTile = new TileSorting();
    #endregion

    #region Methods
    public void ProceduralGeneration(Tilemap tilemap, int width, int height, TileSortingMethod method = TileSortingMethod.Random, Tilemap[] disabledArea = null)
    {
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

                TileBase nextTile = sortTile.Generate(tiles, method);

                if(densityFactor == 1 || Random.Range(0f, 1f) < densityFactor) 
                    tilemap.SetTile(coordinate, nextTile); 
            }
        }
    }

    public void DrawOutsideBoundaries(Tilemap tilemap, int width, int height, int offsetWidth, int offsetHeight, TileSortingMethod method = TileSortingMethod.Random)
    {
        int newWidth = width+(offsetWidth*2);
        int newHeight = height+(offsetHeight*2);

        for (int y = 0; y <= newWidth; y++) 
        {
            for (int x = 0; x <= newHeight; x++) 
            {
                if(y >= offsetHeight && y < height + offsetHeight && x == offsetWidth) x = width+offsetWidth;
                    
                Vector3Int coordinate = new Vector3Int(Mathf.FloorToInt(tilemap.transform.position.x) - ((newWidth+1)/2) + x, 
                        Mathf.FloorToInt(tilemap.transform.position.y) - ((newHeight)/2) + y, 0);

                TileBase nextTile = sortTile.Generate(tiles, method);

                if(densityFactor == 1 || (y == offsetHeight-1 || y == height+offsetHeight) && x > offsetWidth-1 && x < width+offsetWidth || (y > offsetHeight-1 && y < height+offsetHeight && (x == offsetWidth-1 || x == width+offsetWidth)))
                    tilemap.SetTile(coordinate, nextTile);
                else if(Random.Range(0f, 1f) < densityFactor)
                    tilemap.SetTile(coordinate, nextTile);
            }
        }
    }
    #endregion
}