using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeShot : MonoBehaviour {

    float damage;
    float explosiveForce;
    float sizeMultiplier;
    public float explosionRadius;

    [SerializeField]
    float minForceToExplode;

    [SerializeField]
    GameObject explosionParticleSystem;

    public bool isActive = false;
    public float lifeTime;
    float deathTime;
    

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (isActive)
        {
            if (Time.time > deathTime)
            {
                Destroy(gameObject);
            }
        }
	}
    private void OnTriggerEnter(Collider other)
    {
        if (isActive)
        {

            other.SendMessage("OnHitByChargeShot", this, SendMessageOptions.DontRequireReceiver);
            
            if(explosiveForce > minForceToExplode)
            {
                float r = explosionRadius;
                var cols = Physics.OverlapSphere(transform.position, r);
                foreach (var col in cols)
                {
                    if (col.tag == "Enemy")
                    {
                        col.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
                    }
                }

                explosionParticleSystem.SendMessage("SetExplosiveForce", explosiveForce);
                explosionParticleSystem.SendMessage("SetExplosiveRadius", explosionRadius);
                explosionParticleSystem.SendMessage("PlayParticle");
            }

            isActive = false;
            GetComponent<Renderer>().enabled = false;
        }
    }

    public void Activate(float bulletDamage, float explosiveForce, float sizeMultiplier)
    {
        isActive = true;
        deathTime = Time.time + lifeTime;
        this.damage = bulletDamage;
        this.explosiveForce = explosiveForce;
        this.sizeMultiplier = sizeMultiplier;
    }
}
