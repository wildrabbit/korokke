using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

public class HintControl : MonoBehaviour
{
    AudioSource tapSource;
    public Animator sceneAnim;
    public GameObject hint;
    public float hintDelay = 3.0f;

    float elapsed = 0.0f;
    bool tapped = false;


    TapGesture tap;
	// Use this for initialization
	void Start ()
    {
        Cursor.visible = true;
        tap = GetComponent<TapGesture>();
        tapSource = GetComponent<AudioSource>();
        elapsed = 0.0f;
        tapped = false;
        tap.Tapped += PlateTapped;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (!tapped && elapsed < hintDelay) 
        {
            elapsed += Time.deltaTime;
            if (elapsed > hintDelay)
            {
                elapsed = hintDelay;
                hint.gameObject.SetActive(true);
            }
        }
	}

    public void PlateTapped(object o, System.EventArgs a)
    {
        tap.Tapped -= PlateTapped;
        tapSource.Play();
        sceneAnim.SetTrigger("Tapped");
    }
}
