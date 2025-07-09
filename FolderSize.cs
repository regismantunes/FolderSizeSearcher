namespace FolderSizeSearcher
{
    public class FolderSize(string path, long topOnlySize)
    {
        private FolderSize? _parentFolder;

        public string Path { get; } = path;
        public long TopOnlySize { get; } = topOnlySize;
        public long FullSize { get; private set; } = topOnlySize;

        private void SumSubFolderSize(long subFolderTopOnlySize)
        {
            FullSize += subFolderTopOnlySize;
            _parentFolder?.SumSubFolderSize(subFolderTopOnlySize);
        }

        public FolderSize CreateSubFolder(string path, long topOnlySize)
        {
            SumSubFolderSize(topOnlySize);
            return new FolderSize(path, topOnlySize) { _parentFolder = this };
        }
    }
}