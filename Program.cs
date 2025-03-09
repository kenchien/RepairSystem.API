using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RepairSystem.API.Data;
using RepairSystem.API.Models;
using RepairSystem.API.Services;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Any;
using Dapper;

// 創建Web應用程序構建器
var builder = WebApplication.CreateBuilder(args);

// 添加控制器和API瀏覽器服務
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 配置 Swagger
builder.Services.AddSwaggerGen(options =>
{
    // API 文檔基本信息
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "維修系統 API",
        Version = "v1",
        Description = "維修系統後端 API，提供設備維修管理的功能，包括用戶管理、報修單處理和設備管理。",
        Contact = new OpenApiContact
        {
            Name = "維修系統團隊",
            Email = "support@repairsystem.com",
            Url = new Uri("https://repairsystem.example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "MIT 許可證",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
    
    // 添加 JWT 認證配置
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT 授權標頭使用 Bearer 方案。輸入 'Bearer' [空格] 然後輸入您的令牌。例如：'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    
    // 啟用 XML 文檔
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    
    // 按控制器分組
    options.TagActionsBy(api => 
    {
        if (api.GroupName != null)
        {
            return new[] { api.GroupName };
        }
        
        var controllerName = api.ActionDescriptor.RouteValues["controller"];
        if (controllerName != null)
        {
            return new[] { controllerName };
        }
        
        return new[] { "其他" };
    });
    
    // 排序規則
    options.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.RelativePath}");
    
    // 自定義操作 ID 生成器
    options.CustomOperationIds(apiDesc =>
    {
        return apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null;
    });
    
    // 啟用枚舉描述
    options.SchemaFilter<EnumSchemaFilter>();
    
    // 添加全局請求頭
    options.OperationFilter<AddRequiredHeaderParameter>();
});

// 配置CORS策略
builder.Services.AddCors(options =>
{
    // 允許所有來源的請求
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

    // 允許特定來源的請求（前端應用）
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000") // 前端應用URL
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
});

// Configure Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 註冊接口與實現
builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddScoped<IRepairRepository, DapperRepairRepository>();

// 配置JWT身份驗證
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "DefaultKeyForDevelopment"))
        };
    });

// 配置服務選項
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection("FileStorageSettings"));

// 註冊服務實現，使用新的依賴注入方式
builder.Services.AddScoped<IAuthService, DapperAuthService>();
builder.Services.AddScoped<IRepairService, DapperRepairService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileService, DapperFileService>();
builder.Services.AddScoped<IEquipmentService, DapperEquipmentService>();

// 構建應用程序
var app = builder.Build();

// 配置HTTP請求管道
if (app.Environment.IsDevelopment())
{
    // 在開發環境中啟用Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "維修系統 API v1");
        c.RoutePrefix = string.Empty; // 將 Swagger UI 設置為應用程序根路徑
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // 折疊所有接口
        c.DefaultModelsExpandDepth(0); // 隱藏模型
    });
}
else
{
    // 在非開發環境中也啟用 Swagger，但使用不同的設置
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "維修系統 API v1");
        c.RoutePrefix = "api-docs"; // 在生產環境中使用不同的路徑
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.DefaultModelsExpandDepth(0);
    });
}

// 啟用HTTPS重定向
app.UseHttpsRedirection();
// 啟用靜態文件服務
app.UseStaticFiles();

// 配置SPA回退路由
app.MapFallbackToFile("/spa/{*path:nonfile}", "/spa/index.html");

// 啟用CORS策略
app.UseCors("AllowAll");

// 啟用身份驗證和授權
app.UseAuthentication();
app.UseAuthorization();

// 映射控制器路由
app.MapControllers();

// 啟動應用程序
app.Run();

// Enum Schema Filter for Swagger
public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            schema.Type = "string";
            schema.Format = null;
            
            foreach (var name in Enum.GetNames(context.Type))
            {
                schema.Enum.Add(new OpenApiString(name));
            }
        }
    }
}

// Add Required Headers to Swagger UI
public class AddRequiredHeaderParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();
            
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Accept-Language",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString("zh-TW")
            },
            Description = "指定語言，支持的值：en-US, zh-TW"
        });
    }
} 