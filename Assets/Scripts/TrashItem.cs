using UnityEngine;

public class TrashItem : MonoBehaviour
{

    private Vector3 initPosition;
    private Quaternion initRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initPosition = transform.position;
        initRotation = transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetItem()
    {
        gameObject.SetActive(false);

        transform.position = initPosition;
        transform.rotation = initRotation;

        gameObject.SetActive(true);
    }
}
