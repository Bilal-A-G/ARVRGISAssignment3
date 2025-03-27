using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionTest : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputAsset;
    [SerializeField] private InputActionReference actionRef;
    [SerializeField] private GameObject validSphere;

    [Space] 
    
    [SerializeField] private float rayLength;
    [SerializeField] private LayerMask layerMask;

    private RaycastHit _hit;
    private bool _isHitting;

    private void Start()
    {
        inputAsset[actionRef.name].performed += ctx => Select();
    }

    private void Update()
    {
        if (TransitionManager.Instance.isTransitioning)
        {
            return;
        }

        
        _isHitting =
            Physics.Raycast(transform.position, transform.forward, out  _hit, rayLength, layerMask);
        
        validSphere.SetActive(_isHitting);
        validSphere.transform.position = new Vector3(_hit.point.x, 88, _hit.point.z);
    }

    private void Select()
    {
        if (TransitionManager.Instance.isTransitioning)
        {
            return;
        }


        if (_isHitting)
        {
            validSphere.SetActive(false);
            TransitionManager.Instance.TransitionToFuture(_hit.point);
            enabled = false;
        }
    }
}
