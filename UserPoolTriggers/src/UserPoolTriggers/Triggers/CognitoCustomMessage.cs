using Amazon.CognitoIdentityProvider;
using Amazon.Lambda.Core;
using UserPoolTriggers.Models;

namespace UserPoolTriggers.Triggers
{
    public class CognitoCustomMessage
    {
        //--- Fields ---
        private readonly IAmazonCognitoIdentityProvider _cognitoIdentityProviderClient;

        //--- Constructors ---
        public CognitoCustomMessage()
        {
            _cognitoIdentityProviderClient = new AmazonCognitoIdentityProviderClient();
        }
        
        public CustomMessageEvent FunctionHandler(CustomMessageEvent cognitoCustomMessageEvent)
        {
            //LEVEL 4 - Add Custom Message Signup Workflow Trigger
            LambdaLogger.Log("LEVEL 4 - Add Custom Message Signup Workflow Trigger");
            cognitoCustomMessageEvent.Response.EmailSubject = $"HELLO there! {cognitoCustomMessageEvent.Request.CodeParameter}";
            cognitoCustomMessageEvent.Response.EmailMessage = $"{cognitoCustomMessageEvent.UserName}, thank you for signing up! Here is your code {cognitoCustomMessageEvent.Request.CodeParameter}";
            cognitoCustomMessageEvent.Response.SmsMessage = $"{cognitoCustomMessageEvent.UserName}, thank you for signing up! Here is your code: {cognitoCustomMessageEvent.Request.CodeParameter}";
            return cognitoCustomMessageEvent;
            
        }        
        
    }
}