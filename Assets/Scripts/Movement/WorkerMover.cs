using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.Movement;
using DG.Tweening;
using Harvey.Farm.Workers;
using Harvey.Farm.Events;

public class WorkerMover : BaseMover
{
    WorkerStats stats;

    Animator anim;
    int isMovingHash;

    Tween _currentMoveTween;

    protected override void Awake()
    {
        base.Awake();
        stats = GetComponent<WorkerStats>();
        anim = GetComponent<Animator>();
        isMovingHash = Animator.StringToHash("IsMoving");
    }

    void OnEnable()
    {
        GameEvents.OnStopAllTweens += StopAllTweens;
    }
    protected override void OnDisable()
    {
        GameEvents.OnStopAllTweens -= StopAllTweens;
        _currentMoveTween?.Kill();
        _currentMoveTween = null;
        base.OnDisable();
    }
    void OnDestroy()
    {
        GameEvents.OnStopAllTweens -= StopAllTweens;
    }

    public override void StopAllTweens()
    {
        base.StopAllTweens();
        _currentMoveTween?.Kill();
        _currentMoveTween = null;
        anim.SetBool(isMovingHash, false);
    }

    public override IEnumerator MoveAlong(List<Vector3> wps, System.Action<int> onArrive, int resumeTile = 0)
    {
        anim.SetBool(isMovingHash, true);

        anim.Play("Hop", 0, Random.value);
        float originalSpeed = anim.speed;
        anim.speed = Random.Range(0.9f, 1.1f);

        for (int i = 0; i < wps.Count; i++)
        {
            float dist = Vector3.Distance(rootTransform.position, wps[i]);
            float time = dist / stats.Model.WalkSpeed;

            _currentMoveTween = TranslateTo(wps[i], time);
            yield return _currentMoveTween.WaitForCompletion();
            onArrive?.Invoke(i);
        }
        _currentMoveTween = null;

        anim.SetBool(isMovingHash, false);
        anim.speed = originalSpeed;
    }

    public override IEnumerator ReturnToHome()
    {
        anim.SetBool(isMovingHash, true);
        anim.Play("Hop", 0, Random.value);
        float originalSpeed = anim.speed;
        anim.speed = Random.Range(0.9f, 1.1f);

        var homePos = stats.GetHome().transform.position;
        float dist = Vector3.Distance(rootTransform.position, homePos);
        float time = dist / stats.Model.WalkSpeed;

        _currentMoveTween = TranslateTo(homePos, time);
        yield return _currentMoveTween.WaitForCompletion();
        _currentMoveTween = null;

        anim.SetBool(isMovingHash, false);
        anim.speed = originalSpeed;
    }
}
