public static class Emotion
{
    private static double Happy = 5;
    private static double Sad = 1;
    private static double Afraid = 0;
    private static double Angry = 1;

    public static double Happiness
    {
        get
        {
            return Happy;
        }
    }
    public static double Sadness
    {
        get
        {
            return Sad;
        }
    }
    public static double Afraidness
    {
        get
        {
            return Afraid;
        }
    }
    public static double Anger
    {
        get
        {
            return Angry;
        }
    }

    public static void RelayEmotion(int Happy, int Sad, int Afraid, int Angry)
    {
        Emotion.Happy += Emotion.Happy >= 1 ? Happy : 0;
        Emotion.Sad += Emotion.Sad >= 1 ? Sad : 0;
        Emotion.Afraid += Emotion.Afraid >= 1 ? Afraid : 0;
        Emotion.Angry += Emotion.Angry >= 1 ? Angry : 0;
    }
}