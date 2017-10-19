using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Amazon.Lambda.APIGatewayEvents;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace CognitoRedirect
{

    public class Function
    {
        //-- Methods ---
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest cognitoTriggerEvent,
            ILambdaContext context)
        {
            LambdaLogger.Log(JsonConvert.SerializeObject(cognitoTriggerEvent));

            string locationRedirect;
            cognitoTriggerEvent.QueryStringParameters.TryGetValue("url", out locationRedirect);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.Redirect,
                Body = "",
                Headers = new Dictionary<string, string> {{"Location", locationRedirect}}
            };

            return response;
        }
    }
}