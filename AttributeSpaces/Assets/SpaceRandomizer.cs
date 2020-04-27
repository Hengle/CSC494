using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceRandomizer : MonoBehaviour
{
    public OVRInput.Controller controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (DesignSpaceManager.instance.main_index == GetComponent<DesignSpace>().DesignSpaceID && OVRInput.GetDown(OVRInput.Button.Three, controller))
        {
            //Look at the progression view and use that to tell you which spots to avoid in your random search 
            //ProgressionViewManager.instance.UpdateProgressionList(this);


            List<DesignSpace> ProgressionList = new List<DesignSpace>();

            //  ProgressionList.Add(DesignSpaceManager.instance._DesignSpaceList[designSpace.directAncestorID]);
            if (GetComponent<DesignSpace>().directAncestorID >= 0)
            {
                for (int i = 0; i < DesignSpaceManager.instance._DesignSpaceList[GetComponent<DesignSpace>().directAncestorID].directDescendents.Count; i++)
                {
                    if (DesignSpaceManager.instance._DesignSpaceList[GetComponent<DesignSpace>().directAncestorID].directDescendents[i].DesignSpaceID < GetComponent<DesignSpace>().DesignSpaceID)
                    {
                        ProgressionList.Add(DesignSpaceManager.instance._DesignSpaceList[GetComponent<DesignSpace>().directAncestorID].directDescendents[i]);
                    }

                }
            }


            System.Random random = new System.Random();

            //Generate a random location for the new ball
            float x_loc = (float)random.Next(0, 100) / 100f;
            float y_loc = (float)random.Next(0, 100) / 100f;
            float z_loc = (float)random.Next(0, 100) / 100f;

            if (!GetComponent<DesignSpace>().x_attr)
            {
                x_loc = 0f;
            }
            if (!GetComponent<DesignSpace>().y_attr)
            {
                y_loc = 0f;
            }
            if (!GetComponent<DesignSpace>().z_attr)
            {
                z_loc = 0f;
            }
            Vector3 randomized_location = new Vector3(x_loc, y_loc, z_loc);

            bool retry = true;

            for (int i = 0; i < ProgressionList.Count; i++)
            {
                if (ProgressionList[i].controlCube.transform.localPosition == randomized_location)
                {
                    x_loc = (float)random.Next(0, 100) / 100f;
                    y_loc = (float)random.Next(0, 100) / 100f;
                    z_loc = (float)random.Next(0, 100) / 100f;
                }
                /*
                if (Math.Abs((float)ProgressionViewManager.instance.ProgressionList[i].controlCube.transform.localPosition.x - x_loc) <= 0.001)
                {
                    x_loc = (float)random.Next(0, 100) / 100f;
                    retry = true;
                }
                if (Math.Abs((float)ProgressionViewManager.instance.ProgressionList[i].controlCube.transform.localPosition.y - y_loc) <= 0.001) {
                    x_loc = (float)random.Next(0, 100) / 100f;
                    retry = true;
                }
                if (Math.Abs((float)ProgressionViewManager.instance.ProgressionList[i].controlCube.transform.localPosition.z - z_loc) <= 0.001){
                    x_loc = (float)random.Next(0, 100) / 100f;
                    retry = true;
                }
                else {
                    retry = false;
                }
                */
            }


            randomized_location = new Vector3(x_loc, y_loc, z_loc);
            //Now move the control cube to that location

            //EVERY ATTRIBUTE SPACE IS CHANGING AT ONCE FOR SOME REASON
            SavedSpaceManager.instance.SaveSpace(GetComponent<DesignSpace>());

            GetComponent<DesignSpace>().lastSavedControlCubeLocation = GetComponent<DesignSpace>().controlCube.transform.localPosition;
            GetComponent<DesignSpace>().controlCube.transform.localPosition = randomized_location;


            //Also redo if it matches a constraint

        }
    }
}
