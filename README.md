# mongo-gridfs-poc
Example how to use MongoDB GridFS with .NET to upload and download files.

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017"
  },
  "MongoDatabase": "yourDatabaseName"
}
```

### User Secrets (optional)
In DEBUG mode, you can use User Secrets to store sensitive credentials.

```shell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:MongoDB" "mongodb://username:password@host:port"
```

## How to Use
1. Place the file you want to upload in the project directory (e.g., test_file.txt)
2. Run the project:
    ```shell
    dotnet run
    ```
3. The file will be uploaded to MongoDB and downloaded as downloaded_test_file.txt