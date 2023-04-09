using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using SwiftCode.BBS.Common;
using SwiftCode.BBS.Repostories.EFContext;
using System.Text;

namespace SwiftCode.BBS.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            //注入访问配置类
            services.AddSingleton(new Appsettings(Configuration));
            services.AddSwaggerGen(c =>
            {
                //开启小锁
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                //在header中添加token,传递到后台
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权（数据将在请求头中进行传输）直接在下框中输入Bearer{token}(注意两者间是一个空格)/",
                    Name = "Authorization", //jwt默认的参数名称
                    In = ParameterLocation.Header, //jwt默认存放Authorization信息的位置（请求头中）
                    Type = SecuritySchemeType.ApiKey
                });
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SwiftCode.BBS.API",
                    Version = "v1",
                    Description = "框架文件说明",
                    Contact = new OpenApiContact
                    {
                        Email = "1120848233@qq.com",
                        Name = "SwiftCode",
                    }
                });
                var bashPath = AppContext.BaseDirectory;
                var xmlAPIPath = Path.Combine(bashPath, "SwiftCode.BBS.API.xml");
                c.IncludeXmlComments(xmlAPIPath, true);

                var xmlModelPath = Path.Combine(bashPath, "SwiftCode.BBS.Model.xml");
                c.IncludeXmlComments(xmlModelPath, true);


            });
            services.AddAuthentication(c =>
            {
                c.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                var audienceConfig = Configuration["Audience:Audience"];
                var symmertricKeyAsBase64 = Configuration["Audience:Secret"];
                var iss = Configuration["Audience:Issuer"];
                var keyByteArray = Encoding.ASCII.GetBytes(symmertricKeyAsBase64);
                var signingKey = new SymmetricSecurityKey(keyByteArray);
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = iss,
                    ValidateAudience = true,
                    ValidAudience = audienceConfig,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,//默认是7分钟，可以设置为0
                    RequireExpirationTime = true

                };

            });
            services.AddAuthorization(option =>
            {
                option.AddPolicy("Client", policy =>
                {
                    policy.RequireRole("Client").Build();
                });
                option.AddPolicy("Admin", policy =>
                {
                    policy.RequireRole("Admin").Build();
                });
                option.AddPolicy("SystemOrAdmin", policy =>
                {
                    policy.RequireRole("System", "Admin").Build();
                });
                option.AddPolicy("SystemAndAdmin", policy =>
                {
                    policy.RequireRole("Admin").RequireRole("System").Build();
                });


            });
            services.AddDbContext<SwiftCodeBBSContext>();
            services.AddControllers();
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                c.RoutePrefix = "";
            });


            app.UseRouting();
            //认证
            app.UseAuthentication();
            //授权
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });



        }
    }
}
