using Unity.VisualScripting;
using UnityEngine;

public class ManagerComplete : MonoBehaviour
{
    //Values can be edited in the inspector. Values set to -1 will be ignored during completion check.
    [SerializeField] private int target_housed = -1;
    [SerializeField] private int target_employed = -1;
    [SerializeField] private int target_fed = -1;
    [SerializeField] private int target_health = -1;
    [SerializeField] private int target_nature = -1;
    [SerializeField] private bool check_dollars = false;
    [SerializeField] private bool check_power = false;
    
    private ManagerStats stats;
    private bool level_completed = false;
    private GameObject[] confetti;
    private float time_of_completion;
    private bool celebrated = false;

    private void Awake()
    {
        stats = GetComponent<ManagerStats>();
        confetti = GameObject.FindGameObjectsWithTag("Confetti");
        foreach (GameObject confetto in confetti)
        {
            confetto.SetActive(false);
        }
    }

    private void Update()
    {
        //Checks if enouhg time has passed to disable confetti cannons
        if (!celebrated && level_completed)
        {
            if (Time.time - time_of_completion > 5)
            {
                foreach (GameObject confetto in confetti)
                {
                    confetto.SetActive(false);
                    celebrated = true;
                }
            }
        }
    }

    public void CheckComplete()
    {
        //Checks if all of the requirements for the level have been completed.
        bool complete = true;

        if (check_dollars)
        {
            int running_cost = stats.GetStat("Running Cost");
            int income = stats.GetStat("Income");
            if (running_cost > income || income == 0)
            {
                print("Running cost is greater than income");
                complete = false;
            }
        }

        if (target_housed != -1)
        {
            int housed = stats.GetStat("Housed");
            if (housed < target_housed)
            {
                print("Housed is below threshold");
                complete = false;
            }
        }

        if (target_employed != -1)
        {
            int employed = stats.GetStat("Employed");
            if (employed < target_employed)
            {
                print("Employed is below threshold");
                complete = false;
            }
        }

        if (target_fed != -1)
        {
            int fed = stats.GetStat("Fed");
            if (fed < target_fed)
            {
                print("Fed is below threshold");
                complete = false;
            }
        }

        if (target_health != -1)
        {
            int health = stats.GetStat("Mental Health");
            if (health < target_health)
            {
                print("Mental Health is below threshold");
                complete = false;
            }
        }

        if (check_power)
        {
            int e_prod = stats.GetStat("Electricity Production");
            int e_cons = stats.GetStat("Electricity Consumption");
            if (e_cons > e_prod || e_prod <= 0)
            {
                print("Electricity consumption is greater than electricity production");
                complete = false;
            }
        }

        if (target_nature != -1)
        {
            int nature = stats.GetStat("Nature");
            if (nature < target_nature)
            {
                print("Nature is below threshold");
                complete = false;
            }
        }

        if (complete)
        {
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        print("Level Complete");
        //Fire confetti when level has been completed for the first time.
        if (!level_completed)
        {
            level_completed = true;
            foreach (GameObject confetto in confetti)
            {
                confetto.SetActive(true);
            }

            time_of_completion = Time.time;
        }
    }
}
