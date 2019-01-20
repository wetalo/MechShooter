using UnityEngine;
using System.Collections;
using System;

public class DroneEnemy : MonoBehaviour
{
    public GameObject[] graphicPieces;


    public float speed = 1.0f;
    public Color startColor;
    public Color endColor;
    public bool repeatable = false;
    float startTime;
    public GameObject Target = null;
    public float movementSpeed;
    public bool isDead;


    /*public Animator anim;
	public Rigidbody rbody;

	//private float inputH;
	//private float inputV;
	private bool run;*/

	// Use this for initialization
	void Start ()
    {
       


        /*
            anim = GetComponent<Animator>();
            rbody = GetComponent<Rigidbody>();
            run = false;*/
    }

    // Update is called once per frame
    void Update ()
    {
        float t = (Mathf.Sin(Time.time - startTime) * speed * 20);
        foreach (GameObject graphicPiece in graphicPieces)
        {
            graphicPiece.GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
        }

      if (!isDead)
            {
                if (Target == null)
                {
                    //SearchForTarget();
                }
                //FollowTarget();
            }

       /* startTime = Time.time;
        if (!repeatable)
        {
            float t = (Time.time - startTime) * speed;
            GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
        }
        else
        {
            float t = (Mathf.Sin(Time.time - startTime) * speed);
            GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
        }*/
        /*
		if(Input.GetKeyDown("1"))
		{
			anim.Play("WAIT01",-1,0f);
		}
		if(Input.GetKeyDown("2"))
		{
			anim.Play("WAIT02",-1,0f);
		}
		if(Input.GetKeyDown("3"))
		{
			anim.Play("WAIT03",-1,0f);
		}
		if(Input.GetKeyDown("4"))
		{
			anim.Play("WAIT04",-1,0f);
		}

		if(Input.GetMouseButtonDown(0))
		{
			int n = Random.Range(0,2);

			if(n == 0)
			{
				anim.Play ("DAMAGED00",-1,0f);
			}
			else
			{
				anim.Play ("DAMAGED01",-1,0f);
			}
		}

		if(Input.GetKey(KeyCode.LeftShift))
		{
			run = true;
		}
		else
		{
			run = false;
		}

		if(Input.GetKey(KeyCode.Space))
		{
			anim.SetBool("jump",true);
		}
		else
		{
			anim.SetBool("jump", false);
		}

		inputH = Input.GetAxis ("Horizontal");
		inputV = Input.GetAxis ("Vertical");

		anim.SetFloat("inputH",inputH);
		anim.SetFloat("inputV",inputV);
		anim.SetBool ("run",run);

		float moveX = inputH*20f*Time.deltaTime;
		float moveZ = inputV*50f*Time.deltaTime;

		if(moveZ <= 0f)
		{
			moveX = 0f;
		}
		else if(run)
		{
			moveX*=3f;
			moveZ*=3f;
		}

		rbody.velocity = new Vector3(moveX,0f,moveZ);*/

	
}

    

    void SearchForTarget()
    {
        Vector3 center = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        Collider[] hitColliders = Physics.OverlapSphere(center, 30);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if(hitColliders[i].transform.tag == "Player")
            {
                Target = hitColliders[i].transform.gameObject;
            }
            i++;
        }
    }

    private void FollowTarget()
    {
        if(Target != null)
        {

            Vector3 targetPosition = Target.transform.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);

            float distance = Vector3.Distance(Target.transform.position, this.transform.position);
            if (distance > 1)
            {
                transform.Translate(Vector3.forward * movementSpeed);
            }
        }
    }
}











