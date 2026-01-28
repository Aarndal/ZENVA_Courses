using UnityEngine;
using UnityEngine.InputSystem;

public class MouseClick : MonoBehaviour
{
    [SerializeField]
    private Camera gameCamera;

    private InputAction _click;

    void Awake()
    {
        _click = new InputAction(binding: "<Mouse>/leftButton");

        _click.performed += ctx =>
        {
            Vector3 currentCoordinates = Mouse.current.position.ReadValue();

            if (Physics.Raycast(gameCamera.ScreenPointToRay(currentCoordinates), out RaycastHit hit))
            {
                hit.collider.GetComponentInParent<IClickable>()?.OnClick();
            }

        };
        _click.Enable();
    }
}
