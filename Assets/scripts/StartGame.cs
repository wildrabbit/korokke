using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour {

    public void ChangeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("game");
    }
}
