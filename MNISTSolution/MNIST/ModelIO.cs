using Microsoft.ML.Data;

namespace MNIST {
    /// <summary>
    /// Represents the feature and label of a MNIST digit.
    /// <para></para>
    /// Generic inputs through a CSV with 785 columns.
    /// <para></para>
    /// An input is provided by the actual digit (0) and pixel data (1...784),
    /// where the label (digit) is mutually exclusive and can only be (0...9).
    /// </summary>
    public class HandwrittenDigit {
        // use float type to minimize memory cost //

        [LoadColumn(0)]
        public float Number = default;

        [ColumnName("PixelData"), LoadColumn(1, 784)]
        [VectorType(784)]
        public float[] PixelData = default;

    }

    /// <summary>
    /// Represents the prediction of the trained network when provided an input (features).
    /// </summary>
    public class HandwrittenDigitPrediction {

        [ColumnName("Score")]
        public float[] Score { get; set; }
    }

}
