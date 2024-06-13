using System;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.eviola
{
    public class Misc
    {
        /// <summary>
        /// Return true if component is not null; it breaks editor exectution if it's null.
        /// The idea here is to try and compensate for MonoBehaviour not having a proper constructor where to do inject needed dependencies: the editor break will force you to check what you missed
        /// </summary>
        static public bool CheckNotNull<T>(T component)where T: class
        {
            if (component == null) {
                Debug.LogError("Needed component is NULL");
#if UNITY_EDITOR
                Debug.Break();
#endif
                return false;
            }
            return true;
        }

        /// <summary>
        /// fire an action if c is not null; returns c!=null
        /// </summary>
        static public bool DoIfNotNull<T>(T c, Action<T> a) where T : Behaviour
        {
            if (c == null)
            {
                Debug.LogError("Null component");
                return false;
            }
            a(c);
            return true;
        }

        /// <summary>
        /// allows to specify different beahviours for edit and play mode
        /// </summary>
        static public void DoDiffererntActionForEditorAndPlaymode(Action doInEditMode, Action doinPlayMode) {
            if (Application.isPlaying)
            {
                if (doinPlayMode != null) doinPlayMode();
            }
            else { 
                if(doInEditMode != null) doInEditMode();
            }
        }


        /// <summary>
        /// return true if idx is a good index for a
        /// </summary>
        static public bool IsGoodIndex<T>(int idx, List<T> a)
        {
            return idx>=0 && idx<a.Count;
        }

        /// <summary>
        /// return true if idx is a good index for a
        /// </summary>
        static public bool IsGoodIndex<T>(int idx, T[] a) { 
            return idx>= 0 && idx<a.Length;
        }

        static public bool ActivateGameObjectInList(List<GameObject> gos, int idx) {
            if (gos == null) return false;
            if (!IsGoodIndex(idx, gos)) return false;
            for (int i = 0; i < gos.Count; i++)
            {
                gos[i].SetActive(idx==i);
            }
            return true;
        }
    }
}