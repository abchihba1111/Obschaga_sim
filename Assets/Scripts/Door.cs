using UnityEngine;
using System.Collections;

public class Door : OpenableObject
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _openTrigger = "Open";
    [SerializeField] private string _closeTrigger = "Close";
    [SerializeField] private GameObject DoorObject;
    private AudioSource DoorSoundSource;

    void Start()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        DoorSoundSource = DoorObject.GetComponent<AudioSource>();
    }

    public override IEnumerator Close()
    {
        if (_animator != null)
        {
            _animator.SetTrigger(_closeTrigger);
        }

        DoorSoundSource.Play();

        float elapsedTime = 0f;
        while (elapsedTime < _openOrCloseTime)
        {
            _openOrCloseLerp = 1f - (elapsedTime / _openOrCloseTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _openOrCloseLerp = 0f;
    }

    public override IEnumerator Open()
    {
        if (_animator != null)
        {
            _animator.SetTrigger(_openTrigger);
        }

        DoorSoundSource.Play();

        float elapsedTime = 0f;
        while (elapsedTime < _openOrCloseTime)
        {
            _openOrCloseLerp = elapsedTime / _openOrCloseTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _openOrCloseLerp = 1f;
    }
}