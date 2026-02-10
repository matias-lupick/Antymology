using UnityEngine;

public class Ant : MonoBehaviour
{
    int ageRate = 1;

    public int health = 50;



    public void Step() 
    {
        health += -ageRate;
    }
}
