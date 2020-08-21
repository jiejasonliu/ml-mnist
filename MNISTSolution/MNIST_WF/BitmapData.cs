﻿using System;
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
        /// Scale the bitmap, inputs should be greater than 0.
        /// </summary>
        public void Scale(float width, float height) {
            var brush = new SolidBrush(Color.Black);
            float scale = Math.Min((width / bitmap.Width), (height / bitmap.Height));

            bitmap = new Bitmap(bitmap, (int)width, (int)height);
            bitmap.Save("test.png");
        }

        /// <summary>
        /// Scales the bitmap using interpolation, image is substantially higher quality.
        /// <para></para>
        /// Inputs should be greater than 0.
        /// </summary>
        public void HDScale(int width, int height) {
            var destRect = new Rectangle(0, 0, width, height);
            var destBitmap = new Bitmap(width, height);

            destBitmap.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

            using (var graphics = Graphics.FromImage(destBitmap)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(bitmap, destRect, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            bitmap = destBitmap;
            bitmap.Save("test_hd.png");
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