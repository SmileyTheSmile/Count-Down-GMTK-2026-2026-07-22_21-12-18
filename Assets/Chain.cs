using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    [SerializeField] BoxCollider2D _collider;
    [SerializeField] private float reloadInterval = 1.0f;

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<Player>().CanHook = true;
    }

}
