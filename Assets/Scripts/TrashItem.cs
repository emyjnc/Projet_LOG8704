using Oculus.Interaction;
using UnityEngine;

public class TrashItem : MonoBehaviour
{

    private Vector3 initPosition;
    private Quaternion initRotation;
    private bool hasStartedTimer = false;
    
    private void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initPosition = transform.position;
        initRotation = transform.rotation;
        GameManager.Instance.RegisterTrash();

    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStartedTimer)
        {
            if(Vector3.Distance(transform.position,initPosition) > 1f)
            {
                hasStartedTimer = true;
                GameManager.Instance.startTimer();
            }
        }
    }

    public void ResetItem()
    {
        gameObject.SetActive(false);

        transform.position = initPosition;
        transform.rotation = initRotation;

        gameObject.SetActive(true);
    }

   
}
