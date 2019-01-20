using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure; // Required in C#

public class Weapon : MonoBehaviour {

    //Controller variables
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    protected GamePadState state;
    GamePadState prevState;

    protected enum WeaponHand
    {
        right,
        left
    }

    [SerializeField]
    protected WeaponHand weaponHand;
    
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	protected virtual void Update () {

        SetController();
    }

    void SetController()
    {
        if (!playerIndexSet || !prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }


        prevState = state;
        state = GamePad.GetState(playerIndex);
    }
}
