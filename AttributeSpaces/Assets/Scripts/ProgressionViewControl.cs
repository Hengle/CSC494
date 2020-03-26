using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Unity.Mathematics;

public class ProgressionViewControl : MonoBehaviour
{

    public GameObject ProgressionPanel;
    public GameObject SpacePanel;
    //public GameObject FilterParent;
    public OVRInput.Controller controller;

    static SavedSpaceManager savedSpaceManager;

    public List<List<DesignSpace>> Progressions;

    //The key is the index into the saved space list (for the originals) and the values are the location in the progression view list
    Dictionary<int, int> SavedSpaceIndexToProgressionViewIndex;

    bool animate;
    // Start is called before the first frame update
    void Start()
    {
        SavedSpaceIndexToProgressionViewIndex = new Dictionary<int, int>();
        savedSpaceManager = SavedSpaceManager.instance;
        SpacePanel.gameObject.SetActive(false);
        animate = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (OVRInput.Get(OVRInput.Button.Three, controller) || OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, controller) > 0.0f)
        {
            //TODO take out everything with TEMP in it 
            savedSpaceManager.UpdateSpaceContents();
            SpacePanel.gameObject.SetActive(true);
            //FilterParent.SetActive(false);
        }
        else
        {
            SpacePanel.gameObject.SetActive(false);
        }
       
        if (OVRInput.GetDown(OVRInput.Button.Three, controller))
        {
            for (int i = 0; i < savedSpaceManager.SavedSpaces.Count; i++)
            {
                Vector3 newLocation = new Vector3(savedSpaceManager.SavedSpaces[i].transform.localPosition.x, (float)savedSpaceManager.SavedSpaces[i].originalSpaceIndex * 0.1f, savedSpaceManager.SavedSpaces[i].transform.localPosition.z);
                StartCoroutine(AnimateSavedSpace(savedSpaceManager.SavedSpaces[i].transform, newLocation));
            }



            
        }
        if (OVRInput.GetUp(OVRInput.Button.Three, controller))
        {
            print("Up");
            animate = false;
            for (int i = 0; i < savedSpaceManager.SavedSpaces.Count; i++)
            {   //Take the y position of the first object because it's bound to be the right y
                Vector3 newLocation = new Vector3(savedSpaceManager.SavedSpaces[i].transform.localPosition.x, savedSpaceManager.SavedSpaces[0].transform.localPosition.y, savedSpaceManager.SavedSpaces[i].transform.localPosition.z);
                //Vector3 newLocation = savedSpaceManager.SavedSpaces[i].transform.localPosition - new Vector3(0f, (float)savedSpaceManager.SavedSpaces[i].originalSpaceIndex * 0.3f, 0f);
                StartCoroutine(AnimateSavedSpace(savedSpaceManager.SavedSpaces[i].transform, newLocation));
            }

        }


    }
    IEnumerator AnimateSavedSpace(Transform savedSpace, Vector3 newLocation){
        while (math.distance(savedSpace.localPosition, newLocation) > 1e-100) {
            savedSpace.localPosition = math.lerp(savedSpace.localPosition, newLocation, 0.15f);
            yield return 0;
        }
       
    }


}
