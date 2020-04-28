using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace CleanArchitecture.WebUI.Filters
{
    public class IgnoreSkipPageCountOperationProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var skipPageCountParameter = context.OperationDescription.Operation.Parameters.FirstOrDefault(para => para.Name == "SkipPageCount");

            if(skipPageCountParameter != null)
            {
                context.OperationDescription.Operation.Parameters.Remove(skipPageCountParameter);
            }
            return true;
        }
    }
}
