using System;
using UnityEngine;

public class WindMill : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;


    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
