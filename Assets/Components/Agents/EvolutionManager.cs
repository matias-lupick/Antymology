using UnityEngine;

public class EvolutionManager : Singleton<EvolutionManager>
{
    public Brain[] inTrial;

    int generationSize = 12;
    int onTrial = -1;
    int generationCount = 0;

    [SerializeField] float probability = 0.01f;
    [SerializeField] float additive = 0.2f;
    [SerializeField] float multiplicative = 0.3f;

    [SerializeField] bool logResults = true;

    [SerializeField] string loadTrait = "";
    [SerializeField] string saveTrait = "";
    [SerializeField] bool saveAtEnd = false;

    void Start()
    {
        //scores = new int[generationSize];
        inTrial = new Brain[generationSize];

        if (loadTrait == "")
        {
            for (int i = 0; i < generationSize; i++)
            {
                inTrial[i] = new Brain();
            }
        }
        else 
        {
            for (int i = 0; i < generationSize; i++)
            {
                inTrial[i] = new Brain(loadTrait);
            }
        }
    }

    public void ReportScore(int score) 
    {
        inTrial[onTrial].score = score;
    }

    public void Next() 
    {
        onTrial += 1;

        if (onTrial == generationSize) 
        {
            NextGeneration();
            onTrial = 0;
        }

        AntManager.Instance.SetBrain(inTrial[onTrial]);
    }

    protected void NextGeneration() 
    {
        int high;
        int highInd;

        Brain[] nextGen = new Brain[generationSize];

        generationCount += 1;

        for (int i = 0; i < generationSize / 3; i++) 
        {
            high = -1;
            highInd = 0;

            for (int j = 0; j < generationSize; j++) 
            {
                if (inTrial[j] == null)
                    continue;

                if (inTrial[j].score > high) 
                {
                    high = inTrial[j].score;
                    highInd = j;
                }
            }

            if (i == 0) 
            {
                if (logResults)
                {
                    Debug.Log("End of Generation " + generationCount);
                    Debug.Log("Best: " + high);
                }

                if (saveAtEnd) 
                {
                    inTrial[highInd].Save(saveTrait);
                    saveAtEnd = false;
                }
            }

            inTrial[highInd].Reset();
            nextGen[i * 3] = inTrial[highInd].Clone();
            nextGen[i * 3 + 1] = inTrial[highInd].Clone();
            nextGen[i * 3 + 1].Mutate(probability, additive, multiplicative);
            nextGen[i * 3 + 2] = inTrial[highInd].Clone();
            nextGen[i * 3 + 2].Mutate(probability, additive, multiplicative);

            //nextGen[i * 2].TestUnique(nextGen[i * 2 + 1]);


            inTrial[highInd] = null;
            
        }

        inTrial = nextGen;

        if (ConfigurationManager.Instance.progressSeed)
            ConfigurationManager.Instance.Seed += 1;
    }
}
