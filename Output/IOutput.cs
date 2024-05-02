namespace FolderSizeSearcher.Output
{
    public interface IOutput
    {
        public void WriteLine(string message);
        public void Clear();
        public void ClearLine();
    }
}
