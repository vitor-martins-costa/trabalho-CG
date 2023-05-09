using System.Collections;
using UnityEngine;

namespace AdvancedHelicopterControllerwithShooting
{
    public class BulletScript : MonoBehaviour
    {
        public GameObject explosionPrefab;
        public int DamagePower = 5;
        IEnumerator Start()
        {
            yield return new WaitForSeconds(3);
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if ((collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("Collapsable")))
            {
                GameObject muzzle = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                muzzle.transform.eulerAngles = new Vector3(-90, 0, 0);

                if (collision.collider.CompareTag("Collapsable"))
                {
                    Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                        rb.useGravity = true;
                        rb.AddExplosionForce(10, transform.position, 1, 1f);
                        Destroy(collision.collider.gameObject, 10);
                    }
                }
                else if(collision.collider.CompareTag("Enemy"))
                {
                    collision.collider.GetComponent<EnemyAI>().GetDamage(DamagePower);
                }
                Destroy(gameObject);
            }
        }
    }
}
