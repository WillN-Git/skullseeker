using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotController : MonoBehaviour
{
    ///===============================================
    /// 
    ///                 MEMBERS
    /// 
    ///===============================================

    [SerializeField]
    private float m_bulletSpeed = 5f;

    [SerializeField]
    private int m_damageAmount = 10;

    private Rigidbody2D _rigidbody;


    ///===============================================
    /// 
    ///                 FUNCTIONS
    /// 
    ///===============================================

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();        
    }

    void Update()
    {
        Vector3 ballDirection = transform.localScale.x * transform.right;
        transform.position += (m_bulletSpeed * Time.deltaTime) * ballDirection;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            collision.GetComponent<EnemyController>().Damage(m_damageAmount);

        // Destroy(gameObject);
    }
}
