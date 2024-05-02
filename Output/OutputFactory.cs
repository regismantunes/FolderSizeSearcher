namespace FolderSizeSearcher.Output
{
    public static class OutputFactory
    {
        public static IOutput GetOutput(OutputType type)
        {
            switch (type)
            {
                case OutputType.Console:
                    return new ConsoleOutput();
                default:
                    throw new ArgumentException("Invalid output type");
            }
        }
    }
}
