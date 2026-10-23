using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private Material highPositionMaterial;
    [SerializeField] private Material lowPositionMaterial;
    [SerializeField] private GameObject platform;
    private MeshRenderer rend;


    [SerializeField] private Transform player;
    [SerializeField] private float radius = 6f;
    [SerializeField] private float turnSpeedofChars = 5f;


    void Start()
    {
        rend = platform.GetComponent<MeshRenderer>();
    } 
    public void AtLowestPoint()
    {
        rend.sharedMaterial = lowPositionMaterial;
    }

    public void AtHighestPoint()
    {
        rend.sharedMaterial = highPositionMaterial;
    }

    void Update()
    {
        bool near = Vector3.Distance(player.position, transform.position) < radius;

        foreach (Animator dancer in GetComponentsInChildren<Animator>())
        {
            // Skip the platform itself and any object without an Animator Controller
            if (dancer.transform == transform || dancer.runtimeAnimatorController == null) continue;

            dancer.SetBool("isPlayerNear", near);

            if (near)
            {
                // Direction to the player, flattened so the character stays upright
                Vector3 toPlayer = player.position - dancer.transform.position;
                toPlayer.y = 0f;

                // Turn smoothly towards the player
                Quaternion target = Quaternion.LookRotation(toPlayer);
                dancer.transform.rotation = Quaternion.Slerp(
                    dancer.transform.rotation, target, turnSpeedofChars * Time.deltaTime);
            }
        }
    }


    // Shows the detection radius in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}   