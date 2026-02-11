using UnityEngine;
using Antymology.Terrain;

public class Brain
{
    float[] input;
    public BrainLayer[] layers;
    float[] output;


    /// <summary>
    /// number of values to route form output back into input
    /// </summary>
    int memorySize = 2;

    public Brain() 
    {
        //3 for block type, 1 for random, 1 for hunger, 1 for is queen, 1 for neighbor
        input = new float[memorySize + (int)PheromoneType.size + 7];
        output = new float[(int)Ant.Action.size + memorySize];

        layers = new BrainLayer[1];
        layers[0] = new BrainLayer(input, output);
        layers[0].InitRandom();
    }

    public Ant.Action GetAction(Ant d) 
    {
        int count = 0;
        int action = 0;
        float best = 0f;

        /*int rand = Random.Range(0, 8);

        if (rand == 2)
            rand = 0;

        return (Ant.Action)rand;*/

        for (int i = 0; i < (int)PheromoneType.size; i++) 
        {
            input[count++] = (float)d.airBlock.pheromoneDeposits[i];
        }

        input[count++] = d.groundBlock is MulchBlock ? 1f : 0f;
        input[count++] = d.groundBlock is AcidicBlock ? 1f : 0f;
        input[count++] = d.groundBlock is NestBlock ? 1f : 0f;
        input[count++] = d.isQueen ? 1f : 0f;
        input[count++] = d.other == null ? 1f : 0f;
        input[count++] = (float)d.health / d.maxHealth;
        input[count++] = Random.Range(0f, 1f);

        for (int i = 0; i < layers.Length; i++) 
        {
            layers[i].Forwards();
        }

        //find most stimulated neuron
        for (int i = 1; i < (int)Ant.Action.size; i++) 
        {
            if (output[i] > best) 
            {
                best = output[i];
                action = i;
            }
        }

        //remember values for next time
        for (int i = (int)Ant.Action.size; i < output.Length; i++) 
        {
            input[count++] = output[i];
        }

        return (Ant.Action)action;
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

        public void InitRandom() 
        {
            for (int i = 0; i < outSize; i++) 
            {
                bias[i] = Random.Range(-1f, 1f);

                for (int j = 0; j < inSize; j++)
                {
                    weights[j, i] = Random.Range(-1f, 1f);
                }
            }
        }

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
