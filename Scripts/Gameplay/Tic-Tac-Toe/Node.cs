using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum XOType
{
    None,
    X,
    O
}
public class Node : MonoBehaviour
{
    AudioSource aSource;
    private void Awake()
    {
        aSource = GetComponent<AudioSource>();
    }

    [Header("ID")]
    public int id;

    [Header("Neighbours:")]
    public Node dl;
    public Node d;
    public Node dr;
    public Node l;
    public Node r;
    public Node ul;
    public Node u;
    public Node ur;
    [Space]
    public bool downLeft;
    public bool down;
    public bool downRight;
    public bool left;
    public bool right;
    public bool upLeft;
    public bool up;
    public bool upRight;

    [Header("Type:")]
    public XOType type;

    [Header("Vfx")]
    public GameObject vfx;
    [Space]
    public GameObject winTrail;  //This will be something else...

    public void SetNeighbours(Node DL, Node D, Node DR, Node L, Node R, Node UL, Node U, Node UR)
    {
        dl = DL;
        d = D;
        dr = DR;
        l = L;
        r = R;
        ul = UL;
        u = U;
        ur = UR;
    }
    [HideInInspector] public Ball ball;

    [Header("Materials:")]
    public Material[] mat = new Material[2];  //mat[0] => player; mat[1] => npc
    private Material ballMat;
    private void OnTriggerEnter(Collider Ball)
    {
        if (Ball.GetComponent<Ball>() != null)
        {
            ball = Ball.GetComponent<Ball>();
            SetNode();
        }
    }

    public void SetNode()
    {
        down = left = right = downLeft = upLeft = downRight = upRight = up = true;

        GetComponent<BoxCollider>().enabled = false;

        type = ball.xTurn ? XOType.X : XOType.O;
        ballMat = ball.xTurn ? mat[1] : mat[0];

        UIController.instance.SetPlayerPts(ball.xTurn, 2);

        ball.DestroyBall();

        GetComponent<MeshRenderer>().material = ballMat;

        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, new Vector3(.55f, .55f, .55f), 0.5f).setEaseOutElastic();
        Instantiate(vfx, transform.position + new Vector3(0, 1, 0), Quaternion.identity);

        //Set the field... and all that good stuff :|
        GameController.instance.activeNodes.Remove(this);
        GameController.instance.usedNodes.Add(this);
        GameController.instance.CheckForWin();

        GameController.instance.CreateBall();
        if (SoundController.instance.soundOn)
            aSource.Play();
    }

    public void CheckWin()
    {
        //Check the sourrounding nodes:
        #region Check All Nodes:
        if(!GameController.instance.win)
        {
            if (type != XOType.None)
            {
                //Down Check:
                if (dl)
                    if (dl.downLeft && dl.type == type)
                    {
                        if (dl.dl.downLeft && dl.dl.type == type)
                        {
                            //Create trail vfx with the animation going from transform.position to dl.dl.transform.position...
                            GameObject win = Instantiate(winTrail);
                            win.transform.SetParent(transform.parent);
                            win.transform.localPosition = transform.localPosition + new Vector3(0, .35f, 0);
                            WinSlash slash = win.GetComponent<WinSlash>();
                            slash.SetTarget(dl.dl.gameObject.transform.localPosition, type);

                            GameController.instance.GameEnded(type);
                            return;
                        }
                    }
                if (down)
                    if (d.down && d.type == type)
                    {
                        if (d.d.down && d.d.type == type)
                        {
                            GameObject win = Instantiate(winTrail);
                            win.transform.SetParent(transform.parent);
                            win.transform.localPosition = transform.localPosition + new Vector3(0, .35f, 0);
                            WinSlash slash = win.GetComponent<WinSlash>();
                            slash.SetTarget(d.d.gameObject.transform.localPosition, type);

                            GameController.instance.GameEnded(type);
                            return;
                        }
                    }
                if (downRight)
                    if (dr.downRight && dr.type == type)
                    {
                        if (dr.dr.downRight && dr.dr.type == type)
                        {
                            GameObject win = Instantiate(winTrail);
                            win.transform.SetParent(transform.parent);
                            win.transform.localPosition = transform.localPosition + new Vector3(0, .35f, 0);
                            WinSlash slash = win.GetComponent<WinSlash>();
                            slash.SetTarget(dr.dr.gameObject.transform.localPosition, type);

                            GameController.instance.GameEnded(type);
                            return;
                        }
                    }

                //Mid Check:
                if (left)
                    if (l.left && l.type == type)
                    {
                        if (l.l.left && l.l.type == type)
                        {
                            GameObject win = Instantiate(winTrail);
                            win.transform.SetParent(transform.parent);
                            win.transform.localPosition = transform.localPosition + new Vector3(0, .35f, 0);
                            WinSlash slash = win.GetComponent<WinSlash>();
                            slash.SetTarget(l.l.gameObject.transform.localPosition, type);

                            GameController.instance.GameEnded(type);
                            return;
                        }
                    }
                if (right)
                    if (r.right && r.type == type)
                    {
                        if (r.r.right && r.r.type == type)
                        {
                            GameObject win = Instantiate(winTrail);
                            win.transform.SetParent(transform.parent);
                            win.transform.localPosition = transform.localPosition + new Vector3(0, .35f, 0);
                            WinSlash slash = win.GetComponent<WinSlash>();
                            slash.SetTarget(r.r.gameObject.transform.localPosition, type);

                            GameController.instance.GameEnded(type);
                            return;
                        }
                    }

                //Up Check:
                if (upLeft)
                    if (ul.upLeft && ul.type == type)
                    {
                        if (ul.ul.upLeft && ul.ul.type == type)
                        {
                            GameObject win = Instantiate(winTrail);
                            win.transform.SetParent(transform.parent);
                            win.transform.localPosition = transform.localPosition + new Vector3(0, .35f, 0);
                            WinSlash slash = win.GetComponent<WinSlash>();
                            slash.SetTarget(ul.ul.gameObject.transform.localPosition, type);

                            GameController.instance.GameEnded(type);
                            return;
                        }
                    }
                if (up)
                    if (u.up && u.type == type)
                    {
                        if (u.u.up && u.u.type == type)
                        {
                            GameObject win = Instantiate(winTrail);
                            win.transform.SetParent(transform.parent);
                            win.transform.localPosition = transform.localPosition + new Vector3(0, .35f, 0);
                            WinSlash slash = win.GetComponent<WinSlash>();
                            slash.SetTarget(u.u.gameObject.transform.localPosition, type);

                            GameController.instance.GameEnded(type);
                            return;
                        }
                    }
                if (upRight)
                    if (ur.upRight && ur.type == type)
                    {
                        if (ur.ur.upRight && ur.ur.type == type)
                        {
                            GameObject win = Instantiate(winTrail);
                            win.transform.SetParent(transform.parent);
                            win.transform.localPosition = transform.localPosition + new Vector3(0, .35f, 0);
                            WinSlash slash = win.GetComponent<WinSlash>();
                            slash.SetTarget(ur.ur.gameObject.transform.localPosition, type);

                            GameController.instance.GameEnded(type);
                            return;
                        }
                    }
            }
        } 

        #endregion

        //If you won set win to true!
        //GameController.instance.win = true;
    }
}
