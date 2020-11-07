using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TerrainLayerData", menuName = "ScriptableObjects/TerrainLayer", order = 1)]
public class TerrainLayer : ScriptableObject
{
    #region Variables
    [Tooltip("This layer's Tilemap")]
    public Tilemap tilemap;
    [Tooltip("This layer's tile")]
    public TileBase[] tile;
    public float[] spawnPercentage;
    #endregion

    #region Methods
    public TileBase SortTile()
    {
        int randomIndex = Random.Range(0, tile.Length);
        return tile[randomIndex];
    }
    #endregion
}