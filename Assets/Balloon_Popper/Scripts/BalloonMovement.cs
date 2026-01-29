using UnityEngine;

public class BalloonMovement : MonoBehaviour
{
    [SerializeField, Range (0.01f, 1.5f)]
    private float floatSpeed = 1.0f;
    [SerializeField, Range (0.01f, 1.0f)]
    private float horizontalFloatAmplitude = 0.5f;
    [SerializeField, Range (0.01f, 10.0f)]
    private float horizontalFloatFrequenze = 2.0f;

    private void Update()
    {
        transform.Translate(Time.deltaTime * floatSpeed * Vector3.up);
        transform.Translate(Mathf.Sin(2 * Mathf.PI * Time.time / horizontalFloatFrequenze) * horizontalFloatAmplitude * Time.deltaTime * Vector3.right);
    }
}
