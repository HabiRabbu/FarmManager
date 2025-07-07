using System;
using UnityEngine;

namespace Harvey.Farm.UI.Radial
{
    [Serializable]
    public struct RadialMenuItem
    {
        public string name;
        public Sprite icon;
        public Action  onClick;
    }
}
