using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            var bitmap = new BitmapData(bmp);
            bitmap.HDScale(28, 28);

            var data = bitmap.ToFloatArray();
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
