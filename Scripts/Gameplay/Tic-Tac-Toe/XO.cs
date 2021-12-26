using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;


public class XO : MonoBehaviour
{
    public enum Board
    {
        Small,
        Medium,
        Large
    }
    [Header("Board (small = 3x3, medium = 4x4, large = 5x5):")]
    public Board board;
    private int boardSize = 25;  //The actual size is (n+2)x(n+2) => 3x3 board is actually a 5x5 board => 25 fields.
    [Space]
    public List<Node> nodes = new List<Node>();

    [Header("Node Object:")]
    public Node node;

    [Header("Boards:")]
    public GameObject[] xoBoard = new GameObject[3];

    //Node distance range is [x = z = (+-) 0.325;]
    public void CreateMap()
    {
        #region Main Data:
        float minusNum = 0;
        #endregion

        #region Board Setup:
        //Remember to add the "Board Grid Lines" depending on the boardSize... >XD
        //It can be just a simple list of game objects... 3 types idk :S
        //Set ti to the center of the map and you gucci C:
        switch (board)
        {
            case Board.Small:
                boardSize = 25;
                minusNum = -2;
                GameObject map = Instantiate(xoBoard[0], new Vector3(0, -.5f, 0), Quaternion.identity);
                map.transform.localScale = Vector3.zero;
                LeanTween.scale(map, xoBoard[0].transform.localScale, .75f).setEaseOutBounce();
                break;

            case Board.Medium:
                boardSize = 36;
                minusNum = -2.5f;
                GameObject map2 = Instantiate(xoBoard[1], new Vector3(0, -.5f, 0), Quaternion.identity);
                map2.transform.localScale = Vector3.zero;
                LeanTween.scale(map2, xoBoard[1].transform.localScale, .5f).setEaseOutBounce();
                break;

            case Board.Large:
                boardSize = 49;
                minusNum = -3;
                GameObject map3 = Instantiate(xoBoard[2], new Vector3(0, -.5f, 0), Quaternion.identity);
                map3.transform.localScale = Vector3.zero;
                LeanTween.scale(map3, xoBoard[2].transform.localScale, .5f).setEaseOutBounce();
                break;

            default:
                break;
        }
        float n = Mathf.Sqrt(boardSize);
        float m = n - 2;
        transform.position = new Vector3(-(n + minusNum), transform.position.y, -(n + minusNum));
        #endregion

        #region Define Nodes:
        //Calculate visible ones => add mesh filter, renderer and box collider => trigger... :/
        for (int i = 0; i < boardSize; i++)
        {
            GameObject testGNode = Instantiate(node.gameObject);
            Node nodeUsed = testGNode.GetComponent<Node>();
            nodeUsed.name = $"Field: {i + 1}";
            nodeUsed.id = i + 1;

            nodeUsed.transform.parent = transform;
            nodes.Add(nodeUsed);

            //Calculation: (m + 3) + m*(m+2) + (m + 3)
            if (i < m + 3 || i >= (m + 3) + m * (m + 2))
                testGNode.SetActive(false);

            
        }

        for (int num = 1; num < n; num++)
        {
            nodes[num * (int)n].gameObject.SetActive(false);
            nodes[(num * (int)n) - 1].gameObject.SetActive(false);
        }
        #endregion

        #region Node Pos:
        int testId = 0;
        for (int z = 1; z <= Mathf.Sqrt(boardSize); z++)
        {
            for (int x = 1; x <= Mathf.Sqrt(boardSize); x++)
            {
                nodes[testId].transform.localPosition = new Vector3(x, 0, z);
                testId++;
            }
        }
        #endregion

        CalculateNodeNeighbors((int)n);
    }

    public IEnumerator<float> _SummonBall()
    {
        yield return Timing.WaitForSeconds(.75f);
        GameController.instance.CreateBall();
    }

    #region Calculate Neighbours:
    private void CalculateNodeNeighbors(int n)
    {
        foreach(var nds in nodes)
        {
            if (nds.gameObject.activeSelf)
            {
                int dl = nds.id - (n + 1);
                int d = nds.id - n;
                int dr = nds.id - (n - 1);
                int l = nds.id - 1;
                int r = nds.id + 1;
                int ul = nds.id + (n - 1);
                int u = nds.id + n;
                int ur = nds.id + (n + 1);

                nds.SetNeighbours(
                    nodes[dl - 1], nodes[d - 1], nodes[dr - 1],
                    nodes[l - 1],
                    nodes[r - 1], 
                    nodes[ul - 1], nodes[u - 1], nodes[ur - 1]);

                GameController.instance.activeNodes.Add(nds);
            }
        }
    }
    #endregion
}
