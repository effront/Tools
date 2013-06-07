using System;
using System.Collections.Generic;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Client.Document;

namespace UniqueAttributeDataFix
{
    public class SetSagaUniqueValueInMetadata
    {
        public void Run(ISagaInfo sagaInfo)
        {
            PrintHelper.PrintFunctionCallDetails("SetSagaUniqueValueInMetadata", sagaInfo);
            var documentStore = new DocumentStore
            {
                Url = sagaInfo.RavenUrl,
                DefaultDatabase = sagaInfo.DatabaseName
            };

            documentStore.Initialize();
            const int pageSize = 100;
            var commandsList = new List<ICommandData>();
            for (var start = 0; ; start += pageSize)
            {
                var documents = documentStore.DatabaseCommands.GetDocuments(start, pageSize);
                foreach (var document in documents)
                {
                    if (document.Key.StartsWith(sagaInfo.SagaKeyPrefix) && !DocumentHelper.HasServicebusUniqueValue(document))
                    {
                        var body = document.DataAsJson;
                        var uniqueValue = body.Value<string>(sagaInfo.UniqueField);
                        var patchRequests = new[]
                            {
                                new PatchRequest
                                    {
                                        Type = PatchCommandType.Modify,
                                        Name = "@metadata",
                                        Nested = new[]
                                            {
                                                new PatchRequest
                                                    {
                                                        Type = PatchCommandType.Set,
                                                        Name = "NServiceBus-UniqueValue",
                                                        Value = uniqueValue,
                                                    }
                                            }
                                    }
                            };
                        ICommandData command = new PatchCommandData
                        {
                            Key = document.Key,
                            Patches = patchRequests
                        };
                        commandsList.Add(command);
                    }
                }
                if (documents.Length < pageSize) break;
            }
            Console.WriteLine("Attempt to patch {0} saga documents", commandsList.Count);
            var results = documentStore.AsyncDatabaseCommands.BatchAsync(commandsList.ToArray());
            PrintHelper.PrintResults("SetSagaUniqueValueInMetadata", results);
        }
    }
}
