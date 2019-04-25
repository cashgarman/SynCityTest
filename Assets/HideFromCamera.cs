using UnityEngine;

public class HideFromCamera : MonoBehaviour
{
    public SelectiveCamera camera;
    
    void Start()
    {
        // Register this object with the specified camera
        camera.AddHiddenObject(this);
    }
}