using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeController : MonoBehaviour
{
    public GameObject TimelineView;
    public GameObject TimelineGrabbable;

    public GameObject ProgressionView;
    public GameObject ProgressionGrabbable;

    public GameObject SimilarityView;
    public GameObject SimilarityGrabbable;


    // Start is called before the first frame update
    void Start()
    {
        //Timeline view is visible by default
        TimelineView.SetActive(true);
        TimelineGrabbable.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //Similarity
        if (!StartingSpaceSelector.instance || !EndingSpaceSelector.instance) { 
            //Do nothing because it's too early anyway
        }
        else if (StartingSpaceSelector.instance.startingSpace && EndingSpaceSelector.instance.endingSpace) {
            if (TimelineView.activeSelf)
            {
                TimelineView.SetActive(false);
                TimelineGrabbable.SetActive(false);
            }
            if (ProgressionView.activeSelf) {
                ProgressionView.SetActive(false);
                ProgressionGrabbable.SetActive(false);
            }
            
            SimilarityView.SetActive(true);
            SimilarityGrabbable.SetActive(true);
        }
        //Progression
        else if (!StartingSpaceSelector.instance.startingSpace && EndingSpaceSelector.instance.endingSpace)
        {
            if (TimelineView.activeSelf)
            {
                TimelineView.SetActive(false);
                TimelineGrabbable.SetActive(false);
            }
            if (SimilarityView.activeSelf)
            {
                SimilarityView.SetActive(false);
                SimilarityGrabbable.SetActive(false);
            }
            ProgressionView.SetActive(true);
            ProgressionGrabbable.SetActive(true);
        }
        //Timeline
        else if (!StartingSpaceSelector.instance.startingSpace && !EndingSpaceSelector.instance.endingSpace)
        {
            if (ProgressionView.activeSelf)
            {
                ProgressionView.SetActive(false);
                ProgressionGrabbable.SetActive(false);
            }
            if (SimilarityView.activeSelf)
            {
                SimilarityView.SetActive(false);
                SimilarityGrabbable.SetActive(false);
            }
            TimelineView.SetActive(true);
            TimelineGrabbable.SetActive(true);
        }
    }

}
