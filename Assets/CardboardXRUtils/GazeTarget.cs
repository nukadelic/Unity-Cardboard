using UnityEngine;
using UnityEngine.Events;

namespace CardboardXRUtils
{
    public class GazeTarget : MonoBehaviour
    {
        public UnityEvent<GazeTarget> onGazeExit;
        public UnityEvent<GazeTarget, RaycastHit> onGazeStay;
        public UnityEvent<GazeTarget, RaycastHit> onGazeEnter;
        public UnityEvent<GazeTarget> onGazeClick;

        internal bool hasGaze = false;
        public bool HasGaze => hasGaze;
        internal virtual void OnGazeEnter( RaycastHit hit ) {}
        internal virtual void OnGazeStay( RaycastHit hit ) {}
        internal virtual void OnGazeExit( ) { }
        internal virtual void OnGazeClick( ) { }
    }
}

