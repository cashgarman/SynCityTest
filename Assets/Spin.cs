using UnityEngine;

public class Spin : MonoBehaviour
{
    public float degreesPerSecond;
    public Vector3 axis;

    public void Update()
    {
        transform.Rotate(axis, degreesPerSecond * Time.deltaTime);
    }
}
