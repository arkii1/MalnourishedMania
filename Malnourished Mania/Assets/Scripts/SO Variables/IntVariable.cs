using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/Variables/Int Variable")]
    public class IntVariable : NumberVariable
    {
        public int value;

        public void Set(int v)
        {
            value = v;
        }

        public void Set(NumberVariable v)
        {
            if (v is FloatVariable)
            {
                FloatVariable f = (FloatVariable)v;
                value = Mathf.RoundToInt(f.value);
            }

            if (v is IntVariable)
            {
                IntVariable i = (IntVariable)v;
                value = i.value;
            }
        }

        public void Add(int v)
        {
            value += v;
        }

        public void Add(NumberVariable v)
        {
            if (v is FloatVariable)
            {
                FloatVariable f = (FloatVariable)v;
                value += Mathf.RoundToInt(f.value);
            }

            if (v is IntVariable)
            {
                IntVariable i = (IntVariable)v;
                value += i.value;
            }
        }

        public void Minus(int v)
        {
            value -= v;
        }

        public void Minus(NumberVariable v)
        {
            if (v is FloatVariable)
            {
                FloatVariable f = (FloatVariable)v;
                value -= Mathf.RoundToInt(f.value);
            }

            if (v is IntVariable)
            {
                IntVariable i = (IntVariable)v;
                value -= i.value;
            }
        }

        public void Multiply(int v)
        {
            value *= v;
        }

        public void Multiply(NumberVariable v)
        {
            if (v is FloatVariable)
            {
                FloatVariable f = (FloatVariable)v;
                value *= Mathf.RoundToInt(f.value);
            }

            if (v is IntVariable)
            {
                IntVariable i = (IntVariable)v;
                value *= i.value;
            }
        }

        public void Divide(int v)
        {
            value /= v;
        }

        public void Divide(NumberVariable v)
        {
            if (v is FloatVariable)
            {
                FloatVariable f = (FloatVariable)v;
                value = Mathf.RoundToInt(value / f.value);
            }

            if (v is IntVariable)
            {
                IntVariable i = (IntVariable)v;
                value /= i.value;
            }
        }
    }
}
