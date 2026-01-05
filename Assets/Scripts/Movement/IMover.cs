using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using UnityEngine;

namespace Harvey.Farm.Movement
{
    public interface IMover
    {
        Sequence seq { get; set; }
        void StopAllTweens();
        IEnumerator MoveTo(Vector3 worldPos);
        IEnumerator MoveAlong(List<Vector3> waypoints, System.Action<int> onArrive, int resumeTile = 0);
        IEnumerator ReturnToHome();
        void TeleportTo(Vector3 worldPos);
    }
}
