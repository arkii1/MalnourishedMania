using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/Variables/Bool Variable")]
    public class BoolVariable : ScriptableObject
    {
        public bool value;

        public bool setFalseOnEnable = false;

        private void OnEnable()
        {
            if (setFalseOnEnable)
                value = false;
        }

        public void Set(bool v)
        {
            value = v;
        }

        public void Set(BoolVariable v)
        {
            value = v.value;
        }

        public void Reverse()
        {
            value = !value;
        }

        public bool Compare(bool v)
        {
            return v == value;
        }

        public bool Compare(BoolVariable v)
        {
            return value == v.value;
        }
    }
}

