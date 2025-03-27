using UnityEngine;
using UnityEngine.InputSystem;

public class ReturnBack : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputAsset;
    [SerializeField] private InputActionReference actionRef;
    
    private void Start()
    {
        inputAsset[actionRef.name].performed += ctx => Return();
    }

    void Return()
    {
        Debug.Log("Return");
        TransitionManager.Instance.TransitionToPresent();
    }
}
