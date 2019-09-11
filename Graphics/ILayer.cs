using System;


namespace NoZ {
    public interface ILayer {
        int SortOrder { get; }

        void BeginLayer(GraphicsContext gc);
        void EndLayer(GraphicsContext gc);
    }
}
