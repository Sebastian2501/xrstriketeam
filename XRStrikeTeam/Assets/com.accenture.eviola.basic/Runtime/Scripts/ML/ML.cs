using Accenture.eviola.Math;
using System;
using UnityEngine;

namespace Accenture.eviola.ML
{
    /// <summary>
    /// A multidimensional float vector with arbitrary dimensions
    /// </summary>
    [System.Serializable]
    public class MultiDimensionalFloat {
        protected int _numDimensions = 1;
        [SerializeField]
        protected float[] _values;

        public MultiDimensionalFloat() { SetNumDimensions(1); }
        public MultiDimensionalFloat(int numDimensions) { SetNumDimensions(numDimensions); }

        /// <summary>
        /// set the dimensionality
        /// </summary>
        public void SetNumDimensions(int numDimensions) {
            if (numDimensions < 1) numDimensions = 1;
            _values = new float[numDimensions];
        }

        public float this[int index] { 
            get {
                if (!Misc.IsGoodIndex(index, _values))
                {
                    throw new IndexOutOfRangeException("Invalid index!");
                }
                else
                {
                    return _values[index];
                }
            }
            set
            {
                if (!Misc.IsGoodIndex(index, _values))
                {
                    throw new IndexOutOfRangeException("Invalid index!");
                }
                else
                {
                    _values[index] = value;
                }
            }
        }

        /// <summary>
        /// zero all values
        /// </summary>
        public void Zero() {
            for (int i = 0; i < _values.Length; i++) {
                _values[i] = 0;
            }
        }

        /// <summary>
        /// return the vector magnitude
        /// </summary>
        public float Magnitude() {
            if(_values.Length==1)return _values[0];
            float f = 0;
            for (int i = 0; i < _values.Length; i++) { 
                f += _values[i]*_values[i];
            }   
            return System.MathF.Sqrt(f);
        }

        /// <summary>
        /// return true if a and b have the same number of dimensions
        /// </summary>
        static public bool AreSameSize(MultiDimensionalFloat a, MultiDimensionalFloat b) {
            if (a == null || b == null) return false;
            return a.GetNumDimensions() == b.GetNumDimensions();
        }

        /// <summary>
        /// return true if a has the same number of dimensions as this 
        /// </summary>
        public bool IsSameSize(MultiDimensionalFloat a) {
            return AreSameSize(this, a);
        }

        /// <summary>
        /// return the number of dimensions
        /// </summary>
        public int GetNumDimensions() { return _numDimensions; }

        /// <summary>
        /// perform an arbitrary element by element action 
        /// </summary>
        static public MultiDimensionalFloat PerformElementByElementOperation(MultiDimensionalFloat a, MultiDimensionalFloat b, Func<float, float, float> theAction) {
            if (!AreSameSize(a, b)) {
                Debug.LogError("a and b need to be the same size");
                return new MultiDimensionalFloat();
            }
            if (theAction == null) {
                Debug.LogError("action cannot be null");
                return new MultiDimensionalFloat();
            }
            MultiDimensionalFloat res = new MultiDimensionalFloat(a.GetNumDimensions());
            for (int i = 0; i < res.GetNumDimensions(); i++) {
                res[i] = theAction(a[i], b[i]);
            }
            return res;
        }

        /// <summary>
        /// perform an arbitrary element by float action
        /// </summary>
        static public MultiDimensionalFloat PerformElementsByFloatOperation(MultiDimensionalFloat a, float f, Func<float, float, float> theAction) {
            if (a == null) {
                Debug.LogError("a needs to exist");
                return new MultiDimensionalFloat();
            }
            if (theAction == null)
            {
                Debug.LogError("action cannot be null");
                return new MultiDimensionalFloat();
            }
            MultiDimensionalFloat res = new MultiDimensionalFloat(a.GetNumDimensions());
            for (int i = 0; i < res.GetNumDimensions(); i++)
            {
                res[i] = theAction(a[i], f);
            }
            return res;
        }

        /// <summary>
        /// perform arbitrary element by MultiDimensionalFloat action
        /// </summary>
        static public MultiDimensionalFloat PerformPerElementOperation(MultiDimensionalFloat a, Func<float, float> theAction) {
            if (a == null)
            {
                Debug.LogError("a needs to exist");
                return new MultiDimensionalFloat();
            }
            if (theAction == null)
            {
                Debug.LogError("action cannot be null");
                return new MultiDimensionalFloat();
            }
            MultiDimensionalFloat res = new MultiDimensionalFloat(a.GetNumDimensions());
            for (int i = 0; i < res.GetNumDimensions(); i++)
            {
                res[i] = theAction(a[i]);
            }
            return res;
        }

        static public MultiDimensionalFloat Sum(MultiDimensionalFloat a, MultiDimensionalFloat b) { return PerformElementByElementOperation(a,b, (float f1, float f2) => { return f1 + f2; }); }
        static public MultiDimensionalFloat Subtract(MultiDimensionalFloat a, MultiDimensionalFloat b) { return PerformElementByElementOperation(a, b, (float f1, float f2) => { return f1 - f2; }); }
        static public MultiDimensionalFloat Multiply(MultiDimensionalFloat a, MultiDimensionalFloat b) { return PerformElementByElementOperation(a, b, (float f1, float f2) => { return f1 * f2; }); }
        static public MultiDimensionalFloat Divide(MultiDimensionalFloat a, MultiDimensionalFloat b) { return PerformElementByElementOperation(a, b, (float f1, float f2) => { return f1 / f2; }); }
        static public MultiDimensionalFloat MultiplyFloat(MultiDimensionalFloat a, float f) { return PerformElementsByFloatOperation(a,f,(float f1, float f2) => { return f1* f2; }); }

        static public MultiDimensionalFloat Abs(MultiDimensionalFloat a) { return PerformPerElementOperation(a, (float f) => { return System.MathF.Abs(f); }); }
        static public MultiDimensionalFloat Min(MultiDimensionalFloat a, MultiDimensionalFloat b) { return PerformElementByElementOperation(a,b, (float f1, float f2) => { return System.MathF.Min(f1, f2); }); }

        static public MultiDimensionalFloat operator +(MultiDimensionalFloat a, MultiDimensionalFloat b) { return Sum(a,b); }
        static public MultiDimensionalFloat operator -(MultiDimensionalFloat a, MultiDimensionalFloat b) { return Subtract(a, b); }
        static public MultiDimensionalFloat operator *(MultiDimensionalFloat a, MultiDimensionalFloat b) { return Multiply(a, b); }
        static public MultiDimensionalFloat operator *(MultiDimensionalFloat a, float b) { return MultiplyFloat(a, b); }
        static public MultiDimensionalFloat operator /(MultiDimensionalFloat a, MultiDimensionalFloat b) { return Divide(a, b); }
    }

    [System.Serializable]
    public class Float4 : MultiDimensionalFloat
    {
        public Float4() { SetNumDimensions(4); }
        public Float4(float f1, float f2, float f3, float f4) {
            SetNumDimensions(4);
            Set(f1,f2,f3,f4);
        }
        public void Set(float f1, float f2, float f3, float f4) {
            _values[0] = f1; _values[1] = f2; _values[2] = f3; _values[3] = f4;
        }
    }

    [System.Serializable]
    public class Float6 : MultiDimensionalFloat
    {
        public Float6() { SetNumDimensions(6);}
    }

    public struct MultiDimensionalFloatCalculator : ICalculator<MultiDimensionalFloat> {
        public MultiDimensionalFloat Sum(MultiDimensionalFloat a, MultiDimensionalFloat b) { return a + b; }
        public MultiDimensionalFloat Subtract(MultiDimensionalFloat a, MultiDimensionalFloat b) { return a - b; }
        public MultiDimensionalFloat Multiply(MultiDimensionalFloat a, MultiDimensionalFloat b) { return a * b; }
        public MultiDimensionalFloat Divide(MultiDimensionalFloat a, MultiDimensionalFloat b) { return a / b; }
        public MultiDimensionalFloat MultiplyFloat(MultiDimensionalFloat a, float f) { return a*f; }
    }

}