using System;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.eviola.ML
{
    /// <summary>
    /// Dynamic Time Warp implementation
    /// </summary>
    public class DTW
    {
        /// <summary>
        /// Given 2 arrays of values,
        /// calculate dynamic time warp
        /// and return the score
        /// </summary>
        static public float CalcDtwScore(MultiDimensionalFloat[] a, MultiDimensionalFloat[] b) {
            if (a == null || b == null) {
                throw new ArgumentNullException("a and b need to exist");
            }
            if (a.Length == 0 || b.Length == 0) {
                throw new ArgumentOutOfRangeException("a and b need to have elements");
            }
            if (a[0].GetNumDimensions() != b[0].GetNumDimensions()) {
                throw new ArgumentOutOfRangeException("a and b need have the same number of dimensions");
            }

            float[] aM = MakeMagnitudeArray(a);
            float[] bM = MakeMagnitudeArray(b);
            float[] costMatrix = MakeCostMatrix(aM, bM);
            List<int> warpPath = MakeWarpPath(costMatrix, a.Length, b.Length);

            return CalcWarpScore(costMatrix, warpPath);
        }

        static private float[] MakeMagnitudeArray(MultiDimensionalFloat[] a) {
            float[] m = new float[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                m[i] = a[i].Magnitude();
            }
            return m;
        }

        static private float[] MakeCostMatrix(float[] a, float[] b)
        {
            float[] m = new float[a.Length * b.Length];
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < b.Length; j++)
                {
                    int curIdx = Math.Remap.CartesianToArrayIndex(j, i, b.Length);
                    float toBeAdded = 0;
                    if (i != 0 || j != 0) {
                        if (i == 0)
                        {
                            toBeAdded = m[Math.Remap.CartesianToArrayIndex(j - 1, i, b.Length)];
                        }
                        else if (j == 0)
                        {
                            toBeAdded = m[Math.Remap.CartesianToArrayIndex(j, i - 1, b.Length)];
                        }
                        else {
                            int prevRowColIdx = Math.Remap.CartesianToArrayIndex(j - 1, i - 1, b.Length);
                            int prevColIdx = Math.Remap.CartesianToArrayIndex(j - 1, i, b.Length);
                            int prevRowIdx = Math.Remap.CartesianToArrayIndex(j, i - 1, b.Length);
                            toBeAdded = Mathf.Min(m[prevRowColIdx], Mathf.Min(m[prevColIdx], m[prevRowIdx]));
                        }
                    }
                    m[curIdx] = Mathf.Abs(a[i] - b[j]) + toBeAdded;
                }
            }
            return m;
        }

        private static List<int> MakeWarpPath(float[] costMatrix, int w, int h) {
            List<int> p = new List<int>();
            int x = w - 1;
            int y = h-1;
            int i = Math.Remap.CartesianToArrayIndex(x, y, w);
            p.Add(i);
            while (x > 0) {
                if (y == 0)
                {
                    x--;
                    i = Math.Remap.CartesianToArrayIndex(x, y, w);
                    p.Add(i);
                }
                else {
                    //0 X
                    //1 2
                    int[] idx = new int[3];
                    float[] vals = new float[3];
                    idx[0] = Math.Remap.CartesianToArrayIndex(x - 1, y, w);
                    idx[1] = Math.Remap.CartesianToArrayIndex(x - 1, y - 1, w);
                    idx[2] = Math.Remap.CartesianToArrayIndex(x, y - 1, w);
                    vals[0] = costMatrix[idx[0]];
                    vals[1] = costMatrix[idx[1]];
                    vals[2] = costMatrix[idx[2]];
                    float minVal = Mathf.Min(vals[0], Mathf.Min(vals[1], vals[2]));
                    for (int j = 0; j < 3; j++) {
                        if (minVal == vals[j]) {
                            i = idx[j];
                            j = 5;
                        }
                    }
                    Math.Remap.ArrayIndexToCartesian(i, w, ref x, ref y);
                    p.Add(i);
                }
            }
            return p;
        }

        private static float CalcWarpScore(float[] costMatrix, List<int> warpPath) {
            float f = 0;
            for (int i = 0; i < warpPath.Count;i++) { 
                f += costMatrix[warpPath[i]];
            }
            f /= (float)warpPath.Count;
            return f;
        }
    }
}