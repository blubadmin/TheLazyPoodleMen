using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMotionController : MonoBehaviour
{
    private float scroll_speed = 3.0f;
    private float scroll_time;
    private GameObject Owner;

    // Start is called before the first frame update
    void Start()
    {
        scroll_time = scroll_speed /50f;
    
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    void FixedUpdate() 
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - scroll_time,transform.position.z);
        if (transform.position.y < -25)
        {
            Destroy(gameObject);
        }
    
      
    
    }


}
