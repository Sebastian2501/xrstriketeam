
namespace Accenture.eviola.ML
{
    public class TimeSeries
    {
        //TODO: consider bool mask?
        public static float Distance<T>(float[] a, float[] b) {
            if (a == null || b == null) throw new System.ArgumentNullException("Both time series need to exist");
            if (a.Length != b.Length) throw new System.ArgumentException("time series should have the same lenght");
            float sqRes = 0.0f;
            for (int i = 0; i < a.Length; i++) {
                float d = a[i] - b[i];
                float sqCur = d * d;
                sqRes += sqCur;
            }
            return (float)System.Math.Sqrt(sqRes);
        }
        
    }
}