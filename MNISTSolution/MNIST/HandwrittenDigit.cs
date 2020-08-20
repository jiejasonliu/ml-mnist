using System;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using CNTK;

namespace MNIST {
    /// <summary>
    /// Represents the feature and label of a MNIST digit.
    /// <para></para>
    /// Generic inputs through a CSV with 785 columns.
    /// <para></para>
    /// An input is provided by the actual digit (0) and pixel data (1...784),
    /// where the label (digit) is mutually exclusive and can only be (0...9).
    /// </summary>
    class HandwrittenDigit {

        // use float type to minimize memory cost //

        [LoadColumn(0)]
        public float Number = default;

        [VectorType(784)]
        public float[] PixelData = default;

        // GET: feature(s)
        public float[] GetFeatures() => PixelData;

        // GET: label
        // 10-dimension vector with binary results (1.0 or 0.0) if neuron is active
        public float[] GetLabel() => new float[] {
            Number == 0 ? 1.0F : 0.0F,
            Number == 1 ? 1.0F : 0.0F,
            Number == 2 ? 1.0F : 0.0F,
            Number == 3 ? 1.0F : 0.0F,
            Number == 4 ? 1.0F : 0.0F,
            Number == 5 ? 1.0F : 0.0F,
            Number == 6 ? 1.0F : 0.0F,
            Number == 7 ? 1.0F : 0.0F,
            Number == 8 ? 1.0F : 0.0F,
            Number == 9 ? 1.0F : 0.0F,
        };

    }
}
