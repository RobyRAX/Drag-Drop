using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonObject : MonoBehaviour
{
    public ObjectIdentity ObjectIdentity;

    public Image iconImage;

    public void SetIdentity(ObjectIdentity identity)
    {
        this.ObjectIdentity = identity;

        UpdateImage();
    }

    public void UpdateImage()
    {
        iconImage.sprite = ObjectIdentity.icon;
    }

    public void OnClick()
    {
        ObjectSpawner.Instance.SetSelectedObject(ObjectIdentity);
    }

    public void OnPointerDown()
    {
        ObjectSpawner.Instance.SetSelectedObject(ObjectIdentity);

        ObjectSpawner.Instance.SpawnObjectOnRay();
    }

    public void OnPointerUp()
    {
        ObjectSpawner.Instance.ReleaseObject();
    }
}
