using Microsoft.ML;
using MNIST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MNIST_WF {
    public partial class MNIST_Form : Form {

        // Point.Empty represents null
        private Point lastPoint;
        private Bitmap bmp;
        private Pen pen;
        private Graphics g;
        private bool mouseActive;

        public MNIST_Form() {
            InitializeComponent();
            LoadFreeDrawBox();
        }
        private void LoadFreeDrawBox() {
            lastPoint = Point.Empty;
            bmp = new Bitmap(FreeDrawBox.Width, FreeDrawBox.Height);
            pen = new Pen(Color.Black, 8);
            pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            pen.MiterLimit = pen.Width * 1.25f;
            pen.LineJoin = LineJoin.Round;
            FreeDrawBox.Image = bmp;
            g = Graphics.FromImage(FreeDrawBox.Image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            mouseActive = false;
        }

        private void Clear_Click(object sender, EventArgs e) {
            if (FreeDrawBox.Image != null) {
                LoadFreeDrawBox();
                Refresh();
            }
        }

        private void Evaluate_Click(object sender, EventArgs e) {
            // preprocess bitmap
            var preprocessed = new BitmapPreprocessor(bmp).PreprocessImage();
            var bitmap = new BitmapData(preprocessed);

            // convert bitmap to dataset
            var data = new HandwrittenDigit {
                PixelData = bitmap.ToFloatArray()
            };

            // train model (load from file if model exists)
            MNISTModel model = new MNISTModel();
            model.Train();

            // create PredictionEngine
            PredictionWrapper<HandwrittenDigit, HandwrittenDigitPrediction> wrapper =
                new PredictionWrapper<HandwrittenDigit, HandwrittenDigitPrediction>(model.Context, model.Transformer);
            wrapper.InitPredictionEngine();

            // prediction result
            HandwrittenDigitPrediction prediction;
            wrapper.Predict(data, out prediction);

            // find highest confidence
            // find max score and retrieve number
            int max = 0;
            float maxScore = prediction.Score[0];

            // iterate through all scoresto find max
            for (int i = 1; i < 10; i++) {
                // update max under correct condition
                if (prediction.Score[i] > maxScore) {
                    max = i;
                    maxScore = prediction.Score[i];
                }
            }

            // update output label
            PredictionLabel.Text = max.ToString();
        }

        private void FreeDrawBox_MouseDown(object sender, MouseEventArgs e) {
            lastPoint = e.Location;
            mouseActive = true;
        }
        
        private void FreeDrawBox_MouseUp(object sender, MouseEventArgs e) {
            mouseActive = false;
        }

        private void FreeDrawBox_MouseMove(object sender, MouseEventArgs e) {
            // verify mouse is active and position is valid
            if (mouseActive && lastPoint != null)
                g.DrawLine(pen, lastPoint, e.Location);
        
            // update picture
            FreeDrawBox.Refresh();
            lastPoint = e.Location;
        }
    }
}
