using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var jwtKeyEnv = Environment.GetEnvironmentVariable("OPENWHISTLE_JWT_KEY") ?? throw new InvalidOperationException("JWT_KEY_ENV");
var jwtIssuerEnv = Environment.GetEnvironmentVariable("OPENWHISTLE_ISSUER_KEY") ?? throw new InvalidOperationException("JWT_ISSUER_ENV");
var jwtAudienceEnv = Environment.GetEnvironmentVariable("OPENWHISTLE_AUDIENCE_KEY") ?? throw new InvalidOperationException("JWT_AUDIENCE_ENV");

builder.Services.AddAuthentication(options =>
                                   {
                                       options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                       options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                   })
    .AddJwtBearer(options =>
                  {
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuer = true,
                          ValidateAudience = true,
                          ValidateLifetime = true,
                          ValidateIssuerSigningKey = true,
                          ValidIssuer = jwtIssuerEnv,
                          ValidAudience = jwtAudienceEnv,
                          IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtKeyEnv))
                      };
                  });

builder.Services.AddAuthorization();

// Add services to the container.
// Get max file size env
var maxFileSizeEnv = Environment.GetEnvironmentVariable("OPENWHISTLE_FILEUPLOAD_MAXFILESIZE");
int maxFileSize = int.TryParse(maxFileSizeEnv, out var parsedValue) 
                      ? parsedValue * 1024 * 1024 
                      : 128 * 1024 * 1024;
// set form file upload length limit
builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = maxFileSize; });

var connectionString = Environment.GetEnvironmentVariable("OPENWHISTLE_SQLITE_CONNSTRING");
string dbHost = Environment.GetEnvironmentVariable("OPENWHISTLE_DB_HOST") ?? throw new InvalidOperationException();
string dbName = Environment.GetEnvironmentVariable("OPENWHISTLE_DB_DATABASE") ?? throw new InvalidOperationException();
string dbUser = Environment.GetEnvironmentVariable("OPENWHISTLE_DB_USERNAME") ?? throw new InvalidOperationException();
string dbPassword = Environment.GetEnvironmentVariable("OPENWHISTLE_DB_PASSWORD") ?? throw new InvalidOperationException();

var connString = $"server={dbHost};database={dbName};uid={dbUser};password={dbPassword}";
builder.Services.AddDbContextPool<OpenWhistleDbContext>(options => options.UseMySql(connString, ServerVersion.AutoDetect(connString)));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
                                                         {
                                                             options.User.RequireUniqueEmail = false;
                                                             
                                                             options.Password.RequireDigit = false;
                                                             options.Password.RequiredLength = 14;
                                                             options.Password.RequireNonAlphanumeric = true; 
                                                             options.Password.RequireUppercase = true;
                                                             options.Password.RequireLowercase = true;
                                                         })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<OpenWhistleDbContext>();

// builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
