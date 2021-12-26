using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    #region Singleton
    public static GameController instance;
    [HideInInspector] public List<Ball> balls = new List<Ball>();
    private void Awake()
    {
        if (instance != null)
            return;

        instance = this;
        aSource = GetComponent<AudioSource>();
    }
    #endregion

    AudioSource aSource;
    [Header("Arena:")]
    public GameObject[] arena;
    int mapId;

    private void Start()
    {
        mapId = SaveData.instance.inLeague ? 0 : 1;
        Instantiate(arena[mapId]);

        int mapSize;
        if (SaveData.instance.lvl > 0)
        {
            if (SaveData.instance.lvl % 10 == 0) mapSize = 2;
            else if (SaveData.instance.lvl % 3 == 0) mapSize = 1;
            else mapSize = 0;
        } else mapSize = 0;

        switch (mapSize)
        {
            case 0:
                board.board = XO.Board.Small;
                break;

            case 1:
                board.board = XO.Board.Medium;
                break;

            case 2:
                board.board = XO.Board.Large;
                break;
        }

        board.CreateMap();
    }

    #region Start From Menu:
    bool matchStarted;
    private void Update()
    {
        if (!matchStarted && Mathf.Abs(players[0].joystic.Horizontal) > 0.35f)
        {
            matchStarted = true;
            UIController.instance.OnValueChangedCheck();
        }
    }
    #endregion

    public void StartMatch()
    {
        win = false;

        foreach(Player p in players)
            p.MatchStart();

        Timing.RunCoroutine(board._SummonBall().CancelWith(gameObject));

        if (SaveData.instance.lvl > 1)
            Timing.RunCoroutine(_SpawnPowerups().CancelWith(gameObject));
    }

    #region Powerups:
    float spawnRate;
    float spawnChance;
    bool canSpawn;
    IEnumerator<float> _SpawnPowerups()
    {
        if (!win)
        {
            spawnRate = Random.Range(1.0f, 5.0f);
            spawnChance = Random.Range(.0f, 1f);
            yield return Timing.WaitForSeconds(spawnRate);
            canSpawn = spawnChance >= .35f ? true : false;
            Powerup.instance.SpawnPowerup(canSpawn);

            if (!win)
                Timing.RunCoroutine(_SpawnPowerups().CancelWith(gameObject));
        }
    }
    #endregion

    [Header("Turn Data:")]
    public Player[] players = new Player[2];    //Player =  players[0]; NPC = players[1]; Just so you know >:)
    public bool playerTurn;
    public bool xTurn = true;
    [Space]
    public Ball ball;
    public Vector3 ballSpawnPos = new Vector3(0, 1.25f, 0);

    [Header("VFX:")]
    public GameObject ballVfx;
    public void CreateBall()
    {
        if (!win && balls.Count == 0)
            Timing.RunCoroutine(_CreateBall().CancelWith(gameObject));
    }

    [Header("Board:")]
    public XO board;
    public List<Node> activeNodes = new List<Node>();
    public List<Node> usedNodes = new List<Node>();
    
    /// <summary>
    /// Call this when ball hits the wall => from Wall.cs
    /// </summary>
    public void RandomField(bool playerWall, Ball b)
    {
        //Turn the node based on playerWall bool -.-
        if (activeNodes.Count > 0 || !win)
        {
            xTurn = playerWall;
            int rand = Random.Range(0, activeNodes.Count);

            activeNodes[rand].ball = b;
            b.xTurn = playerWall;
            activeNodes[rand].SetNode();
        }
    }
    IEnumerator<float> _CreateBall()
    {
        yield return Timing.WaitForSeconds(.5f);
        playerTurn = !playerTurn;
        Instantiate(ballVfx, ballSpawnPos, Quaternion.identity);
        GameObject Ball = Instantiate(ball.gameObject, ballSpawnPos, Quaternion.identity);
        Ball b = Ball.GetComponent<Ball>();
        balls.Add(b);

        //Ball Setup:
        if (playerTurn) b.GotTarget(players[0]); else b.GotTarget(players[1]);
        b.CheckForTurn(playerTurn);

        if (SoundController.instance.soundOn)
        {
            if (!aSource.isPlaying)
            {
                aSource.time = .2f;
                aSource.Play();
            }
            yield return Timing.WaitForSeconds(1.25f);
            aSource.Stop();
        }
    }

    #region WIN:
    [HideInInspector] public bool win;

    public void CheckForWin()
    {
        for(int n = 0; n < usedNodes.Count; n++)
        {
            if (win) break;
            else if (usedNodes[n].type != XOType.None)
            {
                usedNodes[n].CheckWin();
            }
        }
        if (!win)
            if (activeNodes.Count <= 0)
                GameEnded(XOType.None);
    }

    [Header("Confetti:")]
    public GameObject conf;

    public void GameEnded(XOType t)
    {
        //Game Over! :)
        win = true;
        if (t != XOType.None)
        {
            //Collect pts... etc...
            //We have a winner!
            if (t == XOType.X)
            {
                Timing.RunCoroutine(_PlayerWon().CancelWith(gameObject));
                Instantiate(conf);
                players[0].PlayerWon();
                players[1].PlayerLost();
            }
            if (t == XOType.O)
            {
                Timing.RunCoroutine(_NpcWon().CancelWith(gameObject));
                players[1].PlayerWon();
                players[0].PlayerLost();
            }
        } else {
            UIController.instance.Draw();
        }

        UIController.instance.GameEnded();
    }
    #endregion

    IEnumerator<float> _PlayerWon()
    {
        yield return Timing.WaitForSeconds(.5f);
        UIController.instance.PlayerWon();
    }

    IEnumerator<float> _NpcWon()
    {
        yield return Timing.WaitForSeconds(.5f);
        UIController.instance.NpcWon();
    }

    public void LoadNextLevel()
    {
        Timing.RunCoroutine(_LoadWaitTime().CancelWith(gameObject));
    }

    IEnumerator<float> _LoadWaitTime()
    {
        yield return Timing.WaitForSeconds(.75f);
        if (SaveData.instance.lvl % 10 == 1 && SaveData.instance.lvl > 1 && !SaveData.instance.bonusFix)
        {
            SaveData.instance.bonusFix = true;
            SaveData.instance.SaveGame();
            SceneManager.LoadSceneAsync(1);
        }
        else
        {
            if (SaveData.instance.bonusFix && SaveData.instance.lvl % 10 != 1)
            {
                SaveData.instance.bonusFix = false;
                SaveData.instance.SaveGame();
            }

            SceneManager.LoadSceneAsync(0);
        }
    }

    #region Power:
    public void Twice()
    {
        if (!win)
            Timing.RunCoroutine(_CreateBall().CancelWith(gameObject));
    }
    #endregion

    [Header("Skins Unlocked:")]
    public List<int> bodyUnlocked = new List<int>();    //From 0 to 8
}
