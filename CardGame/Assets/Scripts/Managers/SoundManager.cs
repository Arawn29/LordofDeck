using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    List<AudioSource> sources = new List<AudioSource>();
    [SerializeField] private  List<AudioClip> Clips = new List<AudioClip>();

    //----------------------------------------------------------//
    public static float MainSoundVolume;
    public static float OtherSoundVolume;
    public Slider MainSlider;
    public Slider OtherSlider;
    public AudioSource mainSound;

    public GameObject SoundPanel;
    public GameObject MainSoundIconObj;
    public GameObject OtherSoundIconObj;
    public Sprite VolumeIconSprite;
    public Sprite MuteIconSprite;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        DontDestroyOnLoad(Instance);
        mainSound.volume = MainSlider.value;
    }
    private void Start()
    {
        MainSlider.value = MainSoundVolume;
        OtherSlider.value = OtherSoundVolume;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneChanged;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneChanged;
    }
    private void OnSceneChanged(Scene arg0, LoadSceneMode arg1)
    {
        foreach (var item in transform.GetComponents<AudioSource>())
        {
            Destroy(item);
        }
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (SoundPanel.activeInHierarchy)
                SoundPanel.SetActive(false);
            else
            {
                SoundPanel.SetActive(true);
            }
        }
    }
    public void PlaySoundLoop(AudioClip clip)
    {
        if (clip != null)
        {

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.volume = OtherSoundVolume;
            sources.Add(source);
            source.clip = clip;
            source.loop = true;
            sources.Add(source);
            source.Play();

        }
    }
    public void PlaySound(string clipName)
    {
        foreach (var item in Clips)
        {
            if (item.name == clipName)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.volume = OtherSoundVolume;
                sources.Add(source);
                source.clip = item;
                sources.Add(source);
                source.Play();

            }
        }
    }
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.volume = OtherSoundVolume;
            source.clip = clip;
            source.loop = false;
            sources.Add(source);
            source.Play();

        }
    }
    public void PlaySoundsTogether(List<AudioClip> clips)
    {
        foreach (AudioClip clip in clips)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = OtherSoundVolume;
            sources.Add(audioSource);
            audioSource.clip = clip;
            audioSource.loop = false;
            audioSource.Play();
        }
    }
    public void PlaySoundsSequence(List<AudioClip> clips)
    {
        StartCoroutine(PlaySoundSequenceCor(clips));
    }

    IEnumerator PlaySoundSequenceCor(List<AudioClip> clips)
    {
        for (int i = 0; i < clips.Count; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = OtherSoundVolume;
            sources.Add(audioSource);
            audioSource.clip = clips[i];
            audioSource.loop = false;
            audioSource.Play();
            yield return new WaitForSeconds(clips[i].length);
            
        }
    }

    public void StopSound()
    {
        foreach (var item in sources)
        {
            Destroy(item);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlaySound(string clipName)
    {
        foreach (var item in Clips)
        {
            if (item.name == clipName)
            {
                PlaySound(item);
            }
        }
    }

    public void OnMainSoundChanged()
    {
        MainSoundVolume = MainSlider.value;
        mainSound.volume = MainSlider.value;
    }
    public void OnOtherSoundChanged()
    {
       OtherSoundVolume = OtherSlider.value;
    }
    public void MainSoundButton()
    {
        if (MainSoundVolume > 0f)
        {
            MainSoundIconObj.GetComponent<Image>().sprite = MuteIconSprite;
            MainSlider.value = 0f;
            MainSoundVolume = 0f;
        }
        else
        {
            MainSoundIconObj.GetComponent<Image>().sprite = VolumeIconSprite;
            MainSlider.value = 1f;
            MainSoundVolume = 1f;
        }
    }
    public void OtherSoundButton()
    {
        if (OtherSoundVolume > 0f)
        {
            OtherSoundIconObj.GetComponent<Image>().sprite = MuteIconSprite;
            OtherSlider.value = 0f;
            OtherSoundVolume = 0f;
        }
        else
        {
            OtherSoundIconObj.GetComponent<Image>().sprite = VolumeIconSprite;
            OtherSlider.value = 1f;
            OtherSoundVolume = 1f;
        }
    }
    //--------------------------------------------------------//
}
