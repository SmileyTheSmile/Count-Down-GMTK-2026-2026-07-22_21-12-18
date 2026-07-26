using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    [SerializeField] BoxCollider2D _collider;
    [SerializeField] private float reloadInterval = 1.0f;
    private float timer = 0.0f;
    private bool isReloading = false;

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("Trigger Entered");
    //    _collider.enabled = false;
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<Player>().CanHook = true;
    }

    //public void Reload()
    //{
    //    _collider.enabled = false;
    //    isReloading = true;
    //    timer = 0f;
    //}

    //void Update()
    //{
    //    if (!isReloading) return;

    //    Debug.Log("Collider disabled, waiting to reload...");

    //    timer += Time.deltaTime;

    //    if (timer >= reloadInterval)
    //    {
    //        _collider.enabled = true;
    //        timer = 0f;
    //    }
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("Trigger Entered");
    //    _collider.enabled = false;
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    Debug.Log("Trigger Exited");
    //    _collider.enabled = true;
    //}
}
