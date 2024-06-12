using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public static ObjectSpawner Instance;

    public List<ObjectToSpawnSO> objectToSpawn;
    public Dictionary<string, ObjectToSpawnSO> objectToSpawnDict;

    /*[Header("Reference")]
    public Transform spawnPosVisualisation;*/

    [Header("UI")]
    public Transform buttonParent;
    public Transform selectedFrame;
    public float smoothTime;

    [Header("Prefab Reference")]
    public GameObject buttonPrefab;

    [Header("Read-Only")]
    [SerializeField] ObjectIdentity _selectedObjectIdentity;
    public Dictionary<string, Transform> allButtonDict;
    [SerializeField] GameObject _objectTemp;
    [SerializeField] bool _allowRelease;

    private void Awake()
    {
        objectToSpawnDict = new Dictionary<string, ObjectToSpawnSO>();
        allButtonDict = new Dictionary<string, Transform>();

        foreach (var obj in objectToSpawn)
        {
            objectToSpawnDict.Add(obj.identity.objectName, obj);
        }

        Instance = this;
    }

    private void Start()
    {
        foreach (var obj in objectToSpawnDict)
        {
            GameObject btnClone = Instantiate(buttonPrefab, buttonParent);
            btnClone.GetComponent<ButtonObject>().SetIdentity(obj.Value.identity);

            allButtonDict.Add(obj.Key, btnClone.transform);
        }
    }

    private void Update()
    {
        if (_selectedObjectIdentity.objectName != "")
        {
            if (selectedFrame.gameObject.activeSelf == false)
            {
                selectedFrame.gameObject.SetActive(true);
            }

            selectedFrame.transform.position =
                Vector3.Lerp(selectedFrame.transform.position,
                                allButtonDict[_selectedObjectIdentity.objectName].transform.position,
                                smoothTime * Time.deltaTime);
        }
        else
        {
            if (selectedFrame.gameObject.activeSelf == true)
            {
                selectedFrame.gameObject.SetActive(false);
            }
        }

#if UNITY_EDITOR
        /*if (Input.GetMouseButtonDown(0))
        {
            SpawnObjectOnRay();
        }
        else if(Input.GetMouseButton(0))
        {
            MoveObjectOnRay();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            ReleaseObject();
        }*/

        if (Input.GetMouseButton(0))
        {
            MoveObjectOnRay();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ReleaseObject();
        }
#endif
    }

    public void SpawnObjectOnRay()
    {
        if (_selectedObjectIdentity.objectName == "")
            return;

        // Convert the mouse position to a ray
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {

            _objectTemp = Instantiate(_selectedObjectIdentity.objectPrefab);
            _objectTemp.transform.position = hit.point + Vector3.up * 1.5f;
            _objectTemp.GetComponent<Rigidbody>().useGravity = false;
            _objectTemp.gameObject.tag = "Untagged";
        }
    }

    public void MoveObjectOnRay()
    {
        if (_objectTemp == null)
            return;

        LayerMask enviLayer = LayerMask.GetMask("Environment");

        // Convert the mouse position to a ray
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform the raycast with the specified LayerMask
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, enviLayer))
        {
            if (hit.collider.tag == "SpawnPlace")
            {
                _objectTemp.transform.position = hit.point + Vector3.up * 1.5f;
                _allowRelease = true;
            }
            else if (hit.collider.tag == "NotSpawnPlace")
            {
                _objectTemp.transform.position = hit.point + Vector3.up * 1.5f;
                _allowRelease = false;
            }
            else if (hit.collider.tag != "SpawnObject")
            {
                _objectTemp.transform.position = hit.point + Vector3.up * 1.5f;
            }
        }
    }

    public void ReleaseObject()
    {
        if (_objectTemp == null)
            return;

        _objectTemp.gameObject.tag = "SpawnObject";
        _objectTemp.GetComponent<Rigidbody>().useGravity = true;

        if (!_allowRelease)
        {
            Destroy(_objectTemp, 0.5f);
        }

        _objectTemp = null;
    }

    public void SetSelectedObject(ObjectIdentity obj)
    {
        _selectedObjectIdentity = obj;
    }
}
