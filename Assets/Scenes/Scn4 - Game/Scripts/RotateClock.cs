using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Tooltip("Degrees to rotate per second. Use 6 for realistic ticking.")]
    public float speed = 0.6666667f;

    void Update()
    {
        // Smoothly rotates the UI asset clockwise on the Z-axis
        transform.Rotate(0f, 0f, -speed * Time.deltaTime);
    }
}