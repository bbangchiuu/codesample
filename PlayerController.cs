using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    float speedMove = 3f;
    Vector2 moment;
    Rigidbody rigidbody;

    [SerializeField]
    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponentInChildren<Rigidbody>();
    }
    
    // Update is called once per frame
    void Update()
    {
        moment = InputSystem.instance.JoystickVector;
        if (moment.x != 0 || moment.y != 0)
        {
            animator.SetBool("Run", true);
            //player
            transform.Translate(new Vector3(moment.x * speedMove * Time.deltaTime, 0, moment.y * speedMove * Time.deltaTime), camera.transform);
            //transform.position = new Vector3(transform.position.x, previousCameraPlayer, transform.position.z);

            float rote = Mathf.Rad2Deg * Mathf.Atan2(moment.x, moment.y) + camera.transform.eulerAngles.y;
            transform.eulerAngles = new Vector3(0, rote, 0);
        }
        else
        {
            animator.SetBool("Run", false);
        }
        if (InputSystem.instance.IsJumpButtonPressed)
        {
            animator.SetTrigger("Jump");
            //rigidbody.AddForce(transform.up * 200f);
        }
    }
}
