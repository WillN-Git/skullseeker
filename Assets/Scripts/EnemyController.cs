using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    ///===============================================
    /// 
    ///                 MEMBERS
    /// 
    ///===============================================

    [SerializeField]
    private float m_movementSpeed = 2.5f;

    [SerializeField]
    private int m_health = 100;

    [SerializeField]
    private Rigidbody2D m_rigidbody;

    [SerializeField]
    private float m_actionRadius;

    [SerializeField]
    private Transform m_playerTransform;

    private Vector3 m_directionToMove;

    private bool m_isChasing;

    ///===============================================
    /// 
    ///                 FUNCTIONS
    /// 
    ///===============================================
    
    void Start()
    {
        m_playerTransform = FindObjectOfType<PlayerController>().transform;
    }

    void Update()
    {
        Attack();
    }

    private void Attack()
    {
        bool bplayerNear = Vector3.Distance(transform.position, m_playerTransform.position) < m_actionRadius;
        m_isChasing = bplayerNear;
        m_directionToMove = m_isChasing ?  (m_playerTransform.position - transform.position) : Vector3.zero;

        m_directionToMove.Normalize();
        m_rigidbody.velocity = m_movementSpeed * m_directionToMove;
    }

    public void Damage(int amount)
    {
        m_health -= amount;

        if (m_health < 0) Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_actionRadius);
    }
}
