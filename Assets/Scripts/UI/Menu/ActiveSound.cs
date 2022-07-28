using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSound : MonoBehaviour
{
    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private AudioSource source;
    
    void OnEnable()
    {
        source.clip = clip;
        source.Play();
    }
}
