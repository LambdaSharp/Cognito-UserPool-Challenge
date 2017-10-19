using System.Collections.Generic;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
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
        
        public CustomMessageBase FunctionHandler(CustomMessageBase cognitoCustomMessageEvent)
        {
            //LEVEL 4 - Add Custom Message Signup Workflow Trigger
            LambdaLogger.Log("LEVEL 4 - Add Custom Message Signup Workflow Trigger");

            return cognitoCustomMessageEvent;
            
        }        
        
    }
}