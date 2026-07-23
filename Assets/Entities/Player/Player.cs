
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private CollisionCheck checkDown;
    [SerializeField] private CollisionCheck checkUp;
    [SerializeField] private CollisionCheck checkLeft;
    [SerializeField] private CollisionCheck checkRight;

    [SerializeField] private Rigidbody2D rigidbody;

    [SerializeField] private Vector2 gravity = new Vector2(0.00f, -9.81f);
    [SerializeField] private Vector2 gravityDirection = new Vector2(0.00f, -1f);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            gravity = new Vector2(0.00f, 9.81f);
            rigidbody.velocity = new Vector2(0f, rigidbody.velocity.y);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            gravity = new Vector2(0.00f, -9.81f);
            rigidbody.velocity = new Vector2(0f, rigidbody.velocity.y);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            gravity = new Vector2(-9.81f, 0.00f);
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0f);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            gravity = new Vector2(9.81f, 0.00f);
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0f);
        }

        rigidbody.velocity += gravity * Time.deltaTime;
    }
}