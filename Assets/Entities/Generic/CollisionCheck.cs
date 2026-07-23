using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    public bool IsColliding { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IsColliding = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        IsColliding = false;
    }
}
