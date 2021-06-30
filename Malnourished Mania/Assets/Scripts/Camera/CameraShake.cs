using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class CameraShake : MonoBehaviour
    {

    #region variables
    public bool camShakeAcive = true; 
    [Range(0, 1)]
    [SerializeField] float trauma;
    [SerializeField] float traumaMult = 16;
    [SerializeField] float traumaMag = 0.8f; 
    [SerializeField] float traumaRotMag = 17f; 
    [SerializeField] float traumaDepthMag = 0.6f; 
    [SerializeField] float traumaDecay = 1.3f;

        float timeCounter = 0;
    #endregion

    #region accessors
    public float Trauma //accessor is used to keep trauma within 0 to 1 range
    {
        get
        {
            return trauma;
        }
        set
        {
            trauma = Mathf.Clamp01(value);
        }
    }
    #endregion

    #region methods
    float GetFloat(float seed)
    {
        return (Mathf.PerlinNoise(seed, timeCounter) - 0.5f) * 2f;
    }

    Vector3 GetVec3()
    {
        return new Vector3(
            GetFloat(1),
            GetFloat(10),
            GetFloat(100) * traumaDepthMag
            );
    }

    private void Update()
    {
        if (Trauma > 0)
        {
            timeCounter += Time.deltaTime * Mathf.Pow(Trauma, 0.3f) * traumaMult;
            Vector3 newPos = GetVec3() * traumaMag * Trauma;
            GetComponent<CameraFollow>().CamOffset = newPos;
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (newPos.z * traumaRotMag)));
            Trauma -= Time.deltaTime * traumaDecay * (Trauma + 0.3f);
        }
        else 
        {
       
            transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }
    #endregion
}
}

