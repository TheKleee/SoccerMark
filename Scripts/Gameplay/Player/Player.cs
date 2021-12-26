using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public enum Type
{
    player,
    npc
}
public class Player : MonoBehaviour
{
    [Header("Player Type:")]
    public Type type;

    AudioSource aSource;

    [HideInInspector] public Animator anim;
    Camera cam;
    //Gameplay:
    //X-axis moves from -2 to 2

    //Npc: Check where ball is on the x-axis and move parallel to it if you are catching the ball
    //else move to a random position from -2 to 2 on x-axis. Repeat this in small intervals.
    //Use MEC to do this.

    //Player: Move from -2 to 2 on the x-axis based on the invisible joystic input.

    //Aaaand that's all folks : D

    [Header("Joystic:")]
    public Joystick joystic;    //Might not use it later... let's test >:)
    #region NPC:
    float randNPC;
    Ball ball;
    bool proNPC;
    List<Ball> balls = new List<Ball>();
    float formerBallDist;
    [SerializeField] bool isIdle = true;
    private float proChance = 1.0f;

    [Header("Training:")]
    [Range(5.0f, 10.0f)] public float kickPower = 5f;   //+.05 per level - upgrade
    [Range(10.0f, 20.0f)] public float moveSpeed = 10f; //+1 per level - upgrade
    [Range(1.0f, 2.0f)] public float bonusSize = 1f;    //+.1 per level - upgrade
    IEnumerator<float> _NPC()
    {
        //Check if pro xD
        //Level based: pro => lvl something...
        //Search for a ball:
        balls.AddRange(GameController.instance.balls);
        foreach(var b in balls)
        {
            float dist = Vector3.Distance(transform.position, b.transform.position);
            if(formerBallDist < dist)
            {
                formerBallDist = dist;
                ball = b;
            }
        }
        balls.Clear();
        float pro = Random.Range(0.0f, 2.0f);
        proNPC = false;
        ball = FindObjectOfType<Ball>();

        if (SaveData.instance.lvl <= 1) proChance = 2.1f;
        else if (SaveData.instance.lvl <= 5) proChance = 1.25f;
        else if (SaveData.instance.lvl <= 15) proChance = 1.0f;
        else if (SaveData.instance.lvl <= 30) proChance = 0.75f;
        else proChance = 0.25f;

        if (pro >= proChance)
        { 
            proNPC = true;
            yield return Timing.WaitForSeconds(.35f);
        }
        else
        {
            float randomWait = Random.Range(.5f, 1.25f);
            yield return Timing.WaitForSeconds(randomWait);
            randNPC = Random.Range(-2.0f, 2.0f);
            float distance = Mathf.Abs(transform.position.x - randNPC);
            if (distance > .75f)
            {
                while (distance > .5f)
                {
                    if (distance <= 0 || !pForce)
                        break;

                    distance = Mathf.Abs(transform.position.x - randNPC);
                    yield return Timing.WaitForSeconds(.25f);
                }
            }
            else
            {
                yield return Timing.WaitForSeconds(.75f);
            }
        }
        if (pForce)
            Timing.RunCoroutine(_NPC().CancelWith(gameObject));
    }
    Vector3 checkPosition = Vector3.zero;
    float posWay;

    #endregion
    private void Update()
    {
        //If match started...
        aSource = GetComponent<AudioSource>();
        if (pForce && !GameController.instance.win)
        {
            if (type == Type.player)
            {
                //Calculate input here...
                //Use the joystic...
                if (joystic.Horizontal != 0)
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(
                        2 * joystic.Horizontal,   //Joystic movement... from -2 to 2...
                        transform.position.y,
                        transform.position.z
                        ), moveSpeed * Mathf.Abs(joystic.Horizontal * 1.25f) * Time.deltaTime);
                }

                if (isIdle && !isKicking)
                {
                    Timing.RunCoroutine(_IdleCheck().CancelWith(gameObject));
                }

            } else {
                if (proNPC && ball != null && ball.transform.position.x <= 2.5f && ball.transform.position.x >= -2.5f) randNPC = ball.transform.position.x;
                //NPC Movement:
                transform.position = Vector3.Lerp(transform.position, new Vector3(
                        randNPC,   //From -2.5f to 2.5f...
                        transform.position.y,
                        transform.position.z
                        ), 5 * Time.deltaTime);

                if (isIdle && !isKicking)
                {
                    Timing.RunCoroutine(_NpcIdleCheck().CancelWith(gameObject));
                }
            }
        }
    }
    IEnumerator<float> _IdleCheck()
    {
        isIdle = false;
        checkPosition = transform.position;
        yield return Timing.WaitForSeconds(.025f);
        posWay = transform.position.x - checkPosition.x;
        if (posWay >= 0.025f && !isKicking)
        {
            anim.Play("Slide Right");
        }
        else if (posWay <= -0.025f && !isKicking)
        {
            anim.Play("Slide Left");
        }
        yield return Timing.WaitForSeconds(.15f);
        isIdle = true;
    }
    IEnumerator<float> _NpcIdleCheck()
    {
        isIdle = false;
        checkPosition = transform.position;

        yield return Timing.WaitForSeconds(.025f);
        posWay = transform.position.x - checkPosition.x;
        if (posWay >= 0.025f && !isKicking)
        {
            anim.Play("Slide Left");
        }
        else if (posWay <= -0.025f && !isKicking)
        {
            anim.Play("Slide Right");
        }
        yield return Timing.WaitForSeconds(.15f);
        isIdle = true;
    }
    private void LateUpdate()
    {
        if (type == Type.player)
        {
            Vector3 desiredPos = cam.transform.position;
            Vector3 lerpPos = Vector3.Slerp(desiredPos, transform.position, .25f * Time.deltaTime);

            if (transform.position.x > 1.25f && joystic.Horizontal != 0 || transform.position.x < -1.25f && joystic.Horizontal != 0)
            {
                if (Mathf.Abs(cam.transform.position.x) <= 2f)
                    cam.transform.position = new Vector3(lerpPos.x, cam.transform.position.y, cam.transform.position.z);
            }
            else
                cam.transform.position = Vector3.Slerp(cam.transform.position, new Vector3(0, cam.transform.position.y, cam.transform.position.z), 1.75f * Time.deltaTime);
        }

    }

    #region Force By Movement:
    [HideInInspector] public float playerForce;
    private bool pForce;
    Vector3 movedPos;   //Moved position

    [Header("Body Size:")]
    public Transform bodySize;
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        if (type == Type.player)
        {
            cam = Camera.main;
            UIController.instance.CheckUpgLvls();
        }
        else if (SaveData.instance.lvl % 10 == 0 && SaveData.instance.lvl > 0) bodySize.localScale *= 1.8f;
        else bodySize.localScale *= Random.Range(1.0f, 1.15f);
    }

    #region Upgrades:
    public void Upgrades()
    {
        bonusSize = SaveData.instance.bonusSize;
        kickPower = SaveData.instance.kickPower;
        moveSpeed = SaveData.instance.moveSpeed;

        bodySize.localScale = Vector3.one * SaveData.instance.bonusSize;
    }
    public void SetBonusSize(int cost)
    {
        if (SaveData.instance.points >= cost)
        {
            SaveData.instance.points -= cost;
            SaveData.instance.sizeLvl++;
            SaveData.instance.bonusSize = bonusSize += .1f;
            SaveData.instance.SaveGame();
            UIController.instance.CheckUpgLvls();
        }
    }
    public void SetKickPower(int cost)
    {
        if (SaveData.instance.points >= cost)
        {
            SaveData.instance.points -= cost;
            SaveData.instance.kickLvl++;
            SaveData.instance.kickPower = kickPower += .05f;
            SaveData.instance.SaveGame();
            UIController.instance.CheckUpgLvls();
        }
    }
    public void SetMoveSpeed(int cost)
    {
        if (SaveData.instance.points >= cost)
        {
            SaveData.instance.points -= cost;
            SaveData.instance.speedLvl++;
            SaveData.instance.moveSpeed = moveSpeed += 1f;
            SaveData.instance.SaveGame();
            UIController.instance.CheckUpgLvls();
        }
    }
    #endregion

    public void MatchStart()
    {
        pForce = true;
        Timing.RunCoroutine(_PlayerForce().CancelWith(gameObject));

        if (type != Type.player)
            Timing.RunCoroutine(_NPC().CancelWith(gameObject));
    }
    public void MatchEnd()
    {
        pForce = false;
    }
    IEnumerator<float> _PlayerForce()
    {
        while (pForce)
        {
            if (!pForce)
                break;

            //Calculate the movement vector...
            movedPos = transform.position;
            yield return Timing.WaitForSeconds(.1f);
            playerForce = (Mathf.Abs((transform.position.x - movedPos.x) * 10) <= 5) ? (transform.position.x - movedPos.x) * 10 : 5;
        }
    }
    #endregion
    
    private void OnCollisionEnter(Collision ball)
    {
        if(ball.transform.GetComponent<Ball>() != null)
        {
            if(SoundController.instance.soundOn)
                aSource.Play();
            if (type == Type.player)
                ball.transform.GetComponent<Ball>().CheckForTurn(true);
            else
                ball.transform.GetComponent<Ball>().CheckForTurn(false);
        }
    }

    [HideInInspector] public bool isKicking;
    private void OnTriggerEnter(Collider ball)
    {
        if (ball.GetComponent<Ball>() != null)
        {
            ball.GetComponent<Ball>().pSpeed = kickPower;
            if (!isKicking)
                Kick();
        }
    }
    
    public void Kick()
    {
        Timing.RunCoroutine(_KickAnim().CancelWith(gameObject));
    }

    IEnumerator<float> _KickAnim()
    {
        isIdle = false;
        isKicking = true;
        if (transform.position.x > 0) anim.Play("Kick Right");
        else anim.Play("Kick Left");

        yield return Timing.WaitForSeconds(.15f);
        isKicking = false;
        isIdle = true;
    }

    #region END:
    public void PlayerWon()
    {
        anim.Play("Won");
        Timing.RunCoroutine(_End().CancelWith(gameObject));
    }
    public void PlayerLost()
    {
        anim.Play("Lost");
        Timing.RunCoroutine(_End().CancelWith(gameObject));
    }
    IEnumerator<float> _End()
    {
        yield return Timing.WaitForSeconds(.2f);
    }
    #endregion
}
