using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure; // Required in C#

public class CameraLook : MonoBehaviour
{

    Vector2 mouseLook;
    Vector2 smoothV;
    public float sensitivity = 5.0f;
    public float smoothing = 2.0f;

    public float maxVeritcalAim;
    public float minVerticalAim;

    GameObject character;

    //Controller Variables
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;

    // Use this for initialization
    void Start()
    {
        character = this.transform.parent.gameObject;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

        SetController();
        var md = new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);

        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        mouseLook += smoothV;
        

        if (-mouseLook.y < maxVeritcalAim && -mouseLook.y > minVerticalAim)
        {
            transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        }

        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
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
