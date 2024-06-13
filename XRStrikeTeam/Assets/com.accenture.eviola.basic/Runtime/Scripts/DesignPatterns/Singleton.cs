using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Accenture.eviola.DesignPatterns
{

    public class Singleton<T> : MonoBehaviour 
                               where T : MonoBehaviour
    {
        private static T _instance = null;

        public static T Instance { get { return _instance; } }

        public static bool IsInstanceReady() { return _instance != null; }

        /// <summary>
        /// first comes, first served
        /// call this fella from awake
        /// </summary>
        public void InitInstance() {
            if (Instance == null) {
                _instance = this as T;
                DontDestroyOnLoad(Instance);
            }
        }

        private void Awake()
        {
            InitInstance();
        }
    }
}