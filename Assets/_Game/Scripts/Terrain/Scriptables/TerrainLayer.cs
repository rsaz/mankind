using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TerrainLayerData", menuName = "ScriptableObjects/TerrainLayer", order = 1)]
public class TerrainLayer : ScriptableObject
{
    #region Variables
    [Tooltip("This layer's tile")] public TileBase[] tile;
    [Tooltip("Percentage of each individual tile"), Range(0,1)] public float[] spawnPercentage;
    [Tooltip("Specify how often it has to draw a tile"), Range(0,1)] public float densityFactor = 0.3f;
    #endregion

    #region Methods
    public TileBase SortTile()
    {
        int randomIndex = Random.Range(0, tile.Length);
        return tile[randomIndex];
    }

    public void ProceduralGeneration(Tilemap tilemap, int width, int height, Tilemap[] disabledArea = null)
    {
        for (int x = 0; x <= width ; x++) 
        {
            for (int y = 0; y <= height; y++) 
            {
                Vector3Int coordinate = new Vector3Int(Mathf.FloorToInt(tilemap.transform.position.x) - ((width+1)/2) + x, 
                        Mathf.FloorToInt(tilemap.transform.position.y) - ((height+1)/2) + y, 0);

                TileBase placedTile = tilemap.GetTile(coordinate);
                TileBase disabledTile = null;

                if(disabledArea != null)
                {
                    foreach (Tilemap map in disabledArea)
                    {
                        disabledTile = map.GetTile(coordinate);
                        if(disabledTile) break;
                    }
                }

                if(Random.Range(0f, 1f) < densityFactor && !placedTile && !disabledTile) 
                    tilemap.SetTile(new Vector3Int(Mathf.FloorToInt(tilemap.transform.position.x) - ((width+1)/2) + x, 
                        Mathf.FloorToInt(tilemap.transform.position.y) - ((height+1)/2) + y, 0), tile[Random.Range(0, tile.Length)]); 
            }
        }
    }
    #endregion
}