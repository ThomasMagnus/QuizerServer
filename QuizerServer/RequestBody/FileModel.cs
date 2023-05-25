using System.Text.Json.Serialization;

namespace Quizer.RequestBody
{
    public class FileModel
    {
        [JsonPropertyName("file")]
        public IFormFile? File { get; set; }

        [JsonPropertyName("subjectName")]
        public string? SubjectName { get; set; }

        [JsonPropertyName("groupName")] 
        public string? GroupName { get; set; }
    }
}
