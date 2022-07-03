using System;
using System.Linq;
using UnityEngine;

public class AttackAudioManager : MonoBehaviour
{
    private AudioSource source;
    [SerializeField]
    private AudioClip[] sounds;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        WorldRules.volume = PlayerPrefs.GetFloat("Volume");
        source.volume = WorldRules.volume;
    }

    // Update is called once per frame
    void Update()
    {
        if (source.volume != WorldRules.volume)
        {
            source.volume = WorldRules.volume / 100.0f;
            PlayerPrefs.SetFloat("Volume", WorldRules.volume);
        }
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