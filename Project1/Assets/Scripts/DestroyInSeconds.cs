using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInSeconds : MonoBehaviour {

    [SerializeField]
    float lifetimeInSeconds = 1;
    float destroyTime;
	// Use this for initialization
	void Start () {
        destroyTime = Time.time + lifetimeInSeconds;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time > destroyTime)
        {
            Destroy(gameObject);
        }

	}
}
