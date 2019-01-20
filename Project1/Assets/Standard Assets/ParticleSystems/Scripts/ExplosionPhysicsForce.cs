using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Effects
{
    public class ExplosionPhysicsForce : MonoBehaviour
    {
        public float explosionForce = 4;
        public float blastRadius;
        public AudioClip explosionSound;
        AudioSource source;

        private void Start()
        {
            source = GetComponent<AudioSource>();
        }

        void SetExplosiveForce(float explosionForce)
        {
            this.explosionForce = explosionForce;
        }

        void SetExplosiveRadius(float explosionRadius)
        {
            this.blastRadius = explosionRadius;
        }


        public IEnumerator Blast()
        {
            // wait one frame because some explosions instantiate debris which should then
            // be pushed by physics force
            yield return null;

            source.PlayOneShot(explosionSound);

            float multiplier = GetComponent<ParticleSystemMultiplier>().multiplier;

            float r = blastRadius;
            var cols = Physics.OverlapSphere(transform.position, r);
            var rigidbodies = new List<Rigidbody>();
            foreach (var col in cols)
            {
                if (col.tag == "Block" && col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody))
                {
                    rigidbodies.Add(col.attachedRigidbody);
                }
            }
            

            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(explosionForce*multiplier, transform.position, r, 1*multiplier, ForceMode.Impulse);
            }
        }
    }
}
