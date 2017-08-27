using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bark : MonoBehaviour {

    float elapsed = 0;
    float delay = 0;
	// Use this for initialization
	void Start () {
        elapsed = 0;
        delay = Random.Range(0.0f, 1.0f);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (elapsed < delay)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= delay)
            {
                GetComponent<AudioSource>().Play();
            }
        }
		
	}
}
