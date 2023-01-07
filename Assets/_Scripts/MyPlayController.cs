using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyPlayController : MonoBehaviour
{
    
    //�ⲿ�ɵ��Բ���
    public Animator PlayerAnimator;
    [Header("վ��ƽ̨�㼶")]
    public LayerMask m_raycastMask;
    [Header("��������ײ��")]
    public CapsuleCollider CCollider;

    public float MoveSpeedMultiplier;//һ���ƶ��ٶ�
    public float skin;
    public float StepOffset;
    public float SlopeLimit;//�¶�����
    public float RunSpeed;//�ܲ��ٶ�

    

    //���ⲿ�鿴���ڲ�����
    [SerializeField]
    [Tooltip("��Ծ״̬ (��)")]
    private int jumpTimes;
    [SerializeField]
    private Vector3 velocity;

    


    //�ڲ��������
    [SerializeField]
    private Vector3 m_inputDir;
    private Vector3 groundNormal;
    private float PlayerHeight;
    private float PlayerRadius;
    private RaycastHit raycastHit;
    private RaycastHit raycastHitWall;



    void Start()
    {
        //�ⲿ�ɵ�������ʼ��
        MoveSpeedMultiplier = 50.0f;
        StepOffset = 0.31f;
        skin = 0.01f;
        RunSpeed = 2.5f;

        
        //�����ڲ�����ʼ��

        PlayerHeight = CCollider.height;
        PlayerRadius = CCollider.radius;
    }

    
    private void FixedUpdate()
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
        if (Physics.Raycast(transform.position, velocity.normalized, out raycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius, m_raycastMask) && raycastHitWall.normal != groundNormal)
        {
            
            Debug.DrawRay(transform.position, velocity, Color.green);
            float normalside = Vector3.Dot(velocity, raycastHitWall.normal);
            velocity -= raycastHitWall.normal * normalside;
            Debug.Log("���ļ�⵽ǽ��");

        }


        
        


        Debug.DrawRay(transform.position, groundNormal, Color.red);
        Debug.DrawRay(transform.position, velocity, Color.black);

        //Velocity Apply
        gameObject.transform.position += velocity * Time.fixedDeltaTime;
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
    void MoreTimesDetectWallAndDeleVel()
    {
        RaycastHit InnerRaycastHitWall;
        //����ͷ�����ǽ��
        if ((Physics.Raycast(new Vector3(transform.position.x, transform.position.y + PlayerHeight / 2, transform.position.z), velocity.normalized, out InnerRaycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius, m_raycastMask) && InnerRaycastHitWall.normal != groundNormal))
        {
            
            float normalside = Vector3.Dot(velocity, InnerRaycastHitWall.normal);
            velocity -= InnerRaycastHitWall.normal * normalside;

            //�����ɫ���������Ĵ�ǽbug
            if (89.9f > Vector3.Angle(groundNormal, InnerRaycastHitWall.normal) || Vector3.Angle(groundNormal, InnerRaycastHitWall.normal) > 90.1f)
            {
                Vector3 Dir = InnerRaycastHitWall.point - transform.position;
                Dir = Vector3.ProjectOnPlane(Dir, groundNormal).normalized;
                float normalside2 = Vector3.Dot(velocity, Dir);
                velocity -= Dir * normalside2;
                Debug.LogWarning("�����ɫ���������Ĵ�ǽbug");
            }

            Debug.Log("ͷ����⵽ǽ��");
            MoreTimesDetectWallAndDeleVel();
        }


        //�������ļ��ǽ��
        else if (Physics.Raycast(transform.position, velocity.normalized, out InnerRaycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius + skin, m_raycastMask) && InnerRaycastHitWall.normal != groundNormal)
        {

            Debug.DrawRay(transform.position, velocity, Color.green);
            float normalside = Vector3.Dot(velocity, InnerRaycastHitWall.normal);
            velocity -= InnerRaycastHitWall.normal * normalside;
            Debug.Log("���ļ�⵽ǽ��");
            MoreTimesDetectWallAndDeleVel();
        }

        //����Ų����ǽ��
        else if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - PlayerHeight / 2 + StepOffset, transform.position.z), velocity.normalized, out InnerRaycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius, m_raycastMask) && InnerRaycastHitWall.normal != groundNormal)
        {
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - PlayerHeight / 2 + StepOffset, transform.position.z), velocity, Color.green);
            float normalside = Vector3.Dot(velocity, InnerRaycastHitWall.normal);
            velocity -= InnerRaycastHitWall.normal * normalside;
            Debug.Log("�Ų���⵽ǽ��");
            MoreTimesDetectWallAndDeleVel();
        }

        //������������Ҳ���ǽ�棬��ֹ���ñ�Ե��ǽ��ģ
        else if (Physics.Raycast(transform.position + Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity.normalized, out InnerRaycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius + skin, m_raycastMask) && InnerRaycastHitWall.normal != groundNormal)
        {
            Debug.DrawRay(transform.position + Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity, Color.green);
            float normalside = Vector3.Dot(velocity, InnerRaycastHitWall.normal);
            velocity -= InnerRaycastHitWall.normal * normalside;
            Debug.Log("����⵽ǽ��");
            MoreTimesDetectWallAndDeleVel();

        }
        else if (Physics.Raycast(transform.position - Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity.normalized, out InnerRaycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius + skin, m_raycastMask) && InnerRaycastHitWall.normal != groundNormal)
        {
            Debug.DrawRay(transform.position - Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity, Color.green);
            float normalside = Vector3.Dot(velocity, InnerRaycastHitWall.normal);
            velocity -= InnerRaycastHitWall.normal * normalside;
            Debug.Log("�Ҳ��⵽ǽ��");
            MoreTimesDetectWallAndDeleVel();

        }
        //����ͷ������������Ҳ���ǽ�棬��ֹ���ñ�Ե��ǽ��ģ
        else if (Physics.Raycast(transform.position + new Vector3(0, (PlayerHeight / 2), 0)+ Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity.normalized, out InnerRaycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius + skin, m_raycastMask) && InnerRaycastHitWall.normal != groundNormal)
        {
            Debug.DrawRay(transform.position + Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity, Color.green);
            float normalside = Vector3.Dot(velocity, InnerRaycastHitWall.normal);
            velocity -= InnerRaycastHitWall.normal * normalside;
            Debug.Log("���ϲ��⵽ǽ��");


            //�����ɫ���������Ĵ�ǽbug
            if (89.9f > Vector3.Angle(groundNormal, InnerRaycastHitWall.normal) || Vector3.Angle(groundNormal, InnerRaycastHitWall.normal) > 90.1f)
            {
                Vector3 Dir = InnerRaycastHitWall.point - transform.position;
                Dir = Vector3.ProjectOnPlane(Dir, groundNormal).normalized;
                float normalside2 = Vector3.Dot(velocity, Dir);
                velocity -= Dir * normalside2;
                Debug.LogWarning("�����ɫ���������Ĵ�ǽbug");
            }


        }
        else if (Physics.Raycast(transform.position+new Vector3(0,(PlayerHeight/2),0) - Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity.normalized, out InnerRaycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius + skin, m_raycastMask) && InnerRaycastHitWall.normal != groundNormal)
        {
            Debug.DrawRay(transform.position - Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity, Color.green);
            float normalside = Vector3.Dot(velocity, InnerRaycastHitWall.normal);
            velocity -= InnerRaycastHitWall.normal * normalside;
            Debug.Log("���ϲ��⵽ǽ��");


            //�����ɫ���������Ĵ�ǽbug
            if (89.9f > Vector3.Angle(groundNormal, InnerRaycastHitWall.normal) || Vector3.Angle(groundNormal, InnerRaycastHitWall.normal) > 90.1f)
            {
                Vector3 Dir = InnerRaycastHitWall.point - transform.position;
                Dir = Vector3.ProjectOnPlane(Dir, groundNormal).normalized;
                float normalside2 = Vector3.Dot(velocity, Dir);
                velocity -= Dir * normalside2;
                Debug.LogWarning("�����ɫ���������Ĵ�ǽbug");
            }

        }


    }




}
