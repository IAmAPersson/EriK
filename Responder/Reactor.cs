using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

public static class Reactor
{
    public static Dictionary<string, int[]> Reactions = new Dictionary<string, int[]>();

    public static void React(string Input)
    {
        Emotion.RelayEmotion(Reactions[Input][0], Reactions[Input][1], Reactions[Input][2], Reactions[Input][3]);
    }

    public static void Save()
    {
        string SaveString = JsonConvert.SerializeObject(Reactions);
        string Path = AppDomain.CurrentDomain.BaseDirectory + "\\Reactions.json";
        var Temp = File.Create(Path);
        Temp.Close();
        File.WriteAllText(Path, SaveString);
    }

    public static void Load()
    {
        try
        {
            Reactions = JsonConvert.DeserializeObject<Dictionary<string, int[]>>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\Reactions.json"));
        }
        catch
        {
            Reactions = new Dictionary<string, int[]>();
        }
    }
}