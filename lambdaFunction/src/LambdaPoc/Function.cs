using System;
using opa_library;
using Amazon.Lambda.Core;
using System.Threading.Tasks;


[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace LambdaPoc{

    public class Function
    {
        /// <summary>
        /// Queries OPA and with structured data and return it's policy decision-making.
        /// For more information: https://www.openpolicyagent.org/docs/latest/
        /// </summary>
        /// <param name="input">JSON with "policy" as string and data and input as JSON, type: System.Text.Json.JsonElement</param>
        /// <param name="context"></param>
        /// <returns>OPA raw result from evaluating the input, type: System.String</returns>
        public async Task<bool> FunctionHandler(Object input, ILambdaContext context)
        {
            return await Opa.Eval(input);
        }
    }
}