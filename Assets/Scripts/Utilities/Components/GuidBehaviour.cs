using UnityEngine;

[DisallowMultipleComponent]
public class GuidBehaviour : MonoBehaviour
{
    [SerializeField] string id;
    public string Id => GetId();

    public void SetId(string newId)
    {
        if (!string.IsNullOrEmpty(newId))
            id = newId;
    }
    public string GetId()
    {
        if (!string.IsNullOrEmpty(id))
            return id;
        
        id = System.Guid.NewGuid().ToString("N");
        return id;
    }

    public void GenerateNewGuid()
    {
        id = System.Guid.NewGuid().ToString("N");
    }

    void Awake()
    {
        if (string.IsNullOrEmpty(id))
            id = System.Guid.NewGuid().ToString("N");
    }
}
