using System;
using System.Collections.Generic;
using UnityEngine;
using Antymology.Terrain;

public class PheromoneManager : Singleton<PheromoneManager>, Tickable
{
    /// <summary>
    /// Determines how fast pheromones evaporate and diffuse
    /// </summary>
    public PheromoneProperties[] properties;

    private AbstractBlock[,,] blocks;


    void Start()
    {
        TimeManager.Instance.tickables.Add(this);
    }

    public void Tick() 
    {
        for (int pheromone = 0; pheromone < (int)PheromoneType.size; pheromone++) {

            foreach (AbstractBlock active in blocks)
            {
                if (active is AirBlock)
                {
                    if (!((AirBlock)active).pheromoneDeposits.ContainsKey(-pheromone))
                        ((AirBlock)active).pheromoneDeposits[-pheromone] = 0;
                }
            }

            Diffuse(pheromone, 0, 0, 1);
            Diffuse(pheromone, 0, 0, -1);
            Diffuse(pheromone, 0, 1, 0);
            Diffuse(pheromone, 0, -1, 0);
            Diffuse(pheromone, 1, 0, 0);
            Diffuse(pheromone, -1, 0, 0);

            foreach (AbstractBlock active in blocks)
            {
                if (active is AirBlock)
                {
                    ((AirBlock)active).pheromoneDeposits[-pheromone] += 
                        ((AirBlock)active).pheromoneDeposits[pheromone] * 
                        (1f - properties[pheromone].evaporationPercent);
                }
            }

            foreach (AbstractBlock active in blocks)
            {
                if (active is AirBlock)
                {
                    ((AirBlock)active).pheromoneDeposits[pheromone] = ((AirBlock)active).pheromoneDeposits[-pheromone];
                }
            }

            foreach (AbstractBlock active in blocks)
            {
                if (active is AirBlock)
                {
                    ((AirBlock)active).pheromoneDeposits[-pheromone] = 0;
                }
            }
        }
    }

    ///diffuses a pheremone along the specified direction
    private void Diffuse(int pheromone, int dx, int dy, int dz) 
    {
        for (int x = 1; x < blocks.GetLength(0) - 1; x++) //edges do not get pheremones
        {
            for (int y = 1; y < blocks.GetLength(0) - 1; y++)
            {
                for (int z = 1; z < blocks.GetLength(0) - 1; z++)
                {
                    if (blocks[x, y, z] is AirBlock && blocks[x + dx, y + dy, z + dy] is AirBlock)
                    {
                        ((AirBlock)blocks[x, y, z]).pheromoneDeposits[-pheromone] =
                            ((AirBlock)blocks[x + dx, y + dy, z + dy]).pheromoneDeposits[pheromone] *
                            properties[pheromone].diffusionPercent;
                    }
                }
            }
        }
    }
}

public enum PheromoneType 
{
    ant = 0,  //released by all ants
    nest = 1, //released by nest blocks (not implemented)
    red = 2,  //released by ant brain
    blue = 3, //released by ant brain
    size = 4,
}

[System.Serializable]
public class PheromoneProperties 
{
    public float evaporationPercent;
    public float diffusionPercent;
}