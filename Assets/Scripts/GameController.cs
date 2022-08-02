using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public GameObject[] Platforms;
    public GameObject[] Powerups;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject blackdog;
    [SerializeField] private TMP_Text CurrentScore;
    [SerializeField] private GameObject Background;
    private PlayerController playerController;
    private BlackDogBehaviour dogcontroller;
    private float GenX;
    private float GenY;
    private float rs;
    private bool isPaused = false;
    public int score;

    public FMOD.Studio.EventInstance ambience;
    public FMOD.Studio.EventInstance gameOver;

    FMOD.Studio.EventInstance platformMoveStone;


    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        Application.targetFrameRate = 50; //This should stop jumping acting differently on different platforms.
        Instantiate(Platforms[0], new Vector3(0, 30, 2),Quaternion.identity);
        StartCoroutine(ItemGenerator());
        StartCoroutine(PlatformGenerator());
        StartCoroutine(BGmover());
        StartCoroutine(Score());
        playerController = player.GetComponent<PlayerController>();
        dogcontroller = blackdog.GetComponent<BlackDogBehaviour>();
        rs = playerController.runSpeed;
        Time.timeScale = 1.0f;
        ambience = FMODUnity.RuntimeManager.CreateInstance("event:/environment/fx_ambience_river");
        gameOver = FMODUnity.RuntimeManager.CreateInstance("event:/fx_generic/fx_system_game_over");
        platformMoveStone = FMODUnity.RuntimeManager.CreateInstance("event:/environment/fx_level_platform_rock_move");
        ambience.start();

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
        while (true)
        {
            GameObject Powerup = Powerups[Random.Range(0, Powerups.Length)];
            Instantiate(Powerup, new Vector3(0, 25, 0), Quaternion.identity);
            yield return new WaitForSeconds(10);
        }
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
            GameObject Platform = Platforms[Random.Range(0, Platforms.Length)];
            Instantiate(Platform, new Vector3(GenX, GenY, 2),Quaternion.identity);
            platformMoveStone.start();
            yield return new WaitForSeconds(2);
        }

    }

    IEnumerator BGmover()
    {
        while (true)
        {
            Instantiate(Background, new Vector3(0, 40, 15), Quaternion.identity);
            yield return new WaitForSeconds(10);
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
    public void pickup(string what)
    {
        if (what == "Wolfsbane")
        {
            blackdog.transform.position = new Vector3(0, -30, 0);
        }
    }

    IEnumerator Score()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (player.GetComponent<PlayerController>().Playeralive == true)
            {
                score += 1;
                CurrentScore.text = string.Concat("Score: ", score);
                
            }
        }
    }

}
