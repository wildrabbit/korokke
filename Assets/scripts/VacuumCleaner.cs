using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumCleaner : MonoBehaviour
{
    AudioSource leAudio;
    private void Awake()
    {
        leAudio = GetComponent<AudioSource>();
    }
    public void OnFinished()
    {
        leAudio.Stop();
        gameObject.SetActive(false);
    }

    public void PlayAudio()
    {
        leAudio.Play();        
    }
}
