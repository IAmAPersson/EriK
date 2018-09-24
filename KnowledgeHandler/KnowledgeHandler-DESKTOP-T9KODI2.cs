using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

public static class KnowledgeHandler
{
    public static Dictionary<string, Probil> Knowledge = new Dictionary<string, Probil>();
    private static string LastInput = "";
    private static string LastInputRaw = "";
    private static string LastOutput = "";

    public static void Save()
    {
        string SaveString = JsonConvert.SerializeObject(Knowledge);
        string Path = AppDomain.CurrentDomain.BaseDirectory + "\\Knowledge.json";
        var Temp = File.Create(Path);
        Temp.Close();
        File.WriteAllText(Path, SaveString);
    }

    public static void Load()
    {
        try
        {
            Knowledge = JsonConvert.DeserializeObject<Dictionary<string, Probil>>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\Knowledge.json"));
        }
        catch
        {
            Knowledge = new Dictionary<string, Probil>();
        }
    }

    private static string Command(string Input)
    {
        switch (Input)
        {
            case "help":
                return "!good - Reinforce last response\n!bad - Discourage last song\n!save - Save current knowledge databanks to .json\n!add - Add a new query/response set";
            case "good":
                if (DLDistance(LastInput, LastInputRaw) < 2)
                {
                    Knowledge[LastInput].Reinforce();
#if DEBUG
                    Console.WriteLine("DEBUG: Reinforced \"" + LastOutput + "\" as a response to \"" + LastInput + "\"");
#endif
                }
                else
                {
                    Knowledge.Add(LastInputRaw, new Probil(LastOutput, LastOutput, LastOutput, LastOutput));
                    Console.WriteLine("In order from Happy, Sad, Afraid, to Angry, how should I emotionally respond to \"" + LastInputRaw + "\"? (Ideally -3 to +3)");
                    int[] Reactions_ = new int[4];
                    for (int i = 0; i < 4; i++)
                        Reactions_[i] = Convert.ToInt32(Console.ReadLine());
                    Reactor.Reactions.Add(LastInputRaw, Reactions_);
#if DEBUG
                    Console.WriteLine("DEBUG: Created new link between \"" + LastInputRaw + "\" and \"" + LastOutput + "\"");
#endif
                }
                return "Reinforcement successful.";
            case "bad":
                if (DLDistance(LastInput, LastInputRaw) < 2)
                {
                    Knowledge[LastInput].Discourage();
                }
                else
                {
                    Console.WriteLine("In order from Happy, Sad, Afraid, to Angry, how should I emotionally respond to \"" + LastInputRaw + "\"? (Ideally -3 to +3)");
                    int[] Reactions_ = new int[4];
                    for (int i = 0; i < 4; i++)
                        Reactions_[i] = Convert.ToInt32(Console.ReadLine());
                    Reactor.Reactions.Add(LastInputRaw, Reactions_);
                    Console.WriteLine("In order from Happy, Sad, Afraid, to Angry, please tell me some appropriate responses to \"" + LastInputRaw + "\".");
                    string[] Knowledges_ = new string[4];
                    for (int i = 0; i < 4; i++)
                        Knowledges_[i] = Console.ReadLine();
                    Knowledge.Add(LastInputRaw, new Probil(Knowledges_[0], Knowledges_[1], Knowledges_[2], Knowledges_[3]));
                }
                return "Discouragement successful.";
            case "save":
                Save();
                Reactor.Save();
                return "Saved.";
            case "add":
                Console.WriteLine("To what query do you want to add on to?");
                string TheInput = Console.ReadLine();
                Console.WriteLine("What new output do you want to assign?");
                string NewOutput = Console.ReadLine();
                Knowledge[TheInput].Add(NewOutput);
                return "Addition successful.";
            case "feelings":
                return "Happiness: " + Emotion.Happiness + "\nSadness: " + Emotion.Sadness + "\nAfraidness: " + Emotion.Afraidness + "\nAnger: " + Emotion.Anger;
            case "new":
                Console.WriteLine("What new phrase do you want to recognize?");
                string Phrase = Console.ReadLine();
                Console.WriteLine("In order from Happy, Sad, Afraid, to Angry, how should I emotionally respond to this? (Ideally -3 to +3)");
                int[] Reactions = new int[4];
                for (int i = 0; i < 4; i++)
                    Reactions[i] = Convert.ToInt32(Console.ReadLine());
                Reactor.Reactions.Add(Phrase, Reactions);
                Console.WriteLine("In order from Happy, Sad, Afraid, to Angry, please tell me some appropriate responses.");
                string[] Knowledges = new string[4];
                for (int i = 0; i < 4; i++)
                    Knowledges[i] = Console.ReadLine();
                Knowledge.Add(Phrase, new Probil(Knowledges[0], Knowledges[1], Knowledges[2], Knowledges[3]));
                return "New response successful.";
            default:
                return "Unknown command '" + Input + "'";
        }
    }

    public static string GetResponse(string Input)
    {
        if (Input[0] == '!')
            return Command(Input.Substring(1));
        LastInputRaw = Input;
        Tuple<int, string> Shortest = new Tuple<int, string>(int.MaxValue, "");
        foreach (string s in Knowledge.Keys.ToArray())
        {
            if (DLDistance(s, LastOutput) < 3)
            {
                try
                {
                    Knowledge[LastInput].Add(Input);
#if DEBUG
                    Console.WriteLine("DEBUG: Associated \"" + Input + "\" as a potential response to \"" + LastOutput + "\"");
#endif
                }
                catch { }
            }
            int Distance = DLDistance(s, Input);
            if (Distance < Shortest.Item1)
                Shortest = new Tuple<int, string>(Distance, s);
        }
#if DEBUG
        Console.WriteLine("DEBUG: Assuming input '" + Shortest.Item2 + "' with a distance of " + Shortest.Item1);
#endif
        Input = Shortest.Item2;
        LastInput = Input;
        Reactor.React(Input);
        return LastOutput = Knowledge[Input].Get();
    }

    private static int DLDistance(string Original, string Modified)
    {
        //Not my algorithm, I used someone else's under the terms of their license.
        //Original: https://gist.github.com/wickedshimmy/449595/cb33c2d0369551d1aa5b6ff5e6a802e21ba4ad5c

        int len_orig = Original.Length;
        int len_diff = Modified.Length;

        var matrix = new int[len_orig + 1, len_diff + 1];
        for (int i = 0; i <= len_orig; i++)
            matrix[i, 0] = i;
        for (int j = 0; j <= len_diff; j++)
            matrix[0, j] = j;

        for (int i = 1; i <= len_orig; i++)
        {
            for (int j = 1; j <= len_diff; j++)
            {
                int cost = Modified[j - 1] == Original[i - 1] ? 0 : 1;
                var vals = new int[] {
                matrix[i - 1, j] + 1,
                matrix[i, j - 1] + 1,
                matrix[i - 1, j - 1] + cost
            };
                matrix[i, j] = vals.Min();
                if (i > 1 && j > 1 && Original[i - 1] == Modified[j - 2] && Original[i - 2] == Modified[j - 1])
                    matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + cost);
            }
        }
        return matrix[len_orig, len_diff];
    }
}