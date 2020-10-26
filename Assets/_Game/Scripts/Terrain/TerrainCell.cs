using UnityEngine;
public class TerrainCell
{
    private int id;
    public int ID { get => id; }

    private Vector3 position;
    public Vector3 Position { get => position; }

    private bool available;
    public bool Available { get => available; }

    private int ownerId;
    public int OwnerId 
    { 
        get => ownerId; 
        set { available = false; ownerId = value; }
    }

    public TerrainCell(int id, Vector3 position, bool available = true, int ownerId = -1)
    {
        this.id = id;
        this.position = position;
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
}
