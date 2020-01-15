using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMPonCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
    }
    void OnCollisionEnter(Collision collision)
    {
        print("collision enter");
        GetComponent<Rigidbody>().isKinematic = false;
    }
    void OnCollisionStay(Collision collision)
    {
        print("collision stay");
        GetComponent<Rigidbody>().isKinematic = false;
    }
    void OnCollisionExit(Collision collision)
    {
        print("collision exit");
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
