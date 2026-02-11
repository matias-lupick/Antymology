using UnityEngine;

public class Brain
{
    float[] input;
    public BrainLayer[] layers;
    float[] output;


    /// <summary>
    /// number of values to route form output back into input
    /// </summary>
    int memorySize;

    public Brain() 
    {
        input = new float[memorySize + (int)PheromoneType.size + 4];
        output = new float[(int)Ant.Action.size + 10];
    }

    public Ant.Action GetAction(Ant d) 
    {
        int rand = Random.Range(0, 8);

        if (rand == 2)
            rand = 0;

        return (Ant.Action)rand;
    }

    public class BrainLayer 
    {
        public float[] input;
        public float[] output;

        public float[,] weights;
        public float[] bias;

        public int inSize;
        public int outSize;

        public BrainLayer() { }

        public BrainLayer(float[] input, float[] output) 
        {
            this.input = input;
            this.output = output;

            inSize = input.Length;
            outSize = output.Length;

            weights = new float[inSize, outSize];
            bias = new float[outSize];
        }

        //public void 

        public void Forwards() 
        {
            for (int j = 0; j < outSize; j++)  {
                output[j] = bias[j];

                for (int i = 0; i < inSize; i++)
                {
                    output[j] += weights[i, j] * input[i];
                }
            }
        }
    }
}
