using System.Collections.Generic;
using UnityEngine;

public class ManagerStats : MonoBehaviour
{
    [SerializeField] private Dictionary<string, int> stats = new Dictionary<string, int>{
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

    public void AlterStat(string statName, int amount){
        stats[statName] = stats[statName] + amount;
    }

    public int GetStat(string, statName){
        return stats[statName]
    }
}
