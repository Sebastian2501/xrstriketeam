using System;

namespace Accenture.eviola.DataStructures
{
    /// <summary>
    /// genegic ring buffer
    /// </summary>
    public class RingBuffer<T>
    {
        private T[] _buf;
        private int _idxHead = 0;
        private int _sz = 10;

        public RingBuffer(int sz) { Init(sz); }

        public void Init(int sz)
        {
            if (sz <= 0) sz = 1;
            _sz = sz;
            _buf = new T[_sz];
        }

        public void Clear()
        {
            Init(_sz);
        }

        public T this[int idx]
        {
            get { return GetElement(idx); }
            set { SetElement(idx, value); }
        }

        private int AdjustedIndex(int idx)
        {
            if (_idxHead > idx)
            {
                return _idxHead - idx - 1;
            }
            else
            {
                return Count() + _idxHead - idx - 1;
            }
        }

        public T GetElement(int idx)
        {
            int i = AdjustedIndex(idx);
            if (!Misc.IsGoodIndex(i, _buf))
            {
                throw new IndexOutOfRangeException("Invalid index " + i);
            }
            return _buf[i];
        }

        public void SetElement(int idx, T val)
        {
            int i = AdjustedIndex(idx);
            if (!Misc.IsGoodIndex(i, _buf))
            {
                throw new IndexOutOfRangeException("Invalid index " + i);
            }
            _buf[i] = val;
        }

        public void Add(T newVal)
        {
            _buf[_idxHead] = newVal;
            _idxHead++;
            if (_idxHead >= Count())
            {
                _idxHead = 0;
            }
        }

        public int Count()
        {
            return _buf.Length;
        }

        public T Front() { return GetElement(0); }
        public T Back() { return GetElement(_idxHead); }
    }

    /// <summary>
    /// fixed size buffer that discards old elements as you add new ones
    /// </summary>
    public class FixedSizeBuffer<T> {
        private T[] _buf;
        private int _idxStart = 0;
        private int _sz = 10;
        private int _numFilled = 0;

        public FixedSizeBuffer(int sz) { Init(sz); }

        public void Init(int sz) {
            if (sz <= 0) sz = 1;
            _sz = sz;
            _buf = new T[_sz];
            _idxStart = 0;
            _numFilled = 0;
        }

        public void Clear() { Init(_sz); }

        public int Size() { return _sz; }
        public int CountFilled() { return _numFilled; }

        public T this[int idx]
        {
            get {
                    if (!Misc.IsGoodIndex(idx, _buf))
                    {
                        throw new IndexOutOfRangeException("Invalid index " + idx);
                    }
                    return GetVirtualElement(idx); 
            }
            set {
                if (!Misc.IsGoodIndex(idx, _buf))
                {
                    throw new IndexOutOfRangeException("Invalid index " + idx);
                }
                _buf[CalcVirtualIndex(idx)] = value;
            }
        }

        public void Add(T el) {
            if (!IsFilled()) {
                _buf[_numFilled] = el;
                _numFilled++;
                return;
            }
            AdvanceStartIdx();
            _buf[CalcVirtualIndex(_sz-1)] = el;
        }

        public T GetOldest() { return _buf[_idxStart]; }

        public T GetNewest() {
            if (!IsFilled()) {
                int idx = 0;
                if (_numFilled >= 2){ 
                    idx = _numFilled-1;
                }
                return _buf[idx];
            }
            return GetVirtualElement(_sz-1);
        }

        public T[] ToArray()
        {
            T[] a = new T[_sz];
            for (int i = 0; i < _sz; i++)
            {
                a[i] = this[i];
            }
            return a;
        }

        private T GetVirtualElement(int idx) { return _buf[CalcVirtualIndex(idx)]; }

        public bool IsFilled() { return _numFilled >= _sz; }

        private void AdvanceStartIdx() {
            _idxStart++;
            if(_idxStart>=_sz)_idxStart = 0;
        }

        private int CalcVirtualIndex(int idx) {
            if (!IsFilled()) return idx;
            int v = _idxStart + idx;
            if (v >= _sz) { 
                v = v%_sz;
            }
            return v;
        }
    }
}
