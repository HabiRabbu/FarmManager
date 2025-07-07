using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(WorkerMover))]
public class WorkerBounce : MonoBehaviour
{
    [SerializeField] Transform meshesRoot;
    [SerializeField, Min(0f)] float hopHeight = 0.12f;
    [SerializeField, Min(0f)] float hopTime   = 0.25f;

    Sequence bounceSeq;

    void Awake()
    {
        if (!meshesRoot) meshesRoot = transform;
        bounceSeq = DOTween.Sequence().SetAutoKill(false).Pause();

        // -- lift / drop --
        bounceSeq.Append(meshesRoot.DOLocalMoveY(hopHeight, hopTime)
                                   .SetLoops(-1, LoopType.Yoyo)
                                   .SetEase(Ease.InOutSine).SetRelative());

        // -- squash / stretch --
        bounceSeq.Join(meshesRoot.DOScaleY(0.85f, hopTime)
                                 .SetLoops(-1, LoopType.Yoyo)
                                 .SetEase(Ease.InOutSine).SetRelative());
        bounceSeq.Join(meshesRoot.DOScaleX(1.10f, hopTime)
                                 .SetLoops(-1, LoopType.Yoyo)
                                 .SetEase(Ease.InOutSine).SetRelative());
        bounceSeq.Join(meshesRoot.DOScaleZ(1.10f, hopTime)
                                 .SetLoops(-1, LoopType.Yoyo)
                                 .SetEase(Ease.InOutSine).SetRelative());
    }

    public void PlayBounce()  => bounceSeq.Play();
    public void StopBounce()  => bounceSeq.Pause().Rewind();
}
