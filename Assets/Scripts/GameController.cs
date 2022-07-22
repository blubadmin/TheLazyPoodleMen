using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject Platform;
    public GameObject Powerup;
    [SerializeField] private GameObject player;


    private PlayerController playerController;
    private float GenX;
    private float GenY;
    private float rs;
    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 50; //This should stop jumping acting differently on different platforms.
        Instantiate(Platform, new Vector3(0, 30, 2),Quaternion.identity);
        StartCoroutine(ItemGenerator());
        StartCoroutine(PlatformGenerator());
        playerController = player.GetComponent<PlayerController>();
        rs = playerController.runSpeed;
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseGame();
        }
    }

    IEnumerator ItemGenerator()
    {
        yield return new WaitForSeconds(5);
    }

    IEnumerator PlatformGenerator()
    {
        //Create a platform within 2+(0.5*runspeed)- 2*runspeed+1) of previous platform, where x is between +- 10.3 and y > 25 every runspeed seconds
        // get current X and Y of last instantiated Platform
        while (true)
        {

            GameObject ClosestPlatform = FindClosestTag("Platform");
            
            float Ypos = ClosestPlatform.transform.position.y;
            float Xpos = ClosestPlatform.transform.position.x;
            float lowx = Xpos - 2f - (1.5f * rs);
            float highx = Xpos + 2f + (1.5f * rs);

            //Min and Max distance are lower/higher of 10, Xposition + radius + some portion of runspeed, +10
            float MinX = Mathf.Max(lowx,-11.3f);//+(player.runspeed) 
            float MaxX = Mathf.Min(highx,11.3f);//+(player.runspeed) 
            float MinY = Mathf.Max((Ypos + 2f + (0.5f*rs)),25f);
            float MaxY = Ypos + 2f + (1.5f*rs);
            GenX = Random.Range(MinX,MaxX);
            GenY = Random.Range(MinY,MaxY);
            Instantiate(Platform, new Vector3(GenX, GenY, 2),Quaternion.identity);
            yield return new WaitForSeconds(2);
        }

    }

    public GameObject FindClosestTag(string Tag)
    {
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag(Tag);
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = new Vector3 (0,30,0);
            foreach (GameObject go in gos)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
            return closest;
        }
    }
        
    public void PauseGame()
    {
        if (isPaused == false)
        {
            isPaused = true;
            Time.timeScale = 0.0f;
        }
        else
        {
            isPaused = false;
            Time.timeScale = 1.0f;
        }
    }


}
