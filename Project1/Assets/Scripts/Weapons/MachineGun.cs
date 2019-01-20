using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : Weapon {

    enum MachineGunStates
    {
        isShooting,
        isReloading
    }

    [Header("Fire Rate Variables")]
    [SerializeField]
    float fireRate;
    [SerializeField]
    float infectedFireRate;

    float fireTimer;

    bool canFire = true;

    [SerializeField]
    GameObject machineGunBarrel;
    [SerializeField]
    float barrelRotationSpeed;


    [SerializeField]
    float arbitraryDistanceToFire = 200f;
    Vector3 projectileTarget;
    GameObject projectileHit;
    [SerializeField]
    Camera fpsCam;

    [SerializeField]
    ParticleSystem muzzleFlash;

    [SerializeField]
    GameObject bulletImpactParticlePrefab;
    [SerializeField]
    float damage;

    [Space]
    [Header("Mech Piece")]
    [SerializeField]
    MechPiece mechPiece;
    bool hitTarget = false;

    Vector3 hitNormal;

    AudioSource source;

    public AudioClip fireSound;

    // Use this for initialization
    void Start () {
        if (weaponHand == WeaponHand.left)
        {
            barrelRotationSpeed *= -1;
        }

        source = GetComponent<AudioSource>();
        
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
        IncrementTimers();

        //If triggerPulled
        if(TriggerPulled() && !mechPiece.isDead)
        {
            Shoot();
        }
	}

    bool TriggerPulled()
    {
        return (weaponHand == WeaponHand.left && state.Triggers.Left > 0) || (weaponHand == WeaponHand.right && state.Triggers.Right > 0);
    }

    void IncrementTimers()
    {
        if (!canFire)
        {
            fireTimer += Time.deltaTime;
            if((!mechPiece.isInfected && fireTimer > fireRate) || (mechPiece.isInfected && fireTimer > infectedFireRate))
            {
                canFire = true;
            }
        }
    }

    void Shoot()
    {
        if (canFire)
        {
            //Find Target to hit
            CastOutToTarget();
            FireAtTarget();
        }

        
        //Do visual animations
        AnimateFiring();
    }

    void FireAtTarget()
    {
        muzzleFlash.Play();
        source.PlayOneShot(fireSound);
        fireTimer = 0;
        canFire = false;
        GameObject.Instantiate(bulletImpactParticlePrefab, projectileTarget,Quaternion.LookRotation(hitNormal));

        if(projectileHit != null)
        {
            projectileHit.SendMessageUpwards("TakeDamage", this.damage, SendMessageOptions.DontRequireReceiver);
        }
    }

    void CastOutToTarget()
    {
        int layer_mask = LayerMask.GetMask("Default", "Enemy");
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit,Mathf.Infinity, layer_mask))
        {
            projectileTarget = hit.point;
            //The object the projectile hit
            projectileHit = hit.collider.gameObject;
            hitNormal = hit.normal;
            hitTarget = true;
        }
        else
        {
            projectileTarget = fpsCam.transform.TransformPoint(Vector3.forward * arbitraryDistanceToFire);
            hitTarget = false;
        }
    }

    void AnimateFiring()
    {
        machineGunBarrel.transform.Rotate(Vector3.right * Time.deltaTime * barrelRotationSpeed);
    }
}
