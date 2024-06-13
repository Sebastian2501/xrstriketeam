using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Accenture.eviola.ML
{
    [System.Serializable]
    public class GenericTimeSeriesSO<T> : ScriptableObject
    {
        public List<T> Values;

        virtual public void SetValues(List<T> values){ 
            Values = values;
        }

#if UNITY_EDITOR
#endif
    }
}