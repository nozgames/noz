using System;
using System.Collections.Generic;
using System.Text;

namespace NoZ {
    public enum AudioPortDirection {
        Input,
        Output
    }

    [SerializedType]
    public class AudioPort {
        public AudioPortDirection Direction {
            get; private set;
        }

        public AudioNode ParentNode {
            get; private set;
        }

        public bool IsConnected => ConnectedPort != null;

        public bool IsInput {
            get {
                return Direction == AudioPortDirection.Input;
            }
        }

        public bool IsOutput {
            get {
                return Direction == AudioPortDirection.Output;
            }
        }

        public AudioPort ConnectedPort {
            get; internal set;
        }

        public AudioPort(AudioNode parentNode, AudioPortDirection direction) {
            Direction = direction;
            ParentNode = parentNode;
        }

        public virtual void Pulse(float normalizedTime) { }
    }
}
