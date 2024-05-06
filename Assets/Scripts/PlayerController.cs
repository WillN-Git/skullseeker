using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    ///===============================================
    /// 
    ///                 MEMBERS
    /// 
    ///===============================================

    [SerializeField]
    private float m_movementSpeed = 2.5f;

    [SerializeField]
    private Rigidbody2D m_rigidbody;

    [SerializeField]
    private Transform m_weaponArm;

    [SerializeField]
    private GameObject m_defaultBullet;

    private Camera m_mainCamera;
    private Animator m_playerAnimator;
    public Vector2 m_shotDirection;

    // Player Input

    private bool bPlayerLeftClick { get { return Input.GetMouseButtonDown(0); } }
    private bool bPlayerRightClick { get { return Input.GetMouseButtonDown(1); } }
    private Vector2 PlayerMovementDirection { get { return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized; } }


    ///===============================================
    /// 
    ///                 FUNCTIONS
    /// 
    ///===============================================



    void Start()
    {
        m_mainCamera = Camera.main;
        m_playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        UpdatePlayerMovement();
    }

    private void UpdatePlayerMovement()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 playerPosition_worldSpace = transform.localPosition;

        Vector3 playerPosition_screenSpace = m_mainCamera.WorldToScreenPoint(playerPosition_worldSpace);

        m_shotDirection =  mousePosition - playerPosition_screenSpace;

        float faceDirection = Mathf.Abs(m_shotDirection.x) / m_shotDirection.x; // faceDirection € {-1, 1}
        float theta = Mathf.Atan2(m_shotDirection.y, Mathf.Abs(m_shotDirection.x)) * Mathf.Rad2Deg * faceDirection; // theta € [-pi/2, pi/2]

        m_weaponArm.rotation = Quaternion.Euler(0, 0, theta);
        m_rigidbody.velocity = m_movementSpeed * PlayerMovementDirection;
        transform.localScale = new Vector3(faceDirection, 1f, 1f);

        // Animate player movement
        m_playerAnimator.SetBool("IsWalking", PlayerMovementDirection != Vector2.zero);

        if (bPlayerLeftClick) _ShotOneBullet(m_weaponArm.position, m_weaponArm.rotation);
    }

    private void _ShotOneBullet(Vector2 firePosition, Quaternion fireRotation)
    {
        var bulletObject = Instantiate(m_defaultBullet, firePosition, fireRotation);
        bulletObject.transform.localScale = transform.localScale;
    }
}
