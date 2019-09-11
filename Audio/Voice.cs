using System;

namespace NoZ {
    public struct Voice {
        public static readonly Voice Error = Create(0, 0);

        public uint Id { get; private set; }
        public uint Instance { get; private set; }

        public static Voice Create (uint id, uint instance) {
            return new Voice { Id = id, Instance = instance };
        }

        public bool IsError {
            get {
                return Instance == 0;
            }
        }
    }
}
