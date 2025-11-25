using UnityEngine;

public enum ItemType
{
    Trash,
    Recyclable,
    Compostable,
}

public class ItemComponent : MonoBehaviour
{
    public ItemType type = ItemType.Trash;
}
