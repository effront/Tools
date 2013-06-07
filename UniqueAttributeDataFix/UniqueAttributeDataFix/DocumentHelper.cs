using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using Raven.Json.Linq;

namespace UniqueAttributeDataFix
{
    public class DocumentHelper
    {
        public static List<string> GetExistingSagaUniqueIdentityUniqueValues(DocumentStore documentStore, IServcorpSagaInfo sagaInfo)
        {
            const int pageSize = 100;
            var keys = new List<string>();
            for (var start = 0; ; start += pageSize)
            {
                var documents = documentStore.DatabaseCommands.GetDocuments(start, pageSize);
                foreach (var document in documents)
                {
                    if (document.Key.StartsWith(sagaInfo.SagaUniqueIdentityKeyPrefix))
                    {
                        keys.Add(document.DataAsJson.Value<string>("UniqueValue"));
                    }
                }

                if (documents.Length < pageSize) break;
            }

            return keys;
        }

        public static bool HasServicebusUniqueValue(JsonDocument doc)
        {
            RavenJToken token;
            return doc.Metadata.TryGetValue("NServiceBus-UniqueValue", out token);
        }

        public static string GenerateUniqueIdentityRecordId(string uniqueValue, IServcorpSagaInfo sagaInfo)
        {
            // use MD5 hash to get a 16-byte hash of the string
            using (var provider = new MD5CryptoServiceProvider())
            {
                var inputBytes = Encoding.Default.GetBytes(uniqueValue);
                byte[] hashBytes = provider.ComputeHash(inputBytes);

                // generate a guid from the hash:
                var value = new Guid(hashBytes);

                var id = string.Format("{0}/{1}", sagaInfo.SagaUniqueIdentityKeyPrefix, value);

                // raven has a size limit of 255 bytes == 127 unicode chars
                if (id.Length > 127)
                {
                    // generate a guid from the hash:

                    var key =
                        new Guid(
                            provider.ComputeHash(Encoding.Default.GetBytes(sagaInfo.SagaDataTypeFullName + sagaInfo.UniqueField)));

                    id = string.Format("MoreThan127/{0}/{1}", key, value);
                }

                return id;
            }
        }
    }
}
