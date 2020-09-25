using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntensisKatzeService1.Models
{
    public class RemoteWork
    {

        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        // external Id, easier to reference: 1,2,3 or A, B, C etc.
        [BsonRepresentation(BsonType.ObjectId)]
        public string employeeId { get; set; }

        public DateTime date { get; set; }

        public int minutes { get; set; }

        public string text { get; set; }

        public string _class { get; set; }
        [BsonIgnore]
        public DateTime? CreatedAt => _id != null ? new ObjectId(_id).CreationTime.ToLocalTime() : (DateTime?)null;


    }
}
