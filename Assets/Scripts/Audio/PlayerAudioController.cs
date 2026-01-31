using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] greetAudio;
    public AudioClip[] agreeAudio;
    public AudioClip[] linesAudio;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayGreetAudio()
    {
        int i = Random.Range(0, greetAudio.Length);
        audioSource.PlayOneShot(greetAudio[i]);
    }

    public void PlayAgreeAudio()
    {
        int i = Random.Range(0, agreeAudio.Length);
        audioSource.PlayOneShot(agreeAudio[i]);
    }
    // Update is called once per frame
    void Update()
    {

    }
}