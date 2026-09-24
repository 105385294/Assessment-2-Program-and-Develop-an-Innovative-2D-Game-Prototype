using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField] private int population = 0;
    [SerializeField] private ManagerComplete complete;

    public void Awake()
    {
        stats["Running Cost"] = population;
        complete = GetComponent<ManagerComplete>();
    }

    public void ChangeStatByBuilding(string buildingType, int posNeg)
    {
        print("Attempted stat change");
        //Pick which stats should be changed based on the type of building and whether it's being built or demolished.
        switch (buildingType)
        {
            case "Apartment":
                AlterStat("Running Cost", 2000 * posNeg);
                AlterStat("Housed", 50 * posNeg);
                AlterStat("Electricity Consumption", 2000 * posNeg);
                break;
            case "Warehouse":
                AlterStat("Employed", 15 * posNeg);
                AlterStat("Electricity Production", 10000 * posNeg);
                AlterStat("Running Cost", 1000 * posNeg);
                AlterStat("Income", 500 * posNeg);
                break;
            case "Office":
                AlterStat("Employed", 30 * posNeg);
                AlterStat("Income", 1000 * posNeg);
                AlterStat("Running Cost", 1500 * posNeg);
                break;
            case "Cafe":
                AlterStat("Fed", 60 * posNeg);
                AlterStat("Employed", 20 * posNeg);
                AlterStat("Running Cost", 750 * posNeg);
                AlterStat("Income", 750 * posNeg);
                break;
            case "Library":
                AlterStat("Mental Health", 45 * posNeg);
                AlterStat("Employed", 10 * posNeg);
                AlterStat("Running Cost", 3000 * posNeg);
                break;
            case "Park":
                print("Park selected");
                AlterStat("Running Cost", 4500 * posNeg);
                AlterStat("Employed", 10 * posNeg);
                AlterStat("Nature", 50 * posNeg);
                AlterStat("Mental Health", 20 * posNeg);
                break;
            default:
                print("No building type found.");
                break;
        }
        complete.CheckComplete();
    }

    private void AlterStat(string statName, int amount)
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
