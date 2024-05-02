namespace FolderSizeSearcher
{
    public class FolderSize
    {
        private FolderSize? _parentFolder;

        public FolderSize(string path, long topOnlySize)
        {
            Path = path;
            TopOnlySize = topOnlySize;
            FullSize = topOnlySize;
        }

        public string Path { get; }
        public long TopOnlySize { get; }
        public long FullSize { get; private set; }

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