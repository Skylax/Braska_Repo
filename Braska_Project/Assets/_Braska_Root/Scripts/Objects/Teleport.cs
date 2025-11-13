using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{
    [Header("Spawn stats")]
    [SerializeField] string sceneToLoad;
    [SerializeField] Vector3 newSpawnPoint;
    [SerializeField] float newSpawnView;

    [Header("Status stats")]
    [SerializeField] bool playerInside;
    [SerializeField] int requiredOrbs;
    public bool isActive;

    [Header("Object references")]
    [SerializeField] GameObject teleportLight;
    [SerializeField] GameObject teleportParticles;

    private void Awake()
    {
        teleportParticles.SetActive(false);
    }

    private void Start()
    {
        playerInside = false;

        if (ScenesManager.instance.collectedOrbs < requiredOrbs)
        {
            isActive = false;
        }

        if (ScenesManager.instance.collectedOrbs == requiredOrbs)
        {
            Highlight();
        }

        if (!isActive)
        {
            teleportLight.SetActive(false);
        }
    }

    private void Update()
    {
        if (ObjectManager.instance.hasOrb)
        {
            Invoke(nameof(Highlight), 0.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bark") && playerInside)
        {
            OrderTeleport();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && isActive)
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    private void OrderTeleport()
    {
        AudioManager.Instance.PlaySFX(9);
        ScenesManager.instance.TeleportPlayer(sceneToLoad, newSpawnPoint, newSpawnView);
    }

    public void Highlight()
    {
        teleportParticles.SetActive(true);
    }
}
