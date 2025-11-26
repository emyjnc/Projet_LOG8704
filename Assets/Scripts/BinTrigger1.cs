using Oculus.Haptics;
using Unity.VisualScripting;
using UnityEngine;
using Oculus.Haptics;

public class BinTrigger1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public string acceptedTag;
    public HapticSource haptics;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        TrashItem trash = other.GetComponent<TrashItem>();

        if (trash == null) return;

        if (other.CompareTag(acceptedTag))
        {
            Debug.Log("bien joué ! +1");
            
            GameManager.Instance.AddScore();
            GameManager.Instance.TrashDisposed();
            other.gameObject.SetActive(false);
          
            
            //Destroy(other.gameObject);
            return;

        }
        else
        {
            if(haptics != null)
            {
                haptics.Play();
            }
            Debug.Log("NOOO, trompé");
            GameManager.Instance.RemoveScore();
            trash.ResetItem();
            
            return;

        }

    }
}   

