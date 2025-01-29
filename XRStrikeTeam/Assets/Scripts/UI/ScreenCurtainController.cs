using Accenture.eviola;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Accenture.XRStrikeTeam.Presentation
{
    public class ScreenCurtainController : MonoBehaviour
    {
        [Header("External Components")]
        [SerializeField]
        private Animator _animator = null;

        private readonly string _nameIsOpaqueParam = "IsOpaque";
        private int _idIsOpaQueParam = -1;
        private bool _bOpaque = false;

        #region Control

        public bool IsOpaque() { return _bOpaque; }

        public void SetOpaque(bool b) {
            if (b == IsOpaque()) return;
            _bOpaque = b;
            _animator.SetBool(_idIsOpaQueParam, _bOpaque);
        }

        public void ToggleOpaque() { SetOpaque(!IsOpaque()); }

        #endregion

        #region Monobehaviour
        private void Awake()
        {
            Misc.CheckNotNull(_animator);

            _idIsOpaQueParam = Animator.StringToHash(_nameIsOpaqueParam);
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ScreenCurtainController))]
    public class ScreenCurtainControllerEditor : Editor{
        private ScreenCurtainController _target { get { return (ScreenCurtainController)target; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorUI.Button("Toggle", () => { _target.ToggleOpaque(); });
        }
    }
#endif
}