using UnityEngine;

public class Ant : MonoBehaviour
{
    int ageRate = 1;
    int mulchHeal = 20;

    int maxHealth = 50;
    public int health = 50;

    public Ant other;


    public void Step() 
    {
        health += -ageRate;
    }

    public void Eat() 
    {
        
    }

    public void Share() 
    {
        if (health > 10 && other.health < maxHealth - 10) { 
            other.health += 10;
            health += -10;
        }
    }

    public void Dig() 
    {
        
    }

    public void Forwards() 
    {
    
    }

    public void TurnLeft() 
    {
    
    }

    public void TurnRight() 
    {
        
    }

    public Vector3Int Pos() 
    {
        return new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
    }
}
