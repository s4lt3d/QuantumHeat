using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FireBulletOnActivate : MonoBehaviour
{
    public GameObject bullet;
    public Transform spawnPoint;
    public float fireSpeed = 20;
    public AudioSource source;
    public AudioClip clip;

    private XRGrabInteractable grabbable;

    private void Start()
    {
        // Initialize grabbable and add FireBullet listener
        grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireBullet);
    }

    // The update method is not used, so it's removed for readability

    public void FireBullet(ActivateEventArgs arg = null)
    {
        // Create a bullet and launch it
        GameObject spawnedBullet = SpawnBullet();
        LaunchBullet(spawnedBullet);

        // Play the gunshot sound
        source.PlayOneShot(clip);
    }

    // Method to spawn a bullet
    private GameObject SpawnBullet()
    {
        GameObject spawnedBullet = Instantiate(bullet);
        spawnedBullet.transform.position = spawnPoint.position;
        return spawnedBullet;
    }

    // Method to launch the bullet
    private void LaunchBullet(GameObject bullet)
    {
        bullet.GetComponent<Rigidbody>().velocity = spawnPoint.forward * fireSpeed;

        // Destroy the bullet after 5 seconds
        Destroy(bullet, 5);
    }
}
