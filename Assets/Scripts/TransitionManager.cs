using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public enum TransitionType
    {
        Present,
        Future
    }


    [Header("Present & Future Objects")]
    [SerializeField] private GameObject presentObject;
    [SerializeField] private GameObject futureObject;
    [SerializeField] private GameObject transitionBtn;
    [SerializeField] private float futureTargetY;

    [Space]
    
    
    [Header("XR")]
    
    [SerializeField] private GameObject eyeXROrigin;
    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private Quaternion startingRotation;
    [SerializeField] private GameObject floorXROrigin;

    
    
    [SerializeField] private float timeToTransition = 2f;

    public static TransitionManager Instance;
    
    
    private TransitionType _type;

    private Vector3 _startPos;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        eyeXROrigin.transform.position = startingPosition;
        eyeXROrigin.transform.rotation = startingRotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
          //  Transition(TransitionType.Future);
        }
    }

    public void Transition()
    {
        Debug.Log("do trans");
        transitionBtn.SetActive(false);

        _ = TransitionTask();
    }


    private async UniTask TransitionTask()
    {
        Debug.Log("Start transition....");
        _startPos = futureObject.transform.position;

        
        float camCurrentTime = 0;
        while (camCurrentTime < timeToTransition)
        {
            camCurrentTime += Time.deltaTime;
            ProcessTransition(camCurrentTime);
            
            await UniTask.Yield();
        }
        
        Debug.Log("End transition....");
        eyeXROrigin.SetActive(false);
        floorXROrigin.SetActive(true);
    }


    private void ProcessTransition(float time)
    {
        
        eyeXROrigin.transform.position = Vector3.Lerp(startingPosition, floorXROrigin.transform.position, time / timeToTransition);
        eyeXROrigin.transform.rotation = Quaternion.Lerp(startingRotation, floorXROrigin.transform.rotation, time / timeToTransition);
        
        futureObject.transform.position = Vector3.Lerp(_startPos, new Vector3(futureObject.transform.position.x, futureTargetY, futureObject.transform.position.z),
            time *2 / timeToTransition);

    }
    
    float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
    }
}
