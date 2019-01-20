using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeGun : Weapon {

    enum ChargeGunStates
    {
        isIdle,
        isCharging,
        isReloading
    }
    
    [Space]
    [Header("Gun variables")]
    [SerializeField]
    float fireRate;
    float fireTimer;

    [SerializeField]
    float chargeTime;

    [Space]
    [Header("Bullet Charging")]
    [SerializeField]
    float minDamage;
    [SerializeField]
    float maxDamage;
    [SerializeField]
    float minExplosiveForce;
    [SerializeField]
    float maxExplosiveForce;
    [SerializeField]
    float maxProjectileSizeMultiply;
    [SerializeField]
    float projectileSpeed;


    float chargeTimer;
    float currentSizeMultiplier;
    float currentBulletDamage;
    float currentExplosiveForce;


    bool canFire = true;

    [Space]
    [SerializeField]
    Transform gunBarrel;


    [SerializeField]
    float arbitraryDistanceToFire = 200f;
    Vector3 projectileTarget;
    [SerializeField]
    Camera fpsCam;

    [SerializeField]
    ParticleSystem muzzleFlash;

    [SerializeField]
    GameObject projectilePrefab;
    GameObject projectileInstance;
    Vector3 projectInstanceInitialScale;

    [SerializeField]
    Material initialProjectileMaterial;
    [SerializeField]
    Material targetProjectileMaterial;
    Renderer projectileRenderer;

    public Material feedbackMaterial;

    [SerializeField]
    ParticleSystem chargeGunParticles;

    ChargeGunStates gunState = ChargeGunStates.isIdle;

    [SerializeField]
    Color initialParticleColor;
    [SerializeField]
    Color targetParticleColor;

    [SerializeField]
    MechPiece piece;

    AudioSource audioSource;

    public AudioClip chargeSound;
    public AudioClip releaseSound;
    public AudioClip infectedReleaseSound;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
        IncrementTimers();

        if (piece.isDead)
        {
            canFire = false;
        }

        if (gunState == ChargeGunStates.isIdle)
        {
            if (TriggerPulled() && canFire)
            {
                BeginCharging();
            }
        }

        Charge();
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
            if(fireTimer > fireRate)
            {
                canFire = true;
            }
        }
    }

    

    void CastOutToTarget()
    {
        int layer_mask = LayerMask.GetMask("Default", "Enemy");
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit,Mathf.Infinity, layer_mask))
        {
            projectileTarget = hit.point;
        }
        else
        {
            projectileTarget = fpsCam.transform.TransformPoint(Vector3.forward * arbitraryDistanceToFire);
        }
    }

    void BeginCharging()
    {
        gunState = ChargeGunStates.isCharging;
        projectileInstance = Instantiate(projectilePrefab, gunBarrel.transform.position, Quaternion.LookRotation(gunBarrel.forward));
        projectileInstance.transform.parent = gunBarrel.transform;
        chargeGunParticles.Play();

        //Base settings for charge shots
        currentSizeMultiplier = 1.0f;
        currentBulletDamage = minDamage;
        currentExplosiveForce = minExplosiveForce;
        chargeTimer = 0f;
        projectInstanceInitialScale = projectileInstance.transform.localScale;
        projectileRenderer = projectileInstance.GetComponent<Renderer>();
        projectileRenderer.material = initialProjectileMaterial;
    }

    float flashTimer;
    bool isgoingUp = true;

    void Charge()
    {
        if(gunState == ChargeGunStates.isCharging)
        {
            if (TriggerPulled() && !piece.isInfected)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(chargeSound);
                }
                if (chargeTimer < chargeTime)
                {
                    ParticleSystem.MainModule newMain = chargeGunParticles.main;

                    newMain.startColor = Color.Lerp(initialParticleColor, targetParticleColor, chargeTimer); ;
                    projectileRenderer.material.Lerp(initialProjectileMaterial, targetProjectileMaterial, chargeTimer);
                    chargeTimer += Time.deltaTime;
                    currentSizeMultiplier = 1 + ((maxProjectileSizeMultiply - 1f) * (chargeTimer / chargeTime));
                    currentBulletDamage = minDamage + ((maxDamage - minDamage) * (chargeTimer / chargeTime));
                    currentExplosiveForce = minExplosiveForce + ((maxExplosiveForce - minExplosiveForce) * (chargeTimer / chargeTime));

                    if (currentSizeMultiplier > maxProjectileSizeMultiply)
                    {
                        currentSizeMultiplier = maxProjectileSizeMultiply;
                    }

                    if(currentBulletDamage > maxDamage)
                    {
                        currentBulletDamage = maxDamage;
                    }

                    if(currentExplosiveForce > maxExplosiveForce)
                    {
                        currentExplosiveForce = maxExplosiveForce;
                    }

                    projectileInstance.transform.localScale = projectInstanceInitialScale * currentSizeMultiplier;
                }
                // Changing the material while full charge is being held
                else
                {
                    projectileRenderer.material.Lerp(targetProjectileMaterial,feedbackMaterial , flashTimer);
                    if (isgoingUp)
                    {
                        flashTimer += Time.deltaTime*3;
                        if(flashTimer>= 1)
                        {
                            flashTimer = 1;
                            isgoingUp = false;
                        }
                    }
                    else
                    {
                        flashTimer -= Time.deltaTime*3;
                        if(flashTimer <= 0)
                        {
                            flashTimer = 0;
                            isgoingUp = true;
                        }
                    }

                }
            } else
            {
                audioSource.Stop();
                if (!piece.isInfected)
                {
                    audioSource.PlayOneShot(releaseSound);
                } else if (piece.isInfected)
                {
                    audioSource.PlayOneShot(infectedReleaseSound);
                }
                projectileInstance.transform.parent = null;
                CastOutToTarget();
                projectileInstance.GetComponent<ChargeShot>().Activate(currentBulletDamage, currentExplosiveForce, currentSizeMultiplier);
                projectileInstance.GetComponent<Rigidbody>().AddForce((projectileTarget - projectileInstance.transform.position).normalized * projectileSpeed, ForceMode.Impulse);
                StopCharging();
            }

        }
    }

    void StopCharging()
    {
        chargeGunParticles.Stop();
        canFire = false;
        fireTimer = 0f;
        gunState = ChargeGunStates.isIdle;
    }


}
