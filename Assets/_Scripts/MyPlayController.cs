using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyPlayController : MonoBehaviour
{
    
    //外部可调试参数
    public Animator PlayerAnimator;
    [Header("站立平台层级")]
    public LayerMask m_raycastMask;
    [Header("胶囊体碰撞盒")]
    public CapsuleCollider CCollider;

    public float MoveSpeedMultiplier;//一般移动速度
    public float skin;
    public float StepOffset;
    public float SlopeLimit;//坡度限制
    public float RunSpeed;//跑步速度

    

    //供外部查看的内部参数
    [SerializeField]
    [Tooltip("跳跃状态 (段)")]
    private int jumpTimes;
    [SerializeField]
    private Vector3 velocity;

    


    //内部处理参数
    [SerializeField]
    private Vector3 m_inputDir;
    private Vector3 groundNormal;
    private float PlayerHeight;
    private float PlayerRadius;
    private RaycastHit raycastHit;
    private RaycastHit raycastHitWall;



    void Start()
    {
        //外部可调参数初始化
        MoveSpeedMultiplier = 50.0f;
        StepOffset = 0.31f;
        skin = 0.01f;
        RunSpeed = 2.5f;

        
        //函数内参数初始化

        PlayerHeight = CCollider.height;
        PlayerRadius = CCollider.radius;
    }

    
    private void FixedUpdate()
    {
        
        //输入控制
            m_inputDir.x = Input.GetAxis("Horizontal");
            m_inputDir.z = Input.GetAxis("Vertical");
            m_inputDir.y = 0;

        //一般情况下本地方向转化为世界方向，攀爬模式下不转换
        m_inputDir = transform.TransformDirection(m_inputDir);
        
        //跑步：条件不能在下蹲模式或攀爬模式下跑步
        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_inputDir = m_inputDir * RunSpeed;
        }
        //速度初步设定：根据一定规则来设定速度，分为四种情况：攀爬模式，正常移动，斜面滑行和自由落体

        velocity.x = m_inputDir.x * MoveSpeedMultiplier * Time.fixedDeltaTime;
        velocity.z = m_inputDir.z * MoveSpeedMultiplier * Time.fixedDeltaTime;
        velocity = Vector3.ProjectOnPlane(velocity, groundNormal);
        //velocity = transform.TransformDirection(velocity);
        

        //运动检测
        if (Physics.Raycast(transform.position, velocity.normalized, out raycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius, m_raycastMask) && raycastHitWall.normal != groundNormal)
        {
            
            Debug.DrawRay(transform.position, velocity, Color.green);
            float normalside = Vector3.Dot(velocity, raycastHitWall.normal);
            velocity -= raycastHitWall.normal * normalside;
            Debug.Log("中心检测到墙面");

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
        //人物头部检测墙面
        if ((Physics.Raycast(new Vector3(transform.position.x, transform.position.y + PlayerHeight / 2, transform.position.z), velocity.normalized, out InnerRaycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius, m_raycastMask) && InnerRaycastHitWall.normal != groundNormal))
        {
            
            float normalside = Vector3.Dot(velocity, InnerRaycastHitWall.normal);
            velocity -= InnerRaycastHitWall.normal * normalside;

            //解决角色在锐角区域的穿墙bug
            if (89.9f > Vector3.Angle(groundNormal, InnerRaycastHitWall.normal) || Vector3.Angle(groundNormal, InnerRaycastHitWall.normal) > 90.1f)
            {
                Vector3 Dir = InnerRaycastHitWall.point - transform.position;
                Dir = Vector3.ProjectOnPlane(Dir, groundNormal).normalized;
                float normalside2 = Vector3.Dot(velocity, Dir);
                velocity -= Dir * normalside2;
                Debug.LogWarning("解决角色在锐角区域的穿墙bug");
            }

            Debug.Log("头部检测到墙面");
            MoreTimesDetectWallAndDeleVel();
        }


        //人物中心检测墙面
        else if (Physics.Raycast(transform.position, velocity.normalized, out InnerRaycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius + skin, m_raycastMask) && InnerRaycastHitWall.normal != groundNormal)
        {

            Debug.DrawRay(transform.position, velocity, Color.green);
            float normalside = Vector3.Dot(velocity, InnerRaycastHitWall.normal);
            velocity -= InnerRaycastHitWall.normal * normalside;
            Debug.Log("中心检测到墙面");
            MoreTimesDetectWallAndDeleVel();
        }

        //人物脚部检测墙面
        else if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - PlayerHeight / 2 + StepOffset, transform.position.z), velocity.normalized, out InnerRaycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius, m_raycastMask) && InnerRaycastHitWall.normal != groundNormal)
        {
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - PlayerHeight / 2 + StepOffset, transform.position.z), velocity, Color.green);
            float normalside = Vector3.Dot(velocity, InnerRaycastHitWall.normal);
            velocity -= InnerRaycastHitWall.normal * normalside;
            Debug.Log("脚步检测到墙面");
            MoreTimesDetectWallAndDeleVel();
        }

        //人物左右两侧也检测墙面，防止利用边缘卡墙穿模
        else if (Physics.Raycast(transform.position + Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity.normalized, out InnerRaycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius + skin, m_raycastMask) && InnerRaycastHitWall.normal != groundNormal)
        {
            Debug.DrawRay(transform.position + Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity, Color.green);
            float normalside = Vector3.Dot(velocity, InnerRaycastHitWall.normal);
            velocity -= InnerRaycastHitWall.normal * normalside;
            Debug.Log("左侧检测到墙面");
            MoreTimesDetectWallAndDeleVel();

        }
        else if (Physics.Raycast(transform.position - Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity.normalized, out InnerRaycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius + skin, m_raycastMask) && InnerRaycastHitWall.normal != groundNormal)
        {
            Debug.DrawRay(transform.position - Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity, Color.green);
            float normalside = Vector3.Dot(velocity, InnerRaycastHitWall.normal);
            velocity -= InnerRaycastHitWall.normal * normalside;
            Debug.Log("右侧检测到墙面");
            MoreTimesDetectWallAndDeleVel();

        }
        //人物头部的左右两侧也检测墙面，防止利用边缘卡墙穿模
        else if (Physics.Raycast(transform.position + new Vector3(0, (PlayerHeight / 2), 0)+ Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity.normalized, out InnerRaycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius + skin, m_raycastMask) && InnerRaycastHitWall.normal != groundNormal)
        {
            Debug.DrawRay(transform.position + Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity, Color.green);
            float normalside = Vector3.Dot(velocity, InnerRaycastHitWall.normal);
            velocity -= InnerRaycastHitWall.normal * normalside;
            Debug.Log("左上侧检测到墙面");


            //解决角色在锐角区域的穿墙bug
            if (89.9f > Vector3.Angle(groundNormal, InnerRaycastHitWall.normal) || Vector3.Angle(groundNormal, InnerRaycastHitWall.normal) > 90.1f)
            {
                Vector3 Dir = InnerRaycastHitWall.point - transform.position;
                Dir = Vector3.ProjectOnPlane(Dir, groundNormal).normalized;
                float normalside2 = Vector3.Dot(velocity, Dir);
                velocity -= Dir * normalside2;
                Debug.LogWarning("解决角色在锐角区域的穿墙bug");
            }


        }
        else if (Physics.Raycast(transform.position+new Vector3(0,(PlayerHeight/2),0) - Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity.normalized, out InnerRaycastHitWall, velocity.magnitude * Time.fixedDeltaTime + PlayerRadius + skin, m_raycastMask) && InnerRaycastHitWall.normal != groundNormal)
        {
            Debug.DrawRay(transform.position - Vector3.Cross(velocity.normalized, Vector3.up).normalized * PlayerRadius, velocity, Color.green);
            float normalside = Vector3.Dot(velocity, InnerRaycastHitWall.normal);
            velocity -= InnerRaycastHitWall.normal * normalside;
            Debug.Log("右上侧检测到墙面");


            //解决角色在锐角区域的穿墙bug
            if (89.9f > Vector3.Angle(groundNormal, InnerRaycastHitWall.normal) || Vector3.Angle(groundNormal, InnerRaycastHitWall.normal) > 90.1f)
            {
                Vector3 Dir = InnerRaycastHitWall.point - transform.position;
                Dir = Vector3.ProjectOnPlane(Dir, groundNormal).normalized;
                float normalside2 = Vector3.Dot(velocity, Dir);
                velocity -= Dir * normalside2;
                Debug.LogWarning("解决角色在锐角区域的穿墙bug");
            }

        }


    }




}
