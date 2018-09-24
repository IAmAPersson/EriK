using System;

class EntryClass
{
    public static void Main()
    {
        KnowledgeHandler.Load();
        Reactor.Load();
        while (true)
        {
            string Input = Console.ReadLine();
            string Response = KnowledgeHandler.GetResponse(Input);
            Console.WriteLine(Response);
        }
    }
}