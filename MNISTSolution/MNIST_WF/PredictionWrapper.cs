using Microsoft.ML;
using Microsoft.ML.Data;
using System;

namespace MNIST_WF {
    /// <summary>
    /// Performs a prediction on the specified MLContext with a loaded model.
    /// </summary>
    public class PredictionWrapper<Features, Label> where Features : class, new() 
                                                    where Label    : class, new() {

        // neural network data
        private MLContext mlContext;
        private ITransformer transformer;

        // prediction pipeline
        private PredictionEngine<Features, Label> engine;

        /// <summary>
        /// Load an MLContext into the PredictionWrapper.
        /// <para></para>
        /// Construction requires two type arguments for the input features and output label.
        /// </summary>
        public PredictionWrapper(MLContext context, ITransformer transformer) {
            this.mlContext = context;
            this.transformer = transformer;
            this.engine = default;
        }

        /// <summary>
        /// Prepare the PredictionEngine for a single query.
        /// </summary>
        public void InitPredictionEngine() {
            engine = mlContext.Model.CreatePredictionEngine<Features, Label>(transformer);
        }

        /// <summary>
        /// Query the PredictionEngine pipeline. 
        /// <para></para>
        /// Requires the PredictionEngine to be loaded from <see cref="InitPredictionEngine()" />.
        /// </summary>
        /// <typeparam name="Features">Type IDataView or a generic array</typeparam>
        /// <param name="features">Input data (features)</param>
        /// <param name="Prediction">Provides the input prediction object with the predicted data.</param>
        public void Predict(Features features, out Label resultType) {
            if (engine != null)
                resultType = engine.Predict(features);
            else
                throw new NullReferenceException("The PredictionEngine has not been initalized.");
        }
        
    }
}
