using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public VelocityEstimator head;
    public VelocityEstimator leftHand;
    public VelocityEstimator rightHand;

    public float sensitivity = 0.8f;
    public float minTimeScale = 0.05f;

    private float initialFixedDeltaTime;

    void Start()
    {
        initialFixedDeltaTime = Time.fixedDeltaTime;
    }


    void Update()
    {
        float velocityMagnitude = head.GetVelocityEstimate().magnitude + leftHand.GetVelocityEstimate().magnitude + rightHand.GetVelocityEstimate().magnitude;

        Time.timeScale = Mathf.Clamp01(minTimeScale + velocityMagnitude * sensitivity);

        Time.fixedDeltaTime = initialFixedDeltaTime * Time.timeScale;
    }
}
