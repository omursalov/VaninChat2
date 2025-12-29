using System.Text;
using VaninChat2.Dto;

namespace VaninChat2.Workers
{
    public class FileWorker
    {
        public FileObj CreateTxtFile(string fileName, string content)
        {
            if (!fileName.EndsWith(".txt"))
                fileName = $"{fileName}.txt";

            var result = new FileObj(fileName);

            using (var fs = File.Create(fileName))
            {
                var title = Encoding.Unicode.GetBytes(content);
                fs.Write(title, 0, title.Length);
            }

            return result;
        }
    }
}