using UnityEngine;

namespace AdvancedHelicopterControllerwithShooting
{
    public class MissileScript : MonoBehaviour
    {
        public GameObject explosionPrefab;
        public int DamagePower = 25;
        public Transform particle_following;
        public bool isEnemyMissile = false;

        private void OnCollisionEnter(Collision collision)
        {
            if(isEnemyMissile)
            {
                if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Helicopter"))
                {
                    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    if (particle_following != null)
                    {
                        particle_following.parent = null;
                    }
                    if(collision.collider.CompareTag("Helicopter"))
                    {
                        HelicopterController.Instance.GetDamage(DamagePower);
                    }
                    Destroy(gameObject);
                }
            }
            else
            {
                if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Enemy"))
                {
                    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    if (particle_following != null)
                    {
                        particle_following.parent = null;
                    }
                    Vector3 explosionPos = transform.position;
                    Collider[] colliders = Physics.OverlapSphere(explosionPos, 15);
                    foreach (Collider hit in colliders)
                    {
                        if (hit.CompareTag("Enemy"))
                        {
                            hit.GetComponent<EnemyAI>().GetDamage(DamagePower);
                        }
                    }
                    Destroy(gameObject);
                }
                else if (collision.collider.CompareTag("Collapsable"))
                {
                    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    Vector3 explosionPos = transform.position;
                    Collider[] colliders = Physics.OverlapSphere(explosionPos, 10);
                    foreach (Collider hit in colliders)
                    {
                        Rigidbody rb = hit.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.isKinematic = false;
                            rb.useGravity = true;
                            rb.AddExplosionForce(100, explosionPos, 10, 3.0F);
                            Destroy(hit.gameObject, 10);
                        }
                    }
                    Destroy(gameObject);
                }
            }
        }
    }
}
