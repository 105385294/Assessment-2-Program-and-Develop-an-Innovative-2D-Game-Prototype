using UnityEngine;
using UnityEngine.UI;
using System;

public class UpdateText : MonoBehaviour
{
    public Text text;
    public ManagerStats stats;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame
    
    void Update()
    {
        string runningCost = stats.GetStat("Running Cost").ToString();
        string income = stats.GetStat("Income").ToString();
        string population = stats.GetStat("Population").ToString();
        string housed = stats.GetStat("Housed").ToString();
        string employed = stats.GetStat("Employed").ToString();
        string fed = stats.GetStat("Fed").ToString();
        string mentalHealth = stats.GetStat("Mental Health").ToString();
        string elecConsumption = stats.GetStat("Electricity Consumption").ToString();
        string elecProduction = stats.GetStat("Electricity Production").ToString();
        string nature = stats.GetStat("Nature").ToString();
        
        text.text = runningCost + "\n" + income + "\n" + population + "\n" + housed + "\n" + employed + "\n" + fed + "\n" + mentalHealth + "\n" + elecConsumption + "\n" + elecProduction + "\n" + nature ;
    }
}
