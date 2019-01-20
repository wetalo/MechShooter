using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MechPiece : MonoBehaviour {


    public string pieceName;

    public bool isInfected = false;

    [Space]
    [Header("Health Variables")]
    [SerializeField]
    float maxHealth;
    [SerializeField]
    public float currentHealth;
    [SerializeField]
    float damagedPercentage;
    [SerializeField]
    float criticalPercentage;
    [SerializeField]
    Text healthText;


    [Space]
    [Header("Materials")]
    [SerializeField]
    Material healthyMaterial;
    [SerializeField]
    Material damagedMaterial;
    [SerializeField]
    Material criticalMaterial;
    [SerializeField]
    Material deadMaterial;
    [SerializeField]
    Material sickMaterial;

    [Space]
    [Header("Colours")]
    [SerializeField]
    Color healthyColor;
    [SerializeField]
    Color damagedColor;
    [SerializeField]
    Color criticalColor;
    [SerializeField]
    Color deadColor;
    [SerializeField]
    Color sickColor;


    [Space]
    [Header("Healing")]
    [SerializeField]
    float healingTime = 10;
    float timeToStopHealing;
    public bool isDead = false;

    bool isHealing = false;


    // Use this for initialization
    void Start () {
        currentHealth = maxHealth;
	}

    // Update is called once per frame
    void Update() {

        Heal();

        float healthPercentage = currentHealth / maxHealth;

        if (healthPercentage > damagedPercentage)
        {
            this.GetComponent<Renderer>().material = healthyMaterial;
            healthText.color = healthyColor;
        }
        else if (healthPercentage > criticalPercentage)
        {
            this.GetComponent<Renderer>().material = damagedMaterial;
            healthText.color = damagedColor;
        }
        else if (healthPercentage < criticalPercentage && healthPercentage > 0)
        {
            this.GetComponent<Renderer>().material = criticalMaterial;
            healthText.color = criticalColor;
        }
        else if (healthPercentage <= 0)
        {
            this.GetComponent<Renderer>().material = deadMaterial;
            healthText.color = deadColor;
        }

        healthText.text = currentHealth + "/" + maxHealth;


        if (isInfected)
        {
            this.GetComponent<Renderer>().material = sickMaterial;
            healthText.color = sickColor;
        }
	}

    public void Infect()
    {
        this.isInfected = true;
    }

    public void DisInfect()
    {
        this.isInfected = false;
    }

    public void BeginHealing()
    {
        isHealing = true;
        timeToStopHealing = Time.time + healingTime;
    }

    void Heal()
    {
        if (isHealing)
        {
            if(Time.time < timeToStopHealing)
            {
                currentHealth += (maxHealth / healingTime) * Time.deltaTime;
                if(currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }

                if(currentHealth > maxHealth * criticalPercentage)
                {
                    isDead = false;
                }
            }
        }
    }

    public void StopHealing()
    {
        isHealing = false;
        currentHealth = Mathf.Round(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        this.currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
        }
    }
}
