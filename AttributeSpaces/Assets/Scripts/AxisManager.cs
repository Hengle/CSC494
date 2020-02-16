using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisManager : MonoBehaviour
{
    public Axis x_axis;
    public Axis y_axis;
    public Axis z_axis;
    public DesignSpace DS;
    // Start is called before the first frame update
    void Start()
    {
        x_axis = null;
        y_axis = null;
        z_axis = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateXAxis(Axis X)
    {
        x_axis = X;
        DS.x_attr = X;
    }
    public void UpdateYAxis(Axis Y)
    {
        y_axis = Y;
        DS.y_attr = Y;
    }
    public void UpdateZAxis(Axis Z)
    {
        z_axis = Z;
        DS.z_attr = Z;
    }
}
