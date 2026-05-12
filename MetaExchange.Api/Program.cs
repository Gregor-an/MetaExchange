using System.Text.Json.Serialization;
using MetaExchange.Api.Configuration;
using MetaExchange.Core.Services;

namespace MetaExchange.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<ExchangeSettings>(builder.Configuration);
            builder.Services.AddSingleton<IExecutionPlanService, ExecutionPlanService>();

            builder.Services.AddControllers()
                .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.UseInlineDefinitionsForEnums();
                var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

                var coreXmlFile = "MetaExchange.Core.xml";
                var coreXmlPath = Path.Combine(AppContext.BaseDirectory, coreXmlFile);
                if (File.Exists(coreXmlPath))
                    options.IncludeXmlComments(coreXmlPath);
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
