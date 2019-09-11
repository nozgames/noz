using System;
using System.Collections.Generic;
using System.Text;

namespace NoZ {
    public enum PrepareHint {
        Size,
        Read
    }

    [SerializedType]
    public class AudioNode {
        /// <summary>
        /// Unique name of the node within its graph.
        /// </summary>
        public string Name {
            get; set;
        }

        /// <summary>
        /// Graph the node is attached to.
        /// </summary>
        public AudioGraph ParentGraph {
            get; internal set;
        }

        public virtual void Reset() {
        }

        /// <summary>
        /// Called by ports that request a value using a normalizedTime.  If a node
        /// handles this method it should prepare the data on the given port for 
        /// reading.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="normalizedTime"></param>
        public virtual void PrepareRead(AudioPort port, float normalizedTime) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called before data is read from the given port.  This method is used by
        /// connections that do not required a normalized time.
        /// </summary>
        /// <param name="port">Port that data is being requested on</param>
        public virtual void PrepareRead(AudioPort port) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called to read samples from an output bus of a node.  Implement this
        /// method if your node has one or more output busses.  
        /// </summary>
        /// <param name="source">Output bus to read from</param>
        /// <param name="sourceOffset">Position within the output samples to read from</param>
        /// <param name="destination">Buffer to read samples into</param>
        /// <param name="destinationOffset">Offset within buffer to read samples into</param>
        /// <param name="length">Number of samples to read</param>
        /// <returns>The number of samples that were read</returns>
        public virtual int Read (AudioDataOutput source, int sourceOffset, short[] destination, int destinationOffset, int length) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Override on nodes that contain one or more output busses.  The node
        /// should return the number of samples available to be read from the bus.
        /// </summary>
        /// <param name="output">Output bus to retrieve the sample count for.</param>
        /// <returns>Number of samples available to read from this node on the 
        /// given output bus.</returns>
        public virtual int GetAvailableSampleCount (AudioDataOutput output) {
            throw new NotImplementedException();
        }
    }
}
