using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class RotateSpikedBall : MonoBehaviour
    {
        public float speed = 10;
        float elapsed = 0;
        float rotAmount;
        private void Update()
        {
            rotAmount = EaseSin(elapsed * speed) * Time.deltaTime;

            transform.Rotate(transform.forward * speed);
            elapsed += Time.deltaTime;
        }

        float EaseSin(float x)
        {
            float ret = Mathf.Sin(x);
            return Mathf.Abs(ret);
        }
    }
}

