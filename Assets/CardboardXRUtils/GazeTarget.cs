using UnityEngine;
using UnityEngine.Events;

namespace CardboardXRUtils
{
    public class GazeTarget : MonoBehaviour
    {
        public bool GazeActive { internal set; get; } = false;
        public bool GazeJustEntered { internal set; get; } = false;
        public RaycastHit GazeHitPoint { internal set; get; }
        public bool GazeClick => CXR.IsTriggerPressed;
    }
}

