using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Transition : MonoBehaviour
    {
        private void Start()
        {
            transform.position = FindObjectOfType<PlayerManager>().transform.position;
            transform.position += new Vector3(0, 0, -5);
        }

        public void TransitionOut()
        {
            transform.position = FindObjectOfType<PlayerManager>().transform.position;
            transform.position += new Vector3(0, 0, -5);
            GetComponent<Animator>().Play("Transition Out");
        }

        public void CompleteLevel()
        {
            FindObjectOfType<GameUIMaster>().ActivePostLevelScreen();
        }

        public void TriggerStartWaypoint()
        {
            if (FindObjectOfType<StartWaypoint>() != null)
            {
                FindObjectOfType<StartWaypoint>().Trigger();
            }
        }
    }
}

