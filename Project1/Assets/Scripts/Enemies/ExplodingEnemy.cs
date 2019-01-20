using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExplodingEnemy : MonoBehaviour {

    NavMeshAgent agent;

    enum ExplodingEnemyState
    {
        idle,
        isTraveling,
        isPrimingExplosion,
        isExploding
    }

    ExplodingEnemyState state;
    
    public Transform[] nodePoints;
    Transform targetNode;
    int nodeIndex = 0;

    [Space]
    [Header("Exploding distance variables")]
    [SerializeField]
    float closeDistanceToNode = 0.2f;
    [SerializeField]
    float closeDistanceToPlayer = 5f;
    [SerializeField]
    float primingTime = 1;
    float timeToExplode;

    Animator anim;

    Renderer renderer;

    float flashTimer = 0f;
    [Space]
    [Header("Exploding Color Variables")]
    [SerializeField]
    Color startColor;
    [SerializeField]
    Color endColor;
    [SerializeField]
    GameObject[] graphicPieces;
    bool isGoingUp = false;

    [SerializeField]
    GameObject explosionPrefab;




    GameObject player;
    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        state = ExplodingEnemyState.idle;
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        renderer = GetComponent<Renderer>();
        StartTraveling();
	}
	
	// Update is called once per frame
	void Update () {
        Travel();
        PrimeExplosion();
	}

    void StartTraveling()
    {
        state = ExplodingEnemyState.isTraveling;
        nodeIndex =-1;
        GetNextNode();
    }

    void Travel()
    {
        if(state == ExplodingEnemyState.isTraveling)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < closeDistanceToPlayer) {
                StartPriming();
            }
            else if (Vector3.Distance(transform.position, targetNode.position) < closeDistanceToNode)
            {
                GetNextNode();
            }
        }
    }

    void GetNextNode()
    {
        nodeIndex++;
        if (nodeIndex < nodePoints.Length)
        {
            targetNode = nodePoints[nodeIndex];
            agent.SetDestination(targetNode.position);
        } else
        {
            StartPriming();
        }
    }

    void StartPriming()
    {
        state = ExplodingEnemyState.isPrimingExplosion;
        timeToExplode = Time.time + primingTime;
        anim.SetBool("IsWalking", false);
        anim.Play("Agony");
        anim.SetBool("IsExploding", true);
    }

    void PrimeExplosion()
    {
        if (state == ExplodingEnemyState.isPrimingExplosion)
        {
            if (Time.time < timeToExplode)
            {
                foreach (GameObject graphicPiece in graphicPieces)
                {
                    graphicPiece.GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, flashTimer);
                }
                if (isGoingUp)
                {
                    flashTimer += Time.deltaTime * 3;
                    if (flashTimer >= 1)
                    {
                        flashTimer = 1;
                        isGoingUp = false;
                    }
                }
                else
                {
                    flashTimer -= Time.deltaTime * 3;
                    if (flashTimer <= 0)
                    {
                        flashTimer = 0;
                        isGoingUp = true;
                    }
                }
            }
            else
            {
                Explode();
            }
        }
    }

    void Explode()
    {
        state = ExplodingEnemyState.isExploding;
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
