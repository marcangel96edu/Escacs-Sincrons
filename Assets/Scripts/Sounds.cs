using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip click;
    [SerializeField]
    private AudioClip end;
    [SerializeField]
    private AudioClip fail;
    [SerializeField]
    private List<AudioClip> pieces;
    [SerializeField]
    private AudioClip tic;
    private int iterator;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        iterator = 0;
    }

    public void reproduceClick() {
        audioSource.PlayOneShot(click);
    }

    public void reproduceEnd() {
        audioSource.PlayOneShot(end);
    }

    public void reproduceFail() {
        audioSource.PlayOneShot(fail);
    }

     public void reproducePiece() {
        audioSource.PlayOneShot(pieces[iterator]);
        ++iterator;
        iterator %= pieces.Count;
    }


    public void reproduceTic() {
        audioSource.PlayOneShot(tic);
    }

}