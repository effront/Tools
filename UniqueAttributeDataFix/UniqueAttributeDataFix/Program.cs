namespace UniqueAttributeDataFix
{
    class Program
    {
        private static void Main(string[] args)
        {            
            var sagaInfo = new SagaInfo
            {
                DatabaseName = "Put Raven database name here",
                SagaDataTypeFullName = "Put saga data type full name here",
                SagaKeyPrefix = "Put saga key prefix here",
                UniqueField = "Put field name that is decorated by Unique attribute here",
                RavenUrl = "http://localhost:8080",
            };             
            
            new SetSagaUniqueValueInMetadata().Run(sagaInfo);
            new PopulateSagaUniqueIdentityRecords().Run(sagaInfo);            
        }        
    }
}
