using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

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
            services.AddSwaggerGen(c =>
            {
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

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });



        }
    }
}
