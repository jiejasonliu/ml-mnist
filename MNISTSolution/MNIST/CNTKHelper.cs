using System;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using CNTK;

namespace MNIST {
    /// <summary>
    /// Helper functions for CNTK to create neural networks.
    /// </summary>
    public static class CNTKHelper {

        /// <summary>
        /// Adds a new dense layer provided node count and activation function.
        /// </summary>
        /// <returns></returns>
        public static Variable AddLayer(this Variable input, int nodeCount, Func<CNTK.Variable, CNTK.Function> activationFunc, string layerName) {
            return (Variable) activationFunc(AddLayer(input, nodeCount, layerName));
        }

        private static Variable AddLayer(this Variable input, int nodeCount, string layerName) {
            // get compute from gpu or cpu
            var computeDevice = GetCurrentDevice();

            // create times parameter
            var timesParameter = new Parameter(
                shape: NDShape.CreateNDShape(new int[] { nodeCount, CNTK.NDShape.InferredDimension }),
                dataType: DataType.Float,
                initializer: CNTKLib.GlorotUniformInitializer(
                    scale: CNTKLib.DefaultParamInitScale,
                    outputRank: CNTKLib.SentinelValueForInferParamInitRank,
                    filterRank: CNTKLib.SentinelValueForInferParamInitRank, 
                    seed: 1
                ),
                device: computeDevice,
                name: "timesParameter_" + layerName
            );

            // create times function
            var timesFunc = CNTKLib.Times(
                leftOperand: timesParameter,
                rightOperand: input,
                outputRank: 1,
                inferInputRankToMap: 0
            );

            // create plus (new node) parameter
            var plusParameter = new Parameter(
                shape: CNTK.NDShape.CreateNDShape(new int[] { CNTK.NDShape.InferredDimension }),
                initValue: 0.0F,
                device: computeDevice,
                "plusParameter_" + layerName);

            return CNTKLib.Plus(plusParameter, timesFunc, layerName);
        }

        /// <summary>
        /// Returns the first compatible compute GPU. CPU failsafe if no valid GPU found.
        /// </summary>
        public static DeviceDescriptor GetCurrentDevice() {
            // look through all devices
            foreach (var device in CNTK.DeviceDescriptor.AllDevices())
                // found gpu
                if (device.Type == CNTK.DeviceKind.GPU)
                    return device;
            
            // did not find gpu, default to cpu
            return DeviceDescriptor.CPUDevice;
        }
    }
}
