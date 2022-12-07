using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenController : MonoBehaviour
{

    public AudioClip ChickenSound;
     void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();
        if (controller != null)
        {
            controller.CollectedChicken(1);
            controller.PlaySound(ChickenSound);
            Destroy(gameObject);
        }
    }
}
