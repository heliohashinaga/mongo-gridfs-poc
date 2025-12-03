using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using MongoDB.Driver.GridFS;

Console.WriteLine("MongoGridFS started");

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

#if DEBUG
builder.Configuration.AddUserSecrets(typeof(Program).Assembly, true);
#endif

var connectionString = builder.Configuration.GetConnectionString("MongoDB");
var database = builder.Configuration.GetValue<string>("MongoDatabase");

var settings = MongoClientSettings.FromConnectionString(connectionString);
var options = new InstrumentationOptions { CaptureCommandText = true };
settings.ClusterConfigurator = cc => cc.Subscribe(new DiagnosticsActivityEventSubscriber(options));
var mongoClient = new MongoClient(settings);
var mongoDatabase = mongoClient.GetDatabase(database);
var gridFS = new GridFSBucket(mongoDatabase);

var fileName = "test_file.txt";

ObjectId id;

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".csv"] = "text/csv";
provider.Mappings[".zip"] = "application/zip";
provider.TryGetContentType(fileName, out var contentType);

using (var stream = File.OpenRead(fileName))
{
    id = await gridFS.UploadFromStreamAsync(
        Path.GetFileName(fileName), 
        stream,
        new GridFSUploadOptions
        {
            Metadata = new BsonDocument
            {
                { "contentType", contentType }
            }
        });
    Console.WriteLine($"File uploaded with id: {id}");
}

await gridFS.DownloadToStreamAsync(id, File.OpenWrite("downloaded_test_file.txt"));

Console.WriteLine("MongoGridFS finished");
