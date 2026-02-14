using UnityEngine;
using Antymology.Terrain;

public class Ant : MonoBehaviour, Tickable
{
    int ageRate = 1;
    int mulchHeal = 20;

    public int maxHealth = 50;
    public int health = 50;

    public bool isQueen = false;

    public Ant other; //a random other ant on the same tile
    public AirBlock airBlock;
    public AbstractBlock groundBlock;

    public Brain brain;

    int facingX = 1;
    int facingZ = 0;


    public enum Action 
    {
        none,
        eat,
        dig,
        share,
        forwards,
        turnRight,
        tunrLeft,
        makeNest,
        size = 8,
    }


    void Awake()
    {
        TimeManager.Instance.tickables.Add(this);
        AntManager.Instance.ants.Add(this);
        //brain = new Brain();
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

        switch (action)
        {
            case Action.none:
                break;
            case Action.eat:
                Eat();
                break;
            case Action.dig:
                Dig();
                break;
            case Action.share:
                Share();
                break;
            case Action.forwards:
                Forwards();
                break;
            case Action.turnRight:
                TurnRight();
                break;
            case Action.tunrLeft:
                TurnLeft();
                break;
            case Action.makeNest:
                MakeNest();
                break;
            default:
                break;
        }

        if (health < 0)
            Destroy(gameObject);
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

                AntManager.Instance.eaten += 1;
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
            if (WorldManager.Instance.GetBlock(Pos()) is MulchBlock)
            {
                Eat();
                return;
            }

            if (WorldManager.Instance.GetBlock(Pos()) is not ContainerBlock)
            {
                WorldManager.Instance.SetBlock(Pos(), new AirBlock());
                transform.position += Vector3.down;

                if (groundBlock is NestBlock)
                    AntManager.Instance.nests += -1;
            }
        }
    }

    protected void Forwards() 
    {
        Vector3Int pos = Pos() + new Vector3Int(facingX, 0, facingZ);

        for (int y = 1; y >= -1; y--) 
        {
            if (WorldManager.Instance.GetBlock(pos + Vector3Int.up * (y + 1)) is AirBlock &&
                WorldManager.Instance.GetBlock(pos + Vector3Int.up * y) is not AirBlock)
            {
                transform.position += new Vector3Int(facingX, y, facingZ);
            }
        }
    }

    public bool IsCliff() 
    {
        Vector3Int pos = Pos() + new Vector3Int(facingX, 0, facingZ);

        for (int y = 1; y >= -1; y--)
        {
            if (WorldManager.Instance.GetBlock(pos + Vector3Int.up * (y + 1)) is AirBlock &&
                WorldManager.Instance.GetBlock(pos + Vector3Int.up * y) is not AirBlock)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsOpen() 
    {
        Vector3Int pos = Pos() + new Vector3Int(facingX, 0, facingZ);

        return WorldManager.Instance.GetBlock(pos + Vector3Int.up) is not AirBlock;
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

                if (groundBlock is not NestBlock)
                    AntManager.Instance.nests += 1;
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

        if (TimeManager.Instance != null)
            AntManager.Instance.ants.Remove(this);
    }
}
