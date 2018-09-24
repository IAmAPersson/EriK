using System;
using System.Collections.Generic;

public class Probil
{
    public List<Tuple<string, int>>[] __Responses;
    private Tuple<int, int> ID;

    public Probil()
    {
        __Responses = new List<Tuple<string, int>>[4];
        for (int i = 0; i < 4; i++)
            __Responses[i] = new List<Tuple<string, int>>();
        ID = new Tuple<int, int>(0, 0);
    }

    public Probil(string Happy, string Sad, string Afraid, string Angry)
    {
        __Responses = new List<Tuple<string, int>>[] { new List<Tuple<string, int>>() { new Tuple<string, int>(Happy, 1) },
                                                       new List<Tuple<string, int>>() { new Tuple<string, int>(Sad, 1) },
                                                       new List<Tuple<string, int>>() { new Tuple<string, int>(Afraid, 1) },
                                                       new List<Tuple<string, int>>() { new Tuple<string, int>(Angry, 1) }, };
        ID = new Tuple<int, int>(0, 0);
    }

    private int MaxArg(params double[] Arguments)
    {
        int Maximum = 0;

        for (int i = 0; i < Arguments.Length; i++)
            if (Arguments[i] > Arguments[Maximum]) Maximum = i;

        return Maximum;
    }

    private double[] GetPercentages(List<Tuple<string, int>> InspectFrom)
    {
        double[] Output = new double[InspectFrom.Count];
        for (int i = 0; i < Output.Length; i++)
            Output[i] = InspectFrom[i].Item2 / (double)GetSum(InspectFrom);
        return Output;
    }

    private int GetSum(List<Tuple<string, int>> InspectFrom)
    {
        int Partial = 0;
        for (int i = 0; i < InspectFrom.Count; i++)
            Partial += InspectFrom[i].Item2;
        return Partial;
    }

    public string Get()
    {
        double Happy = Emotion.Happiness;
        double Sad = Emotion.Sadness;
        double Afraid = Emotion.Afraidness;
        double Angry = Emotion.Anger;
        
        var InspectFrom = __Responses[MaxArg(Happy, Sad, Afraid, Angry)];
        double Probability = new Random().NextDouble();
        double[] Percentages = GetPercentages(InspectFrom);
        double Temp = 0;

        for (int i = 0; i < Percentages.Length; i++)
        {
            Temp += Percentages[i];
            if (Probability < Temp)
            {
                ID = new Tuple<int, int>(MaxArg(Happy, Sad, Afraid, Angry), i);
                return InspectFrom[i].Item1;
            }
        }
        throw new Exception("Phil why"); //Tell Phil he fucked up again
    }

    public void Reinforce()
    {
        __Responses[ID.Item1][ID.Item2] = new Tuple<string, int>(__Responses[ID.Item1][ID.Item2].Item1, __Responses[ID.Item1][ID.Item2].Item2 + 1);
    }

    public void Discourage()
    {
        if ((__Responses[ID.Item1][ID.Item2].Item2 - 1) > 0)
            __Responses[ID.Item1][ID.Item2] = new Tuple<string, int>(__Responses[ID.Item1][ID.Item2].Item1, __Responses[ID.Item1][ID.Item2].Item2 - 1);
    }

    public void Add(string Response)
    {
        __Responses[ID.Item1].Add(new Tuple<string, int>(Response, 1));
    }
}