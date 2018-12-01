using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Boss.Utilities;

public class BossController : MonoBehaviour
{
    private Animator anim;

    // Character Constants
    public float const_AnimSpeedMultiplier = 1f;
    public float const_MovingTurnSpeed = 360;
    public float const_StationaryTurnSpeed = 180;
    public float const_AttackEvery = 2.5f;
    public int const_HeavyAttackEvery = 5;

    // Animation Parametrs
    private float anim_MovementForward;
    private float anim_MovementRotate;
    private bool anim_FinishedMoving = false;
    private int anim_LightAttack = 0;
    private int anim_HeavyAttack = 0;
    private bool isBusy = false;


    // Ground Parameters
    private Vector3 ground_Normal;
    private float ground_CheckDistance = 0.1f;

    enum LightAttack { LightAttack1 = 1, LightAttack2 = 2, LightAttack3 = 3 };
    enum HeavyAttack { FireAttack = 1, JumpAttack = 2, PunchAttack = 3 };

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(PerformAttack());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Move(Vector3 move, Vector3 rotation, bool reachedTarget)
    {
        if (move.magnitude > 1f)
            move.Normalize();
        anim_FinishedMoving = reachedTarget;

        if (!isBusy)
        {
            move = transform.InverseTransformDirection(move);
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, ground_Normal);

            anim_MovementRotate = Mathf.Atan2(move.x, move.z);
            anim_MovementForward = move.z;

            ApplyExtraTurnRotation(rotation);

        }

        // send input and other state parameters to the animator
        UpdateAnimator(move);
    }

    public IEnumerator PerformAttack()
    {
        while (true)
        {
            for (int i = 1; i <= const_HeavyAttackEvery; i++)
            {
                yield return new WaitForSeconds(const_AttackEvery);

                yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
                if (i == const_HeavyAttackEvery)
                    anim_HeavyAttack = (int)Utilities.GetRandomEnum<HeavyAttack>();
                else
                    anim_LightAttack = (int)Utilities.GetRandomEnum<LightAttack>();
                isBusy = true;

                yield return new WaitForSeconds(1);
                anim_LightAttack = 0;
                anim_HeavyAttack = 0;

                yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
                isBusy = false;
            }
        }
    }

    void UpdateAnimator(Vector3 move)
    {
        // update the animator parameters
        anim.SetBool("CloseEnough", anim_FinishedMoving);
        anim.SetInteger("LightAttack", anim_LightAttack);
        anim.SetInteger("HeavyAttack", anim_HeavyAttack);
        anim.SetFloat("HasteValue", anim_MovementForward, 0.1f, Time.deltaTime);
        anim.SetFloat("RotValue", anim_MovementRotate, 0.1f, Time.deltaTime);

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (move.magnitude > 0)
        {
            anim.speed = const_AnimSpeedMultiplier;
        }
    }

    // private Vector3 prevOrientation;
    // private Quaternion prevOrientation;
    void ApplyExtraTurnRotation(Vector3 rotation)
    {
        // Vector3 currentOrientation = rotation;
        // // Quaternion currentOrientation = move;
        // bool isIdle = anim.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        // if (isIdle && prevOrientation != currentOrientation)
        // {
        //     // Vector3 oldAngles = prevOrientation.eulerAngles;
        //     // Vector3 newAngles = currentOrientation.eulerAngles;

        //     Vector3 oldAngles = prevOrientation;
        //     Vector3 newAngles = currentOrientation;

        //     if(oldAngles.z > newAngles.z)
        //     {
        //         anim_MovementRotate = 1f;
        //     }
        //     else if(oldAngles.z < newAngles.z)
        //     {
        //         anim_MovementRotate = -1f;
        //     }
        // }
        // prevOrientation = currentOrientation;

        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(const_StationaryTurnSpeed, const_MovingTurnSpeed, anim_MovementRotate);
        transform.Rotate(0, anim_MovementRotate * turnSpeed * Time.deltaTime, 0);

    }

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * ground_CheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, ground_CheckDistance))
        {
            ground_Normal = hitInfo.normal;
            anim.applyRootMotion = true;
        }
        // else
        // {
        //     anim_Grounded = false;
        //     ground_Normal = Vector3.up;
        //     anim.applyRootMotion = false;
        // }
    }

    public void RotateHead(Vector3 move)
    {
        Transform head = transform.Find("Head");
        float headRotation = 1f;
        head.Translate(new Vector3(headRotation, 0, 0));

        // head.Rotate(new Vector3(headRotation, 0, 0));
    }

    public bool getIsBusy()
    {
        return isBusy;
    }
}
