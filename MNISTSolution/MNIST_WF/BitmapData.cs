using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace MNIST_WF {
    /// <summary>
    /// Represents a modular bitmap to be usable neural network or model data.
    /// </summary>
    public class BitmapData {

        public Bitmap bitmap { get; private set; }

        /// <summary>
        /// Creates a BitmapData holding a bitmap.
        /// </summary>
        public BitmapData(Bitmap bitmap) {
            this.bitmap = bitmap;
        }

        /// <summary>
        /// Convert the pixel data of the Bitmap to a float array.
        /// </summary>
        public float[] ToFloatArray() {
            float[] buffer = new float[bitmap.Width * bitmap.Height];
            int pointer = 0;

            // get alpha value for each pixel (0 - white, 255 - black)
            for (int row = 0; row < bitmap.Height; row++)
                for (int col = 0; col < bitmap.Width; col++)
                    buffer[pointer++] = bitmap.GetPixel(col, row).A;

            return buffer;
        }
    }
}
