using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object To Spawn Data", menuName = "Custom Data/Object To Spawn Data")]
public class ObjectToSpawnSO : ScriptableObject
{
    public ObjectIdentity identity;
}

[System.Serializable]
public class ObjectIdentity
{
    public string objectName;
    public Sprite icon;
    public GameObject objectPrefab;
}
