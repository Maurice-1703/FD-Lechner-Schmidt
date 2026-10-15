using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private Material highPositionMaterial;
    [SerializeField] private Material lowPositionMaterial;
    [SerializeField] private GameObject platform;
    private MeshRenderer rend;

    void Start()
    {
        rend = platform.GetComponent<MeshRenderer>();
    } 
    public void AtLowestPoint()
    {
        rend.sharedMaterial = lowPositionMaterial;
    }

    public void AtHighestPoint()
    {
        rend.sharedMaterial = highPositionMaterial;
    }
}   