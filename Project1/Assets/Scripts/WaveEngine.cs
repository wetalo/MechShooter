using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct WaveEngineNode
{
    public GameObject prefab;
    public float spawnTime;
    public bool hasBeenSpawned;
    public Transform spawnPoint;
    public Transform[] path;
}


[System.Serializable]
public struct Wave
{
    [SerializeField]
    public float timeBeforeWave;
    [SerializeField]
    public List<WaveEngineNode> waveEngineNodes;
    
}

public class WaveEngine : MonoBehaviour {

    enum WaveEngineState
    {
        RunningWave,
        WaitingForWave
    }

    WaveEngineState currentState = WaveEngineState.WaitingForWave;
    
    [SerializeField]
    List<Wave> waves = new List<Wave>();

    public int currentWaveId;
    float startTime;
    

    // Use this for initialization
    void Start () {
        BeginWave(currentWaveId);
    }

    void BeginWave(int waveId)
    {
        currentWaveId = waveId;
        currentState = WaveEngineState.WaitingForWave;

       
        startTime = Time.time + waves[currentWaveId].timeBeforeWave;
    }
	
	// Update is called once per frame
	void Update () {
        if (currentState == WaveEngineState.WaitingForWave)
        {
            if (Time.time > startTime)
            {
                currentState = WaveEngineState.RunningWave;
                startTime = Time.time;
            }
        }
        else if (currentState == WaveEngineState.RunningWave)
        {
            for (int i = 0; i < waves[currentWaveId].waveEngineNodes.Count; i++)
            {
                WaveEngineNode node = waves[currentWaveId].waveEngineNodes[i];
                if (!node.hasBeenSpawned && node.spawnTime + startTime < Time.time)
                {
                    GameObject tmp = Instantiate(node.prefab, node.spawnPoint.position, node.prefab.transform.rotation);

                    //Set up path here
                    if (tmp.GetComponent<ExplodingEnemy>())
                    {
                        tmp.GetComponent<ExplodingEnemy>().nodePoints = node.path;
                    }

                    node.hasBeenSpawned = true;

                    waves[currentWaveId].waveEngineNodes[i] = node;
                }
            }
        }
	}

    public bool isCurrentWaveCompleted()
    {
        bool isCompleted = true;

        foreach(WaveEngineNode node in waves[currentWaveId].waveEngineNodes)
        {
            if(node.hasBeenSpawned == false)
            {
                isCompleted = false;
                break;
            }
        }
        return isCompleted;
    }

    public bool IsAllWaveCompleted()
    {
        return ((currentWaveId == waves.Count - 1) && isCurrentWaveCompleted());
        
    }

    public void SpawnNextWave()
    {
        BeginWave(currentWaveId + 1);
    }
}
