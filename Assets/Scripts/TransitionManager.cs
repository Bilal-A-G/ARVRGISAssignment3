using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    [Header("Present & Future Objects")]
    [SerializeField] private GameObject presentObject;
    [SerializeField] private GameObject futureObject;
    [SerializeField] private float futureTargetY;
    

    [Space]
    
    
    [Header("XR")]
    
    [SerializeField] private GameObject eyeXROrigin;
    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private Quaternion startingRotation;
    [SerializeField] private GameObject floorXROrigin;

    
    
    [SerializeField] private float timeToTransition = 2f;

    public static TransitionManager Instance;
    
    
    private Vector3 m_StartPos;
    
    public bool isTransitioning { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        futureObject.transform.position = new Vector3(0, 44, 0);
        eyeXROrigin.transform.position = startingPosition;
        eyeXROrigin.transform.rotation = startingRotation;
    }
    

    public void TransitionToFuture(Vector3 target)
    {
        _ = TransitionToFutureTask(target);
    }


    private async UniTask TransitionToFutureTask(Vector3 target)
    {
        isTransitioning = true;
        Debug.Log("Start transition....");
        m_StartPos = futureObject.transform.position;

        floorXROrigin.transform.position = new Vector3(target.x, 88, target.z);

        
        float campusTime = 0;
        while (campusTime < timeToTransition)
        {
            campusTime += Time.deltaTime;
            
            futureObject.transform.position = Vector3.Lerp(m_StartPos,
                new Vector3(futureObject.transform.position.x, futureTargetY, futureObject.transform.position.z),
                EaseInOutQuad(campusTime / timeToTransition));
            
            await UniTask.Yield();
        }

        float camTime = 0;
        while (camTime < timeToTransition)
        {
            camTime += Time.deltaTime;
            eyeXROrigin.transform.position = Vector3.Lerp(startingPosition, floorXROrigin.transform.position, camTime / timeToTransition);
            eyeXROrigin.transform.rotation = Quaternion.Lerp(startingRotation, floorXROrigin.transform.rotation, camTime / timeToTransition);
            await UniTask.Yield();
        }
        
        Debug.Log("End transition....");
        eyeXROrigin.SetActive(false);
        floorXROrigin.SetActive(true);
        isTransitioning = false;
    }

    public void TransitionToPresent()
    {
      //  _ = TransitionToPresentTask();
    }

    private async UniTask TransitionToPresentTask()
    {
        //idk why this wont work :( -- ill fix over the weekend :D
        isTransitioning = true;
        
        m_StartPos = futureObject.transform.position;
        
        eyeXROrigin.SetActive(true);
        floorXROrigin.SetActive(false);

        eyeXROrigin.transform.position = startingPosition;
        eyeXROrigin.transform.rotation = startingRotation;
        /*/

        float camTime = 0;
        while (camTime < timeToTransition)
        {
            camTime += Time.deltaTime;
            eyeXROrigin.transform.position = Vector3.Lerp(floorXROrigin.transform.position, startingPosition, camTime / timeToTransition);
            eyeXROrigin.transform.rotation = Quaternion.Lerp( floorXROrigin.transform.rotation, startingRotation, camTime / timeToTransition);
            await UniTask.Yield();
        }
        /*/
        
        float campusTime = 0;
        while (campusTime < timeToTransition)
        {
            campusTime += Time.deltaTime;
            
            futureObject.transform.position = Vector3.Lerp(m_StartPos,
                new Vector3(futureObject.transform.position.x, 41, futureObject.transform.position.z),
                EaseInOutQuad(campusTime / timeToTransition));
            
            await UniTask.Yield();
        }
        
        
        Debug.Log("End transition....");
        isTransitioning = false;
    }

        
    float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
    }
}
