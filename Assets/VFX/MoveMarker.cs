using UnityEngine;

public class MoveMarker : MonoBehaviour
{
    [SerializeField]
    private float lifetime = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject,lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
