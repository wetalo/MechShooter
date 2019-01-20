using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRocket : MonoBehaviour {


    public float damage;
    public float explosiveForce;
    float sizeMultiplier;
    public float explosionRadius;
    

    [SerializeField]
    GameObject explosionParticleSystem;
    
    public float lifeTime;
    float deathTime;

    bool isActive = true;
    // Use this for initialization
    void Start()
    {
        deathTime = Time.time + lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > deathTime)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isActive)
        {
            float r = explosionRadius;
            var cols = Physics.OverlapSphere(transform.position, r);
            foreach (var col in cols)
            {
                if (col.tag == "Player")
                {
                    col.SendMessageUpwards("OnHitByEnemyRocket", this.damage, SendMessageOptions.DontRequireReceiver);
                }
            }

            
            explosionParticleSystem.SendMessage("SetExplosiveForce", explosiveForce);
            explosionParticleSystem.SendMessage("SetExplosiveRadius", explosionRadius);
            explosionParticleSystem.SendMessage("PlayParticle");
        
            GetComponent<Renderer>().enabled = false;
            isActive = false;
        }
    }
}
