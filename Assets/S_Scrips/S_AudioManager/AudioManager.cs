
using UnityEngine;
using static Unity.VisualScripting.Member;

public class AudioManager : MonoBehaviour
{

    [Header("-------Audio Source-------")]
    [SerializeField] AudioSource SFXSource;

    [Header("-------Audio Clip-------")]
    public AudioClip sortidaGancho;
    public AudioClip entradaGancho;
    public AudioClip jump;
    public AudioClip pinchos;
    public AudioClip parachute;


    public void PlaySFX(AudioClip clip)
    {

        SFXSource.PlayOneShot(clip);
    }


}
