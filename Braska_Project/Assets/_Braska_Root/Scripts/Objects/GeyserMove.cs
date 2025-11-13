using Unity.VisualScripting;
using UnityEngine;

public class GeyserMove : MonoBehaviour
{
    [Header("Geyser stats")]
    public Vector3 startingPoint;
    public float maxHeight;
    private Vector3 currentSpeed = Vector3.zero; // Is just the current speed, it updates itself automarically.
    public bool emitParicles;

    [Header("Object references")]
    public GameObject geyserPlatform;
    public GameObject water;
    public ParticleSystem particlesBase;
    public ParticleSystem particlesTop;

    private bool geyserSoundPlaying = false;

    private void Start()
    {
        startingPoint = geyserPlatform.transform.position;

        emitParicles = false;

        particlesBase.Stop();
        particlesTop.Stop();
    }

    private void Update()
    {
        WarpWater();
    }

    private void WarpWater()
    {
        water.transform.localPosition = geyserPlatform.transform.localPosition / 2;

        water.transform.localScale = new Vector3(
            water.transform.localScale.x,
            geyserPlatform.transform.localPosition.y / 2,
            water.transform.localScale.z
        );
    }

    private void FixedUpdate()
    {
        MovePlatform();
    }

    private void MovePlatform()
    {
        if (ObjectManager.instance.geyserIsUp)
        {
            geyserPlatform.transform.position = Vector3.SmoothDamp( // SmoothDamp adds acceleration and deceleration to the movement.
                geyserPlatform.transform.position,
                new Vector3(
                    geyserPlatform.transform.position.x,
                    startingPoint.y + maxHeight,
                    geyserPlatform.transform.position.z
                ),
                ref currentSpeed,
                ObjectManager.instance.geyserMoveTime * 10 * Time.deltaTime
            );

            if (!emitParicles)
            {
                emitParicles = !emitParicles;
                
                particlesBase.Play();
                particlesTop.Play();
            }
            if (!geyserSoundPlaying)
            {
                AudioManager.Instance.PlaySFX(6);
                geyserSoundPlaying = true;
            }
        }
     
        else
        {
            geyserPlatform.transform.position = Vector3.SmoothDamp(
                geyserPlatform.transform.position,
                startingPoint,
                ref currentSpeed,
                ObjectManager.instance.geyserMoveTime * 10 * Time.deltaTime


            );

            if (emitParicles)
            {
                emitParicles = !emitParicles;

                particlesBase.Stop();
                particlesTop.Stop();
            }
            if (geyserSoundPlaying)
            {
                geyserSoundPlaying = false;
                // Opcional: si quieres que se corte, puedes reproducir un SFX de “parada” o dejarlo
            }
        }
    }
}
