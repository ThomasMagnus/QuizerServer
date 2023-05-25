namespace Quizer.RequestBody
{
    public class AddWorkBody
    {
        public int teacherId { get; set; }
        public string? subject { get; set; }
        public string[]? groups { get; set; }
    }
}
