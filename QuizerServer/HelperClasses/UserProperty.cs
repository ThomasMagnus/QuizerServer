using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuizerServer.HelperClasses
{
    public static class UserProperty
    {
        public static int? id { get; set; }
        public static string? firstname { get; set; }
        public static string? lastname { get; set; }
        public static string? group { get; set; }

    }

    public class UserPropertyCreator
    {
        private string? firstname, lastname, group;
        private int? id;

        public UserPropertyCreator(string? firstname, string? lastname, string? group, int? id)
        {
            this.firstname = firstname;
            this.lastname = lastname;
            this.group = group;
            this.id = id;
        }

        public void CreateUser()
        {
            UserProperty.firstname = firstname;
            UserProperty.lastname = lastname;
            UserProperty.group = group;
            UserProperty.id = id;
        }
    }
}
