# Unity Cardboard SDK Utils

Cardboard emulator, allows to play cardboard based project in Editor without the need to publishing  
No need to toggle defines between builds.  
Check out the demo scene. 

| CardboardPlayer |
|------------|
| <img src="https://raw.githubusercontent.com/nukadelic/Unity-Cardboard/master/Docs/image-01.png" width="280"> |

* Look around : `Right Mouse Button` + `Mouse Move`
* Click to interact and check `CXR.IsTriggerPressed` for value 
* Import `CardboardXRUtils`
* Added `GazeTarget` component, get component and access:  
  * ` bool GazeActive`
  * ` bool GazeJustEntered`
  * ` RaycastHit GazeHitPoint`
  * ` bool GazeClick`
* CardboardPlayer static event actions: 
  * `onGazeExit<GazeTarget>`
  * `onGazeStay<GazeTarget,RaycastHit>`
  * `onGazeEnter<GazeTarget,RaycastHit>`
  * `onGazeClick<GazeTarget>`
