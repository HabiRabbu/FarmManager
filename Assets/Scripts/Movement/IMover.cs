using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harvey.Farm.Movement
{
    public interface IMover
    {
        IEnumerator MoveTo   (Vector3 worldPos);
        IEnumerator MoveAlong(List<Vector3> waypoints, System.Action<int> onArrive);
        IEnumerator ReturnToHome ();
        void        TeleportTo(Vector3 worldPos);
    }
}
