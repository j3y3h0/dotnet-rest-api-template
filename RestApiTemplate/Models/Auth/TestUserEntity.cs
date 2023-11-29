using RepoDb.Attributes.Parameter;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestApiTemplate.Models.Auth
{
    [Table("t_uesr")]
    public class TestUserEntity
    {

        [Name("id")]
        public int id { get; set; }
        [Name("username")]
        public string username { get; set; }
        [Name("password")]
        public string password { get; set; }
        [Name("name")]
        public string name { get; set; }
    }
}
