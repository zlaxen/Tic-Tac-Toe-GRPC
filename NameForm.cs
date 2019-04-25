using Script.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NameForm : MonoBehaviour {
    [SerializeField]
    public TMPro.TMP_InputField InputField;
    private bool IsSubmiting = false;
    private bool LoadingScene = false;
	// Use this for initialization
	void Start () {
        if (InputField == null)
        {
            throw new System.Exception("Missing Input Field");
        }
	}

    private void Update()
    {
        if(LoadingScene == false && NetworkingManager.Instance.GotMatch)
        {
            LoadingScene = true;
            SceneManager.LoadScene("Play");
        }
    }

    // Update is called once per frame
    public void OnSubmitClick () {
        
        if (IsSubmiting)
        {
            return;
        }

        string name = InputField.text.Trim();

        if (name == string.Empty)
        {
            return;
        }

        if (NetworkingManager.Instance.ConnectionState == DarkRift.ConnectionState.Connected||NetworkingManager.Instance.Connect())
        {
            IsSubmiting = true;
            NetworkingManager.Instance.MessageNameToServer(name);
        }
	}
}
