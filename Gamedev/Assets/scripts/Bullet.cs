using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Temp bullet damage
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FlySwarm")) 
        {
            // Damage the fly
            FlyAI fly = other.GetComponent<FlyAI>(); 
            if (fly != null)
            {
                fly.TakeDamage(damage); 
            }

            Destroy(gameObject); 
        }
    }
}
