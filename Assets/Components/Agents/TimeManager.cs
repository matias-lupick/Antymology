using UnityEngine;
using System.Collections.Generic;

public class TimeManager : Singleton<TimeManager>
{
    /// <summary>
    /// Increases ticks per update until frame rate matches value
    /// </summary>
    public double secondsPerFrame = 1f;
    public int ticksPerFrame = 1;
    public bool changeTickRate = false;
    public bool go = false;

    public List<Tickable> tickables = new List<Tickable>();

    // Update is called once per frame
    void Update()
    {
        double time1 = Time.realtimeSinceStartupAsDouble;
        double time2;

        if (go)
        {
            for (int i = 0; i < ticksPerFrame; i++)
            {
                foreach (Tickable active in tickables)
                {
                    active.Tick();
                }
            }

            time2 = Time.realtimeSinceStartupAsDouble;

            if (changeTickRate)
            {
                if (time2 - time1 > secondsPerFrame)
                {
                    ticksPerFrame += 1;
                }
                else
                {
                    ticksPerFrame += -1;
                }
            }
        }
    }
}

/// <summary>
/// Implementing this interface allows for
/// </summary>
public interface Tickable 
{
    public void Tick();
}