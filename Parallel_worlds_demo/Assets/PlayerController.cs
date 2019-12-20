using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class PlayerController : MonoBehaviour
{
    // Singleton
    public static PlayerController _instance;
    public static PlayerController instance { get => _instance ?? FindObjectOfType<PlayerController>(); }

    public HandController leftHand, rightHand;

    public float3 position { get => transform.position; set => transform.position = value; }
    public float3 scale { get => transform.localScale; set => transform.localScale = value; }

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        DesignSpaceManager manager = DesignSpaceManager.instance;
    }
}
