using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
    [SerializeField]
    private GameObject maskPieceGameObject;
    private Transform beltTransform;
    private Transform beltSpawnPointTransform;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float spawnDelaySeconds;
    private float nextSpawnTime;

    private void Awake()
    {
        beltTransform = transform.Find("Belt");
        beltSpawnPointTransform = transform.Find("MaskPieceSpawnPoint");
    }

    void Start()
    {
        nextSpawnTime = Time.time;
    }

    void Update()
    {
        if (nextSpawnTime <= Time.time)
        {
            AddToConveyor();
        }
    }

    void AddToConveyor()
    {
        GameObject createdMaskPiece = Instantiate(maskPieceGameObject, beltSpawnPointTransform.position, beltSpawnPointTransform.rotation, beltTransform);
        createdMaskPiece.GetComponent<Rigidbody2D>().AddForce(new Vector2(-speed, 0.0f));

        nextSpawnTime = Time.time + spawnDelaySeconds;
        Debug.Log("Spawning mask piece.");
    }
}
