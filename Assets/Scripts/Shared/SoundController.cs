using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundController : MonoBehaviourSingleton<SoundController>
{
    public GameObject Source;

    void Start()
    {

    }

    public AudioSource CreateSource(AudioClip sound)
    {
        GameObject obj = Instantiate(Source);
        DontDestroyOnLoad(obj);
        AudioSource source = obj.GetComponent<AudioSource>();
        source.clip = sound;
        //obj.transform.SetParent(gameObject.transform);
        return source;
    }

    public void Play(AudioClip sound)
    {
        //if (_sources == null) Start();
        AudioSource source = GameObject.FindObjectsOfType<AudioSource>().ToList().Find(x => x.clip == sound);
        //Debug.Log(source);

        if (source == default(AudioSource)) source = CreateSource(sound);
        source.Play();
    }
}
