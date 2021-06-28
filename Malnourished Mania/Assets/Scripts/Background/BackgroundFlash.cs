using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class BackgroundFlash : MonoBehaviour
    {
        public void Flash()
        {
            GetComponent<Animator>().SetTrigger("flash");
        }
    }
}

