﻿namespace UniqueAttributeDataFix
{
    public interface ISagaInfo
    {
        string RavenUrl { get; set; }
        string DatabaseName { get; set; }
        string SagaKeyPrefix { get; set; }
        string UniqueField { get; set; }
        string SagaDataTypeFullName { get; set; }
        string SagaUniqueIdentityKeyPrefix { get; }
    }

    public class SagaInfo : ISagaInfo
    {
        public string RavenUrl { get; set; }
        public string DatabaseName { get; set; }
        public string SagaKeyPrefix { get; set; }
        public string UniqueField { get; set; }
        public string SagaDataTypeFullName { get; set; }
        public string SagaUniqueIdentityKeyPrefix { get { return SagaDataTypeFullName + "/" + UniqueField; } }
    }    
}
