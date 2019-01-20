using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infector : MonoBehaviour {

    public float timeBetweenInfections = 40;
    public float timeBeforeFirstInfection = 20;
    float timeTillNextInfection;

	// Use this for initialization
	void Start () {
        timeTillNextInfection = Time.time + timeBeforeFirstInfection;
	}
	
	// Update is called once per frame
	void Update () {
        if(Time.time > timeTillNextInfection)
        {
            timeTillNextInfection = Time.time + timeBetweenInfections;
            Player playerScript = GameObject.Find("Player").GetComponent<Player>();
            MechPiece[] mechPieces = playerScript.mechPieces;
            InfectRandomMechPiece(mechPieces);
        }
    }


    void InfectRandomMechPiece(MechPiece[] mechPieces)
    {
        bool allInfected = true;
        foreach (MechPiece piece in mechPieces)
        {
            if (!piece.isInfected)
            {
                allInfected = false;
                break;
            }
        }

        if (!allInfected)
        {
            bool infectedOne = false;

            while (!infectedOne)
            {
                MechPiece randomPiece = mechPieces[Random.Range(0, mechPieces.Length)];
                if (!randomPiece.isInfected)
                {
                    randomPiece.Infect();
                    infectedOne = true;
                }
            }
        }
    }
}
