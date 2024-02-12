
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PrivateChat.Api.DBContext;
using PrivateChat.Api.Encryption;
using PrivateChat.Api.Entity.Models;
using PrivateChat.Api.JWT;
using PrivateChat.Api.Repositories;
using PrivateChat.Api.SignalR;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAppPolicy", policy =>
    {
        policy.WithOrigins(Configuration.GetSection("ClientAddress").Get<string[]>())
        .AllowAnyHeader()
        .WithMethods("get","post")
        //.AllowAnyMethod()
        .WithExposedHeaders("*");
    });
});

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddControllers();
builder.Services.AddDbContext<AdminContext>(options => options.UseSqlServer(Configuration.GetConnectionString("PrivateChatDb")));

builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 8;
    opt.User.RequireUniqueEmail = true;

}).AddEntityFrameworkStores<AdminContext>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["validIssuer"],
        ValidAudience = jwtSettings["validAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(jwtSettings.GetSection("securityKey").Value))
    };
});

builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
builder.Services.AddScoped<IChatRepository, SqlChatRepository>();
builder.Services.AddSingleton<ISymmetricEncryption, SymmetricEncryption>();
builder.Services.AddScoped<JwtHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors("MyAppPolicy");

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<SignalRHub>("/chatMessage");
});

app.Run();
