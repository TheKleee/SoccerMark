using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Ball : MonoBehaviour
{
    //The ball has lives... ~ 5? => resets after hitting the player
    //The next turn can start when there are no more balls on the field => list of balls is empty!
    //Ball bounces of off anything that doesn't destroy it...
    //Player characters bounce the ball differently => by adding a an impulse upwards (and slowing it down so it can hit center?)...
    AudioSource aSource;

    private Rigidbody rb;
    [Header("Force:")]
    [Range(0, 10)] public float wallForce; //When bouncing off a wall... 

    [Header("Colors:")]
    public Gradient[] col = new Gradient[2];

    #region Target:
    private Transform target;
    private bool gotTarget;

    [Header("Life:")]
    public float lifetime = 2.5f;
    [Range(5, 10)] public int maxHp = 10;
    [SerializeField] private int hp;
    public void GotTarget(Player Target)
    {
        target = Target.transform;
        int minus = GameController.instance.playerTurn ? -1 : 1;
        rb.AddForce(minus * transform.forward * wallForce, ForceMode.Impulse);
    }
    #endregion

    private void Awake()
    {
        saveMaxSpeed = maxSpeed;
        hp = maxHp;
        rb = GetComponent<Rigidbody>();
        aSource = GetComponent<AudioSource>();
        trail = transform.GetChild(0);
        Timing.RunCoroutine(_LifeDuration().CancelWith(gameObject));

        //Get the skins! >:]
        CheckSkins();
    }

    private void FixedUpdate()  //Delete Later!!!
    {
        if (!gotTarget && target != null)  //Delete Later!!!
            transform.position = Vector3.Lerp(transform.position, target.position, 7.5f * Time.deltaTime);  //Delete Later!!!

        if (hp == 0 || lifetime <= 0 || transform.position.y < -1f)
        {
            DestroyBall();
            GameController.instance.CreateBall();
        }

        //Test:
        if (rb.velocity.magnitude >= maxSpeed)
        {
            rb.velocity *= 0.9f;
        }
    }

    [HideInInspector] public bool xTurn;
    private int lifeTick;
    /// <summary>
    /// Call this from player.cs
    /// </summary>
    public void CheckForTurn(bool player)
    {
        lifeTick = player == xTurn ? ++lifeTick : 0;
        if (lifeTick == 3)
        {
            DestroyBall();
            GameController.instance.CreateBall();
            return;
        }

        GameController.instance.xTurn = player;
        if (player)
        {
            xTurn = true;
            trail.GetComponent<TrailRenderer>().colorGradient = col[1];
            skin.transform.GetChild(0).gameObject.SetActive(true);
            skin.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            xTurn = false;
            trail.GetComponent<TrailRenderer>().colorGradient = col[0];
            skin.transform.GetChild(1).gameObject.SetActive(true);
            skin.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    //This needs to be tested.. a lot -.-
    #region Bounce Off:
    Vector3 lastVelocity;
    float speed;
    bool bounced;
    [SerializeField]/*[Range(5.0f, 12.0f)]*/float maxSpeed = 10.0f;
    float saveMaxSpeed;
    [HideInInspector] public bool isDestroyed;
    [HideInInspector] public float pSpeed;
    private void OnCollisionEnter(Collision dir)
    {
        if (!isDestroyed)
        {
            lifetime += .25f;
            Vector3 direction = Vector3.Reflect(lastVelocity.normalized, dir.contacts[0].normal);
            if (dir.transform.GetComponent<Player>() == null)
            {
                hp--;
                if (!bounced)
                {
                    Timing.RunCoroutine(_BounceSpeed().CancelWith(gameObject));
                }
                speed = speed >= maxSpeed ? maxSpeed : rb.velocity.magnitude * wallForce / 5f;

                rb.AddForce(direction * Mathf.Max(speed, 0f), ForceMode.VelocityChange);

                Instantiate(bounceFx, transform.position, Quaternion.identity);

                if (!aSource.isPlaying && SoundController.instance.soundOn)
                    Timing.RunCoroutine(_AudioControl().CancelWith(gameObject));
            }
            else
            {
                if (!dir.transform.GetComponent<Player>().isKicking) dir.transform.GetComponent<Player>().Kick();
                hp = maxHp;
                lifetime += .75f;
                if (!gotTarget)
                    gotTarget = true;

                rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero;

                Vector3 playerDir = new Vector3(dir.transform.GetComponent<Player>().playerForce, 0, 0);
                if (playerDir.x == 0f) playerDir = Vector3.zero;

                Instantiate(playerHitFx, transform.position, Quaternion.identity);

                rb.AddForce((dir.transform.up + direction + playerDir * 1.2f) * pSpeed, ForceMode.Impulse);
                BounceOff();
            }
        }
    }

    IEnumerator<float> _AudioControl()
    {
        aSource.time = .35f;
        aSource.Play();
        yield return Timing.WaitForSeconds(.25f);
        aSource.Stop();

    }
    private void BounceOff()
    {
        //transform.rotation = Quaternion.AngleAxis(Random.Range(90, 120), Vector3.up);
        rb.AddForce((Vector3.up - transform.position) * 1.25f, ForceMode.VelocityChange);
    }

    IEnumerator<float> _BounceSpeed()
    {
        bounced = true;
        do
        {
            if (transform.position.y >= 1.5f)
            {
                //rb.velocity = Vector3.zero;
                rb.AddForce(-Vector3.up * 1.5f, ForceMode.Impulse);
            }
            //if (rb.velocity.magnitude >= 7f) rb.velocity = Vector3.one;
            lastVelocity = rb.velocity;
            yield return 0;
        } while (true);
    }
    #endregion

    IEnumerator<float> _LifeDuration()
    {
        do
        {
            lifetime--;
            yield return Timing.WaitForSeconds(1f);
        } while (lifetime > 0);
    }

    private Transform trail;

    [Header("VFX:")]
    [SerializeField] private GameObject vfx;
    [Space]
    [SerializeField] private GameObject playerHitFx;
    [SerializeField] private GameObject bounceFx;

    public void DestroyBall()
    {
        trail.SetParent(null);
        trail.GetComponent<TrailRenderer>().autodestruct = true;

        //Instantiate vfx...
        Instantiate(vfx, transform.position, Quaternion.identity);

        if (sVfxList.Count > 0)
        {
            sVfxList[0].transform.parent = null;
            sVfxList[0].GetComponent<TrailRenderer>().time = 1;
            sVfxList.Clear();
        }

        isDestroyed = true;
        GameController.instance.balls.Remove(this);
        Destroy(gameObject);

    }

    #region Powers:

    public void Large()
    {
        Vector3 large = transform.localScale * 2.5f;
        LeanTween.scale(gameObject, large, 0.45f);
        Timing.RunCoroutine(_Large().CancelWith(gameObject));
    }
    IEnumerator<float> _Large()
    {
        yield return Timing.WaitForSeconds(1.25f);
        Vector3 large = transform.localScale / 2.5f;
        LeanTween.scale(gameObject, large, 0.45f);
    }
    [Header("Speed Vfx:")]
    public GameObject speedVfx;
    List<GameObject> sVfxList = new List<GameObject>();
    public void Speed()
    {
        maxSpeed *= 2.5f;
        GameObject sVfx = Instantiate(speedVfx, transform);
        sVfx.transform.localPosition = Vector3.zero;
        sVfxList.Add(sVfx);
        Timing.RunCoroutine(_Speed().CancelWith(gameObject));
    }
    IEnumerator<float> _Speed()
    {
        Vector3 formerPos = transform.position;
        yield return Timing.WaitForSeconds(.1f);
        Vector3 direction = transform.position - formerPos;
        rb.AddForce(direction.normalized * Mathf.Max(2 * speed, 0f), ForceMode.VelocityChange);
        yield return Timing.WaitForSeconds(1.25f);
        maxSpeed = saveMaxSpeed;
        sVfxList[0].transform.parent = null;
        sVfxList[0].GetComponent<TrailRenderer>().time = .5f;
        sVfxList.Clear();
    }

    #endregion



    #region Skins:
    [Header("Skins:")]
    public GameObject[] body = new GameObject[9];
    private GameObject oldBody;         //Destroy this when creating a new one!!!
    GameObject skin;
    public void CheckSkins()
    {
        if (oldBody != null)
            Destroy(oldBody);   //We !NO! need !!OLD!! and !!!FRAIL!!! >:C

        skin = Instantiate(body[SaveData.instance.bodyId], transform, false);  //This needs to be tested... I've never set parent from Instante() before >:O
        skin.transform.localPosition = new Vector3(0, skin.transform.localPosition.y, 0);
        oldBody = skin;
    }
    #endregion
}
