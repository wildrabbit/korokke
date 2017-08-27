using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    public Sprite defaultSp;
    public Sprite availableSp;

    bool onTarget;
    Transform target;

    public Image img;
	// Use this for initialization
	void Awake ()
    {
        onTarget = false;
        Cursor.visible = false;	
	}

    public void SetOnTarget(Transform pos)
    {
        if (pos == target) return;

        target = pos;
        onTarget = target != null;

        img.sprite = (onTarget) ? availableSp : defaultSp;
    }
	
	// Update is called once per frame
	void Update ()
    {
        img.transform.position = Input.mousePosition;
        if (onTarget)
        {
            img.transform.Rotate(Vector3.forward, 100 * Time.deltaTime);
        }        
	}
}
