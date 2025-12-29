using IronZip;
using VaninChat2.Dto;

namespace VaninChat2.Workers
{
    public class ZipWorker
    {
        private readonly string _pass;

        public ZipWorker(string pass)
        {
            _pass = pass;
        }

        public FileObj CreateTxtFileAndPutToArchieve(string archieveName, string fileName, string content)
        {
            if (!archieveName.EndsWith(".zip"))
                archieveName = $"{archieveName}.zip";

            var result = new FileObj(archieveName);

            using (var archive = new IronZipArchive())
            {
                using (new FileWorker().CreateTxtFile(fileName, content))
                {
                    archive.Add(fileName);
                    archive.Encrypt(_pass, IronZip.Enum.EncryptionMethods.AES256);
                    archive.SaveAs(archieveName);
                }
            }

            return result;
        }
    }
}