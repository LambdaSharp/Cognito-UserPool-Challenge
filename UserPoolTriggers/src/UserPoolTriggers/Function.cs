using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UserPoolTriggers.Models;
using UserPoolTriggers.Triggers;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace UserPoolTriggers
{

    public class Function
    {
        //--- Fields ---
        public static readonly string Prefix = "lambdasharp";

        //-- Methods ---
        public JObject FunctionHandler(JObject cognitoTriggerEvent, ILambdaContext context)
        {
            // Log the event to cloud watch
            LambdaLogger.Log(JsonConvert.SerializeObject(cognitoTriggerEvent));
            var triggerSource = cognitoTriggerEvent.GetValue("triggerSource").ToString();
            
            switch (triggerSource)
            {
                // Triggers when the user submits request to sign up
                case "PreSignUp_SignUp":
                    
                    // Prevent certain users from signing up
                    LambdaLogger.Log("Case PreSignUp_SignUp");
                    var cognitoPreSignup = new CognitoPreSignup();
                    var triggerPreSignupEvent = cognitoPreSignup.FunctionHandler(cognitoTriggerEvent.ToObject<PreSignupBase>());
                    return JObject.FromObject(triggerPreSignupEvent);
                    
                // Triggers after successful sign up and before the verification code
                case "CustomMessage_SignUp":
                    
                    // send custom verification code message
                    LambdaLogger.Log("Case CustomMessage_SignUp");
                    var cognitoCustomMessage = new CognitoCustomMessage();
                    var triggerCustomMessageEvent = cognitoCustomMessage.FunctionHandler(cognitoTriggerEvent.ToObject<CustomMessageEvent>());
                    return JObject.FromObject(triggerCustomMessageEvent);
                    
                // Triggers after successful sign in and/or MFA code    
                case "PostConfirmation_ConfirmSignUp":
                    
                    // send user email with url to s3 bucket and login
                    LambdaLogger.Log("Case PostConfirmation_ConfirmSignUp");
                    var cognitoPostConfirmation = new CognitoPostConfirmation(Prefix);
                    var triggerPostConfirmationEvent = cognitoPostConfirmation.FunctionHandler(cognitoTriggerEvent.ToObject<PostConfirmationBase>());
                    return JObject.FromObject(triggerPostConfirmationEvent);
                
                default:
                    // Log any triggers we do not implement
                    LambdaLogger.Log($"No custom implementations for: {triggerSource}");
                    break;
            }
            
            return cognitoTriggerEvent;
        }

    }
}