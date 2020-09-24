//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using MongoDB.Bson;
//using MongoDB.Bson.IO;
//using MongoDB.Bson.Serialization;
//using MongoDB.Bson.Serialization.Serializers;

//namespace IntensisKatzeService1
//{
//    public sealed class StringOrInt32Serializer : BsonStringSerializer
//    {
//        public override object Deserialize(BsonReader bsonReader, Type nominalType,
//            Type actualType,)
//        {
//            var bsonType = bsonReader.CurrentBsonType;
//            switch (bsonType)
//            {
//                case BsonType.Null:
//                    bsonReader.ReadNull();
//                    return null;
//                case BsonType.String:
//                    return bsonReader.ReadString();
//                default:
//                    var message = string.Format("Cannot deserialize BsonString or BsonInt32 from BsonType {0}.", bsonType);
//                    throw new BsonSerializationException(message);
//            }
//        }

//        public override void Serialize(BsonWriter bsonWriter, Type nominalType,
//            object value, IBsonSerializationOptions options)
//        {
//            if (value != null)
//            {
//                bsonWriter.WriteString(value.ToString());
//            }
//            else
//            {
//                bsonWriter.WriteNull();
//            }
//        }
//    }
//}
