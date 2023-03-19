using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Bomb : MonoBehaviour
{
    public event Action<Bomb> OnCollisionEvent; 

    [SerializeField] private Collider bombCollider;
    [SerializeField] private GameObject bombMesh;
    [SerializeField] private ParticleSystem particle;

    private void OnEnable()
    {
        bombCollider.enabled = true;
        bombMesh.SetActive(true);
        particle.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            bombCollider.enabled = false;
            bombMesh.SetActive(false);
            particle.gameObject.SetActive(true);
            particle.Play();
            StartCoroutine(OnPlayParticles());
        }
    }

    private IEnumerator OnPlayParticles()
    {
        yield return new WaitUntil(() => particle.isStopped);
        OnCollisionEvent?.Invoke(this);
    }
}