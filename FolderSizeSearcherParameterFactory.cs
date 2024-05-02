namespace FolderSizeSearcher
{
    public static class FolderSizeSearcherParameterFactory
    {
        public static FolderSizeSearcherParameter GetParameter(string[] args)
        {
            var parameter = new FolderSizeSearcherParameter()
            {
                InitialPath = string.Empty,
                Taken = -1,
            };

            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-p":
                        if (i + 1 < args.Length)
                        {
                            parameter.InitialPath = args[i + 1];

                            if (!Directory.Exists(parameter.InitialPath))
                                throw new ArgumentException("Error: Invalid folder path");

                            i++;
                        }
                        else
                            throw new ArgumentException("Error: Missing folder path after -p");
                        break;
                    case "-t":
                        if (i + 1 < args.Length)
                        {
                            if (int.TryParse(args[i + 1], out int value))
                            {
                                if (value <= 0)
                                    throw new ArgumentException("Error: Number of folders need be greater then zero");

                                parameter.Taken = value;
                                i++;
                            }
                            else
                                throw new ArgumentException("Error: Invalid number of folders after -t");
                        }
                        else
                            throw new ArgumentException("Error: Missing number of folders after -t");
                        break;
                    default:
                        throw new ArgumentException($"Error: Unrecognized parameter {args[i]}");
                }
            }

            if (parameter.InitialPath == string.Empty)
                parameter.InitialPath = @"C:\";

            if (parameter.Taken == -1)
                parameter.Taken = 20;

            return parameter;
        }
    }
}
