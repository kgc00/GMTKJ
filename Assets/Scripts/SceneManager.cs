using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Just a bunch of simple functions to allow the user to interact with the simulation.
public class SceneManager : MonoBehaviour
{
    public Text movementText;
    public static SceneManager instance;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        UpdateText(6);
    }

    public void UpdateText(int mpValue)
    {
        //movementText.text = ("Movement Points Left This Turn: " + mpValue);
        print("Movement Points Left This Turn: " + mpValue);
    }

    public void UpdateTextEndTurn()
    {
        if (GetActorStatus())
        {
            movementText.text = ("No movement points left!  Click end turn!");
        }
    }

    public void EndTurn()
    {
        if (GetActorStatus())
        {
            //aStar.NextTurn();
        }
    }

    public void DisplayMoves()
    {
        if (GetActorStatus())
        {
            //aStar.DisplayMoves();
        }
    }

    public bool GetActorStatus()
    {
        // if (aStar.CurrentUnitState == AStar.UnitState.idle)
        // {
        //     return true;
        // }
        // else
        // {
        //     return false;
        // }
        return true;
    }
}

