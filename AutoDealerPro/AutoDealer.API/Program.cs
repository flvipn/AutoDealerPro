var builder = WebApplication.CreateBuilder(args);

// 1. SERVICII
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();          

var app = builder.Build();

// 2. Aici curge cererea
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run(); 