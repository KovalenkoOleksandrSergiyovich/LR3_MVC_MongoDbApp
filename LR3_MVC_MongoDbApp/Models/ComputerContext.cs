using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace LR3_MVC_MongoDbApp.Models
{
    public class ComputerContext
    {
        IMongoDatabase database;
        IGridFSBucket gridFS;
        public ComputerContext()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            var connection = new MongoUrlBuilder(connectionString);
            MongoClient client = new MongoClient(connectionString);
            database = client.GetDatabase(connection.DatabaseName);
            gridFS = new GridFSBucket(database);
        }
        public IMongoCollection<Computer> Computers
        {
            get { return database.GetCollection<Computer>("Computers"); }
        }
        public async Task<IEnumerable<Computer>> GetComputers(int? year, string name)
        {
            var builder = new FilterDefinitionBuilder<Computer>();
            var filter = builder.Empty;
            if(!String.IsNullOrWhiteSpace(name))
            {
                filter &= builder.Regex("Name", new BsonRegularExpression(name));
            }
            if(year.HasValue)
            {
                filter &= builder.Eq("Year", year.Value);
            }
            return await Computers.Find(filter).ToListAsync();
        }
        public async Task<Computer>GetComputer(string id)
        {
            return await Computers.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }
        public async Task Create(Computer c)
        {
            await Computers.InsertOneAsync(c);
        }
        public async Task Update(Computer c)
        {
            await Computers.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(c.Id)), c);
        }
        public async Task Remove(string id)
        {
            await Computers.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
        public async Task<byte[]>GetImage(string id)
        {
            return await gridFS.DownloadAsBytesAsync(new ObjectId(id));
        }
        public async Task StoreImage(string id, Stream imageStream, string imageName)
        {
            Computer c = await GetComputer(id);
            if(c.HasImage())
            {
                await gridFS.DeleteAsync(new ObjectId(c.ImageId));
            }
            ObjectId imageId = await gridFS.UploadFromStreamAsync(imageName, imageStream);
            c.ImageId = imageId.ToString();
            var filter = Builders<Computer>.Filter.Eq("_id", new ObjectId(c.Id));
            var update = Builders<Computer>.Update.Set("ImageId", c.ImageId);
            await Computers.UpdateOneAsync(filter, update);
        }
    }
}
