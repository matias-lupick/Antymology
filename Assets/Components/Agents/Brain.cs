using UnityEngine;

public class Brain
{
    public float[] input;
    public float[] output;

    public Ant.Action GetAction(Ant d) 
    {
        int rand = Random.Range(0, 7);

        return (Ant.Action)rand;
    }
}
