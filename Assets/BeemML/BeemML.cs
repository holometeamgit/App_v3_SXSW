using System.Linq;
using UnityEngine;

namespace TensorFlowLite
{
    public class BeemML : BaseImagePredictor<float>
    {
        private Texture _inputTex;
        
        [System.Serializable]
        public class Options
        {
            [FilePopup("*.tflite")] public string modelFile = string.Empty;
            public Accelerator accelerator = Accelerator.GPU;
            public ComputeShader compute = null;
        }
        
        // TODO remove this in next version
        private static readonly Color32[] COLOR_TABLE = new Color32[]
        {
            ToColor(0xFF00_0000), // Black
            ToColor(0xFF80_3E75), // Strong Purple
            ToColor(0xFFFF_6800), // Vivid Orange
            ToColor(0xFFA6_BDD7), // Very Light Blue
            ToColor(0xFFC1_0020), // Vivid Red
            ToColor(0xFFCE_A262), // Grayish Yellow
            ToColor(0xFF81_7066), // Medium Gray
            ToColor(0xFF00_7D34), // Vivid Green
            ToColor(0xFFF6_768E), // Strong Purplish Pink
            ToColor(0xFF00_538A), // Strong Blue
            ToColor(0xFFFF_7A5C), // Strong Yellowish Pink
            ToColor(0xFF53_377A), // Strong Violet
            ToColor(0xFFFF_8E00), // Vivid Orange Yellow
            ToColor(0xFFB3_2851), // Strong Purplish Red
            ToColor(0xFFF4_C800), // Vivid Greenish Yellow
            ToColor(0xFF7F_180D), // Strong Reddish Brown
            ToColor(0xFF93_AA00), // Vivid Yellowish Green
            ToColor(0xFF59_3315), // Deep Yellowish Brown
            ToColor(0xFFF1_3A13), // Vivid Reddish Orange
            ToColor(0xFF23_2C16), // Dark Olive Green
            ToColor(0xFF00_A1C2), // Vivid Blue
        };

        // model output
        private readonly float[,,] outputs0; // height, width, 21
        
        // data for GPU transforming output array back to texture
        private readonly ComputeShader compute;
        private readonly ComputeBuffer labelBuffer;
        private readonly ComputeBuffer colorTableBuffer;
        
        private readonly RenderTexture labelTex;

        private readonly Texture2D labelTex2D;
        private readonly int labelToTexKernel;

        public BeemML(Options options) : base(options.modelFile, options.accelerator)
        {
            resizeOptions = new TextureResizer.ResizeOptions() {
                aspectMode = AspectMode.None,//AspectMode.Fit,
                rotationDegree = 0,
                mirrorHorizontal = false,
                mirrorVertical = false,
                width = width,
                height = height,
            };
            
            var oShape0 = interpreter.GetOutputTensorInfo(0).shape;

            Debug.Assert(oShape0[1] == height);
            Debug.Assert(oShape0[2] == width);

            outputs0 = new float[oShape0[1], oShape0[2], oShape0[3]];
            labelTex2D = new Texture2D(width, height, TextureFormat.RGBA32, 0, false);

            // Init compute shader resources
            labelTex = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            labelTex.enableRandomWrite = true;
            labelTex.Create();
            labelBuffer = new ComputeBuffer(height * width, sizeof(float) * 21);
            colorTableBuffer = new ComputeBuffer(21, sizeof(float) * 4);

            compute = options.compute;
            int initKernel = compute.FindKernel("Init");
            compute.SetInt("Width", width);
            compute.SetInt("Height", height);
            compute.SetTexture(initKernel, "Result", labelTex);
            compute.Dispatch(initKernel, width, height, 1);

            labelToTexKernel = compute.FindKernel("LabelToTex");

            // Init RGBA color table
            var table = COLOR_TABLE.Select(c => c.ToRGBA()).ToList();
            colorTableBuffer.SetData(table);
        }

        public override void Dispose()
        {
            base.Dispose();

            if (labelTex != null)
            {
                labelTex.Release();
                Object.Destroy(labelTex);
            }

            if (labelTex2D != null)
            {
                Object.Destroy(labelTex2D);
            }

            labelBuffer?.Release();
            colorTableBuffer?.Release();
        }
    
        /// <summary>
        /// Call model's image processing
        /// </summary>
        /// <param name="inputTex">input texture</param>
        public override void Invoke(Texture inputTex)
        {
            _inputTex = inputTex;
            ToTensor(inputTex, input0);

            interpreter.SetInputTensorData(0, input0);
            interpreter.Invoke();
            interpreter.GetOutputTensorData(0, outputs0);
        }
        
        /// <summary>
        /// Computing final texture after receiving output of the neural network 
        /// </summary>
        /// <returns>Mask texture</returns>
        public RenderTexture GetResultTexture()
        {
            labelBuffer.SetData(outputs0);
            compute.SetBuffer(labelToTexKernel, "LabelBuffer", labelBuffer);
            compute.SetBuffer(labelToTexKernel, "ColorTable", colorTableBuffer);
            compute.SetTexture(labelToTexKernel, "Result", labelTex);
            
            compute.Dispatch(labelToTexKernel, 224 / 8, 224 / 8, 1);

            return labelTex;
        }

        private static Color32 ToColor(uint c)
        {
            return Color32Extension.FromHex(c);
        }
    }
}