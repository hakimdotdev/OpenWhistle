using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Get max file size env
var maxFileSizeEnv = Environment.GetEnvironmentVariable("OPENWHISTLE_FILEUPLOAD_MAXFILESIZE");
var maxFileSize = maxFileSizeEnv != null
                      ? int.Parse(maxFileSizeEnv) * 1024 * 1024
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
