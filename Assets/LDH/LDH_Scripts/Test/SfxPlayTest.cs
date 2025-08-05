using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SfxPlayTest : MonoBehaviour
{

    [SerializeField] private AudioClip _audioClip;

    [SerializeField] private string _audioClipKey;

    public void PlayAudioClip()
    {
        Manager.Sound.Play(_audioClip);
    }

    public void PlayAudioClipByKey()
    {
        Manager.Sound.PlaySfxByKey(_audioClipKey);
    }


    public void MoveScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void PlayFire()
    {
        Manager.Sound.PlayFire();
    }

    public void PlayBlank()
    {
        Manager.Sound.PlayBlank();
    }
}
