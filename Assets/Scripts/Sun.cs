using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public Camera cam;

    public Transform sun;

    public float sunScale;

    // Start is called before the first frame update
    void Start()
    {
        sun.localScale = Vector3.one * (sunScale * (cam.farClipPlane / 1000));
        sun.localPosition = new Vector3(sun.localPosition.x, sun.localPosition.y, -cam.farClipPlane);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = cam.transform.position;
    }
}
