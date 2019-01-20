using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRocketLauncher : MonoBehaviour {

    [Space]
    [Header("Shooting")]
    [SerializeField]
    Transform bulletSpawnPoint;
    [SerializeField]
    GameObject rocketPrefab;
    [SerializeField]
    float rocketSpeed;

    GameObject player;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShootAtPlayer()
    {
        GameObject rocketInstance = Instantiate(rocketPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Vector3 vectorTowardPlayer = (player.transform.position - rocketInstance.transform.position).normalized;
        rocketInstance.GetComponent<Rigidbody>().AddForce(vectorTowardPlayer * rocketSpeed, ForceMode.Impulse);

    }
}
