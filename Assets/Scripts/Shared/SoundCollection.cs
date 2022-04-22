using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCollection : MonoBehaviourSingletonPersistent<SoundCollection>
{
    public List<AudioClip> Sounds;

    public AudioClip GetSound(string name)
    {
        //Debug.Log(name);
        //Debug.Log(Sounds[2].ToString().Split(' ')[0]);
        return Sounds.Find(x => x.ToString().Split(' ')[0] == name);
    }
}
