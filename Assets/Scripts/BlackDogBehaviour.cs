using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackDogBehaviour : MonoBehaviour
{
    private bool dogactive;
    [SerializeField] float chasedistance = 5.0f;
    [SerializeField] float dogspeed = 5.0f;
    [SerializeField] float dogwait = 5.0f;
    private float step;
    private GameObject target;
    private Rigidbody2D dogsbody;
    private PlayerController PC;
    [SerializeField] GameObject Player;
    // Start is called before the first frame update



    void Start()
    {
        dogactive = false;
        StartCoroutine(WaitToStart());
        dogsbody = GetComponent<Rigidbody2D>();
        PC = Player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (dogactive == true)
        {
            float curdif = Vector3.Distance(Player.transform.position, transform.position);
            if (curdif> chasedistance && dogsbody.transform.position.y < Player.transform.position.y) //if close to or in front of player, chase them down.
            {
                target = GetClosestPlatform();
            }
            else
            {
                target = Player;
            }
            Debug.Log("Before:" + dogsbody.transform.position);
            dogsbody.transform.position = Vector3.MoveTowards(dogsbody.transform.position, new Vector3(target.transform.position.x,target.transform.position.y,0), step);
            Debug.Log("After:" + dogsbody.transform.position);
        }
    }


    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(dogwait);
        dogactive = true;
        step = dogspeed * Time.deltaTime;
        Debug.Log("Dogs Awake");

    }


    private GameObject GetClosestPlatform()//Dog can identify closest platform
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Platform");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = dogsbody.transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance && go.transform.position.y > dogsbody.transform.position.y)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PC.Die("Dog");
            Debug.Log("Hit Player");
        }
    }

}
