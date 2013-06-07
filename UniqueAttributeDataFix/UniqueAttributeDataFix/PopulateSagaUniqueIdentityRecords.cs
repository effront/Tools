using System.Collections.Generic;
using Raven.Abstractions.Commands;
using Raven.Client.Document;
using Raven.Json.Linq;

namespace UniqueAttributeDataFix
{
    public class PopulateSagaUniqueIdentityRecords
    {
        public void Run(ISagaInfo sagaInfo)
        {
            PrintHelper.PrintFunctionCallDetails("PopulateSagaUniqueIdentityRecords", sagaInfo);
            var documentStore = new DocumentStore
            {
                Url = sagaInfo.RavenUrl,
                DefaultDatabase = sagaInfo.DatabaseName
            };

            documentStore.Initialize();

            var existingSagaUniqueIdentityUniqueValues = DocumentHelper.GetExistingSagaUniqueIdentityUniqueValues(documentStore, sagaInfo);
            var commandsList = new List<ICommandData>();
            var dictionarytest = new Dictionary<string, int>();
            const int pageSize = 100;
            for (var start = 0; ; start += pageSize)
            {
                var sagaDocuments = documentStore.DatabaseCommands.GetDocuments(start, pageSize);
                foreach (var document in sagaDocuments)
                {
                    if (document.Key.StartsWith(sagaInfo.SagaKeyPrefix) && DocumentHelper.HasServicebusUniqueValue(document))
                    {
                        var body = document.DataAsJson;
                        var uniqueValue = body.Value<string>(sagaInfo.UniqueField);
                        if (existingSagaUniqueIdentityUniqueValues.Exists(x => x == uniqueValue))
                        {
                            PrintHelper.PrintMessage(string.Format("Skip uniquevalue {0} because it already exists", uniqueValue));
                            continue; // Already has SagaUniqueIdentity record
                        }
                        var sagaId = document.Key.Replace(sagaInfo.SagaKeyPrefix + "/", "");

                        var documentBody = new RavenJObject();
                        documentBody.Add("SagaId", sagaId);
                        documentBody.Add("UniqueValue", uniqueValue);
                        documentBody.Add("SagaDocId", document.Key);

                        var documentMetadata = new RavenJObject();
                        documentMetadata.Add("Content-Type", "application/json; charset=utf-8");
                        documentMetadata.Add("Raven-Entity-Name", "SagaUniqueIdentity");
                        documentMetadata.Add("Raven-Clr-Type",
                                             "NServiceBus.SagaPersisters.Raven.SagaUniqueIdentity, NServiceBus.Core");

                        ICommandData command = new PutCommandData
                        {
                            Key = DocumentHelper.GenerateUniqueIdentityRecordId(uniqueValue, sagaInfo),
                            Document = documentBody,
                            Metadata = documentMetadata,
                        };

                        if (dictionarytest.ContainsKey(uniqueValue))
                        {
                            dictionarytest[uniqueValue] += 1;
                        }
                        else
                        {
                            dictionarytest[uniqueValue] = 1;
                        }

                        commandsList.Add(command);
                    }
                }

                if (sagaDocuments.Length < pageSize) break;
            }
            PrintHelper.PrintMessage(string.Format("Attempt to insert {0} SagaUniqueIdentity documents", commandsList.Count));
            foreach (var item in dictionarytest.Keys)
            {
                if (dictionarytest[item] > 1)
                {
                    PrintHelper.PrintMessage("the following has duplicate " + item + " : " + dictionarytest[item]);
                }
            }
            var results = documentStore.AsyncDatabaseCommands.BatchAsync(commandsList.ToArray());
            PrintHelper.PrintResults("PopulateSagaUniqueIdentityRecords", results);
        }
    }
}
