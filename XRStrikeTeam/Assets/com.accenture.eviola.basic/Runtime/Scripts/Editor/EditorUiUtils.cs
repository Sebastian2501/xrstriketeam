using System;
using UnityEditor;
using UnityEngine;

namespace Accenture.eviola
{
    public class EditorUI {
        /// <summary>
        /// a header label, just like [Header] in MonoBehavious
        /// </summary>
        public static void Header(string s) {
            EditorGUILayout.LabelField(s, EditorStyles.boldLabel);
        }

        /// <summary>
        /// a toggle firing an action when value changes
        /// </summary>
        public static void Toggle(string label, bool curValue, Action<bool> doOnChange) {
            bool newVal = GUILayout.Toggle(curValue, label);
            if (newVal!=curValue) {
                if(doOnChange!=null) doOnChange(newVal);
            }
        }

        public static void ChangingField<T>(string label, T curValue, Action<T> doOnChange, Func<string, T, T> widgetFunc) {
            if (widgetFunc == null) return;
            T newVal = widgetFunc(label, curValue);
            if (!newVal.Equals(curValue)) { 
                if(doOnChange!=null)doOnChange(newVal);
            }
        }

        /// <summary>
        /// a slider firing an action when value changes
        /// </summary>
        public static void Slider(string label, float curValue, float min, float max, Action<float> doOnChange) {
            float newVal = EditorGUILayout.Slider(label, curValue, min, max);
            if (newVal != curValue) { 
                if(doOnChange != null) doOnChange(newVal);
            }
        }

        public static void IntField(string label, int curValue, Action<int> doOnChange) {
            ChangingField(label, curValue, doOnChange, (string s, int i) => { return EditorGUILayout.IntField(s, i); });
        }

        public static void StringField(string label, string curValue, Action<string> doOnChange) {
            ChangingField(label, curValue, doOnChange, (string s, string v) => { return EditorGUILayout.TextField(s,v); });
        }

        public static void FloatField(string label, float curValue, Action<float> doOnChange) {
            ChangingField(label, curValue, doOnChange, (string s, float f) => { return EditorGUILayout.FloatField(s, f); });
        }

        public static void Vector3Field(string label, Vector3 curValue, Action<Vector3> doOnChange) {
            ChangingField(label, curValue, doOnChange, (string s, Vector3 v) => { return EditorGUILayout.Vector3Field(s, v); });
        }

        public static void ObjectField<T>(string label, T curValue, Action<T> doOnChange) where T: UnityEngine.Object 
        {
            ChangingField(label, curValue, doOnChange, (string s, T v) => {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(s, EditorStyles.label);
                T o =  EditorGUILayout.ObjectField(v, typeof(T), true) as T; 
                EditorGUILayout.EndHorizontal();
                return o;
            });
        }

        /// <summary>
        /// a button firing an action when value changes
        /// </summary>
        public static void Button(string label, Action doOnClick) {
            if (GUILayout.Button(label)) {
                if (doOnClick != null) doOnClick();
            }
        }

        /// <summary>
        /// a dropdown menu firing an action when value changes
        /// </summary>
        public static void DropDown(string label, int curValue, string[] options, Action<int> doOnChange) {
            EditorGUILayout.LabelField(label);
            int newVal = EditorGUILayout.Popup(curValue, options);
            if (newVal != curValue) {
                if (doOnChange != null) doOnChange(newVal);
            }
        }

        public static void DropDownEnum<T>(string label, int curValue, Action<int> doOnChange) {
            string[] ss = Enum.GetNames(typeof(T));
            DropDown(label, curValue, ss, doOnChange);
        }

        public static void DropDownWebcamDevices(string label, int curValue, Action<int> doOnChange) {
            if (WebCamTexture.devices.Length < 1) {
                EditorGUILayout.LabelField(label + ": no devices found");
                return;
            }
            string[] ss = VideoInput.Webcam.GetAvailableCameraDeviceNames().ToArray();
            DropDown(label, curValue, ss, doOnChange);
        }

        [System.Serializable]
        public class Foldout {
            [SerializeField]
            private bool _bOpen = false;

            public string Label = "";

            public bool Render() {
                _bOpen = EditorGUILayout.Foldout(_bOpen, Label);
                return _bOpen;
            }

            public bool Render(string label) {
                Label = label;
                return Render();
            }
        }
    }
}