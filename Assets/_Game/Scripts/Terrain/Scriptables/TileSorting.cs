using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileSortingMethod {
    Random,
    Sequential
}
public class TileSorting
{
    int tempIndex = 0;
    // Update is called once per frame
    public TileBase Generate(TileBase[] tiles, TileSortingMethod method)
    {
        TileBase generatedTile;
        switch (method)
        {
            case TileSortingMethod.Random:
                generatedTile = RandomTile(tiles);
                break;
            case TileSortingMethod.Sequential:
                generatedTile = SequentialTile(ref tempIndex, tiles);
                break;
            default:
                generatedTile = RandomTile(tiles);
                break;
        }

        return generatedTile;
    }

    public TileBase RandomTile(TileBase[] tiles = null)
    {
        int randomIndex = Random.Range(0, tiles.Length);

        return tiles[randomIndex];
    }

    public TileBase SequentialTile(ref int tileIndex, TileBase[] tiles = null)
    {
        TileBase nextTile = tiles[tileIndex];
        tileIndex = tileIndex == tiles.Length-1 ? 0 : tileIndex+1;

        return nextTile;
    }
}
