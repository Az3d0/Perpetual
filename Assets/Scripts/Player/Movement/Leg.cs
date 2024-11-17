using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Leg
{
    private UnityEngine.InputSystem.InputAction m_action;

    private GameObject m_hip;
    private GameObject m_knee;
    private GameObject m_foot;


    private float m_thighLength;
    private float m_shinLength; //max foot height

    private float m_hipAngle = 0;
    private float m_kneeAngle = 0;

    private float m_intensityPercentage;
    private float m_footGroundedTreshHold;

    private Vector3 m_hipLocalStartingPostition;
    private Vector3 m_kneeLocalStartingPostition;
    private Vector3 m_footLocalStartingPosition;

    private Vector3 m_targetFootRelativePosition;
    private Vector3 m_targetKneeRelativePosition;

    public Vector3 TargetKneeLocalPosition => m_targetKneeRelativePosition;
    public Vector3 TargetFootLocalPosition => m_targetFootRelativePosition;

    public Leg(GameObject hip, UnityEngine.InputSystem.InputAction action, float legGroundedTreshHold)
    {
        m_hip = hip;
        m_knee = hip.transform.GetChild(0).gameObject;
        m_foot = m_knee.transform.GetChild(0).gameObject;
        m_action = action;
        m_footGroundedTreshHold = legGroundedTreshHold;

        m_hipLocalStartingPostition = Vector3.zero;
        m_kneeLocalStartingPostition = m_knee.transform.position - m_hipLocalStartingPostition;
        m_footLocalStartingPosition = m_foot.transform.position - m_hip.transform.position;

        m_targetKneeRelativePosition = m_kneeLocalStartingPostition;
        m_targetFootRelativePosition = m_footLocalStartingPosition;

        m_thighLength = Vector3.Distance(m_hip.transform.position, m_knee.transform.position);
        m_shinLength = Vector3.Distance(m_knee.transform.position, m_foot.transform.position);

        m_action.performed += OnAction;
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        RaiseFoot(1 - context.ReadValue<float>());
    }

    /// <summary>
    /// Raise foot between minimum and maximum limits. intensityPercentage must be a value between 0-1.
    /// </summary>
    /// <param name="intensityPercentage"></param>
    public void RaiseFoot(float intensityPercentage)
    {
        if (IsGrounded(intensityPercentage)) return;

        if (intensityPercentage < 0f || intensityPercentage > 1f)
        {
            intensityPercentage = Mathf.Clamp01(intensityPercentage);
            Debug.Log("intensityPercentage value not between 0-1. Clamped to limit");
        }
        m_intensityPercentage = intensityPercentage - m_footGroundedTreshHold;
        Vector3 newFootPosition = m_targetFootRelativePosition;
        newFootPosition.y = m_footLocalStartingPosition.y + m_shinLength * m_intensityPercentage;
        m_targetFootRelativePosition = newFootPosition;

        LegIK();
    }

    public void MoveFoot()
    {

    }
    public void LegIK()
    {
        float hipToFootDistance = Vector3.Distance(m_hipLocalStartingPostition, m_targetFootRelativePosition);
        float c = hipToFootDistance;
        float b = m_shinLength;
        float a = m_thighLength;


        float newHipAngle = LawOfCosine(a, c, b) * Mathf.Rad2Deg;
        float deltaHipAngle = newHipAngle - m_hipAngle;
        m_hipAngle = newHipAngle;

        float newKneeAngle = LawOfCosine(a, b, c) * Mathf.Rad2Deg - 180;
        float deltaKneeAngle = newKneeAngle - m_kneeAngle;
        m_kneeAngle = newKneeAngle;

        m_hip.transform.Rotate(Vector3.right, deltaHipAngle);

        m_knee.transform.Rotate(Vector3.right, deltaKneeAngle);
    }

    public bool IsGrounded(float intensityPercentage)
    {
        if (intensityPercentage <= m_footGroundedTreshHold) return true;
        else return false;
    }
    /// <summary>
    /// returns rad angle in front of the final parameter "c"
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    private float LawOfCosine(float a, float b, float c)
    {
        return Mathf.Acos((SquareNum(a) + SquareNum(b) - SquareNum(c)) / (2 * a * b));
    }
    private float SquareNum(float num)
    {
        return Mathf.Pow(num, 2);
    }
    public void UnsubscribeFromInputAction()
    {
        m_action.performed -= OnAction;
    }
}
