using System.Collections.Generic;
using UnityEngine;

public class ManagerStats : MonoBehaviour
{
    //Dictionary containing all of the stats used in the game.
    private Dictionary<string, int> stats = new Dictionary<string, int>{
        {"Running Cost", 0},
        {"Income", 0},
        {"Population", 0},
        {"Housed", 0},
        {"Employed", 0},
        {"Fed", 0},
        {"Mental Health", 0},
        {"Electricity Consumption", 0},
        {"Electricity Production", 0},
        {"Nature", 0}
    };

    public void AlterStat(string statName, int amount)
    {
        //Adds the given value to the given stat.
        stats[statName] = stats[statName] + amount;
    }

    public int GetStat(string statName)
    {
        //Returns the value of the given stat.
        return stats[statName];
    }
}
