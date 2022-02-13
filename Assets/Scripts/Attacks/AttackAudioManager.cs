using System;
using System.Linq;
using UnityEngine;

public class AttackAudioManager : MonoBehaviour
{
    private AudioSource source;
    public  AudioClip[] sounds;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(string soundName)
    {
        source.clip = sounds.FirstOrDefault(n => n.name == soundName);
        if (source.clip != null)
        {
            source.Play();
        }
    }
}