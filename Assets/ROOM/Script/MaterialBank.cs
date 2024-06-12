using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MaterialBank : MonoBehaviour
{
    public Material[] materials;
    
    public Dictionary<string, Material> materialsDict;

    Renderer _renderer;
    int _materialIndex = 0;

    private void Awake()
    {
        materialsDict = new Dictionary<string, Material>();

        for (int i = 0; i < materials.Length; i++)
        {
            materialsDict.Add(materials[i].name, materials[i]);
        }

        _renderer = GetComponent<Renderer>();
    }

    public void ChangeMaterial()
    {
        MaterialPropertyBlock _materialProperty = new MaterialPropertyBlock();

        Material[] newMats = new Material[1];

        newMats[0] = materials[_materialIndex];

        _renderer.materials = newMats;

        _renderer.GetPropertyBlock(_materialProperty);

        _materialIndex++;
        if(_materialIndex > materials.Length - 1)
            _materialIndex = 0;
    }

    public void ChangeMaterial(int materialIndex)
    {
        MaterialPropertyBlock _materialProperty = new MaterialPropertyBlock();

        Material[] newMats = new Material[materialIndex];

        _renderer.materials = newMats;

        _renderer.GetPropertyBlock(_materialProperty);
    }

    public void ChangeMaterial(string materialName)
    {

    }
}
