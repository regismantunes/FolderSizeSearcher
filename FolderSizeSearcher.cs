using RA.Utilities.Extensions;
using RA.Utilities.Folder;
using RA.Utilities.Output;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;

namespace FolderSizeSearcher
{
    public class FolderSizeSearcher(IOutput output)
    {
        public IOutput Output { get; } = output;
        private FolderSearcher FolderSearcher { get; } = new FolderSearcher();

        public async Task SearchAsync(FolderSizeSearcherParameter parameter)
        {
            await SearchAsync(parameter.InitialPath, parameter.Taken);
        }

        public async Task SearchAsync(string initialPath, int taken)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Output.WriteLine("Executing folder size searcher.");
            Output.WriteLine("This process can take some minutes...");

            var folderSearcherOptions = new FolderSearcherOptions
            {
                FileSystemRights = FileSystemRights.Delete,
                InitialPath = initialPath,
                FindForFiles = true,
                FindForDirectories = true,
            };

            var foldersSize = new FolderSizeCollection();

            var taskSearcher = FolderSearcher.SearchAsync(folderSearcherOptions, path =>
            {
                if (Directory.Exists(path))
                    foldersSize.AddSize(path, 0);
                else
                {
                    var fileInfo = new FileInfo(path);
                    foldersSize.AddSize(fileInfo.DirectoryName, fileInfo.Length);
                }
            });

            Output.WriteLine(initialPath);
            while (FolderSearcher.IsRunning)
            {
                await taskSearcher.DelayOrCompleted(1000);
                
                Output.ClearLine();
                Output.WriteLine(FolderSearcher.LastPath);
            }

            Output.Clear();

            var result = foldersSize.GetFolderSizes()
                .OrderByDescending(x => x.FullSize)
                .Take(taken);

            foreach (var folder in result)
            {
                var size = (double)folder.FullSize;
                var sizeType = 0;
                while (size > 1024d)
                {
                    size /= 1024d;
                    sizeType++;
                    if (sizeType == 5)
                        break;
                }

                var sizeTypeStr = sizeType == 0 ? "B" :
                                  sizeType == 1 ? "KB" :
                                  sizeType == 2 ? "MB" :
                                  sizeType == 3 ? "GB" :
                                  sizeType == 4 ? "TB" : "PB";

                Output.WriteLine($"{folder.Path} Size: {size:N3}{sizeTypeStr}");
            }

            stopwatch.Stop();
            Output.WriteLine($"Time to execute: {stopwatch.GetElapsedTimeText()}");
        }
    }
}
