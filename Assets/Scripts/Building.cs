using System;
using Unity.XR.CoreUtils;
using UnityEngine;

public class Building : MonoBehaviour, IInteractable
{
    private static readonly int Scale = Shader.PropertyToID("_Scale");
    
    private const float HIGHLIGHT_SCALE = 1.1F;
    private const float DEFAULT_SCALE = 1F;


    [SerializeField] private Material highlightMaterial;
    [SerializeField] private GameObject text;
    [SerializeField] private Transform spawnPoint;

    private MeshRenderer _meshRenderer;
    
    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        
        _meshRenderer.AddMaterial(highlightMaterial);
    }

    public void Highlight()
    {
        _meshRenderer.materials[1].SetFloat(Scale, HIGHLIGHT_SCALE);
        text.SetActive(true);
        
    }

    public void Unhighlight()
    {
        _meshRenderer.materials[1].SetFloat(Scale, DEFAULT_SCALE);
        text.SetActive(false);

    }
    
    public Transform GetSpawnPoint()
    {
        return spawnPoint;
    }
}
