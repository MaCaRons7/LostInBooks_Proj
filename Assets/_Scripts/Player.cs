using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    
    //�ⲿ�ɵ��Բ���
    public Animator PlayerAnimator;
    [Header("����ϰ��㼶")]
    public LayerMask m_raycastWallMask;
    [Header("��������ײ��")]
    public CapsuleCollider CCollider;

    public float MoveSpeedMultiplier=50f;//һ���ƶ��ٶ�
    public float skin=0.05f;
    public float StepOffset=0.31f;
    public float SlopeLimit=60f;//�¶�����
    public float RunSpeed=2.5f;//�ܲ��ٶ�

    
    //���װ��״̬
    public enum ArmState
    {
        Null,
        Melee
    };

    //��ҿ���״̬
    public enum PlayerStatement
    {
        Waiting,
        Moving,
        Push
    };



    //�ڲ�����
    [SerializeField]
    private Vector3 m_inputDir;
    Transform playerTransform;
    private Vector3 groundNormal;
    private float PlayerHeight;
    private float PlayerRadius;
    private RaycastHit raycastHit;
    private RaycastHit raycastHitWall;
    private float meleeAttackDamage;
    private float meleeAttackCD;
    private ArmState weaponState;
    [SerializeField]
    private PlayerStatement playerStatement;
    private PlayerSensor playerSensor;
    private Vector3 playerMovementWorldSpace = Vector3.zero;
    
    //���ⲿ�鿴���ڲ�����
    [SerializeField]
    private Vector3 velocity;

    [SerializeField] private bool playerFreeze;
    
    #region �������
    
    bool pushStateChanged;
    Transform interactPoint;
    Transform rightHandTarget;
    Transform leftHandTarget;
    MovableObject movableObject;
    [SerializeField]
    bool isPushPressed;
    private bool isLoadPressed;
    private bool isInLoading;
    
    #endregion

    [SerializeField]
    private ShellType currentShellType = ShellType.NoneType;

    void Start()
    {
        //�ⲿ�ɵ�������ʼ��
        // MoveSpeedMultiplier = 50.0f;
        // StepOffset = 0.31f;
        // skin = 0.01f;
        // RunSpeed = 2.5f;

        
        //�����ڲ�����ʼ��
        playerTransform = transform;
        PlayerHeight = CCollider.height;
        PlayerRadius = CCollider.radius;
        playerSensor = GetComponent<PlayerSensor>();
    }
    

    private void Update()
    {
        isPushPressed = Input.GetKey(KeyCode.F) ? true:false;
        Push();

        isLoadPressed = Input.GetKey(KeyCode.R) ? true : false;
        SetLoadTurret();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeShellFromCaisson();
        }
        
        
        SwitchPlayerStates();
        if(!playerFreeze)
        MoveSystem();
    }
    /// <summary>
    /// �����л���ҵĸ���״̬
    /// </summary>
    void SwitchPlayerStates()
    {
        switch (playerStatement)
        {
            //վ����̬����(��)��ת��
            case PlayerStatement.Waiting:

                if(pushStateChanged)
                {
                    playerStatement = PlayerStatement.Push;
                }
                else if(velocity.sqrMagnitude>0.01f)
                {
                    playerStatement = PlayerStatement.Moving;
                }
                break;
            
            case PlayerStatement.Moving:

                if(velocity.sqrMagnitude<=1f)
                {
                    playerStatement = PlayerStatement.Waiting;
                }
                else if (pushStateChanged)
                {
                    playerStatement = PlayerStatement.Push;
                }
                break;

            case PlayerStatement.Push:
                if(pushStateChanged)
                {
                    playerStatement = PlayerStatement.Waiting;
                }
                break;
        }
        
        pushStateChanged = false;



        // if (isAimPressed)
        // {
        //     armState = ArmState.Aim;
        // }
        // else
        // {
        //     armState = ArmState.Normal;
        // }
    }
    private void MoveSystem()
    {
        //�������
        m_inputDir.x = Input.GetAxis("Horizontal");
        m_inputDir.z = Input.GetAxis("Vertical");
        m_inputDir.y = 0;
        //һ������±��ط���ת��Ϊ���緽������ģʽ�²�ת��
        m_inputDir = transform.TransformDirection(m_inputDir);
        //�ܲ��������������¶�ģʽ������ģʽ���ܲ�
        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_inputDir = m_inputDir * RunSpeed;
        }
        //�ٶȳ����趨������һ���������趨�ٶȣ���Ϊ�������������ģʽ�������ƶ���б�滬�к���������
        velocity.x = m_inputDir.x * MoveSpeedMultiplier * Time.fixedDeltaTime;
        velocity.z = m_inputDir.z * MoveSpeedMultiplier * Time.fixedDeltaTime;
        velocity = Vector3.ProjectOnPlane(velocity, groundNormal);
        //velocity = transform.TransformDirection(velocity);
        

        //�˶����
        if (Physics.Raycast(transform.position, velocity.normalized, out raycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius, m_raycastWallMask) && raycastHitWall.normal != groundNormal)
        {
            
            Debug.DrawRay(transform.position, velocity, Color.green);
            float normalside = Vector3.Dot(velocity, raycastHitWall.normal);
            velocity -= raycastHitWall.normal * normalside;
            Debug.Log("���ļ�⵽ǽ��");

        }

        Debug.DrawRay(transform.position, groundNormal, Color.red);
        Debug.DrawRay(transform.position, velocity, Color.black);

        //Velocity Apply
        gameObject.transform.position += velocity * Time.deltaTime;
        AnimationUpdate(m_inputDir);

    }
    void AnimationUpdate(Vector3 InputDir)
    {
        PlayerAnimator.SetFloat("Vel_Vertical",InputDir.z);
        PlayerAnimator.SetFloat("Vel_Horizontal",InputDir.x);
        if (InputDir.x > 0.01f)
        {
            PlayerAnimator.SetBool("Idle", false);
        }
        else if(InputDir.x < -0.01f)
        {
            PlayerAnimator.SetBool("Idle", false);
        }
        else
        {
            PlayerAnimator.SetBool("Idle", true);
        }

        if (InputDir.z > 0.01f)
        {

            PlayerAnimator.SetBool("Idle", false);
        }
        else if (InputDir.z < -0.01f)
        {

            PlayerAnimator.SetBool("Idle", false);
        }



    }

    void TakeShellFromCaisson()
    {
        Caisson caisson = playerSensor.InteractObjectCheck<Caisson>(transform.position,playerTransform, playerMovementWorldSpace);
        if (caisson!=null&&caisson.TakeOut())
        {
            currentShellType = caisson.caissonShellType;
            // Debug.Log("��ȡ�ɹ�");
        }
    }
    void Push()
    {
        pushStateChanged = false;
        if(isPushPressed && playerStatement != PlayerStatement.Push)
        {
            movableObject = playerSensor.MovableObjectCheck(playerTransform, playerMovementWorldSpace);
            if(movableObject)
            {
                InteractPoint interact = movableObject.GetInteractPoint(playerTransform);
                interactPoint = interact.interactPoint;
                rightHandTarget = interact.rightHandTarget;
                leftHandTarget = interact.leftHandTarget;
                
                pushStateChanged = true;
            }
        }
        else if(!isPushPressed && playerStatement == PlayerStatement.Push)
        {
            movableObject = null;
            pushStateChanged = true;
        }

        
        if(movableObject!=null&&playerStatement == PlayerStatement.Push)
        {
            movableObject.transform.Translate(velocity*Time.deltaTime,transform);
        }
        
        
        
    }

    void SetLoadTurret()
    {
        if (isLoadPressed&&!isInLoading&&currentShellType!=ShellType.NoneType)
        {
            StartCoroutine(LoadTurret());
        }
    }

    IEnumerator LoadTurret()
    {
        isInLoading = true;
        playerFreeze = true;
        movableObject = playerSensor.MovableObjectCheck(playerTransform, playerMovementWorldSpace);
        if (movableObject != null)
        {
            currentShellType = ShellType.NoneType;
            yield return new WaitForSeconds(movableObject.GetComponent<BaseTurret>().loadingTime);
        }

        isInLoading = false;
        playerFreeze = false;

        movableObject.GetComponent<BaseTurret>().SetFire();
    }
    

}
