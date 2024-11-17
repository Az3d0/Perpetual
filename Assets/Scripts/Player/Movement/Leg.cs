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

    private float m_previousHipAngle = 0;
    private float m_previousKneeAngle = 0;


    private Vector3 m_hipLocalStartingPostition;
    private Vector3 m_kneeLocalStartingPostition;
    private Vector3 m_footLocalStartingPosition;

    private Vector3 m_targetFootRelativePosition;
    private Vector3 m_targetKneeRelativePosition;

    public Vector3 TargetFootLocalPosition => m_targetFootRelativePosition;
    public float ExtensionIntensity;

    public Leg(GameObject hip, UnityEngine.InputSystem.InputAction action)
    {
        m_hip = hip;
        m_knee = hip.transform.GetChild(0).gameObject;
        m_foot = m_knee.transform.GetChild(0).gameObject;
        m_action = action;

        m_hipLocalStartingPostition = Vector3.zero;
        m_kneeLocalStartingPostition = m_knee.transform.position - m_hipLocalStartingPostition;
        m_footLocalStartingPosition = m_foot.transform.position - m_hip.transform.position;

        m_targetKneeRelativePosition = m_kneeLocalStartingPostition;
        m_targetFootRelativePosition = m_footLocalStartingPosition;

        m_thighLength = Vector3.Distance(m_hip.transform.position, m_knee.transform.position);
        m_shinLength = Vector3.Distance(m_knee.transform.position, m_foot.transform.position);

        m_action.performed += RaiseLeg;
    }

    public void RaiseLeg(InputAction.CallbackContext context)
    {
        ExtensionIntensity = 1 - context.ReadValue<float>();
        Vector3 newFootPosition = m_targetFootRelativePosition;
        newFootPosition.y = m_footLocalStartingPosition.y + m_shinLength * ExtensionIntensity;
        m_targetFootRelativePosition = newFootPosition;

        InverseKinematics();
    }

    public void InverseKinematics()
    {
        float hipToFootDistance = Vector3.Distance(m_hipLocalStartingPostition, m_targetFootRelativePosition);
        float c = hipToFootDistance;
        float b = m_shinLength;
        float a = m_thighLength;


        float hipAngle = LawOfCosine(a, c, b) * Mathf.Rad2Deg;
        float deltaHipAngle = hipAngle - m_previousHipAngle;
        m_previousHipAngle = hipAngle;

        float kneeAngle = LawOfCosine(a, b, c) * Mathf.Rad2Deg - 180;
        float deltaKneeAngle = kneeAngle - m_previousKneeAngle;
        m_previousKneeAngle = kneeAngle;

        //Vector3 newKneePosition = m_targetKneeRelativePosition;
        //newKneePosition.z = m_thighLength * Mathf.Sin(Mathf.Abs(hipAngle));
        //newKneePosition.y = -m_thighLength * Mathf.Cos(Mathf.Abs(hipAngle));
        //m_targetKneeRelativePosition = newKneePosition;

        m_hip.transform.Rotate(Vector3.right, deltaHipAngle);
        //Vector3 newHipRotation = m_hip.transform.localEulerAngles;
        //newHipRotation.x = hipAngle;
        //m_hip.transform.localEulerAngles = newHipRotation;
        m_knee.transform.Rotate(Vector3.right, deltaKneeAngle);
        //Vector3 newKneeRotation = m_knee.transform.localEulerAngles;

        //newKneeRotation.x = kneeAngle;
        //m_knee.transform.Rotate(Vector3.up, kneeAngle);

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
        m_action.performed -= RaiseLeg;
    }
}
