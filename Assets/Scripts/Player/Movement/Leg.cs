using System;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class Leg
{
    private UnityEngine.InputSystem.InputAction m_lifAction;

    private CustomMovement m_customMovement;
    private GameObject m_hip;
    private GameObject m_knee;
    private GameObject m_foot;


    private float m_thighLength;
    private float m_shinLength; //max foot height

    private float m_hipAngle = 0;
    private float m_kneeAngle = 0;

    private float m_inputIntensityPercentage;
    private float m_liftIntensityPercentage;

    private float m_liftIntensityMinClampValue;

    private Vector3 m_contactPosition;

    //private Vector3 m_hipLocalStartingPostition;
    //private Vector3 m_kneeLocalStartingPostition;
    //private Vector3 m_footLocalStartingPosition;

    private Vector3 m_targetFootPosition;
    private Vector3 m_targetKneePosition;
    private Vector3 m_targetHipPosition;

    private Vector3 m_defaultFootPosition;

    public Leg(GameObject hip, UnityEngine.InputSystem.InputAction action, CustomMovement customMovement)
    {
        m_hip = hip;
        m_knee = hip.transform.GetChild(0).gameObject;
        m_foot = m_knee.transform.GetChild(0).gameObject;
        m_lifAction = action;
        m_customMovement = customMovement;

        m_thighLength = Vector3.Distance(m_hip.transform.position, m_knee.transform.position);
        m_shinLength = Vector3.Distance(m_knee.transform.position, m_foot.transform.position);

        UpdateLegValues();
        m_targetFootPosition = m_defaultFootPosition;


        //m_hipLocalStartingPostition = m_hip.transform.position;
        //m_kneeLocalStartingPostition = m_knee.transform.position - m_hipLocalStartingPostition;
        //m_footLocalStartingPosition = m_foot.transform.position - m_hip.transform.position;

        //m_targetKneePosition = m_kneeLocalStartingPostition;
        //m_targetFootPosition = m_footLocalStartingPosition;



        m_lifAction.performed += OnLiftAction;
    }

    public void OnLiftAction(InputAction.CallbackContext context)
    {
        m_inputIntensityPercentage = 1 - context.ReadValue<float>();
        RaiseFoot(m_inputIntensityPercentage);
    }

    /// <summary>
    /// Raise foot between minimum and maximum limits. intensityPercentage must be a value between 0-1.
    /// </summary>
    /// <param name="intensityPercentage"></param>
    public void RaiseFoot(float intensityPercentage)
    {
        if (IsInputFullyHeld()) return;

        intensityPercentage = Mathf.Clamp01(intensityPercentage);

        m_liftIntensityPercentage = intensityPercentage - m_customMovement.TriggerDeadZone;

        //ensures leg doesn't go underground
        m_liftIntensityPercentage = Mathf.Clamp(intensityPercentage, m_liftIntensityMinClampValue, 1f);

        m_targetFootPosition.y = m_defaultFootPosition.y + m_shinLength * m_liftIntensityPercentage;
        LegIK();
    }

    public void MoveFoot()
    {
        if (IsInputFullyHeld()) return;
        if (IsFootGrounded()) return;

        //m_targetFootPosition.x = m_targetFootPosition.x + m_customMovement.FootMovementRange * m_customMovement.MovementDirection.x;
        m_targetFootPosition.z = m_defaultFootPosition.z + m_customMovement.FootMovementRange * m_customMovement.MovementDirection.z;
        LegIK();
    }

    public void UpdateLegValues()
    {
        m_targetHipPosition = m_hip.transform.position;
        m_defaultFootPosition = new Vector3(m_hip.transform.position.x, m_hip.transform.position.y - m_thighLength - m_shinLength, m_hip.transform.position.z);

        GroundDetectorRay();
    }
    public void LegIK()
    {
        float hipToFootDistance = Vector3.Distance(m_targetHipPosition, m_targetFootPosition);
        float c = hipToFootDistance;
        float b = m_shinLength;
        float a = m_thighLength;
        float offset = m_targetFootPosition.z - m_targetHipPosition.z;

        float gamma = 0;
        if (offset > 0){ gamma = Mathf.Asin(offset / c);}
        if(offset < 0) { gamma = -Mathf.Asin( -offset / c);}

        float gammaInDeg = gamma * Mathf.Rad2Deg;

        float newHipAngle = LawOfCosine(a, c, b) * Mathf.Rad2Deg + gammaInDeg;
        if (float.IsNaN(newHipAngle)) return;
        float deltaHipAngle = newHipAngle - m_hipAngle;
        m_hipAngle = newHipAngle;

        float newKneeAngle = LawOfCosine(a, b, c) * Mathf.Rad2Deg - 180;
        if (float.IsNaN(newKneeAngle)) return;
        float deltaKneeAngle = newKneeAngle - m_kneeAngle;
        m_kneeAngle = newKneeAngle;

        Debug.Log(m_kneeAngle);
        
        m_hip.transform.Rotate(Vector3.right, deltaHipAngle);

        m_knee.transform.Rotate(Vector3.right, deltaKneeAngle);
    }

    public void GroundFoot()
    {
        m_targetFootPosition = m_contactPosition;
        m_liftIntensityMinClampValue = m_liftIntensityPercentage;
        LegIK();
    }
    public void GroundDetectorRay()
    {
        Ray groundDetectorRay = new Ray(m_targetFootPosition, Vector3.down);
        Debug.DrawRay(m_targetFootPosition, Vector3.down, Color.red);
        // needs to ignore objects you can't step on
        if (Physics.Raycast(groundDetectorRay, out RaycastHit hitinfo, 5f))
        {
            m_contactPosition = new Vector3(hitinfo.point.x, hitinfo.point.y + m_customMovement.FootGroundedTreshhold, hitinfo.point.z);
        }
    }
    public void GizmoDebugLeg()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(m_targetFootPosition, 0.1f);
        Gizmos.DrawSphere(m_targetHipPosition, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(m_defaultFootPosition, 0.05f);

        if (IsFootGrounded()) { Gizmos.color = Color.red;}
        else { Gizmos.color = Color.green;}
        Gizmos.DrawSphere(m_contactPosition, 0.05f);

    }
    public bool IsInputFullyHeld()
    {
        return m_inputIntensityPercentage <= m_customMovement.TriggerDeadZone;
    }
    public bool IsFootGrounded()
    {
        return m_targetFootPosition.y <= m_contactPosition.y;
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
        m_lifAction.performed -= OnLiftAction;
    }
}
