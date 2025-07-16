namespace FolderSizeSearcher
{
    public class FolderSizeCollection
    {
        private readonly Dictionary<string, long> _folderSizes = [];
        private readonly object _lock = new();

        public void AddSize(string folderPath, long size)
        {
            lock (_lock)
            {
                if (!_folderSizes.TryAdd(folderPath, size))
                    _folderSizes[folderPath] += size;
            }
        }

        public IEnumerable<FolderSize> GetFolderSizes()
        {
            var folderSizes = new Dictionary<string, FolderSize>();
            foreach (var folderPath in _folderSizes.Keys.Order())
            {
                var folderSize = _folderSizes[folderPath];
                var folderInfo = new DirectoryInfo(folderPath);
                
                var parentFolder = folderInfo.Parent?.FullName;
                if (parentFolder != null &&
                    folderSizes.TryGetValue(parentFolder, out var parentFolderSize))
                {
                    folderSizes.Add(folderPath,
                        parentFolderSize.CreateSubFolder(folderPath, folderSize)
                    );
                }
                else
                {
                    folderSizes.Add(folderPath,
                        new FolderSize(folderPath, folderSize)
                    );
                }
            }

            return folderSizes.Values;
        }
    }
}
