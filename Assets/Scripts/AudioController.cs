using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioClip AudioInvalidMove;
    [SerializeField] private AudioClip AudioPieceMove;
    [SerializeField] private AudioClip AudioPieceBreak;

    private AudioSource audio;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            GetComponent<Animator>().SetTrigger("go");
            PieceDestroyAudio();
        }
    }

    public void Kill() { }

    private void OnEnable()
    {
        audio = GetComponent<AudioSource>();
    }

    public void InvalidMove()
    {
        audio.clip = AudioInvalidMove;
        audio.PlayScheduled(0);
    }

    public void PieceDestroyAudio()
    {
        audio.clip = AudioPieceBreak;
        audio.PlayScheduled(0);
    }

    public void PieceMoveAudio(float time, float fadeTime)
    {
        StartCoroutine(IPieceMoveAudio(time, fadeTime));
    }

    IEnumerator IPieceMoveAudio(float time, float fadeTime)
    {
        AudioSource _audio = gameObject.AddComponent<AudioSource>();
        _audio.spatialBlend = 1;
        _audio.clip = AudioPieceMove;

        float targetVolume = 1;
        float timer = 0;

        // Fade in from a random moment between 0 and 1,5 seconds
        _audio.volume = 0;
        _audio.pitch = Random.Range(0.95f, 1.05f);
        float startValue = Random.Range(0.0f, 1.5f);

        _audio.time = startValue;
        _audio.PlayScheduled(0);
        _audio.SetScheduledEndTime(AudioSettings.dspTime + (14.57f - 13.21f) + time);


        while (_audio.volume < targetVolume - 0.01f)
        {
            _audio.volume = (timer / fadeTime) * targetVolume;
            timer += Time.deltaTime;

            yield return null;
        }

        // Wait until it's almost done
        while (timer < time - fadeTime)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        // Fade out
        while( _audio.volume > 0.01f)
        {
            _audio.volume = (1 - (timer - (time - fadeTime)) / fadeTime) * targetVolume;
            timer += Time.deltaTime;

            yield return null;
        }

        _audio.Stop();
        _audio.volume = targetVolume;
        _audio.pitch = 1;

        Destroy(_audio);
    }
}
