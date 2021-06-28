using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class AnimateBackground : MonoBehaviour
    {
        [SerializeField]
        float scrollSpeed = 0.5f;

        Material mat;

        void Start()
        {
            mat = GetComponent<MeshRenderer>().material;
        }

        void Update()
        {
            mat.mainTextureOffset += Vector2.down * scrollSpeed * Time.deltaTime;
        }
    }
}

