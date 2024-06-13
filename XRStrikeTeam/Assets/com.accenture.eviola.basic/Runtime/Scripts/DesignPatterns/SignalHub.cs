using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Accenture.eviola.DesignPatterns.Signals
{
    public class SignalHub : Singleton<SignalHub>
    {
        public UnityEvent<ISignal> OnNewSignalWithChannel = new UnityEvent<ISignal>();

        public void Fire(ISignal s) {
            switch (s.Type) {
                case SignalType.CHANNEL:
                    RouteSignalWithChannel(s);
                    break;
                case SignalType.TARGET:
                    RouteSignalWithTarget(s);
                    break;
                default: 
                    break;
            }
        }

        private void RouteSignalWithChannel(ISignal s) { 
            OnNewSignalWithChannel.Invoke(s);
        }

        private void RouteSignalWithTarget(ISignal s) { 
        
        }

        #region Instance

        static public void FireSignal(ISignal s) {
            if (!IsInstanceReady()) return;
            Instance.Fire(s);
        }

        static private void SetSignalListener(UnityEvent<ISignal> e, bool bListen, UnityAction<ISignal> callback) {
            if (bListen)
            {
                e.AddListener(callback);
            }
            else { 
                e.RemoveListener(callback);
            }
        }

        static public bool SetChannelSignalListener(bool bListen, UnityAction<ISignal> callback) {
            if (!IsInstanceReady()) return false;
            SetSignalListener(Instance.OnNewSignalWithChannel, bListen, callback);
            return true;
        }

        static public bool AddChannelSignalListener(UnityAction<ISignal> callback) { return SetChannelSignalListener(true, callback); }
        static public bool RemoveChannelSignalListener(UnityAction<ISignal> callback) { return SetChannelSignalListener(false, callback); }

        #endregion
    }
}