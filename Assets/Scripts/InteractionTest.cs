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
    private GameObject _currentInteract;
    private bool _isInteracting;

    private void Start()
    {
    }

    private void OnEnable()
    {
        inputAsset[actionRef.name].performed += Select;

    }

    private void OnDisable()
    {
        inputAsset[actionRef.name].performed -= Select;

    }

    private void Update()
    {
        if (TransitionManager.Instance.isTransitioning)
        {
            return;
        }

        
        _isHitting =
            Physics.Raycast(transform.position, transform.forward, out  _hit, rayLength, layerMask);
        
        if (_isHitting && _currentInteract == null)
        {
            _currentInteract = _hit.collider.gameObject;

            if (_currentInteract.TryGetComponent(out IInteractable iInteractable))
            {
                iInteractable.Highlight();
            }

            _isInteracting = true;
        }
        
        if(!_isHitting)
        {
            if (_currentInteract)
            {
                if (_currentInteract.TryGetComponent(out IInteractable iInteractable))
                {
                    iInteractable.Unhighlight();
                }

                _currentInteract = null;
            }
        }
        
      //  validSphere.SetActive(_isHitting);
      //  validSphere.transform.position = new Vector3(_hit.point.x, 88, _hit.point.z);
    }

    private void Select(InputAction.CallbackContext ctx)
    {
        if (TransitionManager.Instance.isTransitioning)
        {
            return;
        }

        Debug.Log(gameObject.name);
        if (_currentInteract)
        {
            if (_currentInteract.TryGetComponent(out IInteractable iInteractable))
            {
                TransitionManager.Instance.TransitionToState(TransitionManager.TransitionState.FutureOnFloor, iInteractable.GetSpawnPoint().position);
                iInteractable.Unhighlight();
            }
        }
    }
}
