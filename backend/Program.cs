using Supabase;
using TodoApi.Application.Services;
using TodoApi.Domain.Repositories;
using TodoApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORSの設定（フロントエンドからのリクエストを許可）
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // フロントエンドのURL
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Supabaseクライアントの設定
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];

if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
{
    Console.WriteLine("警告: Supabase接続情報が設定されていません。appsettings.jsonを確認してください。");
}

var options = new SupabaseOptions
{
    AutoRefreshToken = true,
    AutoConnectRealtime = true
};

var supabaseClient = new Client(supabaseUrl, supabaseKey, options);

// 依存関係の登録
builder.Services.AddSingleton<Client>(supabaseClient);
builder.Services.AddScoped<ITodoRepository, SupabaseTodoRepository>();
builder.Services.AddScoped<TodoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();

app.MapControllers();

app.Run();