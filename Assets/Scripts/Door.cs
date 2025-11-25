using UnityEngine;
using System.Collections;

public class Door : OpenableObject
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _openTrigger = "Open";
    [SerializeField] private string _closeTrigger = "Close";

    void Start()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    public override IEnumerator Close()
    {
        if (_animator != null)
        {
            _animator.SetTrigger(_closeTrigger);
        }

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