using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    [Header("Variables")]
    [SerializeField]
    GameObject enemyTop;
    [SerializeField]
    EnemyRocketLauncher rocketLauncher;

    [SerializeField]
    GameObject upperModel;
    [SerializeField]
    GameObject lowerModel;

    [Space]
    [Header("Movement")]
    [SerializeField]
    float walkSpeed;
    [SerializeField]
    float targetDistanceFromPlayer;
    [SerializeField]
    float maxDistanceTargetFromPlayer;

    Vector3 dashTarget;
    Vector3 pointDistanceFromPlayer;

    [Space]
    [Header("Dashing")]
    [SerializeField]
    float dashSpeed;
    [SerializeField]
    float dashLength;
    float dashTime;
    [SerializeField]
    float nearPlayerDashCooldown;
    [SerializeField]
    float farPlayerDashCooldown;
    
    [Space]
    [SerializeField]
    float explosionForce;

    [Space]
    [Header("Health")]
    [SerializeField]
    float maxHealth;
    float currentHealth;
    [SerializeField]
    Slider healthSlider;

    float pillarRadius = 5f;

    float timeSinceLastDash;

    
        

    GameObject player;

    NavMeshAgent agent;


    enum EnemyState
    {
        isIdle,
        isWalking,
        isDashing,
    }

    EnemyState state;

    
    

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        float healthPercentage = (currentHealth / maxHealth);
        healthSlider.value = healthPercentage;
        state = EnemyState.isIdle;
	}
	
	// Update is called once per frame
	void Update () {
        

		if(state == EnemyState.isIdle)
        {
            BeginWalk();
        }

        Walk();
        Dash();

        enemyTop.transform.LookAt(player.transform.position);
        
	}

    void BeginWalk()
    {
        state = EnemyState.isWalking;
        agent.speed = walkSpeed;

        timeSinceLastDash = Time.time;
    }

    void Walk()
    {
        if(state == EnemyState.isWalking)
        {
            pointDistanceFromPlayer = player.transform.position + (player.transform.forward.normalized * targetDistanceFromPlayer);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(pointDistanceFromPlayer, out hit, maxDistanceTargetFromPlayer, NavMesh.AllAreas))
            {
                pointDistanceFromPlayer = hit.position;
            }

            agent.SetDestination(pointDistanceFromPlayer);

            float distanceFromPlayer = Vector3.Distance(player.transform.position, transform.position);

            if (distanceFromPlayer < targetDistanceFromPlayer)
            {
                if(Time.time > timeSinceLastDash + nearPlayerDashCooldown)
                {
                    StartDash();
                }
            } else if (distanceFromPlayer > targetDistanceFromPlayer)
            {
                if (Time.time > timeSinceLastDash + farPlayerDashCooldown)
                {
                    StartDash();
                }
            }
        }
    }

    void StartDash()
    {
        rocketLauncher.ShootAtPlayer();
        state = EnemyState.isDashing;
        agent.speed = dashSpeed;
        dashTime = Time.time + dashLength;

        int direction = Random.Range((int)1, (int)3);
        if (direction == 1)
        {
            dashTarget = transform.position + (transform.right.normalized * targetDistanceFromPlayer);
        } else
        {
            dashTarget = transform.position + (transform.right.normalized *-1 * targetDistanceFromPlayer);
        }

        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(dashTarget, out hit, maxDistanceTargetFromPlayer, NavMesh.AllAreas))
        {
            dashTarget = hit.position;
        }

        agent.SetDestination(dashTarget);
    }

    void Dash()
    {
        if(state == EnemyState.isDashing)
        {
            if(Time.time > dashTime)
            {
                StopDash();
            }
        }
    }

    void StopDash()
    {
        state = EnemyState.isIdle;
        agent.speed = walkSpeed;
        timeSinceLastDash = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(state == EnemyState.isDashing && other.gameObject.tag == "Block")
        {
            BlowUpBlocks();
        }
    }

    void BlowUpBlocks()
    {
        var cols = Physics.OverlapSphere(transform.position, pillarRadius);
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
            rb.AddExplosionForce(explosionForce , transform.position, pillarRadius, 1, ForceMode.Impulse);
        }
    }

    void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        float healthPercentage = (currentHealth / maxHealth);
        healthSlider.value = healthPercentage;
        if(currentHealth < 0)
        {
            currentHealth = 0;
        }
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        upperModel.transform.parent = null;
        lowerModel.transform.parent = null;
        upperModel.GetComponent<Rigidbody>().isKinematic = false;
        upperModel.GetComponent<Rigidbody>().AddExplosionForce(5, transform.position, 2, 1, ForceMode.Impulse);
        lowerModel.GetComponent<Rigidbody>().isKinematic = false;
        lowerModel.GetComponent<Rigidbody>().AddExplosionForce(45, transform.position, 2,1,ForceMode.Impulse);

        Destroy(gameObject);
    }
    
}
