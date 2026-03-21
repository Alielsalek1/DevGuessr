namespace Tests.Common.TestContainerDependencies;

public static class SeqProvider
{
    public static void SetSeqUrl(string url)
    {
        Environment.SetEnvironmentVariable("SEQ_URL", url);
    }
}
