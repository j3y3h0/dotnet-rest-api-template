namespace RestApiTemplate.Configurations.Settings
{
    public class Jwt
    {
        public string secretKey { get; set; }
        public int expirationMins { get; set; }
    }
}
