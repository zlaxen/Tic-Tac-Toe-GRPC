using Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardControl : MonoBehaviour {
    public GameObject win1;
    public GameObject win2;

    public int WhoseTurns = 1;
    public int player1Win;
    public int player2Win;

    [SerializeField]
    private GameObject SlateSample;

    [SerializeField]
    private GameObject MyPawnSample;

    [SerializeField]
    private GameObject HisPawnSample;

    private GameObject[] SlateGameObjects;
    ushort[] SlateIn = new ushort[9];
	// Use this for initialization
	void Start () {
        Vector3[] positions = new Vector3[]
        {
            new Vector3(-1.0f, 1.0f),
            new Vector3(0.0f, 1.0f),
            new Vector3(1.0f, 1.0f),

            new Vector3(-1.0f, 0.0f),
            new Vector3(0.0f, 0.0f),
            new Vector3(1.0f, 0.0f),

            new Vector3(-1.0f, -1.0f),
            new Vector3(0.0f, -1.0f),
            new Vector3(1.0f, -1.0f),
        };

        SlateGameObjects = new GameObject[9];

        for (int i = 0; i < 9; i++)
        {
            GameObject slate = Instantiate(SlateSample);

            SlateGameObjects[i] = slate;

            slate.transform.parent = gameObject.transform;
            slate.transform.position = positions[i];

            BoardSlateControl slateControl = slate.GetComponent<BoardSlateControl>();
            if(slateControl != null)
            {
                slateControl.Index = (ushort)i;
            }

            for (int j = 0; j < 9; j++)
            {
                SlateIn[j] = slateControl.Index;
            }
        }

        MatchModel.currentMatch.OnBoardChange.AddListener(OnBoardChanged);
	}

    private ushort SlateIndex;
    private MatchModel.SlateStatus SlateStatus = MatchModel.SlateStatus.NONE;
    private void OnBoardChanged(ushort slateIndex, MatchModel.SlateStatus slateStatus)
    {
        Debug.Log("Board Changed " + slateIndex + " Changed To " + slateStatus);
        SlateIndex = slateIndex;
        SlateStatus = slateStatus;
    }

    // Update is called once per frame 
    public void SlateClicked (ushort slateIndex) {
        MatchModel.currentMatch.ReportSlateTaken(slateIndex);
        player1Win += 1;
	}

    void Update()
    {
        if(SlateStatus != MatchModel.SlateStatus.NONE)
        {
            GameObject pawnGo;
            if (SlateStatus == MatchModel.SlateStatus.MINE)
            {
                pawnGo = Instantiate(MyPawnSample);
                //player1Win += 1;
            }
            else
            {
                pawnGo = Instantiate(HisPawnSample);
                player2Win += 1;
            }

            GameObject slate = SlateGameObjects[SlateIndex];
            pawnGo.transform.parent = slate.transform;
            pawnGo.transform.localPosition = new Vector3();

            SlateStatus = MatchModel.SlateStatus.NONE;
        }

        if(player1Win >= 3)
        {
            win1.SetActive(true);
            Debug.Log("Player 1 Win");
        }
        else if(player2Win >= 3)
        {
            win2.SetActive(true);
            Debug.Log("Player 2 WIn");
        }
        WinnerCheck();
    }

    void WinnerCheck()
    {
        int s1 = SlateIn[0] + SlateIn[1] + SlateIn[2];
        int s2 = SlateIn[3] + SlateIn[4] + SlateIn[5];
        int s3 = SlateIn[6] + SlateIn[7] + SlateIn[8];
        int s4 = SlateIn[0] + SlateIn[3] + SlateIn[6];
        int s5 = SlateIn[1] + SlateIn[4] + SlateIn[7];
        int s6 = SlateIn[2] + SlateIn[5] + SlateIn[8];
        int s7 = SlateIn[0] + SlateIn[4] + SlateIn[8];
        int s8 = SlateIn[2] + SlateIn[4] + SlateIn[6];

        var solutions = new int[] { s1, s2, s3, s4, s5, s6, s7, s8 };
        for (int i = 0; i < solutions.Length; i++)
        {
            if (solutions[i] == 3 * (WhoseTurns + 1))
            {
                Debug.Log("Player" + WhoseTurns + "Win");
            }
        }
    }
}
