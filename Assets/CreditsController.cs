using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
    public float delay = 0.2f;
    public float autoTransitionDelay = 10.0f;
    float elapsed = 0;
	// Use this for initialization
	void Start () {
        Cursor.visible = true;
        elapsed = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (elapsed < autoTransitionDelay)
        {
            elapsed += Time.deltaTime;
            if (elapsed < delay)
            {
                return;
            }
            if (Input.anyKeyDown)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
		
	}
}
