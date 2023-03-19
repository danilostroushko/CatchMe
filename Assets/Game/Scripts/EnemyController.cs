using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Collider enemyCollider;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private GameObject enemyMesh;

    private float speed;
    private Transform target;
    private bool isDeath = false;

    private void OnEnable()
    {
        target = null;
        navMeshAgent.enabled = true;
        enemyCollider.enabled = true;
        enemyMesh.SetActive(true);
        particle.gameObject.SetActive(false);
        isDeath = false;
    }

    public void Init(Transform target, float speed)
    {
        this.target = target;
        this.speed = speed;

        navMeshAgent.speed = speed;
    }

    public void StopEnemy()
    {
        navMeshAgent.enabled = false;
    }

    private void Update()
    {
        if (target != null && !isDeath)
        {
            navMeshAgent.SetDestination(target.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isDeath = true;
            navMeshAgent.enabled = false;
            enemyCollider.enabled = false;
            enemyMesh.SetActive(false);
            particle.gameObject.SetActive(true);
            particle.Play();
        }
    }

}