using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action OnDeath;

    public bool isDeath {  get; private set; }
    public float speed = 10f;

    [Header("Camera settings")]
    public float minZoom = 6f;
    public float maxZoom = 10f;
    public float smoothZoom = 0f;
    public Vector3 offset = Vector3.zero;
    public float smooth = 0f;
    private Vector3 targetPosition = Vector3.zero;
    private Vector3 velPosition = Vector3.zero;
    private float velZoom = 0f;

    private CharacterController characterController;
    

    [HideInInspector]
    public Joystick joystick;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Camera.main.orthographicSize = minZoom;
    }

    private void Update()
    {
        if (isDeath) { return; }
        if (joystick != null)
        {
            Vector3 direction = Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal;
            direction = Quaternion.Euler(0, -45, 0) * direction;
            
            characterController.SimpleMove(direction * speed);
        }
    }

    private void LateUpdate()
    {
        if (gameObject.activeInHierarchy)
        {
            float vel = characterController.velocity.sqrMagnitude / 100f;
            float zoom = PercentToRange(vel, minZoom, maxZoom);
            Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, zoom, ref velZoom, smoothZoom);

            targetPosition = gameObject.transform.position + offset;
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, targetPosition, ref velPosition, smooth);
        }
    }

    private float PercentToRange(float value, float min, float max)
    {
        return (value * (max - min)) + min;
    }

    public void Init(Joystick joystick, float speed)
    {
        isDeath = false;
        this.speed = speed;
        this.joystick = joystick;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bomb") || other.CompareTag("Enemy"))
        {
            isDeath = true;
            OnDeath?.Invoke();
        }
    }
}