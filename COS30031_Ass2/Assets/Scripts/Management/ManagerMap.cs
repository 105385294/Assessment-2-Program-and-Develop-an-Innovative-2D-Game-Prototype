using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

public class ManagerMap : MonoBehaviour
{
    //A list of all of the plots on the map.
    [SerializeField] private List<GameObject> plots;

    private void Awake()
    {
        //Find the list of plots in the hierarchy and assign them to this objects plot list.
        GameObject plot_list = GameObject.Find("Plots");
        Transform[] plot_children = plot_list.GetComponentsInChildren<Transform>();
        foreach (Transform child in plot_children)
        {
            if (child.CompareTag("Plot"))
            {
                plots.Add(child.gameObject);
            }
        }

        print(CheckPlacement(plots[1], 3, 2));
    }

    public bool CheckPlacement(GameObject chosenTile, int length, int width)
    {
        //Check if the requested building can be placed.
        bool can_place_building = true;
        float tile_x = chosenTile.transform.position.x;
        float tile_y = chosenTile.transform.position.y;

        for (int x=0; x<length; x++)
        {
            float current_x = tile_x - x;
            float current_y = tile_y + (x*0.5f);
            for (int y=0; y<width; y++)
            {
                current_x += y;
                current_y += (y*0.5f);
                
                bool found_matching_tile = false;
                foreach (GameObject plot in plots)
                {
                    float plot_x = plot.transform.position.x;
                    float plot_y = plot.transform.position.y;
                    if (plot_x == current_x && plot_y == current_y)
                    {
                        found_matching_tile = true;
                    }
                }

                if (!found_matching_tile)
                {
                    can_place_building = false;
                }
            }
        }
        return can_place_building;
    }
}
