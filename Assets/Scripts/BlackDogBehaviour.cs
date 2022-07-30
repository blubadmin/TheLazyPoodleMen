using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackDogBehaviour : MonoBehaviour
{
    private bool dogactive;
    [SerializeField] float chasedistance = 5.0f;
    public float dogspeed = 5.0f;
    [SerializeField] float dogwait = 5.0f;
    private float step;
    private GameObject target;
    private Rigidbody2D dogsbody;
    private PlayerController PC;
    [SerializeField] GameObject Player;
    // Start is called before the first frame update


    FMOD.Studio.EventInstance dogHowl;
    FMOD.Studio.EventInstance dogGrowl;



    void Start()
    {
        dogactive = false;
        StartCoroutine(WaitToStart());
        dogsbody = GetComponent<Rigidbody2D>();
        PC = Player.GetComponent<PlayerController>();
        dogHowl = FMODUnity.RuntimeManager.CreateInstance("event:/enemy/fx_dog_howl");
        dogGrowl = FMODUnity.RuntimeManager.CreateInstance("event:/enemy/fx_dog_growl");
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
            dogsbody.transform.position = Vector3.MoveTowards(dogsbody.transform.position, new Vector3(target.transform.position.x,target.transform.position.y,0), step);
            
        }
    }


    IEnumerator WaitToStart() //Wait a few seconds to let player get hang of game
    {
        yield return new WaitForSeconds(dogwait);
        dogHowl.start();
        dogactive = true;
        step = dogspeed * Time.deltaTime;
        Debug.Log("Dogs Awake");
        StartCoroutine(DogGrowl());

    }

    IEnumerator DogGrowl()
    {
        while (true)
        {

            yield return new WaitForSeconds(Random.Range(2f, 6f));
            dogGrowl.start();
        }
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

    void OnTriggerEnter2D(Collider2D other) //Dog can kill player
    {
        if (other.tag == "Player")
        {
            PC.Die("Dog");
            Debug.Log("Hit Player");
        }
    }

}
