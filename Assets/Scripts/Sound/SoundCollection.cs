using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCollection : MonoBehaviourSingletonPersistent<SoundCollection>
{
    public List<AudioClip> Sounds;

    public void Start()
    {
        SoundController.Init();
    }

    public AudioClip GetSound(string name)
    {
        return Sounds.Find(x => x.ToString().Split(' ')[0] == name);
    }
}
