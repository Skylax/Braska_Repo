using UnityEditor;
using UnityEngine;

public class OrbFollow : MonoBehaviour
{
    [Header("Progress stats")]
    public int currentLevel;

    [Header("Follow stats")]
    [SerializeField] bool followStart = false;
    [SerializeField] float followSpeed;

    [Header("Object references")]
    public GameObject orbFollow;
    public GameObject teleportLobby;
    

    private void Awake()
    {
        orbFollow = GameObject.Find("OrbFollow");
    }

    private void Start()
    {
        if (currentLevel <= ScenesManager.instance.collectedOrbs)
        {
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (followStart)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                orbFollow.transform.position,
                followSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dig") && !ObjectManager.instance.hasOrb)
        {
            ObjectManager.instance.hasOrb = true;

            AudioManager.Instance.PlaySFX(2);
            Invoke(nameof(FollowStart), 1f);
        }
    }

    private void FollowStart()
    {
        AudioManager.Instance.PlaySFX(3);
        followStart = true;
    }
}
