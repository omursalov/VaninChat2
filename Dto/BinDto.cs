namespace VaninChat2.Dto
{
    public class BinDto
    {
        public BinFileDto[] files { get; set; }
    }

    public class BinFileDto
    {
        public string filename { get; set; }
    }
}