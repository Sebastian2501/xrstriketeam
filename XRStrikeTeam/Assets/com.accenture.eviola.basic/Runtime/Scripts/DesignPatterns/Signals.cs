using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.eviola.DesignPatterns.Signals {

    public enum SignalType { 
        CHANNEL,
        TARGET
    }

    public interface ISignal { 
        SignalType Type { get; }
    }

    public class SignalWithChannel<T> : ISignal{
        public string Channel = "default";
        public T Payload;
        public SignalType Type => SignalType.CHANNEL;
    }    

    public class SignalWithTarget<T> : ISignal
    {
        public Component Target = null;
        public T Payload;
        public SignalType Type => SignalType.TARGET;
    }
}
