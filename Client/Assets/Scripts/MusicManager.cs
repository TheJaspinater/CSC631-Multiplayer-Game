using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    private static FMOD.Studio.Bus MasterMusic;
    private static FMOD.Studio.Bus MasterSFX;
    private static FMOD.Studio.EventInstance Music;

    public Slider mainOptionsMusicSlider;
    public Slider gameOptionsMusicSlider;
    public Slider mainOptionsSFXSlider;
    public Slider gameOptionsSFXSlider;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        MasterMusic = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        MasterSFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");
    }

    // Start is called before the first frame update
    void Start()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance("event:/MC_Main_Menu");
        Music.start();
        Music.release();
    }

    public void StartGame()
    {
        Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Music = FMODUnity.RuntimeManager.CreateInstance("event:/MC_Character_Select");
        Music.start();
        Music.release();
    }

    //Used to change the value of background music as game progresses
    public void SetMusicValue(Slider slider)
    {
        MasterMusic.setVolume(slider.value);
        mainOptionsMusicSlider.value = slider.value;
        gameOptionsMusicSlider.value = slider.value;
    }

    public void SetSFXVolume(Slider slider)
    {
        MasterSFX.setVolume(slider.value);
        mainOptionsSFXSlider.value = slider.value;
        gameOptionsSFXSlider.value = slider.value;
    }

    public void PlayGameSound(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);
    }

    public void PlayUISound(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path);
    }
}