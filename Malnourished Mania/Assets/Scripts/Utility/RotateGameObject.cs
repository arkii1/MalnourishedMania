using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class RotateGameObject : MonoBehaviour
    {
        [SerializeField] float rotationSpeed = 22.5f;

        private void Update()
        {
            gameObject.transform.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime);
        }
    }
}
