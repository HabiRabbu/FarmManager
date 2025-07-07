using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Harvey.Farm.Movement;

public abstract class BaseMover : MonoBehaviour, IMover
{
    [SerializeField] protected Transform rootTransform;

    protected virtual void Awake()
    {
        if (!rootTransform) rootTransform = transform;
    }

    public virtual IEnumerator MoveTo(Vector3 wp)
        => MoveAlong(new List<Vector3> { wp }, null);

    public abstract IEnumerator MoveAlong(List<Vector3> waypoints,
                                          System.Action<int> onArrive);

    public virtual void TeleportTo(Vector3 pos) =>
        rootTransform.position = pos;


    // ───── shared tween helpers ─────
    protected Tween YawLookAt(Vector3 target, float turnTime)
    {
        Vector3 flat = new Vector3(target.x, rootTransform.position.y, target.z);
        return rootTransform.DOLookAt(flat, turnTime, AxisConstraint.Y).SetEase(Ease.Linear);
    }
    protected Tween TranslateTo(Vector3 target, float time) =>
        rootTransform.DOMove(target, time).SetEase(Ease.Linear);
}
