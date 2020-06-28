using UnityEngine;
using UnityEngine.SpatialTracking;
using Google.XR.Cardboard;
using UnityEngine.Events;

/// <summary> Card Board XR Api Wrapper to support during Editor play mode </summary>
public static class CXR
{
    public static bool IsTriggerPressed = false;
}

namespace CardboardXRUtils
{ 

    public class CardboardPlayer : MonoBehaviour
    {
        public static event UnityAction<GazeTarget> OnGazeExit;
        public static event UnityAction<GazeTarget, RaycastHit> OnGazeStay;
        public static event UnityAction<GazeTarget, RaycastHit> OnGazeEnter;
        public static event UnityAction<GazeTarget> OnGazeClick;

        public TrackedPoseDriver poseDriver;

        [Header("Player")]

        [Tooltip("How far does the gaze will project a ray")]
        public float GazeDistanceLimit = 20f;

        bool IsMobile => Application.isMobilePlatform && ! Application.isEditor;

        private void Start( )
        {
            if( IsMobile )
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;

                // Brightness control is expected to work only in iOS
                Screen.brightness = 1.0f;
                
                // Checks if the device parameters are stored and scans them if not.
                if ( ! Api.HasDeviceParams() ) Api.ScanDeviceParams();
            }
            else
            {
                poseDriver.enabled = false;
            }
        }

        private void Update( )
        {
            if( IsMobile )
            {
                if ( Api.IsGearButtonPressed ) Api.ScanDeviceParams();
                
                if ( Api.IsCloseButtonPressed ) Application.Quit();

                if ( Api.HasNewDeviceParams( ) ) Api.ReloadDeviceParams();

                CXR.IsTriggerPressed = Api.IsTriggerPressed;
            }
            else UpdateEditor();

            UpdateGaze();

            if( CXR.IsTriggerPressed && currentGazeTarget != null ) 
            {
                OnGazeClick?.Invoke( currentGazeTarget );
            }
        }

        Vector3 rotStart = Vector3.zero;

        Vector3 rMouseStart = Vector3.zero;

        bool rMouseDown = false;

        [Header("Editor Emulator Mouse Controls")]
        [Range(0.2f,3.8f)]
        [Tooltip("Mouse Acceleration")]
        public float LookSensativy = 0.8f;

        [Tooltip("Discard ease and delta, map location to target rotation")]
        public bool UseInstantMouse = false;

        [Range(0.25f, 1f)]
        [Tooltip("Mouse follow ease multiplier")]
        public float MouseEase = 0.65f;

        [Range(0,25f)]
        [Tooltip("How much tilt the mouse wheel will do")]
        public float TiltScalar = 7f;
        Vector3 rotation = Vector3.zero;
        [Range(0f,1f)]
        [Tooltip("Z-Axis / Tilt multiplier that will return the value to 0 - change domain: [0,1] To match range: [ MAX - MIN ]")]
        public float TildStiffness = 0.12f;

        void UpdateEditor()
        {
            CXR.IsTriggerPressed = Input.GetMouseButton( 0 );

            if( rMouseDown )
            {
                var delta = ( Input.mousePosition - rMouseStart );
                var speed = LookSensativy * Time.deltaTime;
                var R = delta * speed;

                if( ! UseInstantMouse )
                {
                    // do a swap !
                    rotation += new Vector3( -R.y , R.x, 0f );
                }
                else
                {
                    rotation = rotStart + new Vector3( -delta.y, delta.x, 0f );
                }
            }
            
            rotation += new Vector3( 0 , 0 , Input.mouseScrollDelta.x + Input.mouseScrollDelta.y ) * TiltScalar;

            var Q1 = poseDriver.transform.rotation;
            var Q2 = Quaternion.Euler( rotation );
            
            if( UseInstantMouse ) poseDriver.transform.rotation  = Q2;

            else poseDriver.transform.rotation = Quaternion.Lerp( Q1, Q2, MouseEase );

            rotation.z *= ( ( 1f - TildStiffness ) * 0.25f + 0.75f );

            if( Input.GetMouseButtonDown( 1 ) ) 
            {
                rMouseStart = Input.mousePosition;
                rotStart = poseDriver.transform.rotation.eulerAngles;
            }

            rMouseDown = Input.GetMouseButton( 1 );
        }

        void UpdateGaze()
        {
            Ray R = new Ray( poseDriver.transform.position, poseDriver.transform.forward );

            if (Physics.Raycast( R, out RaycastHit Rh, GazeDistanceLimit ) )
            {
                GazeTarget T = Rh.transform.GetComponent<GazeTarget>();

                if( T == null ) BreakGaze( );

                else CaptureGaze( T, Rh );
            }
        }

        GazeTarget currentGazeTarget = null;

        void BreakGaze()
        {
            if( currentGazeTarget == null ) return;

            currentGazeTarget.GazeActive = false;
            currentGazeTarget.GazeHitPoint = default;
            currentGazeTarget.GazeJustEntered = false;

            OnGazeExit?.Invoke( currentGazeTarget );
            
            currentGazeTarget = null;
        }
        void CaptureGaze( GazeTarget target, RaycastHit hit )
        {
            if( currentGazeTarget == target ) 
            {
                target.GazeJustEntered = false;
                target.GazeHitPoint = hit;

                OnGazeStay?.Invoke( target, hit );
                
                return;
            }

            if( currentGazeTarget != null ) BreakGaze();

            target.GazeJustEntered = true;
            target.GazeActive = true;
            target.GazeHitPoint = hit;

            currentGazeTarget = target;

            OnGazeEnter?.Invoke( target, hit );
        }
    }
}