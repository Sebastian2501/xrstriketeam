using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Accenture.eviola.ML
{
    [CreateAssetMenu(fileName = "Float4TimeSeries", menuName ="Accenture/eviola/ML/TimeSeries/CreateFloat4TimeSeries")]
    [System.Serializable]
    public class Float4TimeSeriesSO : GenericTimeSeriesSO<Float4>
    {
#if UNITY_EDITOR
        public static void CreateAsset(string assetPth, List<Float4> values)
        {
            Float4TimeSeriesSO asset = ScriptableObject.CreateInstance<Float4TimeSeriesSO>();
            asset.SetValues(values);
            AssetDatabase.CreateAsset(asset, assetPth);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
#endif
    }

#if UNITY_EDITOR
//    [CustomEditor(typeof(Floa4TimeSeriesSO))]
 //   public class Float4TimeSeriesSOEditor : Editor {

//    }
#endif
}