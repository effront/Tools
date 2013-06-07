using System;
using Raven.Abstractions.Data;

namespace UniqueAttributeDataFix
{
    public class PrintHelper
    {
        public static void PrintResults(string name, System.Threading.Tasks.Task<BatchResult[]> results)
        {
            Console.WriteLine("***");
            var counter = 0;
            foreach (var result in results.Result)
            {
                Console.WriteLine("Result of {0} {1}", name, result.Key);
                counter++;
            }
            Console.WriteLine("Result counts: {0}", counter);
            Console.WriteLine("END: {0} completed at {1}", name, DateTime.Now);
            Console.WriteLine("***");
        }

        public static void PrintFunctionCallDetails(string name, IServcorpSagaInfo sagaInfo)
        {
            Console.WriteLine("***");
            Console.WriteLine("BEGIN: Start running {0} at {1}", name, DateTime.Now);
            Console.WriteLine("{0}: {1}", "SagaDataTypeFullName", sagaInfo.SagaDataTypeFullName);
            Console.WriteLine("{0}: {1}", "DatabaseName", sagaInfo.DatabaseName);
            Console.WriteLine("{0}: {1}", "SagaKeyPrefix", sagaInfo.SagaKeyPrefix);
            Console.WriteLine("{0}: {1}", "SagaUniqueIdentityKeyPrefix", sagaInfo.SagaUniqueIdentityKeyPrefix);
            Console.WriteLine("{0}: {1}", "UniqueField", sagaInfo.UniqueField);
            Console.WriteLine("***");
        }

        public static void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
