using UnityEngine;
using Antymology.Terrain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Brain
{
    public float[] input;
    public BrainLayer[] layers;
    public float[] output;


    /// <summary>
    /// number of values to route form output back into input
    /// </summary>
    int memorySize = 5;

    /// <summary>
    /// The score the ants using this brain recieved
    /// </summary>
    public int score = -1;

    int trueInputSize = 9;

    public Brain() 
    {
        //3 for block type, 1 for random, 1 for hunger,
        //1 for is queen, 1 for neighbor, 1 for cliff, 1 for wall
        input = new float[memorySize + (int)PheromoneType.size + trueInputSize];
        output = new float[(int)Ant.Action.size + memorySize];

        layers = new BrainLayer[1];
        layers[0] = new BrainLayer(input, output);
        layers[0].InitRandom();
    }

    public Brain(Brain template) 
    {
        input = new float[memorySize + (int)PheromoneType.size + trueInputSize];
        output = new float[(int)Ant.Action.size + memorySize];

        layers = new BrainLayer[template.layers.Length];
        for (int i = 0; i < template.layers.Length; i++) 
        {
            layers[i] = new BrainLayer(template.layers[i]);
        }

        layers[0].input = input;
        layers[layers.Length - 1].output = output;
    }

    public Brain(string path) 
    {
        SBrainLayer layer = SBrainLayer.Load(path);

        input = new float[memorySize + (int)PheromoneType.size + trueInputSize];
        output = new float[(int)Ant.Action.size + memorySize];

        layers = new BrainLayer[1];
        layers[0] = new BrainLayer(input, output);
        layers[0].weights = layer.weights;
        layers[0].bias = layer.bias;
        //layers[0].InitRandom();
    }

    public Brain Clone() 
    {
        Brain v = new Brain(this);

        for (int i = 0; i < layers.Length; i++)
            v.layers[i] = layers[i].Clone();
        v.layers[0].input = v.input;
        v.layers[layers.Length - 1].output = v.output;

        return v;
    }

    public void Save(string path) 
    {
        new SBrainLayer(layers[0]).Save(path);
        Debug.Log("Saved to: " + path);
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
        input[count++] = d.IsCliff() ? 1f : 0f;
        input[count++] = d.IsOpen() ? 1f : 0f;
        input[count++] = (float)d.health / d.maxHealth;
        input[count++] = UnityEngine.Random.Range(0f, 1f);

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
                //
            }
        }

        //remember values for next time
        for (int i = (int)Ant.Action.size; i < output.Length; i++) 
        {
            input[count++] = output[i];
        }

        return (Ant.Action)action;
    }

    public void Reset() 
    {
        for (int i = input.Length - 1; i >= input.Length - 1 - (int)Ant.Action.size; i--)
            input[i] = 0f;
        score = 0;
    }


    public void Mutate(float probability, float additive, float multiplicative) 
    {
        foreach (BrainLayer v in layers) {
            v.Mutate(probability, additive, multiplicative); 
        }
    }


    public void TestUnique(Brain v) 
    {
        if (input == v.input)
            Debug.Log("Sim: " + 1);
        if (output == v.output)
            Debug.Log("Sim: " + 2);
        if (layers[0].input == v.layers[0].input)
            Debug.Log("Sim: " + 3);
        if (layers[layers.Length - 1].output == v.layers[v.layers.Length - 1].output)
            Debug.Log("Sim: " + 4);
        if (layers[0].weights == v.layers[0].weights)
            Debug.Log("Sim: " + 5);
        if (layers[0].bias == v.layers[0].bias)
            Debug.Log("Sim: " + 6);
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

        public BrainLayer(BrainLayer template)
        {
            input = template.input;
            output = template.output;

            inSize = input.Length;
            outSize = output.Length;

            weights = template.weights;
            bias = template.bias;
        }

        public void InitRandom() 
        {
            for (int i = 0; i < outSize; i++) 
            {
                bias[i] = UnityEngine.Random.Range(0f, 0.01f);

                for (int j = 0; j < inSize; j++)
                {
                    weights[j, i] = UnityEngine.Random.Range(0f, 0.01f);
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

        public BrainLayer Clone() 
        {
            BrainLayer v = new BrainLayer(input, output);

            for (int i = 0; i < outSize; i++)
            {
                v.bias[i] = bias[i];

                for (int j = 0; j < inSize; j++)
                {
                    v.weights[j, i] = weights[j, i];
                }
            }

            return v;
        }


        public void Mutate(float probability, float add, float mult)
        {
            for (int i = 0; i < outSize; i++)
            {
                if (UnityEngine.Random.Range(0f, 1f) < probability)
                    bias[i] = bias[i] * (1f + UnityEngine.Random.Range(-mult, mult)) +
                        UnityEngine.Random.Range(-add, add);

                for (int j = 0; j < inSize; j++)
                {
                    weights[j, i] = weights[j, i] * (1f + UnityEngine.Random.Range(-mult, mult)) +
                        UnityEngine.Random.Range(-add, add);
                }
            }
        }
    }

    [System.Serializable]
    public class SBrainLayer 
    {
        public float[,] weights;
        public float[] bias;

        public SBrainLayer(BrainLayer a) 
        {
            weights = a.weights;
            bias = a.bias;
        }

        public void Save(string path) 
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(FullPath(path));
            bf.Serialize(file, this);
            file.Close();
        }

        public static SBrainLayer Load(string path)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(FullPath(path), FileMode.Open);
            SBrainLayer data = (SBrainLayer)bf.Deserialize(file);
            file.Close();

            return data;
        }

        private static string FullPath(string path) 
        {
            return Application.dataPath + "/" + "Brain" + "/" + path;
        }
    }
}
