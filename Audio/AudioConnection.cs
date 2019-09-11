using System;

namespace NoZ {
    [SerializedType]
    public class AudioConnection {
        [SerializedMember]
        public AudioPort Input { get; private set; }

        [SerializedMember]
        public AudioPort Output { get; private set; }

        public AudioConnection( ) { }

        public AudioConnection(AudioPort port1, AudioPort port2) {
            Input = port1.IsInput ? port1 : port2;
            Output = port1.IsOutput ? port1 : port2;
        }
    }

}
