using UnityEngine;

public interface IInteractable
{
    void Highlight();
    void Unhighlight();
    
    Transform GetSpawnPoint();
    
}
