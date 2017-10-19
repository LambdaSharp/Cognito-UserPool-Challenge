using System.Collections.Generic;
using System.Linq;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using UserPoolTriggers.Models;

namespace UserPoolTriggers.Triggers
{
    public class CognitoPreSignup
    {        
        public PreSignupBase FunctionHandler(PreSignupBase cognitoPreSignupEvent)
        {
            //## LEVEL 3 - Add Pre sign-up Workflow Trigger
            LambdaLogger.Log("LEVEL 3 - Add Pre sign-up Workflow Trigger");

            // Only allow emails from a whiteList (can be done from UI in lieu of code as well)
            
            // No username can be admin

            // bypass the verification code for sign-ups that have a phone number that starts with 619

            // decline sign-ups that DO NOT have a phone number that starts with 619, 858, or 760
            
            return cognitoPreSignupEvent;
        }
    }
}