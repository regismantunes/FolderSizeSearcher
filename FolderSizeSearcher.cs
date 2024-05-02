using FolderSizeSearcher.Extensions;
using FolderSizeSearcher.Output;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;

namespace FolderSizeSearcher
{
    public class FolderSizeSearcher
    {
        public IOutput Output { get; }

        public FolderSizeSearcher(IOutput output)
        {
            Output = output;
        }

        public void Search(FolderSizeSearcherParameter parameter)
        {
            Search(parameter.InitialPath, parameter.Taken);
        }

        public void Search(string initialPath, int taken)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Output.WriteLine("Executing folder size searcher.");
            Output.WriteLine("This process can take some minutes...");

            var foldersSize = new List<FolderSize>();
            var currentUser = GetCurrentSID();
            var countTask = 0;

            void getFolderSize(string path, FolderSize? parentFolder = null)
            {
                long totalSize = 0;

                try
                {
                    var files = Directory.GetFiles(path);
                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        var rules = fileInfo.GetAccessControl()
                                            .GetAccessRules(true, true, typeof(SecurityIdentifier));
                        foreach (FileSystemAccessRule rule in rules)
                        {
                            if (rule.IdentityReference.Value == currentUser &&
                                (rule.FileSystemRights & FileSystemRights.Delete) == FileSystemRights.Delete &&
                                rule.AccessControlType == AccessControlType.Allow)
                            {
                                totalSize += fileInfo.Length;
                                break;
                            }
                        }
                    }
                }
                catch { }

                var folderSize = parentFolder?.CreateSubFolder(path, totalSize) ?? new FolderSize(path, totalSize);

                getSubFoldersSize(folderSize);

                lock (foldersSize)
                    foldersSize.Add(folderSize);
            }

            void getSubFoldersSize(FolderSize parentFolder)
            {
                try
                {
                    var folders = Directory.GetDirectories(parentFolder.Path);
                    foreach (var folder in folders)
                    {
                        Interlocked.Increment(ref countTask);

                        Task.Run(() =>
                        {
                            getFolderSize(folder, parentFolder);
                            Interlocked.Decrement(ref countTask);
                        });
                    }
                }
                catch { }
            }

            getFolderSize(initialPath);

            Output.WriteLine(initialPath);
            while (true)
            {
                Thread.Sleep(1000);

                if (countTask == 0)
                    break;

                string lastPath;
                lock (foldersSize)
                    lastPath = foldersSize.Last().Path;

                Output.ClearLine();
                Output.WriteLine(lastPath);
            }

            Output.Clear();

            var result = foldersSize.OrderByDescending(x => x.FullSize)
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

        private static string GetCurrentSID()
        {
            return WindowsIdentity.GetCurrent().User?.Value ?? string.Empty;
        }
    }
}
