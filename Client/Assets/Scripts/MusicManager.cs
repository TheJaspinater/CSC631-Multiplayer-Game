using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    private static FMOD.Studio.EventInstance Music;

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
    public void SetMusicValue(string parameterToChange, float amtOfProgress)
    {          
        //Music.setParameterByName(parameterToChange,amtOfProgress);
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
