using System;
using System.Linq;
using UnityEngine;

public class AttackAudioManager : MonoBehaviour
{
    private AudioSource[] source;
    [SerializeField]
    private AudioClip[] sounds;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponents<AudioSource>();
        WorldRules.volume = PlayerPrefs.GetFloat("Volume");
        source[0].volume = WorldRules.volume;
        source[1].volume = WorldRules.volume;
    }

    // Update is called once per frame
    void Update()
    {
        if (source[0].volume != WorldRules.volume)
        {
            source[0].volume = WorldRules.volume / 100.0f;
            PlayerPrefs.SetFloat("Volume", WorldRules.volume);
        }
        if (source[1].volume != WorldRules.volume)
        {
            source[1].volume = WorldRules.volume / 100.0f;
            PlayerPrefs.SetFloat("Volume", WorldRules.volume);
        }
    }

    public void PlaySound(string soundName, int index)
    {
        source[index].clip = sounds.FirstOrDefault(n => n.name == soundName);
        if (source[index].clip != null)
        {
            source[index].Play();
        }
    }
}