using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using Raven.Json.Linq;

namespace UniqueAttributeDataFix
{
    class Program
    {
        private static void Main(string[] args)
        {            
            var allowedParams = new List<string>
                {
                    "VirtualOfficeSalesSaga",
                    "ServicedOfficeSalesSaga",
                    "ContractEventSaga",
                    "BenefitManagementSaga",
                    "AccountEventSaga",
                    "ContactEventSaga",
                    "CreatedCustomerCredentialSaga",
                    "CreateOpportunityInCRM",
                };
            if (args.Length == 0)
            {
                PrintHelper.PrintMessage("Please specify saga name. Select from the following:");
                PrintHelper.PrintMessage(String.Join("\n", allowedParams));
                return;
            }
            
            IServcorpSagaInfo sagaInfo = null;
            switch (args[0])
            {
                case "VirtualOfficeSalesSaga":
                    {
                        sagaInfo = new ServcorpSagaInfo
                        {
                            DatabaseName = "Servcorp.Processes.VirtualOfficeSales",
                            SagaDataTypeFullName = "Servcorp.Processes.VirtualOfficeSales.VirtualOfficeSalesSagaData",
                            SagaKeyPrefix = "VirtualOfficeSalesSaga",
                            UniqueField = "OpportunityUUID",
                        };
                        break;
                    }
                case "ServicedOfficeSalesSaga":
                    {
                        sagaInfo = new ServcorpSagaInfo
                        {
                            DatabaseName = "Servcorp.Processes.ServicedOfficeSales",
                            SagaDataTypeFullName = "Servcorp.Processes.ServicedOfficeSales.ServicedOfficeSalesSagaData",
                            SagaKeyPrefix = "ServicedOfficeSalesSaga",
                            UniqueField = "OpportunityId",
                        };
                        break;
                    }
                case "ContractEventSaga":
                    {
                        sagaInfo = new ServcorpSagaInfo
                        {
                            DatabaseName = "Servcorp.MessageRouter.OTIIS.EventPublisher",
                            SagaDataTypeFullName = "Servcorp.MessageRouter.OTIIS.EventPublisher.Sagas.ContractEventSagaData",
                            SagaKeyPrefix = "ContractEventSaga",
                            UniqueField = "OTIISClientId",
                        };
                        break;
                    }

                case "BenefitManagementSaga":
                    {
                        sagaInfo = new ServcorpSagaInfo
                        {
                            DatabaseName = "Servcorp.Processes.OTIIS",
                            SagaDataTypeFullName = "Servcorp.Processes.BenefitManagement.BenefitManagementSagaData",
                            SagaKeyPrefix = "BenefitManagementSaga",
                            UniqueField = "CustomerId",
                        };
                        break;
                    }
                case "AccountEventSaga":
                    {
                        sagaInfo = new ServcorpSagaInfo
                        {
                            DatabaseName = "Servcorp.Processes.OTIIS",
                            SagaDataTypeFullName = "Servcorp.Processes.OTIIS.Handlers.AccountEventSagaData",
                            SagaKeyPrefix = "AccountEventSaga",
                            UniqueField = "CustomerId",
                        };
                        break;
                    }
                case "ContactEventSaga":
                    {
                        sagaInfo = new ServcorpSagaInfo
                        {
                            DatabaseName = "Servcorp.Processes.OTIIS",
                            SagaDataTypeFullName = "Servcorp.Processes.OTIIS.Handlers.ContactEventSagaData",
                            SagaKeyPrefix = "ContactEventSaga",
                            UniqueField = "ContactId",
                        };
                        break;
                    }
                case "CreatedCustomerCredentialSaga":
                    {
                        sagaInfo = new ServcorpSagaInfo
                        {
                            DatabaseName = "Servcorp.Processes.OTIIS",
                            SagaDataTypeFullName = "Servcorp.Processes.OTIIS.Handlers.CreatedCustomerCredentialSagaData",
                            SagaKeyPrefix = "CreatedCustomerCredentialSaga",
                            UniqueField = "ContactId",
                        };
                        break;
                    }
                case "CreateOpportunityInCRM":
                    {
                        sagaInfo = new ServcorpSagaInfo
                        {
                            DatabaseName = "Servcorp.Processes.CRM",
                            SagaDataTypeFullName = "Servcorp.Processes.CRM.Handlers.CreateOpportunityInCRMData",
                            SagaKeyPrefix = "CreateOpportunityInCRM",
                            UniqueField = "OpportunityUUID",
                        };
                        break;
                    }

            }
            if (sagaInfo == null)
            {
                PrintHelper.PrintMessage("Invalid saga name. Select from the following:");
                PrintHelper.PrintMessage(String.Join("\n", allowedParams));
                return;
            }
            sagaInfo.RavenUrl = "http://localhost:8080";
            new SetSagaUniqueValueInMetadata().Run(sagaInfo);
            new PopulateSagaUniqueIdentityRecords().Run(sagaInfo);            
        }        
    }
}
