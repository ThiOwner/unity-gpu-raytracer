using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCamera : MonoBehaviour
{   
    public float speed = 50.0f;
    public float sensitivity = 2.0f;
    private Camera _camera;

    public InputActionAsset inputActions;
    
    private InputAction _moveAction;
    private InputAction _lookAction;

    private Vector3 _movement;
    private Vector3 _mouseDelta;

    private float _pitch = 0f;
    private float _yaw = 0f;

    private void OnEnable(){
        inputActions.FindActionMap("FreeCamera").Enable();
    }

    private void OnDisable(){
        inputActions.FindActionMap("FreeCamera").Disable();
    }

    private void Awake(){
        _moveAction = inputActions.FindActionMap("FreeCamera").FindAction("Move");
        _lookAction = inputActions.FindActionMap("FreeCamera").FindAction("Look");
        _camera = GetComponent<Camera>();
    }

    private void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update(){
        if (_lookAction != null){
            Vector2 lookDelta = _lookAction.ReadValue<Vector2>();

            _yaw += lookDelta.x * sensitivity;
            _pitch -= lookDelta.y * sensitivity;
            _pitch = Mathf.Clamp(_pitch, -89f, 89f);

            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        if (_moveAction != null){
            Vector3 moveInput = _moveAction.ReadValue<Vector3>();
            Vector3 direction = transform.right * moveInput.x + transform.up * moveInput.y + transform.forward * moveInput.z;
            
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
