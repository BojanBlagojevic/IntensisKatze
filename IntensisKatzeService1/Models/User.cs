using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntensisKatzeService1.Models
{
    public class User
    {

        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        // external Id, easier to reference: 1,2,3 or A, B, C etc.
        public string email { get; set; }

        public bool active { get; set; }

      
        public string password { get; set; }


        [BsonRepresentation(BsonType.ObjectId)]
        public string[] role { get; set; }

        public string _class { get; set; }
    }
}
