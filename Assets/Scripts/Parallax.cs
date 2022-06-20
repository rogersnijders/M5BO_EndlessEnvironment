using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length;
    private float startpos;

    public GameObject cam;
    public float parallax;

    // Start is called before the first frame update
    void Start()
    {
        // if you want to do it by the y, just change x to y
        startpos = transform.position.x;

        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
      float disx = cam.transform.position.x * parallax;
      float disy = cam.transform.position.y * parallax;
        float tmp = cam.transform.position.x * (1-parallax);

        transform.position = new Vector3(startpos + disx, startpos + disy, transform.position.z);

        if (tmp > startpos+length) {
            startpos += length;
        } else if (tmp < startpos - length) {
            startpos -= length;
        }
    }
}
