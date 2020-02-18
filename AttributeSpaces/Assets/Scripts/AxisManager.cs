﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

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
        //Expand the design space box to accomodate the new axis
        DS.spaceBox.transform.localScale = DS.spaceBox.transform.localScale.SetX(1.0f);
        DS.UpdateLocations();
        //TODO update where the objects are in the space
    }
    public void UpdateYAxis(Axis Y)
    {
        y_axis = Y;
        DS.y_attr = Y;
        //Expand the design space box to accomodate the new axis
        DS.spaceBox.transform.localScale = DS.spaceBox.transform.localScale.SetY(1.0f);
        DS.UpdateLocations();
    }
    public void UpdateZAxis(Axis Z)
    {
        z_axis = Z;
        DS.z_attr = Z;
        //Expand the design space box to accomodate the new axis
        DS.spaceBox.transform.localScale = DS.spaceBox.transform.localScale.SetZ(1.0f);
        DS.UpdateLocations();
    }

    public void RemoveXAxis(Axis X)
    {
        x_axis = null;
        DS.x_attr = null;
        DS.spaceBox.transform.localScale = DS.spaceBox.transform.localScale.SetX(0.1f);
        DS.UpdateLocations();
        //TODO update where the objects are in the space
    }
    public void RemoveYAxis(Axis Y)
    {
        y_axis = null;
        DS.y_attr = null;
        DS.spaceBox.transform.localScale = DS.spaceBox.transform.localScale.SetY(0.1f);
        DS.UpdateLocations();
    }
    public void RemoveZAxis(Axis Z)
    {
        z_axis = null;
        DS.z_attr = null;
        DS.spaceBox.transform.localScale = DS.spaceBox.transform.localScale.SetZ(0.1f);
        DS.UpdateLocations();
    }
}
