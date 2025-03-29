using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public enum TransitionState
    {
        Present,
        FutureBirdsEye,
        FutureOnFloor
    }

    [System.Serializable]
    public struct Transition
    {
        public TransitionState state;
        public float birdsEyeHeight;
        public float futureHeight;
        public float presentHeight;
        public bool birdsEyeEnabled;
        public bool floorEnabled;
    }
    
    [Header("Present & Future Objects")]
    [SerializeField] private Transition[] transitions;
    [SerializeField] private GameObject presentObject;
    [SerializeField] private GameObject futureObject;
    
    [SerializeField] private float futureTargetY;
    

    [Space]
    
    
    [Header("XR")]
    
    [SerializeField] private GameObject eyeXROrigin;
    [SerializeField] private GameObject floorXROrigin;

    [Space]
    
    [Header("Starting Positions For BIRDS EYE")]
    
    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private Quaternion startingRotation;

    
    
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
        
        TransitionToState(TransitionState.FutureBirdsEye, Vector3.zero);
    }
    

    public void TransitionToState(TransitionState state, Vector3 floorXRPosition)
    {
        _ = TransitionToStateTask(state, floorXRPosition);
    }

    //should probs take into consideration the rotation 
    private async UniTask TransitionToStateTask(TransitionState state, Vector3 floorXRPosition) 
    {
        if (isTransitioning) return;
        isTransitioning = true;

        int index = GetTransitionIndex(state);
        Transition transition = transitions[index];

        Vector3 startPos = eyeXROrigin.activeSelf ? eyeXROrigin.transform.position : floorXROrigin.transform.position;
        Quaternion startRot = eyeXROrigin.activeSelf ? eyeXROrigin.transform.rotation : floorXROrigin.transform.rotation;

        Vector3 targetPos = floorXRPosition == Vector3.zero ? startPos : floorXRPosition;
        targetPos.y = state switch
        {
            TransitionState.Present => transition.presentHeight,
            TransitionState.FutureBirdsEye => transition.birdsEyeHeight,
            TransitionState.FutureOnFloor => transition.futureHeight,
            _ => startPos.y
        };
    
        Quaternion targetRot = (state == TransitionState.FutureBirdsEye) ? startingRotation : Quaternion.identity;

        Vector3 futureStartPos = futureObject.transform.position;
        Vector3 presentStartPos = presentObject.transform.position;

        Vector3 futureTargetPos = new Vector3(futureStartPos.x, transition.futureHeight, futureStartPos.z);
        Vector3 presentTargetPos = new Vector3(presentStartPos.x, transition.presentHeight, presentStartPos.z);

        float elapsedTime = 0f;
        while (elapsedTime < timeToTransition)
        {
            float t = elapsedTime / timeToTransition;
            if (eyeXROrigin.activeSelf)
            {
                eyeXROrigin.transform.position = Vector3.Lerp(startPos, targetPos, t);
                eyeXROrigin.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            }
            else
            {
                floorXROrigin.transform.position = Vector3.Lerp(startPos, targetPos, t);
                floorXROrigin.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            }
            futureObject.transform.position = Vector3.Lerp(futureStartPos, futureTargetPos, t);
            presentObject.transform.position = Vector3.Lerp(presentStartPos, presentTargetPos, t);
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();
        }

        if (transition.birdsEyeEnabled)
        {
            eyeXROrigin.transform.position = targetPos;
            eyeXROrigin.transform.rotation = targetRot;
            floorXROrigin.SetActive(false);
            eyeXROrigin.SetActive(true);

        }
        else if (transition.floorEnabled)
        {
            floorXROrigin.transform.position = floorXRPosition == Vector3.zero ? targetPos : floorXRPosition;
            floorXROrigin.transform.rotation = targetRot;
            eyeXROrigin.SetActive(false);
            floorXROrigin.SetActive(true);
        }
        else
        {
            eyeXROrigin.transform.position = targetPos;
            eyeXROrigin.transform.rotation = targetRot;
            floorXROrigin.SetActive(false);
            eyeXROrigin.SetActive(true);

        }

        futureObject.transform.position = futureTargetPos;
        presentObject.transform.position = presentTargetPos;

        isTransitioning = false;
    }


    private int GetTransitionIndex(TransitionState state)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            if (state == transitions[i].state)
            {
                return i;
            }
        }

        return 0;
    }
    
    /*/
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
        /*

        float camTime = 0;
        while (camTime < timeToTransition)
        {
            camTime += Time.deltaTime;
            eyeXROrigin.transform.position = Vector3.Lerp(floorXROrigin.transform.position, startingPosition, camTime / timeToTransition);
            eyeXROrigin.transform.rotation = Quaternion.Lerp( floorXROrigin.transform.rotation, startingRotation, camTime / timeToTransition);
            await UniTask.Yield();
        }
        /*
        
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
    /*/
}
