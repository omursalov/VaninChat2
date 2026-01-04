namespace VaninChat2.Models
{
    public class DropboxAccessTokenInfo
    {
        public bool IsSuccess { get; }
        public string Error { get; }

        public DropboxAccessTokenInfo(string error = null)
        {
            IsSuccess = error == null;
            Error = error;
        }
    }
}
