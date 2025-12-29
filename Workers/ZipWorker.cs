using IronZip;
using System.Text;
using VaninChat2.Dto;

namespace VaninChat2.Workers
{
    public class ZipWorker
    {
        public ZipData CreateTxtFileAndPutToArchieve(string archieveName, string fileName, string content)
        {
            using (var archive = new IronZipArchive())
            {
                CreateTxtFile(fileName, content);
                archive.Add(fileName);
                archive.SaveAs(archieveName);
                File.Delete(fileName);
                archive.Encrypt(archieveName, IronZip.Enum.EncryptionMethods.AES256);
            }
        }

        private void CreateTxtFile(string fileName, string content)
        {
            using (var fs = File.Create(fileName))
            {
                var title = Encoding.Unicode.GetBytes(content);
                fs.Write(title, 0, title.Length);
            }
        }
    }
}