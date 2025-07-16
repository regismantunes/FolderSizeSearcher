using RA.Utilities.Output;

namespace FolderSizeSearcher
{
    public class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var parameter = FolderSizeSearcherParameterFactory.GetParameter(args);

                var output = OutputFactory.GetOutput(OutputType.Console);

                new FolderSizeSearcher(output)
                    .SearchAsync(parameter)
                    .Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine($"Press any key to exit.");

                Console.Read();
            }
        }
    }
}