using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    [SerializeField] float scroll_speed = 3.0f;
    private float scroll_time;
    private GameObject Owner;
    private Vector3 target_position;
    private GameController GC;


    // Start is called before the first frame update
    void Start()
    {
        scroll_time = scroll_speed / 50f;
        StartCoroutine(Movement());
        Owner = GameObject.FindGameObjectWithTag("GameController");
        GC = Owner.GetComponent < GameController > ();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target_position, scroll_time);
    }
    void FixedUpdate()
    {
        if (transform.position.y < -25)
        {
            Destroy(gameObject);
        }



    }

    IEnumerator Movement() //Set a target toward which the item moves, random X, lower Y
    {
        while (true)
        {
            target_position = new Vector3(Random.Range(-11f, 11f), transform.position.y - 5f, 0f);
            yield return new WaitForSeconds(5f);
        }
    }
    void OnTriggerEnter2D(Collider2D other) //DoesSomethingWhenPlayerTouches it
    {
        if (other.tag == "Player")
        {
            GC.pickup("Wolfsbane"); //Calls a method in game controller
            Destroy(gameObject); //Destroys itself
        }
    }
}
