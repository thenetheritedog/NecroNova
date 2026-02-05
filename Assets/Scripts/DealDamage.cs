using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private float damage;
   [SerializeField] private float poiseDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            CharacterManager enemy = other.GetComponent<CharacterManager>();
            enemy.TakeDamage(damage, poiseDamage);
        }
    }
}
