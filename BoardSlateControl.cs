using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSlateControl : MonoBehaviour {
    public ushort Index;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(transform.parent != null)
            {
                BoardControl board = transform.parent.gameObject.GetComponent<BoardControl>();
                if(board != null)
                {
                    board.SlateClicked(Index);
                }
            }
        }
    }
}
