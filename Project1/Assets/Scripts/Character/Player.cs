using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    
    public MechPiece[] mechPieces;

    public MechPiece corePiece;

    bool isDead = false;

    GameObject deadText;

    // Use this for initialization
    void Start () {
        deadText = GameObject.Find("DeadText");
        deadText.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if(corePiece.currentHealth <= 0)
        {
            Die();
        }

        if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
	}

    void OnHitByEnemyRocket(float damage)
    {
        TakeDamage(damage);
    }

    void TakeDamage(float damage)
    {
        MechPiece pieceToDamage = GetRandomMechPiece();
        if (pieceToDamage != null)
        {
            pieceToDamage.TakeDamage(damage);
        }
    }

    MechPiece GetRandomMechPiece()
    {
        bool allDead = true;
        foreach (MechPiece piece in mechPieces)
        {
            if (piece.currentHealth > 0)
            {
                allDead = false;
                break;
            }
        }

        if (!allDead)
        {
            bool damagedOne = false;

            while (!damagedOne)
            {
                MechPiece randomPiece = mechPieces[Random.Range(0, mechPieces.Length)];
                if (randomPiece.currentHealth > 0)
                {
                    return randomPiece;
                }
            }
        }

        return null;
    }

    void Die()
    {
        isDead = true;
        deadText.SetActive(true);
        Time.timeScale = 0;
    }

}
