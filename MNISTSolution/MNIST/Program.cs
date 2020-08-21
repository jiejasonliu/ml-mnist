using CNTK;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MNIST {
    public class MNISTModel {

        // executable (DLL) folder
        private static readonly string DLL_DIRECTORY_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


        // dataset path
        public static readonly string TRAIN_DATA_PATH = Path.Combine(DLL_DIRECTORY_PATH, "mnist_train.csv");
        public static readonly string TEST_DATA_PATH = Path.Combine(DLL_DIRECTORY_PATH, "mnist_test.csv");

        // saved schema (model) path
        public static readonly string SCHEMA_MODEL_PATH = Path.Combine("schema", "model.zip");
        public static readonly string ENV_SCHEMA_MODEL_PATH = Path.Combine(DLL_DIRECTORY_PATH, SCHEMA_MODEL_PATH);

        // feature column default name
        private static readonly string DEFAULT_FEATURE_NAME = "Features";
        private static readonly string DEFAULT_LABEL_NAME = "Label";

        /// <summary>
        /// Public access to context, call <see cref="Train()" /> to populate.
        /// </summary>
        public MLContext Context { get; set; }

        /// <summary>
        /// Public access to generated network, call <see cref="Train()" /> to populate.
        /// </summary>
        public ITransformer Transformer { get; set; }

        static void Main(string[] args) {
            MNISTModel model = new MNISTModel();
            model.Train();
        }

        /// <summary>
        /// Returns a trained MLContext model.
        /// </summary>
        /// <returns></returns>
        public void Train() {
            // MLContext init
            Context = new MLContext();

            // try to load existing model
            if (File.Exists(ENV_SCHEMA_MODEL_PATH)) {
                Console.WriteLine("Found existing model at: " + ENV_SCHEMA_MODEL_PATH);
                Transformer = Context.Model.Load(SCHEMA_MODEL_PATH, out _);
            }
            // train model normally
            else {
                // define structure of data (from csv)
                var columnDefinition = new[] {
                    new TextLoader.Column(nameof(HandwrittenDigit.PixelData), DataKind.Single, 1, 784),
                    new TextLoader.Column(nameof(HandwrittenDigit.Number), DataKind.Single, 0)
                };

                // create IDataView object
                var train = LoadDataView(Context, TRAIN_DATA_PATH, columnDefinition);
                var test = LoadDataView(Context, TEST_DATA_PATH, columnDefinition);

                // build the training pipeline
                var pipeline =
                    // convert number column to keys
                    Context.Transforms.Conversion.MapValueToKey(
                        outputColumnName: DEFAULT_LABEL_NAME,
                        inputColumnName: nameof(HandwrittenDigit.Number),
                        keyOrdinality: ValueToKeyMappingEstimator.KeyOrdinality.ByValue
                    )

                    // merge PixelData columns (784) into one specialized Feature column
                    .Append(Context.Transforms.Concatenate(
                        outputColumnName: DEFAULT_FEATURE_NAME,
                        inputColumnNames: nameof(HandwrittenDigit.PixelData)
                    ))

                    // cache data for faster training
                    .AppendCacheCheckpoint(Context)

                    // train using SDCA algorithm (use default names)
                    .Append(Context.MulticlassClassification.Trainers.SdcaNonCalibrated()

                    // convert key back to value
                    .Append(Context.Transforms.Conversion.MapKeyToValue(
                        outputColumnName: nameof(HandwrittenDigit.Number),
                        inputColumnName: DEFAULT_LABEL_NAME
                    ))
                );

                // train the model
                Console.WriteLine("Training model...");

                var watch = System.Diagnostics.Stopwatch.StartNew();
                Transformer = pipeline.Fit(train);
                watch.Stop();
                Console.WriteLine($"Model took {(watch.ElapsedMilliseconds / 1000.0):#.###} seconds");
                Console.WriteLine();

                // save model locally
                Context.Model.Save(Transformer, train.Schema, SCHEMA_MODEL_PATH);
                Console.WriteLine("Model saved at: " + ENV_SCHEMA_MODEL_PATH);
                Console.WriteLine();

                // make predictions on test data
                Console.WriteLine("Evaluating model...");
                var predictions = Transformer.Transform(test);

                // evaluate predictions
                var metrics = Context.MulticlassClassification.Evaluate(
                    data: predictions,
                    labelColumnName: nameof(HandwrittenDigit.Number)
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
                hasHeader: true,
                separatorChar: ','
            );

            return dataView;
        }
    }
}
