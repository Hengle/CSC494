using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class SetVisiblity : MonoBehaviour
{
    public bool visible;

    bool prevVisible;

    // Start is called before the first frame update
    void Start()
    {
        prevVisible = !visible;

        OnValidate();
    }

    void OnValidate()
    {
        SetVisibility(visible);
    }

    public void SetVisibility(bool visible)
    {
        this.visible = visible;

        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = visible;
        }

        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.enabled = visible;
        }

        foreach (LineRenderer renderer in GetComponentsInChildren<LineRenderer>())
        {
            renderer.enabled = visible;
        }
    }
}
