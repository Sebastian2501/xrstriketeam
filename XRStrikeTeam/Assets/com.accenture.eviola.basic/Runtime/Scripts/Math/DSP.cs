using System;
using UnityEngine;
using Accenture.eviola.DataStructures;

namespace Accenture.eviola.Math.DSP {

    

    /// <summary>
    /// Abstract filter relying on a ring buffer
    /// </summary>
    public abstract class GenericRingBufferFilter<T> {
        protected RingBuffer<T> _buf;
        protected int _sz = 10;

        virtual public void Init(int sz) {
            if (sz <= 0) sz = 1;
            _sz = sz;
            _buf = new RingBuffer<T>(sz);
        }

        virtual public void Clear() { Init(_sz); }

        virtual public int Count() { return _buf.Count(); }

        abstract public void Update(T newVal);

        abstract public T GetCurrentValue();
    }

    /// <summary>
    /// Moving average filter, aka poor man's LPF
    /// </summary>
    public class MovingAverageFilter<T, C> : GenericRingBufferFilter<T> where T : new()
                                                                        where C : ICalculator<T>, new() {
        private T _sum = default(T);

        public MovingAverageFilter(int sz) { Init(sz); }

        public override void Init(int sz)
        {
            base.Init(sz);
            _sum = default(T);
        }

        public override void Update(T newVal)
        {
            Number<T, C> n = new Number<T, C>(_sum);
            n += newVal;
            n -= _buf.Back();
            _sum = n;
            _buf.Add(newVal);
        }

        public override T GetCurrentValue() {
            Number<T, C> n = new Number<T, C>(_sum);
            return n / Count();
        }
    }

    public class FloatMovingAverageFilter : MovingAverageFilter<float, FloatCalculator>
    {
        public FloatMovingAverageFilter(int sz) : base(sz) { }
    }

    public class Vector3MovingAverageFilter : MovingAverageFilter<Vector3, Vector3Calculator> {
        public Vector3MovingAverageFilter(int sz) : base(sz) { }
    }

    /// <summary>
    /// EMA filter, aka generic cheap LPF
    /// </summary>
    [System.Serializable]
    public class EMAFilter<T, C> where T : new()
                                  where C : ICalculator<T>, new()
    {
        [SerializeField]
        [Range(0,1)]
        public float Alpha = 0.5f;
        private T _curVal = default(T);
        private bool _bFirstValue = false;

        public EMAFilter() { Init(); }

        public void Init() {
            _curVal = default(T);
            _bFirstValue = false;
        }

        public void Update(T newVal)
        {
            if (_bFirstValue) {
                _curVal = newVal;
                _bFirstValue = true;
                return;
            }
            Number<T, C> a = new Number<T, C>(newVal);
            Number<T, C> b = new Number<T, C>(_curVal);
            a *= Alpha;
            b *= (1.0f - Alpha);
            _curVal = a + b;
        }

        public T GetCurrentValue()
        {
            return _curVal;
        }
    }

    [System.Serializable]
    public class FloatEMAFilter : EMAFilter<float, FloatCalculator> { }

    [System.Serializable]
    public class Vector3EMAFilter : EMAFilter<Vector3, Vector3Calculator> { }
}