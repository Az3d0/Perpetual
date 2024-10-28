using System;
using UnityEditor.PackageManager;
using UnityEditor.UIElements;
using UnityEngine;

public class LimbIKControllerRay : TerrainDetectorRay
{
    [SerializeField]
    private ControlledLimb m_controlledLimb;
    [SerializeField]
    private Transform m_footTransform;
    [SerializeField]
    private Transform m_closestTransformToGround;

    private float m_distanceFromGround;
    private AvatarIKGoal m_AvatarIKGoal;
    private string m_IKWeight;

    private void OnValidate()
    {
        switch (m_controlledLimb)
        {
            case ControlledLimb.LeftFoot:
                m_AvatarIKGoal = AvatarIKGoal.LeftFoot;
                m_IKWeight = "IKLeftFootWeight";
                break;
            case ControlledLimb.RightFoot:
                m_AvatarIKGoal = AvatarIKGoal.RightFoot;
                m_IKWeight = "IKRightFootWeight";
                break;
            case ControlledLimb.LeftHand:
                m_AvatarIKGoal = AvatarIKGoal.LeftHand;
                m_IKWeight = "IKLeftHandWeight";
                break;
            case ControlledLimb.RightHand:
                m_AvatarIKGoal = AvatarIKGoal.RightHand;
                m_IKWeight = "IKRightHandWeight";
                break;
        }
    }

    private void Awake()
    {
        if (m_closestTransformToGround)
        {
            m_distanceFromGround = m_footTransform.position.y - m_closestTransformToGround.position.y;
        }
    }
    protected override void OnHitDetected(RaycastHit hitinfo)
    {
        base.OnHitDetected(hitinfo);
    }

    public void HandleLimbIK(Animator animator)
    {
        animator.SetIKPositionWeight(m_AvatarIKGoal, animator.GetFloat(m_IKWeight));
        animator.SetIKRotationWeight(m_AvatarIKGoal, 1);

        Vector3 footPosition = Hitinfo.point;
        footPosition.y += m_distanceFromGround;

        animator.SetIKPosition(m_AvatarIKGoal, footPosition);
        animator.SetIKRotation(m_AvatarIKGoal, Quaternion.LookRotation(transform.forward, Hitinfo.normal));
        Debug.DrawRay(Hitinfo.point, Hitinfo.normal);
    }
}

[Serializable]
public enum ControlledLimb
{
    LeftFoot,
    RightFoot,
    LeftHand,
    RightHand
}
