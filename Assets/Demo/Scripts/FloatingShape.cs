
using UnityEngine;
using CardboardXRUtils;

[RequireComponent(typeof(GazeTarget))]
public class FloatingShape : MonoBehaviour
{
    public Material OnFocus;
    public Material OnNormal;

    GazeTarget gazer;
    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        gazer = GetComponent<GazeTarget>();
    }

    void Update( )
    {
        if( gazer.GazeActive )
        {
            meshRenderer.material = OnFocus;

            if( CXR.IsTriggerPressed ) 
            {
                var v = Random.onUnitSphere * 1.5f;
                v.z = Random.Range( 0.4f , 1.2f );
                transform.position = v;
            }
        }
        else
        {
            meshRenderer.material = OnNormal;
        }
    }
}
