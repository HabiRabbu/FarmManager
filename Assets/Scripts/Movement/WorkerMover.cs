using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Harvey.Farm.Movement;
using DG.Tweening;
using Harvey.Farm.Workers;

public class WorkerMover : BaseMover
{
    WorkerStats stats;
    Animator anim;
    int isMovingHash;

    protected override void Awake()
    {
        base.Awake();
        stats = GetComponent<WorkerStats>();
        anim = GetComponent<Animator>();
        isMovingHash = Animator.StringToHash("IsMoving");
    }

    public override IEnumerator MoveAlong(List<Vector3> wps, System.Action<int> onArrive)
    {
        anim.SetBool(isMovingHash, true);

        anim.Play("Hop", 0, Random.value);
        float originalSpeed = anim.speed;
        anim.speed = Random.Range(0.9f, 1.1f);

        for (int i = 0; i < wps.Count; i++)
        {
            float dist = Vector3.Distance(rootTransform.position, wps[i]);
            float time = dist / stats.WalkSpeed;

            yield return TranslateTo(wps[i], time).WaitForCompletion();
            onArrive?.Invoke(i);
        }

        anim.SetBool(isMovingHash, false);
        anim.speed = originalSpeed;
    }
}
