using RestApiTemplate.Configurations.Settings;

namespace RestApiTemplate.Configurations
{
    public class AppSettings
    {
        #region Private
        private static string _appMode;
        private static Connections _connections;
        private static Jwt _jsonWebToken;
        #endregion

        #region Public
        public static string MSSQL_DB_CONNECTION;
        public static string MYSQL_DB_CONNECTION;
        #endregion

        #region GET
        public static Jwt jwtOption
        {
            get
            {
                return _jsonWebToken;
            }
        }

        #endregion

        #region SET
        /// <summary>
        /// appsettings의 Mode에 따라 CONNECTION 설정
        /// </summary>
        private static void SetServerMode()
        {
            // MS-SQL
            MSSQL_DB_CONNECTION = _appMode.ToUpper() switch
            {
                "REAL" => _connections.MSSQL_DB_CONNECTION_REAL,
                "TEST" => _connections.MSSQL_DB_CONNECTION_TEST,
                _ => throw new NotImplementedException(),
            };

            // MySQL, MariaDB
            MYSQL_DB_CONNECTION = _appMode.ToUpper() switch
            {
                "REAL" => _connections.MYSQL_DB_CONNECTION_REAL,
                "TEST" => _connections.MYSQL_DB_CONNECTION_TEST,
                _ => throw new NotImplementedException(),
            };
        }
        #endregion

        //SET appsettings.json
        public AppSettings(IConfigurationManager appsettings)
        {
            _appMode = appsettings.GetSection("Mode").Get<string>();
            _connections = appsettings.GetSection("Connection").Get<Connections>();
            _jsonWebToken = appsettings.GetSection("Jwt").Get<Jwt>();

            SetServerMode();
        }
    }
}
