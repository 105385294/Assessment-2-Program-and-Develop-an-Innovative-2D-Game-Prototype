using Unity.VisualScripting;
using UnityEngine;

public class ManagerComplete : MonoBehaviour
{
    //Values can be edited in the inspector. Values set to -1 will be ignored during completion check.
    [SerializeField] private int target_cost = -1;
    [SerializeField] private int target_income = -1;
    [SerializeField] private int target_housed = -1;
    [SerializeField] private int target_employed = -1;
    [SerializeField] private int target_fed = -1;
    [SerializeField] private int target_health = -1;
    [SerializeField] private int target_e_prod = -1;
    [SerializeField] private int target_e_cons = -1;
    [SerializeField] private int target_nature = -1;
    
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

        if (target_cost != -1)
        {
            int running_cost = stats.GetStat("Running Cost");
            if (running_cost > target_cost)
            {
                complete = false;
            }
        }

        if (target_income != -1)
        {
            int income = stats.GetStat("Income");
            if (income < target_income)
            {
                complete = false;
            }
        }

        if (target_housed != -1)
        {
            int housed = stats.GetStat("Housed");
            if (housed < target_housed)
            {
                complete = false;
            }
        }

        if (target_employed != -1)
        {
            int employed = stats.GetStat("Employed");
            if (employed < target_employed)
            {
                complete = false;
            }
        }

        if (target_fed != -1)
        {
            int fed = stats.GetStat("Fed");
            if (fed < target_fed)
            {
                complete = false;
            }
        }

        if (target_health != -1)
        {
            int health = stats.GetStat("Mental Health");
            if (health > target_health)
            {
                complete = false;
            }
        }

        if (target_e_prod != -1)
        {
            int e_prod = stats.GetStat("Electricity Production");
            if (e_prod < target_e_prod)
            {
                complete = false;
            }
        }

        if (target_e_cons != -1)
        {
            int e_cons = stats.GetStat("Electricity Consumption");
            if (e_cons > target_e_cons)
            {
                complete = false;
            }
        }

        if (target_nature != -1)
        {
            int nature = stats.GetStat("Nature");
            if (nature < target_nature)
            {
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
