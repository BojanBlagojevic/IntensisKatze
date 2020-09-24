using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace IntensisKatzeService1.Models
{
    public class Employee
    {
       
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // external Id, easier to reference: 1,2,3 or A, B, C etc.
        public string email { get; set; }
        

       public string firstName { get; set; }

        public string lastName { get; set; }

        public bool mustToApprove { get; set; }

        public int workingExp { get; set; }


        public bool active { get; set; }
     
        [BsonRepresentation(BsonType.ObjectId)]
        public string user { get; set; }
      
        [BsonRepresentation(BsonType.ObjectId)]
        public string superior { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string[] companyRoles { get; set; }

        public string _class { get; set; }

       

    }
}
