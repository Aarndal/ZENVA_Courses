using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "newJumpnRunInputReader", menuName = "InputReader/JumpnRunInputReader")]
public class JumpnRunInputReader : ScriptableObject, _2DJumpnRunInputActions.IPlayerCharacterActions, _2DJumpnRunInputActions.IUIActions
{
    // --- Player Character Events ---
    public event Action<bool> AttackPressed;
    public event Action<bool> JumpPressed;
    public event Action<bool> SprintPressed;

    public event Action<Vector2> MovePerformed;
    public event Action<Vector2> MoveCanceled;

    // --- UI Events ---



    [SerializeField]
    private int defaultActionMapIndex = 0;

    private Guid _currentActionMapId = Guid.Empty;

    public _2DJumpnRunInputActions Input { get; private set; }


    public InputActionMap DefaultActionMap
    {
        get
        {
            if (Input == null)
            {
                Debug.LogErrorFormat("Input asset is not assigned.");
                return null;
            }

            if (defaultActionMapIndex >= Input.asset.actionMaps.Count)
            {
                Debug.LogWarningFormat("Default Action Map index is out of range. Resetting to 0.");
                defaultActionMapIndex = 0;
            }

            return Input.asset.actionMaps[defaultActionMapIndex];
        }
    }
    public InputActionMap CurrentActionMap
    {
        get
        {
            if (Input == null)
            {
                Debug.LogWarning("Input asset is not assigned.");
                return null;
            }
            if (_currentActionMapId == Guid.Empty)
            {
                Debug.LogWarning("Current Action Map ID is empty. Returning Default Action Map.");
                _currentActionMapId = DefaultActionMap.id;
                return DefaultActionMap;
            }

            return Input.asset.FindActionMap(_currentActionMapId);
        }
    }


    #region Unity Lifecycle Methods
    private void Awake()
    {
        if (Input == null)
            Input = new();
    }

    private void OnValidate()
    {
        if (Input == null)
            Input = new();

        if (Input.asset == null)
            return;

        if (defaultActionMapIndex >= Input.asset.actionMaps.Count)
        {
            defaultActionMapIndex = 0;
        }

        _currentActionMapId = Input.asset.actionMaps[defaultActionMapIndex].id;
    }

    private void OnEnable()
    {
        if (Input == null)
            Input = new();

        SetCallbacks(this);
        DisableAllInput();

        // Directly enable the default action map
        var defaultMap = DefaultActionMap;
        if (defaultMap != null)
        {
            defaultMap.Enable();
            _currentActionMapId = defaultMap.id;
        }
    }

    private void OnDisable()
    {
        DisableAllInput();
    }

    private void OnDestroy()
    {
        SetCallbacks(null);
    }
    #endregion

    #region Private Methods
    private void DisableAllInput()
    {
        // Validate the input asset before attempting to disable input
        if (Input == null || Input.asset == null)
        {
            return;
        }

        // Disable all Action Maps in the input asset to ensure no input is processed
        foreach (var actionMap in Input.asset.actionMaps)
        {
            actionMap.Disable();
        }
    }

    /// <summary>
    /// Switch the active Action Map to the specified input Action Map.
    /// This will disable the currently active Action Map and enable the new one, allowing the input reader to process input events from the new Action Map.
    /// </summary>
    /// <param name="inputActionMap"></param>
    private void SwitchActionMap(InputActionMap inputActionMap)
    {
        // Validate the input Action Map
        if (inputActionMap == null || inputActionMap.id == Guid.Empty)
            return;

        if (_currentActionMapId != Guid.Empty && inputActionMap.id == _currentActionMapId)
            return;

        // Disable the current map only if one is actually active
        if (_currentActionMapId != Guid.Empty)
            CurrentActionMap?.Disable();

        inputActionMap.Enable();
        _currentActionMapId = inputActionMap.id;
    }

    /// <summary>
    /// Set the callbacks for the input actions to the provided input reader instance.
    /// This allows the input reader to receive and process input events from the action maps.
    /// </summary>
    /// <param name="inputReader"></param>
    private void SetCallbacks(JumpnRunInputReader inputReader)
    {
        if (Input == null)
            return;

        Input.PlayerCharacter.SetCallbacks(inputReader);
        Input.UI.SetCallbacks(inputReader);
    }
    #endregion

    #region PlayerCharacter Input Callbacks
    public void OnAttack(InputAction.CallbackContext context)
    {
        AttackPressed?.Invoke(context.ReadValueAsButton());
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        JumpPressed?.Invoke(context.ReadValueAsButton());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var moveInput = context.ReadValue<Vector2>();

        if (!Mathf.Approximately(moveInput.x, 0f))
        {
            MovePerformed?.Invoke(moveInput);
            return;
        }

        MoveCanceled?.Invoke(moveInput);
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        SprintPressed?.Invoke(context.ReadValueAsButton());
    }
    #endregion

    #region UI Input Callbacks
    public void OnNavigate(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}