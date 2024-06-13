using System;
using UnityEngine;

namespace Accenture.eviola.Math
{
    
    /// this is Eric Gunnerson idea to do generic math
    
    public interface ICalculator<T> {
        T Sum(T a, T b);
        T Subtract(T a, T b);
        T Multiply(T a, T b);
        T Divide(T a, T b);
        T MultiplyFloat(T a, float b);
    }

    public struct FloatCalculator : ICalculator<float> { 
        public float Sum(float a, float b) { return a+b; }
        public float Subtract(float a, float b) { return a-b; }
        public float Multiply(float a, float b) { return a*b; }
        public float Divide(float a, float b) { return a/b; }
        public float MultiplyFloat(float a, float b) { return Multiply(a, b); }
    }

    public struct Vector3Calculator : ICalculator<Vector3> {
        public Vector3 Sum(Vector3 a, Vector3 b) { return a + b; }
        public Vector3 Subtract(Vector3 a, Vector3 b) { return a - b; }
        public Vector3 Multiply(Vector3 a, Vector3 b) { return new Vector3(a.x*b.x, a.y*b.y, a.z*b.z); }
        public Vector3 Divide(Vector3 a, Vector3 b) { return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z); }
        public Vector3 MultiplyFloat(Vector3 a, float b) { return new Vector3(a.x * b, a.y * b, a.z * b); }
    }

    public struct Number<T, C> where C : ICalculator<T>,new() {
        private T value;
        private static C calculator = new C();
        public  Number(T val) { value = val; }

        public static implicit operator Number<T, C>(T a) { return new Number<T, C>(a); }
        public static implicit operator T(Number<T, C> a) { return a.value; }
        public static Number<T, C> operator +(Number<T, C> a, Number<T, C> b) { return calculator.Sum(a, b); }
        public static Number<T, C> operator -(Number<T, C> a, Number<T, C> b) { return calculator.Subtract(a, b); }
        public static Number<T, C> operator *(Number<T, C> a, Number<T, C> b) { return calculator.Multiply(a, b); }
        public static Number<T, C> operator /(Number<T, C> a, Number<T, C> b) { return calculator.Divide(a, b); }
        public static Number<T, C> operator *(Number<T, C> a, float b) { return calculator.MultiplyFloat(a, b); }
        public static Number<T, C> operator /(Number<T, C> a, float b) { return calculator.MultiplyFloat(a, 1/b); }
    }



    public class Remap
    {
        static public float Map(float val, float inputMin, float inputMax, float outputMin, float outputMax, bool clamp = true)
        {
            float outVal = ((val - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin);
            if (clamp)
            {
                if (outputMax < outputMin)
                {
                    if (outVal < outputMax)
                        outVal = outputMax;
                    else if (outVal > outputMin)
                        outVal = outputMin;
                }
                else
                {
                    if (outVal > outputMax)
                        outVal = outputMax;
                    else if (outVal < outputMin)
                        outVal = outputMin;
                }
            }
            return outVal;
        }

        static public Vector3 Map(Vector3 val, Vector3 inputMin, Vector3 inputMax, Vector3 outputMin, Vector3 outputMax, bool clamp = true)
        {
            Vector3 outVal = new Vector3();
            for (int i = 0; i < 3; i++)
            {
                outVal[i] = Map(val[i], inputMin[i], inputMax[i], outputMin[i], outputMax[i], clamp);
            }
            return outVal;
        }

        static public int CartesianToArrayIndex(int x, int y, int w) {
            return (y * w + x);
        }

        static public void ArrayIndexToCartesian(int i, int w, ref int x, ref int y) {
            y = i / w;
            x = i % w;
        }
    }

    [System.Serializable]
    public class BoundingBox
    {
        public Vector3 Center = new Vector3();
        public Vector3 Size = new Vector3(1, 1, 1);

        private float GetSomethingForAxis(int axis, Func<float> func)
        {
            if (axis < 0 || axis > 2) throw new IndexOutOfRangeException("Invalid Vector3 index!");
            return func();
        }

        private float GetOffsetFromCenterForAxis(int axis, float offset)
        {
            return GetSomethingForAxis(axis, () => {
                return Center[axis] + offset;
            });
        }

        public float GetMin(int axis)
        {
            return GetOffsetFromCenterForAxis(axis, -Size[axis] / 2.0f);
        }

        public float GetMax(int axis)
        {
            return GetOffsetFromCenterForAxis(axis, Size[axis] / 2.0f);
        }

        public float GetMinX() { return GetMin(0); }
        public float GetMinY() { return GetMin(1); }
        public float GetMinZ() { return GetMin(2); }
        public float GetMaxX() { return GetMax(0); }
        public float GetMaxY() { return GetMax(1); }
        public float GetMaxZ() { return GetMax(2); }

        public Vector3 GetMinPoint() { return new Vector3(GetMinX(), GetMinY(), GetMinZ()); }
        public Vector3 GetMaxPoint() { return new Vector3(GetMaxX(), GetMaxY(), GetMaxZ()); }

        public bool IsPointInside(Vector3 pt) {
            return  pt.x >= GetMinX() && pt.x <= GetMaxX() &&
                    pt.y >= GetMinY() && pt.y <= GetMaxY() &&
                    pt.z >= GetMinZ() && pt.z <= GetMaxZ();
        }
    }
}