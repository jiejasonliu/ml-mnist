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

        // saved schema (model) path
        private static readonly string SCHEMA_MODEL_PATH = Path.Combine("schema", "model.zip");
        private static readonly string ENV_SCHEMA_MODEL_PATH = Path.Combine(Environment.CurrentDirectory, SCHEMA_MODEL_PATH);

        // feature column default name
        private static readonly string DEFAULT_FEATURE_NAME = "Features";

        static void Main(string[] args) {
            // MLContext init
            var mlContext = new MLContext();

            // neural network declaration
            ITransformer model;

            // try to load existing model
            if (File.Exists(ENV_SCHEMA_MODEL_PATH)) {
                Console.WriteLine("Found existing model at: " + ENV_SCHEMA_MODEL_PATH);
                DataViewSchema schema;
                model = mlContext.Model.Load(SCHEMA_MODEL_PATH, out schema);
            }
            // train model normally
            else {
                // define structure of data (from csv)
                var columnDefinition = new[] {
                    new TextLoader.Column(nameof(HandwrittenDigit.PixelData), DataKind.Single, 1, 784),
                    new TextLoader.Column(nameof(HandwrittenDigit.Number), DataKind.Single, 0)
                };

                // create IDataView object
                var train = LoadDataView(mlContext, TRAIN_DATA_PATH, columnDefinition);
                var test = LoadDataView(mlContext, TEST_DATA_PATH, columnDefinition);

                // build the training pipeline
                // merge PixelData columns (784) into one specialized Feature column
                var pipeline = mlContext.Transforms.Concatenate(
                    outputColumnName: DEFAULT_FEATURE_NAME,
                    inputColumnNames: nameof(HandwrittenDigit.PixelData))

                    // convert number column to keys
                    .Append(mlContext.Transforms.Conversion.MapValueToKey("Number", nameof(HandwrittenDigit.Number)))

                    // cache data for faster training
                    .AppendCacheCheckpoint(mlContext)

                    // train using SDCA algorithm
                    .Append(mlContext.MulticlassClassification.Trainers.SdcaNonCalibrated(
                        labelColumnName: nameof(HandwrittenDigit.Number),
                        featureColumnName: DEFAULT_FEATURE_NAME
                    )
                );

                // train the model
                Console.WriteLine("Training model...");

                var watch = System.Diagnostics.Stopwatch.StartNew();
                model = pipeline.Fit(train);
                watch.Stop();
                Console.WriteLine($"Model took {(watch.ElapsedMilliseconds / 1000.0):#.###} seconds");
                Console.WriteLine();

                // save model locally
                mlContext.Model.Save(model, train.Schema, SCHEMA_MODEL_PATH);
                Console.WriteLine("Model saved at: " + ENV_SCHEMA_MODEL_PATH));
                Console.WriteLine();

                // make predictions on test data
                Console.WriteLine("Evaluating model...");
                var predictions = model.Transform(test);

                // evaluate predictions
                var metrics = mlContext.MulticlassClassification.Evaluate(
                    data: predictions,
                    labelColumnName: "Number"
                );

                // show evaluation metrics
                Console.WriteLine($"Evaluation metrics: ");
                Console.WriteLine($" MicroAccuracy:    {metrics.MicroAccuracy:0.###}");
                Console.WriteLine($" MacroAccuracy:    {metrics.MacroAccuracy:0.###}");
                Console.WriteLine($" LogLoss:          {metrics.LogLoss:#.###}");
                Console.WriteLine($" LogLossReduction: {metrics.LogLossReduction:#.###}");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Creates an IEnumerable of HandwrittenDigit(s) when provided an MLContext, data path, and the column descriptions.
        /// </summary>
        private static IEnumerable<HandwrittenDigit> LoadDataset(MLContext context, string dataPath, TextLoader.Column[] columnDefinition) {
            // create dataset
            var dataset = context.Data.CreateEnumerable<HandwrittenDigit>(
                data: LoadDataView(context, dataPath, columnDefinition),
                reuseRowObject: false
            );

            return dataset;
        }

        /// <summary>
        /// Creates an IDataView when provided an MLContext, data path, and the column descriptions.
        /// </summary>
        private static IDataView LoadDataView(MLContext context, string dataPath, TextLoader.Column[] columnDefinition) {
            // create IDataView
            var dataView = context.Data.LoadFromTextFile(
                path: dataPath,
                columns: columnDefinition,
                hasHeader: false,
                separatorChar: ','
            );

            return dataView;
        }
    }
}
