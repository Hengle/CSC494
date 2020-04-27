using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class ProgressionViewFocusSelector : MonoBehaviour
{
    ProgressionViewManager progressionViewManager;

    public static ProgressionViewFocusSelector instance { get => _instance ?? FindObjectOfType<ProgressionViewFocusSelector>(); }
    static ProgressionViewFocusSelector _instance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        progressionViewManager = ProgressionViewManager.instance;
    }

    void OnTriggerEnter(Collider collider)
    {
        DesignSpace designSpace = collider.gameObject.GetComponent<DesignSpace>();

        if (designSpace)
        {

            progressionViewManager.hovering.Add(designSpace);

            /*
            progressionViewManager.ProgressionLeaf = designSpace;
            progressionViewManager.ProgressionLeaf.transform.parent = this.transform;
            //progressionViewManager.ProgressionLeaf.transform.localPosition = new Vector3(0f, 0f, 0f);

            progressionViewManager.ProgressionLeaf.transform.AnimateLocalPosition(new Vector3(0f, 0f, 0f));
            progressionViewManager.MoveToProgressionView(designSpace);
            */
        }
    }
    void OnTriggerExit(Collider collider)
    {
        DesignSpace designSpace = collider.gameObject.GetComponent<DesignSpace>();

        if (designSpace)
        {
            progressionViewManager.hovering.Remove(designSpace);

            //progressionViewManager.ProgressionLeaf = null;

        }

    }
}
