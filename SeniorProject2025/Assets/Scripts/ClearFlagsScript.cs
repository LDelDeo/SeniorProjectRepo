using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ClearFlagsScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var camera = GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.Depth;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
