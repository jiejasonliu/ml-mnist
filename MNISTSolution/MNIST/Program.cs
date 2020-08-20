using CNTK;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MNIST {
    class Program {

        // dataset path
        private static readonly string TRAIN_DATA_PATH = Path.Combine(Environment.CurrentDirectory, "mnist_train.csv");
        private static readonly string TEST_DATA_PATH = Path.Combine(Environment.CurrentDirectory, "mnist_test.csv");

        static void Main(string[] args) {
            // MLContext init
            var mlContext = new MLContext();

            // define structure of data (from csv)
            var columnDefinition = new TextLoader.Column[] {
                new TextLoader.Column(nameof(HandwrittenDigit.Number), DataKind.Single, 0),
                new TextLoader.Column(nameof(HandwrittenDigit.PixelData), DataKind.Single, 1, 784)
            };

            // load IEnumerable<HandwrittenDigit> data
            var train = LoadData(mlContext, TRAIN_DATA_PATH, columnDefinition);
            var test = LoadData(mlContext, TEST_DATA_PATH, columnDefinition);

            // map to new enumerable (LINQ)
            var trainData = train.Select(data => data.GetFeatures()).ToArray();
            var trainLabels = train.Select(data => data.GetLabel()).ToArray();
            var testData = test.Select(data => data.GetFeatures()).ToArray();
            var testLabels = test.Select(data => data.GetLabel()).ToArray();

            // build features (CNTK)
            var features = Variable.InputVariable(
                shape: NDShape.CreateNDShape(new int[] { 28, 28 }),
                dataType: DataType.Float
            );

            // build labels (CNTK)
            var labels = Variable.InputVariable(
                shape: NDShape.CreateNDShape(new int[] { 10 }),
                dataType: DataType.Float
            );

            // build the neural network 
            var network = BuildNetwork(features);

        }

        /// <summary>
        /// Creates an IEnumerable of HandwrittenDigit(s) when provided an MLContext, data path, and the column descriptions.
        /// </summary>
        private static IEnumerable<HandwrittenDigit> LoadData(MLContext context, string dataPath, TextLoader.Column[] columnDefinition) {
            // create IDataView
            var dataView = context.Data.LoadFromTextFile(
                path: dataPath,
                columns: columnDefinition,
                hasHeader: true,
                separatorChar: ','
            );

            // create dataset
            var dataset = context.Data.CreateEnumerable<HandwrittenDigit>(
                data: dataView,
                reuseRowObject: false
            );

            return dataset;
        }

        /// <summary>
        /// Builds the network with ReLU input (standard) and Softmax output (mutually exclusive labels).
        /// </summary>
        private static Variable BuildNetwork(Variable input) {
            return null;
        }
    }
}
