using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public VelocityEstimator head;
    public VelocityEstimator leftHand;
    public VelocityEstimator rightHand;

    public float sensitivity = 0.8f;
    public float minSensitivity = 0.05f;

    private float fixedDeltaTime;

    // Start is called before the first frame update
    void Start()
    {
        fixedDeltaTime = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        float vel = head.GetVelocityEstimate().magnitude + leftHand.GetVelocityEstimate().magnitude + rightHand.GetVelocityEstimate().magnitude; 
        

        Time.timeScale = Mathf.Clamp01(vel * sensitivity + minSensitivity);

        fixedDeltaTime = fixedDeltaTime * Time.timeScale;

    }
}
