using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using RepoDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestApiTemplate.Configurations;
using System.Reflection;
using System.Text;

namespace RestApiTemplate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            //RepoDB ORM Init
            GlobalConfiguration.Setup().UseSqlServer(); // MS-SQL
            GlobalConfiguration.Setup().UseMySqlConnector();

            // RepoDB ORM > MS-SQL Server Settings
            SqlServerDbSetting dbSetting = new SqlServerDbSetting();
            bool isForce = true;
            DbSettingMapper.Add<System.Data.SqlClient.SqlConnection>(dbSetting, isForce);
            DbHelperMapper.Add<System.Data.SqlClient.SqlConnection>(new SqlServerDbHelper(), isForce);
            StatementBuilderMapper.Add<System.Data.SqlClient.SqlConnection>(new SqlServerStatementBuilder(dbSetting), isForce);
            builder.Services.AddControllers().AddNewtonsoftJson(); // WEB API 
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddControllersWithViews();

            #region Singleton
            builder.Services.AddSingleton(
                new AppSettings(builder.Configuration)
            );

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddHttpContextAccessor();
            #endregion

            #region Swagger
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET Core REST API Template v1",
                    Description = "���Core REST API ���ø� Swagger UI"
                });

                // Swagger Header ����
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "/api/auth/login API�� ���� ��ū�߱�, JWT ������� Bearer ��� (Example: 'Bearer 12345abcdef')",
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

                //swagger ui ���� Read
                string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            #endregion

            //API JWT ������� ����
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // �߱��� ����
                    ValidateAudience = false, // ������ ����
                    ValidateLifetime = true, // ��ū ���� ����
                    ValidateIssuerSigningKey = true, // �߱��� ���� Ű ����
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.jwtOption.secretKey)), // �߱��� ���� Ű ����
                    ClockSkew = TimeSpan.Zero // ��ū �߱� �ð��� �����ý��� �ð� ��ġ���� Ȯ��
                };
            });

            WebApplication app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger"; //������Ʈ swagger BASE_URL ����
            });

            app.UseRouting();
            app.UseStaticFiles();
            app.UseHttpsRedirection();//HTTP -> HTTPS
            app.UseAuthentication(); //������� ���
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}