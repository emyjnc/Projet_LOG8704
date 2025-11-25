using UnityEngine;

public class ComposterComponent : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<ItemComponent>(out var item) && item.type == ItemType.Compostable)
        {
            // TODO: add point to score
            Destroy(collision.gameObject);
        }
    }
}
