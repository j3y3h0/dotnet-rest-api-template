using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepoDb;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using RestApiTemplate.Configurations;
using RestApiTemplate.Configurations.Settings;
using System.Reflection;
using System.Text;

namespace RestApiTemplate
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //RepoDB ORM Init
            GlobalConfiguration.Setup().UseSqlServer(); // MS-SQL
            GlobalConfiguration.Setup().UseMySqlConnector();

            // RepoDB ORM > MS-SQL Server Settings
            SqlServerDbSetting dbSetting = new SqlServerDbSetting();
            bool isForce = true;
            DbSettingMapper.Add<System.Data.SqlClient.SqlConnection>(dbSetting, isForce);
            DbHelperMapper.Add<System.Data.SqlClient.SqlConnection>(new SqlServerDbHelper(), isForce);
            StatementBuilderMapper.Add<System.Data.SqlClient.SqlConnection>(new SqlServerStatementBuilder(dbSetting), isForce);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(); // WEB API 
            services.AddEndpointsApiExplorer();
            services.AddControllersWithViews();

            #region Singleton
            services.AddSingleton(
                new AppSettings(
                    Configuration.GetSection("Mode").Get<string>(),
                    Configuration.GetSection("Connection").Get<Connections>(),
                    Configuration.GetSection("Jwt").Get<Jwt>()
                )
            );

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();
            #endregion

            #region Swagger
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET 7.0 REST API Template v1",
                    Description = "닷넷7.0 REST API 템플릿 Swagger UI"
                });

                // Swagger Header 설정
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "/api/auth/login API를 통해 토큰발급, JWT 인증헤더 Bearer 사용 (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                //swagger ui 파일 Read
                string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            #endregion

            //API JWT 인증모듈 적용
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // 발급자 검증
                    ValidateAudience = false, // 수신자 검증
                    ValidateLifetime = true, // 토큰 수명 검증
                    ValidateIssuerSigningKey = true, // 발급자 서명 키 검증
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.jwtOption.secretKey)), // 발급자 서명 키 지정
                    ClockSkew = TimeSpan.Zero // 토큰 발급 시간과 서버시스템 시간 일치여부 확인
                };
            });
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger"; //프로젝트 swagger BASE_URL 지정
            });

            app.UseRouting();
            app.UseStaticFiles();
            app.UseHttpsRedirection();//HTTP -> HTTPS
            app.UseAuthentication(); //인증모듈 허용
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
