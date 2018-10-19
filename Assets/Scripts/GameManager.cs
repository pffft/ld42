using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World;

public class GameManager : MonoBehaviour {

    public static BossController Boss 
    {
        get; set;
    }

    public static GameObject Player 
    {
        get; set;
    }

    public static Arena Arena 
    {
        get; set;
    }

    public static GameObject HeldShield 
    {
        get; set;
    }

    public static GameObject PlacedShield
    {
        get; set;
    }

    public static GameObject ThrownShield 
    {
        get; set;
    }

    public static HUD HUD 
    {
        get; set;
    }

    public static AI.EventQueue EventQueue 
    {
        get; set;
    }

	// Use this for initialization
	void Awake ()
    {
        Arena = GameObject.Find("Arena").GetComponent<Arena>();
        Debug.Log("Is arena null? " + (Arena == null));
        Boss = GameObject.Find("Boss").GetComponent<BossController>();
        Player = GameObject.Find("Player");
        HeldShield = GameObject.Find("Held Shield");
        PlacedShield = GameObject.Find("Placed Shield");
        ThrownShield = GameObject.Find("Thrown Shield");
        HUD = GameObject.Find("HUD").GetComponent<HUD>();
        EventQueue = GameObject.Find("Event Queue").GetComponent<AI.EventQueue>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
