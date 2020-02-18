using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject : MonoBehaviour
{
    //Each sceeneobject has a list of attributes
    //This populates the slider that is associated with it
    public List<Attribute> AttributeList = new List<Attribute>();

    //Hide and show the scrollselector
    public GameObject scrollObject;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if a button is clicked in this object, show the scroll object
    }

    public void showScroller() {

    }
    public void hideScroller() {

    }

}
