using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchObjectDetector : MonoBehaviour
{
    public static TouchObjectDetector Instance;

    public EditMode currentEditMode;
    public float holdTime;

    [Header("Edit Mode")]
    public Transform editModeUIParent;
    public Transform editModeBase;
    public Transform editModeMove;
    public Transform editModeRotate;
    public float rotateSpeed;

    [Header("Read-Only")]
    [SerializeField]
    public GameObject lastClickedObj;
    [SerializeField]
    public GameObject selectedSpawnObj;

    [Space(10)]
    [SerializeField]
    int _touchCount;

    [SerializeField]
    Vector3 _touchInWorldPos;
    [SerializeField]
    Vector3 _lastTouchPos;

    [SerializeField]
    float _elapsedHold;

    [SerializeField]
    bool _isMovable;

    [SerializeField]
    bool _isRotateClockwise;

    [SerializeField]
    bool _isRotateCounterClockwise;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ChangeEditMode(EditMode.None);
    }

    private void Update()
    {
        _touchCount = Input.touchCount;

        // Check if there are any touches
        if (Input.touchCount > 0)
        {
            // Get the first touch
            Touch touch = Input.GetTouch(0);

            // Check if the touch just began
            if (touch.phase == TouchPhase.Began)
            {
                DetectClickedObject(touch.position);
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                _elapsedHold = 0;
            }

            DetectSpawnedObject(touch.position);
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            DetectClickedObject(Input.mousePosition);

            _lastTouchPos = Input.mousePosition;
        }
        else if(Input.GetMouseButton(0))
        {
            if(currentEditMode == EditMode.None)
            {
                DetectSpawnedObject(Input.mousePosition);
            }
            else if(currentEditMode == EditMode.Move)
            {
                MoveSpawnedObj(Input.mousePosition);
            }      
            
            if(Input.mousePosition != _lastTouchPos)
            {
                _isMovable = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if(selectedSpawnObj != null)
            {
                selectedSpawnObj.GetComponent<Rigidbody>().useGravity = true;
            }

            lastClickedObj = null;
            _elapsedHold = 0;
            _isMovable = false;
        }            
#endif

        if(_isRotateClockwise && selectedSpawnObj != null)
        {
            selectedSpawnObj.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.World);
        }

        if(_isRotateCounterClockwise && selectedSpawnObj != null)
        {
            selectedSpawnObj.transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0, Space.World);
        }
    }

    private void LateUpdate()
    {
        MoveEditModeUI();
    }

    public void ChangeEditMode(EditMode newMode)
    {
        currentEditMode = newMode;

        switch (currentEditMode)
        {
            case EditMode.None:
                selectedSpawnObj = null;
                _elapsedHold = 0;
                break;
            case EditMode.Active:
                lastClickedObj = null;
                selectedSpawnObj.GetComponent<Rigidbody>().useGravity = false;
                selectedSpawnObj.GetComponent<Rigidbody>().Sleep();
                break;
            case EditMode.Move:
                break;
            case EditMode.Rotate:
                break;
            default:
                break;
        }

        editModeUIParent.gameObject.SetActive(currentEditMode != EditMode.None);
        editModeMove.gameObject.SetActive(currentEditMode == EditMode.Move);
        editModeRotate.gameObject.SetActive(currentEditMode == EditMode.Rotate);
    }
    
    public void DetectClickedObject(Vector3 rayPos)
    {
        // Convert the mouse position to a ray
        Ray ray = Camera.main.ScreenPointToRay(rayPos);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the object hit is the object you are interested in
            if (hit.collider != null)
            {
                lastClickedObj = hit.collider.gameObject;
                _touchInWorldPos = hit.point;

                if (currentEditMode == EditMode.None)
                {
                    if (lastClickedObj.GetComponent<MaterialBank>() != null)
                        lastClickedObj.GetComponent<MaterialBank>().ChangeMaterial();
                }
            }
            else
            {
                lastClickedObj = null;
                _touchInWorldPos = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    /// <param name="rayPos"></param>
    public void DetectSpawnedObject(Vector3 rayPos)
    {
        // Convert the mouse position to a ray
        Ray ray = Camera.main.ScreenPointToRay(rayPos);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the object hit is the object you are interested in
            if (hit.collider != null)
            {
                _touchInWorldPos = hit.point;

                if (hit.collider.gameObject.tag == "SpawnObject")
                {
                    if(currentEditMode == EditMode.None)
                    {
                        if (_elapsedHold < holdTime)
                        {
                            _elapsedHold += Time.deltaTime;
                        }
                        else
                        {
                            selectedSpawnObj = hit.collider.gameObject;
                            ChangeEditMode(EditMode.Active);
                        }
                    }
                }
                //else
                //{
                //    if (currentEditMode == EditMode.Move)
                //    {
                //        if(selectedSpawnObj == lastClickedObj)
                //        {
                //            MoveSpawnedObj();
                //        }
                //    }
                //}
            }
        }
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    public void MoveEditModeUI()
    {
        if (selectedSpawnObj != null)
        {
            editModeUIParent.position = Camera.main.WorldToScreenPoint(selectedSpawnObj.transform.position);
        }
    }

    public void MoveSpawnedObj(Vector3 rayPos)
    {
        LayerMask enviLayer = LayerMask.GetMask("Environment");

        // Convert the mouse position to a ray
        Ray ray = Camera.main.ScreenPointToRay(rayPos);
        RaycastHit hit;

        // Perform the raycast with the specified LayerMask
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, enviLayer))
        {
            if(_isMovable)
            {
                selectedSpawnObj.GetComponent<Rigidbody>().Sleep();
                selectedSpawnObj.transform.position = hit.point + Vector3.up;
            }
        }
    }

    public void RotateSpawnedObj()
    {

    }

    #region CALL_BY_BUTTON_UI
    public void OnClick_CloseBtn()
    {
        ChangeEditMode(EditMode.None);
    }

    public void OnClick_EnterMoveBtn()
    {
        ChangeEditMode(EditMode.Move);
    }

    public void OnClick_EnterRotateBtn()
    {
        ChangeEditMode(EditMode.Rotate);
    }

    public void OnPointerDown_RotateClockwiseBtn()
    { 
        _isRotateClockwise = true;
    }

    public void OnPointerUp_RotateClockwiseBtn()
    {
        _isRotateClockwise = false;
    }

    public void OnPointerDown_RotateCounterClockwiseBtn()
    {
        _isRotateCounterClockwise = true;
    }

    public void OnPointerUp_RotateCounterClockwiseBtn()
    {
        _isRotateCounterClockwise = false;
    }
    #endregion
}

public enum EditMode
{
    None, Active, Move, Rotate
}
