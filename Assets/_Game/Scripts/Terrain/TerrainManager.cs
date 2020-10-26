using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private bool debug = true;
    [SerializeField] private float mapWidth = 80;
    [SerializeField] private float mapHeight = 80;
    [SerializeField, Min(1)] private float cellSize = 10;

    private List<TerrainCell> cells = new List<TerrainCell>(); 
    private int numberOfCellsX;
    private int numberOfCellsY;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        BuildTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Methods
    void BuildTerrain()
    {
        numberOfCellsX = Mathf.FloorToInt(mapWidth/cellSize);
        numberOfCellsY = Mathf.FloorToInt(mapHeight/cellSize);

        for(int i = 0; i < numberOfCellsX; i++)
        {
            for(int j = 0; j < numberOfCellsY; j++)
            {
                int cellID = i+j;
                Vector2 cellPosition = new Vector2(transform.position.x - (mapWidth/2) + (cellSize/2) + (cellSize*i), 
                    transform.position.y - (mapHeight/2) + (cellSize/2) + (cellSize*j));

                TerrainCell newCell = new TerrainCell(cellID, cellPosition);
                cells.Add(newCell);
            }
        }
    }

    public Vector3 RandomCellPosition(int newOwnerId)
    {
        int randomCell = 0;

        //TO DO: Change while for a list of available cells
        do 
        {
            randomCell = Random.Range(0, cells.Count);
        } while(cells[randomCell].Available == false);

        TerrainCell cell = cells[randomCell];
        cell.OwnerId = newOwnerId;
        return cell.Position;
    }
    #endregion

    #region Unity Editor
    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if(!debug) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(mapWidth, mapHeight));

        int _numberOfCellsX = Mathf.FloorToInt(mapWidth/cellSize);
        int _numberOfCellsY = Mathf.FloorToInt(mapHeight/cellSize);
        for(int i = 0; i < _numberOfCellsX; i++)
        {
            for(int j = 0; j < _numberOfCellsY; j++)
            {
                Vector2 cellPosition = new Vector2(transform.position.x - (mapWidth/2) + (cellSize/2) + (cellSize*i), 
                    transform.position.y - (mapHeight/2) + (cellSize/2) + (cellSize*j));

                Gizmos.DrawWireCube(cellPosition, new Vector3(cellSize, cellSize));
            }
        }
    }
    #endif
    #endregion
}
