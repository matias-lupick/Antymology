using UnityEngine;
using System.Collections.Generic;
using Antymology.Terrain;


public class AntManager : Singleton<AntManager>, Tickable
{
    public List<Ant> ants = new List<Ant>();

    void Start()
    {
        TimeManager.Instance.tickables.Add(this);
    }

    public void Tick() 
    {
        Dictionary<Vector3Int, List<Ant>> antsByPos = new Dictionary<Vector3Int, List<Ant>>();
        int randomInt;

        //Set other
        foreach (Ant active in ants) 
        {
            antsByPos[active.Pos()] = new List<Ant>();
        }

        foreach (Ant active in ants)
        {
            antsByPos[active.Pos()].Add(active);
        }

        foreach (Ant active in ants)
        {
            if (antsByPos[active.Pos()].Count == 1) 
            {
                active.other = null; 
            }
            else
            {
                randomInt = Random.Range(0, antsByPos[active.Pos()].Count);

                for (int i = 0; i < 2; i++)
                {
                    if (randomInt + i == antsByPos[active.Pos()].Count)
                        randomInt = -1;

                    if (antsByPos[active.Pos()][randomInt + i] != active)
                        active.other = antsByPos[active.Pos()][randomInt + i];
                }
            }
        }

        //Set block
        foreach (Ant active in ants)
        {
            active.airBlock = (AirBlock)WorldManager.Instance.GetBlock(active.Pos() + Vector3Int.up);
        }
    }
}
