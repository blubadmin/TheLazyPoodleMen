using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour

{
    /*What do i need this to do?
    I need it to move the player on wasd - Done
    I need the player to be able to 'jump' - Done
    (Jumping is moving in a straight line a fixed distance.) - Done
    I need player to be able to find objects 'beneath it'
    I need player to die if it doesn't have an object beneath it or is not jumping.
    
    
   */ 
    private Rigidbody2D body;
    private Animator anim; 


    float horizontal; //horizontal movement
    float vertical; // vertical movement
    float ChangeX; //In air direction modifier
    float ChangeY; //In air direction modifier
    float grace; //Time before env hazard kills player
    float ground_grace; //Time player can walk off platform and remain grounded
    private float hop_frames; //used to make player hop when jumping
    private float jumped = 0.0f;
    private int fall_frames;
    private bool drop_needed;
    private bool grounded;
    [SerializeField] float air_mod = 0.01f; //Weight on in air movement.
    [SerializeField] GameObject DeathScreen;
    [SerializeField] GameObject gameController;
    [SerializeField] TMP_Text HighScore;
    [SerializeField] TMP_Text FinalSCore;

    private GameController GC;
    private float highScore;


    public bool Playeralive;
    public bool jumping;
    public float runSpeed = 5.0f;
    public float jump_length = 100.0f; // jump length is the starting position
    public float jump_decay = 1f; // jump_decay is the jump returns to starting position.
    public float jump_up = 0.5f; //jump_up is a multiplier of jump length that shortens the jump when button released quickly
                                    
    public LayerMask Groundlayer;
    private GameObject shadow;
    private Rigidbody2D shadowtransform;
    private bool doonce;

    // FMOD Class definitions
    FMOD.Studio.EventInstance playerJump;
    FMOD.Studio.EventInstance playerSplash;
    FMOD.Studio.EventInstance playerLand;

    FMOD.Studio.EventInstance playerFootsteps;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        jumping = false;
        grounded = true;
        fall_frames = 0;
        drop_needed = false;
        DeathScreen.SetActive(false);
        GC = gameController.GetComponent<GameController>();
        Playeralive = true;
        highScore = PlayerPrefs.GetFloat("highScore");
        shadow = body.transform.GetChild(0).gameObject;
        shadowtransform = shadow.GetComponent<Rigidbody2D>();
        shadow.SetActive(false);
        playerJump = FMODUnity.RuntimeManager.CreateInstance("event:/player/fx_player_jump");
        playerSplash = FMODUnity.RuntimeManager.CreateInstance("event:/environment/fx_river_splash");
        playerLand = FMODUnity.RuntimeManager.CreateInstance("event:/player/fx_player_land");
        playerFootsteps = FMODUnity.RuntimeManager.CreateInstance("event:/player/fx_player_footsteps");
        playerFootsteps.setParameterByNameWithLabel("footstepType", "stone");
        doonce = false;



    }

    void Update()
    {
        // Gives a value between -1 and 1


        Movement(); //For some reason putting this in fixed update messes with key down detection.
        //OnCollisionEnter2D();

    }

    void FixedUpdate()
    {
        
        //grounded = Physics2D.OverlapCircleAll(transform.position,1.0f);
        //Debug.Log("Grounded: " + grounded + "\nJumping: "+ jumping);



        IsDry();
        IsGrounded();
        //Debug.Log("Jumped: "+jumped);

        //Debug.Log("j " + jumped + "\ngrounded; " + grounded + " jumping: " + jumping);


    }               
    void Movement()
    {
        if (jumping == false && grounded == true)
        { //basic movement
            horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
            vertical = Input.GetAxisRaw("Vertical"); // -1 is down
            body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
            //animation stuff
            anim.SetBool("jumping", false);
            if(Mathf.Abs(horizontal)> Mathf.Abs(vertical))
            {
                anim.SetFloat("Horizontal", horizontal);
                anim.SetFloat("Vertical", 0f);
            }
            else 
            {
                anim.SetFloat("Vertical", vertical);
                anim.SetFloat("Horizontal", 0f);
            }


        }
        if (Input.GetButtonDown("Jump")) {
            //Debug.Log("Button Down");
            if (jumping == false && grounded == true)//jump up
            {
                anim.SetBool("jumping", true);
                jumping = true;
                jumped = jump_length;
                hop_frames = 10f;
                shadow.SetActive(true);
                shadow.transform.position = transform.position;
                shadowtransform.velocity = body.velocity;
                //playerFootsteps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                playerJump.start();
            }
            else { //Debug.Log("Cannot Jump");
                   }
        }
        if (Input.GetButtonUp("Jump")) //shorter jumps
        {
            jumped *= jump_up;
            //Debug.Log("Button Up");
        }
        if (jumping == true)
        {

            ChangeX = Input.GetAxisRaw("Horizontal")*air_mod;//This lets the player control jumping a little
                                                          //bit more by leaning in a direction lean out to go further,
                                                          //lean in to shorten, lean in a new direction to curve it
            ChangeY = Input.GetAxisRaw("Vertical")*air_mod;
            body.velocity = new Vector2(body.velocity.x+ChangeX,body.velocity.y+ChangeY);
            shadowtransform.velocity = new Vector2(shadowtransform.velocity.x + ChangeX, shadowtransform.velocity.y + ChangeY);
        }
        if (jumped > 0.0f)
        {
            jumped -= jump_decay;
            //Debug.Log(tired);
            if (jumped <= jump_decay * 5)//player can jump or start walking up to 5 frames before they land
            {
                jumping = false;
                shadow.SetActive(false);
                playerLand.start();
            }
        }
        else if (jumped < 0.0f) { jumped = 0.0f; }





        //player cannot go above or below certain y
        //if (body.transform.position.y > 15f)
        //{
        //    body.transform.position = new Vector2(transform.position.x,15f);
        //}
        //if (body.transform.position.y <0f)
        //{
        //    body.transform.position = new Vector2(transform.position.x,0f);
        //}
        //player cannot go above or below certain x
        if (body.transform.position.x > 12.3f)
        {
            body.transform.position = new Vector2(12.3f, transform.position.y);
        }
        if (body.transform.position.x < -12.3f)
        {
            body.transform.position = new Vector2(-12.3f, transform.position.y);
        }


        //playersprite does a little hop at start and end of jump
        if (hop_frames > 0)
        {
            hop_frames -= 1f;
            body.transform.position = new Vector2(transform.position.x, transform.position.y + 0.1f);
            drop_needed = true;
        }
        if (jumped < 15 && drop_needed == true)
        {
            drop_needed = false;    
            fall_frames = 10;
                       
        }
        if (fall_frames > 0)
        {
            fall_frames -= 1; body.transform.position = new Vector2(transform.position.x, transform.position.y - 0.1f);//Debug.Log(fall_frames);
        }


    }



    void IsDry()
    {
        ;
        /*
         * Is the player above a platform?
         * */
        if (jumping == true || grounded == true) //The player is always dry if they are jumping or on a platform
        {
            grace = 0.1f; //FIxed update is 50fps, this gives player (grace/.01) frames to react before they will die

        }
        else
        {
            grace -= 0.01f;
            if (grace <= 0f) {
                if (doonce == false)
                {

                    playerSplash.start();
                    doonce = true;
                }
                Die("water");
            }
        }
        //*/


        


    }

    void IsGrounded()
    {
        if(Physics.Raycast(new Vector2(transform.position.x,transform.position.y-1.2f), Vector3.forward, 2, Groundlayer)) //this has an offset so it is centered on players feet
        {
            grounded = true;
            ground_grace = 0.1f;
        }
        else {
            ground_grace -= 0.01f;
            if (ground_grace <= 0.0f)
            {
                grounded = false;
            }
        }
    }


    public void Die(string How) //Maybe move to a game manager
    {
        if(GC.score > highScore) {
            highScore = GC.score;
            PlayerPrefs.SetFloat("highScore", highScore);
        }
        body.velocity = new Vector2(0, 0);
        anim.SetFloat("Horizontal", 0f);
        anim.SetFloat("Vertical", 0f);
        DeathScreen.SetActive(true);
        Debug.Log("You Have Died by"+ How);
        
        HighScore.text = "Your highscore is " + highScore;
        FinalSCore.text = "Your Score was " + GC.score;

        if(How == "water")
        {
            anim.SetBool("Waterdie", true);
            
        }
        if(How == "Dog")
        {
            anim.SetBool("Dogdie",true);
        }

        StartCoroutine(WaittoDie());
        Playeralive = false;
    }

    IEnumerator WaittoDie()
    {
        yield return new WaitForSeconds(0.5f);
        GC.PauseGame();
        GC.ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        GC.gameOver.start();
    }

    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log(collision + "Collided");
    //    //if (collision.gameObject.tag == "Enemy")
    //    //{
    //    //    collision.gameObject.SendMessage("ApplyDamage", 10);
    //    //}
    //}





}
