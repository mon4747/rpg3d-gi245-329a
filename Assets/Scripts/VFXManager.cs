using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField]
    private GameObject doubleRingMarker;
    public GameObject DoubleRingMarker { get { return doubleRingMarker; } }
    public static VFXManager instance;

    [SerializeField]
    private GameObject[] magicVFX;
    public GameObject[] MagicVFX { get { return magicVFX; } }

    [SerializeField]
    private MagicData[] magicData;
    public MagicData[] MagicData { get { return magicData; } }

    private void Awake()
    {
        instance = this;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMagic(int id, Vector3 posA, float time)
    {
        if (magicVFX[id] == null)
        {
            return;
        }

        GameObject objLoad = Instantiate(MagicVFX[id], posA, Quaternion.identity) as GameObject;
        Destroy(objLoad, time);
    }

    public void ShootMagic(int id, Vector3 posA, Vector3 posB, float time)
    {
        if (magicVFX[id] == null)
        {
            return;
        }

        GameObject objShoot = Instantiate(MagicVFX[id], posA, Quaternion.identity);
        objShoot.transform.position = Vector3.LerpUnclamped(posA, posB, time);
        Destroy(objShoot, time);
    }
}
