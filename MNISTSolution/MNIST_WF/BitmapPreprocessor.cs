
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Linq;

namespace MNIST_WF {
    /// <summary>
    /// Preprocesses a digit bitmap in the form of a centered 28x28 image compatible for the MNIST model.
    /// </summary>
    class BitmapPreprocessor {

        public Bitmap Bitmap { get; private set; }

        /// <summary>
        /// Creates a BitmapPreprocessor instance holding an optimized bitmap.
        /// </summary>
        public BitmapPreprocessor(Bitmap bitmap) {
            this.Bitmap = bitmap;
        }

        /// <summary>
        /// Preprocesses an optimized bitmap of a digit to be compatible with the MNIST model.
        /// <para>
        /// Bounds the image in a 20x20 square and finalizes by centering it on a 28x28 bitmap.
        /// </para>
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public Bitmap PreprocessImage() {
            var square = GetBoundingSquare();

            // ensure bitmap is not vacant
            if (square == Rectangle.Empty)
                return null;

            // create 20x20 bitmap
            var init_bmp = CreateBitmapAndScale(square, 20);

            // scale to 28x28 and center
            var offset = GetCenterOffset(init_bmp);
            var scaled_bmp = new Bitmap(28, 28);
            var g = Graphics.FromImage(scaled_bmp);

            g.DrawImage(init_bmp, 2 - offset.X, 3 - offset.Y);
            init_bmp.Dispose();

            return scaled_bmp;
        }

        /// <summary>
        /// Returns the smallest square of the bitmap.
        /// </summary>
        protected Rectangle GetBoundingSquare() {
            // note: x+ is right, y+ is down (c# coord system)
            var x_left = int.MaxValue;   // left
            var x_right = int.MinValue;  // right
            var y_top = int.MaxValue;    // top
            var y_bottom = int.MinValue; // bottom
            bool vacant = true;          // empty bitmap

            // iterate through entire bitmap and search for occupied pixels
            for (int x = 0; x < Bitmap.Width; x++) {
                for (int y = 0; y < Bitmap.Height; y++) {
                    var pixel = Bitmap.GetPixel(x, y);

                    // check if pixel is not white (occupied pixel)
                    if (pixel.A > 0) {
                        // bitmap is not empty
                        vacant = false;

                        // increase left bound
                        if (x < x_left)
                            x_left = x;

                        // increase right bound
                        if (x > x_right)
                            x_right = x;

                        // increase top bound
                        if (y < y_top)
                            y_top = y;

                        // increase bottom bound
                        if (y > y_bottom)
                            y_bottom = y;
                    }
                }
            }
            // no number found (or any drawing of that matter)
            if (vacant)
                return Rectangle.Empty;

            // calculate delta lengths and necessary side length for a square
            var width = x_right - x_left;
            var height = y_bottom - y_top;
            var sideLength = Math.Max(width, height);

            // shift x or y positions to create a square [e.g. (20x30) => (30x30)]
            if (height > width)
                x_left -= (sideLength - width) / 2;
            else
                y_top -= (sideLength - height) / 2;

            // create square now that we have proper balanced dimensions
            return new Rectangle(x_left, y_top, sideLength, sideLength);
        }

        /// <summary>
        /// Create a bitmap of specified size using a square (Rectangle).
        /// </summary>
        /// <param name="rect">must be a square</param>
        /// <param name="side">max side length of square to scale to</param>
        /// <returns></returns>
        protected Bitmap CreateBitmapAndScale(Rectangle scaledRect, int side) {
            // create optimized bitmap
            var map = new Bitmap(side, side);
            map.SetResolution(Bitmap.HorizontalResolution, Bitmap.VerticalResolution);

            // create shape shell
            var shellRect = new Rectangle(0, 0, side, side);

            // create graphics object to render bitmap
            var g = Graphics.FromImage(map);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // draw image on bitmap using graphics instance
            g.DrawImage(Bitmap, shellRect, scaledRect, GraphicsUnit.Pixel);

            return map;
        }

        /// <summary>
        /// Returns the (x, y) offset of the bounded image by computing the center of the pixel mass.
        /// </summary>
        /// <returns></returns>
        protected Point GetCenterOffset(Bitmap map) {
            var vects_2D = new List<Vector2>();

            // iterate through bitmap
            for (int x = 0; x < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    var pixel = map.GetPixel(x, y);

                    // check if pixel is not white (occupied pixel)
                    if (pixel.A > 0)
                        vects_2D.Add(new Vector2(x, y));
                }
            }

            // sum vectors
            var centroid = vects_2D.Aggregate(Vector2.Zero, (current, point) => current + point) / vects_2D.Count();

            // return offset
            return new Point((int)centroid.X - map.Width / 2,
                             (int)centroid.Y - map.Height / 2);
        }
    }
}
