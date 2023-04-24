using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace LR3_MVC_MongoDbApp.Models
{
    public class Computer
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [Display(Name ="Назва моделі")]
        public string Name { get; set; }
        [Display(Name ="Рік випуску")]
        public int Year { get; set; }
        public string ImageId { get; set; }
        public bool HasImage()
        {
            return !String.IsNullOrWhiteSpace(ImageId);
        }
    }
}
