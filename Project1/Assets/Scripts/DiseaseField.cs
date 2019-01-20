using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseField : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Player playerScript = other.GetComponent<Player>();
            MechPiece[] mechPieces = playerScript.mechPieces;
            InfectRandomMechPiece(mechPieces);
        }
    }

    void InfectRandomMechPiece(MechPiece[] mechPieces)
    {
        bool allInfected = true;
        foreach(MechPiece piece in mechPieces)
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
