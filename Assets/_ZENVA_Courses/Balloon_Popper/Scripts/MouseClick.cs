using InteractableSystem;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseClick : MonoBehaviour
{
    [SerializeField]
    private Camera gameCamera;

    private InputAction _click;
    private Action<InputAction.CallbackContext> _clickHandler;

    private void Awake()
    {
        _clickHandler = ctx =>
        {
            Vector3 currentCoordinates = Mouse.current.position.ReadValue();
            if (Physics.Raycast(gameCamera.ScreenPointToRay(currentCoordinates), out RaycastHit hit))
            {
                hit.collider.GetComponentInParent<IClickable>()?.Click();
            }
        };
        _click = new InputAction(binding: "<Mouse>/leftButton");
        _click.performed += _clickHandler;
        _click.Enable();
    }

    private void OnDestroy()
    {
        if (_click != null && _clickHandler != null)
        {
            _click.performed -= _clickHandler;
            _click.Dispose();
            _clickHandler = null;
        }
    }
}
