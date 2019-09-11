using System.IO;

namespace NoZ {
    public interface IGraphicsDriver {
        GraphicsContext CreateContext();

        /// <summary>
        /// Create an empty image.
        /// </summary>
        /// <remarks>
        /// Empty images are uses primarily by the serialization system. 
        /// </remarks>
        /// <returns>Created image.</returns>
        Image CreateImage();

        /// <summary>
        /// Create an image loading its contents from the given stream
        /// </summary>
        /// <param name="stream">Stream to load the image from</param>
        /// <returns>Created image.</returns>
        Image LoadImage(Stream stream);

        Image CreateImage(int width, int height, PixelFormat format);

        Cursor CreateCursor(Image image);

        Cursor CreateCursor(SystemCursor systemCursor);
    }
}
