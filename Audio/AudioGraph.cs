using System;
using System.Collections.Generic;
using NoZ.Serialization;

namespace NoZ {

    [SharedResource]
    [SerializedType]
    public class AudioGraph : AudioStream, IResource, IDeserializedCallback {
        [SerializedMember(IsPrivate = true, IsContent = true, IsReadOnly = true, Name = "Nodes")]
        public List<AudioNode> _nodes { get; private set; } = new List<AudioNode>();

        [SerializedMember(IsPrivate = true, IsReadOnly = true, Name = "Connections")]
        private List<AudioConnection> _connections = new List<AudioConnection>();

        /// <summary>
        /// Exposes the connection list as a read-only list.
        /// </summary>
        public ReadOnlyList<AudioConnection> Connections => new ReadOnlyList<AudioConnection>(_connections);

        /// <summary>
        /// Exposes the node list as a read-only list.
        /// </summary>
        public ReadOnlyList<AudioNode> Nodes => new ReadOnlyList<AudioNode>(_nodes);

        public AudioOutputNode Output { get; private set; }

        public int SamplesPerSecond => Output.SamplesPerSecond;

        /// <summary>
        /// Returns the total sample count for the graph
        /// </summary>
        public int SampleCount => Output.Input.GetAvailableSampleCount();

        Resource IResource.Resource { get; set; }

        public AudioGraph() {
        }

        public AudioGraph(uint samplesPerSecond, AudioChannelFormat channelFormat) {
            Attach(new AudioOutputNode(4096, samplesPerSecond, channelFormat));
        }

        public AudioNode GetNode(string name) {
            foreach (var node in _nodes)
                if (node.Name == name)
                    return node;
            return null;
        }

        public void Attach(AudioNode node) {
            if (null == node)
                throw new ArgumentNullException("node");
            if (node.ParentGraph != null)
                throw new ArgumentException("Node is already a member of a another graph");

            _nodes.Add(node);

            AttachInternal(node);
        }

        private void AttachInternal (AudioNode node) {
            if (node.GetType() == typeof(AudioOutputNode)) {
                if (Output != null)
                    throw new ArgumentException("AudioGraph can only have one output node attached.", "node");

                Output = node as AudioOutputNode;
            }

            node.ParentGraph = this;
        }

        public void Detach(AudioNode node) {
            if (null == node)
                throw new ArgumentNullException("node");
            if (!ReferenceEquals(node.ParentGraph, this))
                throw new ArgumentException("Node is not a member of the given graph");

            node.ParentGraph = null;
            _nodes.Remove(node);
        }

        public void Connect(AudioPort port1, AudioPort port2) {
            if (null == port1)
                throw new ArgumentNullException("port1");
            if (null == port2)
                throw new ArgumentNullException("port1");
            if (port1 == port2)
                throw new ArgumentException("Cannot connect port to itself");
            if (port1.IsConnected)
                throw new ArgumentException("Port is already connected", "port1");
            if (port2.IsConnected)
                throw new ArgumentException("Port is already connected", "port2");
            if (port1.Direction == port2.Direction)
                throw new ArgumentException("Ports cannot be connected to another port of the same direction");
            if (!ReferenceEquals(port1.ParentNode.ParentGraph, this))
                throw new ArgumentException("Parent node of port is not a member of this graph", "port1");
            if (!ReferenceEquals(port2.ParentNode.ParentGraph, this))
                throw new ArgumentException("Parent node of port is not a member of this graph", "port2");

            // TODO: ensure the two ports can connect to each other
            ///      port1.CanConnectTo (port2)

            port1.ConnectedPort = port2;
            port2.ConnectedPort = port1;

            _connections.Add(new AudioConnection(port1, port2));
        }

        public void Disconnect(AudioPort port) {
            if (null == port)
                throw new ArgumentNullException("port");
            if (!port.IsConnected)
                return;
            if (!ReferenceEquals(port.ParentNode.ParentGraph, this))
                throw new ArgumentNullException("Parent node of port is not a member of this graph", "port");

            // Remove the connection from the connections list
            for(int i=0; i<_connections.Count; i++) {
                if(_connections[i].Output == port || _connections[i].Output == port.ConnectedPort) {
                    _connections.RemoveAt(i);
                    break;
                }                   
            }

            // Disconnect the port
            port.ConnectedPort.ConnectedPort = null;
            port.ConnectedPort = null;
        }

        public override void SeekBegin() {
            foreach (var node in _nodes) {
                node.Reset();
            }
        }

        public override long Read(short[] data) {
            throw new NotImplementedException();
        }

        public AudioClip ToClip() {
            int sampleCount = Output.Input.GetAvailableSampleCount();
            if (0 == sampleCount)
                return AudioClip.Create();

            short[] samples = new short[sampleCount];

            Output.Read(0, samples, 0, sampleCount);
            var clip = AudioClip.Create(sampleCount, Output.ChannelFormat, (int)Output.SamplesPerSecond);
            clip.SetData(samples,0);
            return clip;
        }

        public void OnDeserialized() {
            foreach (var node in _nodes)
                AttachInternal(node);            

            if(Output == null)
                Attach(new AudioOutputNode(4096, 44100, AudioChannelFormat.Mono));

            foreach (var con in Connections) {
                con.Input.ConnectedPort = con.Output;
                con.Output.ConnectedPort = con.Input;
            }
        }
    }
}
