using UnityEngine;
using Antymology.Terrain;

public class Ant : MonoBehaviour, Tickable
{
    int ageRate = 1;
    int mulchHeal = 20;

    int maxHealth = 50;
    public int health = 50;

    public bool isQueen = false;

    public Ant other; //a random other ant on the same tile
    public AirBlock airBlock;

    public Brain brain = new Brain();

    int facingX = 1;
    int facingZ = 0;


    public enum Action 
    {
        eat,
        dig,
        share,
        forwards,
        turnRight,
        tunrLeft,
        makeNest,
    }


    void Start()
    {
        TimeManager.Instance.tickables.Add(this);
    }

    /// <summary>
    /// Makes this ant a queen ant
    /// </summary>
    public void MakeQueen() 
    {
        isQueen = true;
        transform.GetChild(0).transform.localScale = Vector3.one * 2;
    }

    public void Tick() 
    {
        Action action;

        health += -ageRate;

        if (WorldManager.Instance.GetBlock(Pos()) is AcidicBlock)
            health += -ageRate;

        action = brain.GetAction(this);
    }


    protected void Eat() 
    {
        if (other == null)
        {
            if (WorldManager.Instance.GetBlock(Pos()) is MulchBlock)
            {
                health += mulchHeal;

                if (health > maxHealth)
                    health = maxHealth;

                WorldManager.Instance.SetBlock(Pos(), new AirBlock());
                transform.position += Vector3.down;
            }
        }
    }

    protected void Share() 
    {
        if (other != null)
        {
            if (health > 10 && other.health < maxHealth - 10)
            {
                other.health += 10;
                health += -10;
            }
        }
    }

    protected void Dig() 
    {
        if (other == null)
        {
            if (WorldManager.Instance.GetBlock(Pos()) is not ContainerBlock)
            {
                WorldManager.Instance.SetBlock(Pos(), new AirBlock());
                transform.position += Vector3.down;
            }
        }
    }

    protected void Forwards() 
    {
        for (int y = 1; y >= -1; y--) 
        {
            if (WorldManager.Instance.GetBlock(Pos() + Vector3Int.up * (y + 1)) is AirBlock &&
                WorldManager.Instance.GetBlock(Pos() + Vector3Int.up * y) is not AirBlock)
            {
                transform.position += new Vector3(facingX, y, facingZ);
            }
        }
    }

    protected void TurnLeft() 
    {
        int v = facingZ;
        facingZ = facingX;
        facingX = -v;
    }

    protected void TurnRight() 
    {
        int v = facingZ;
        facingZ = -facingX;
        facingX = v;
    }

    protected void MakeNest() 
    {
        if (isQueen) 
        {
            if (health > maxHealth / 3 && WorldManager.Instance.GetBlock(Pos()) is not ContainerBlock) {
                health += -maxHealth / 3;
                WorldManager.Instance.SetBlock(Pos(), new NestBlock());
            }
        }
    }

    public Vector3Int Pos() 
    {
        return new Vector3Int((int)transform.position.x, (int)transform.position.y - 1, (int)transform.position.z);
    }


    void OnDestroy() 
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.tickables.Remove(this);
    }
}
