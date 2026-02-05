using UnityEngine;

public class BalloonMovement : MonoBehaviour, IHorizontalMoveable, IVerticalMoveable
{
    [SerializeField, Range (0.01f, 1.5f)]
    private float verticalFloatSpeed = 1.0f;
    [SerializeField, Range (0.01f, 1.0f)]
    private float horizontalFloatAmplitude = 0.5f;
    [SerializeField, Range (0.01f, 10.0f)]
    private float horizontalFloatFrequenze = 2.0f;


    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        if (!this.gameObject.activeInHierarchy)
            return;

        MoveVertical();
        MoveHorizontal();
    }

    public void MoveHorizontal()
    {
        transform.Translate(Mathf.Sin(2 * Mathf.PI * Time.time / horizontalFloatFrequenze) *
                    horizontalFloatAmplitude * Time.fixedDeltaTime * Vector3.right);
    }

    public void MoveVertical()
    {
        transform.Translate(Time.fixedDeltaTime * verticalFloatSpeed * Vector3.up);
    }
}
