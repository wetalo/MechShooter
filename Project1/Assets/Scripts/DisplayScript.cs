using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayScript : MonoBehaviour {
    public bool isMultiMonitor = false;

    [SerializeField]
    Camera camera1;
    [SerializeField]
    Camera weaponCam;
    [SerializeField]
    Camera camera2;

    // Use this for initialization
    void Start()
    {
        // Display.displays[0] is the primary, default display and is always ON.
        // Check if additional displays are available and activate each.
        if (Display.displays.Length > 1)
        {
            isMultiMonitor = true;
            Display.displays[1].Activate();
        }

        /*
        if (isMultiMonitor)
        {
            camera2.targetDisplay = 1;
        } else
        {*/
            camera2.targetDisplay = 0;
            camera1.pixelRect = new Rect(0, 0, (float)(Screen.width * 0.7f), Screen.height);
            weaponCam.pixelRect = new Rect(0, 0, (float)(Screen.width * 0.7f), Screen.height);
            camera2.pixelRect = new Rect((float)(Screen.width * 0.7f), 0, (float)(Screen.width * 0.3f), Screen.height);

       // }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
	
