using System.Collections.Generic;
using System.Linq;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using UserPoolTriggers.Models;

namespace UserPoolTriggers.Triggers
{
    public class CognitoPreSignup
    {
        
        //--- Constants ---
        private static readonly List<string> WhiteListDomains = new List<string>(new string[]
        {
            "gmail.com",
            "live.com"
        });
        
        //--- Methods ---    
        private bool MatchReferralCode(string code)
        {
            return code == "happy";
        }

        private bool MatchEmailDomain(string email)
        {
            return WhiteListDomains.Any(email.Contains);
        }
        
        public PreSignupBase FunctionHandler(PreSignupBase cognitoPreSignupEvent)
        {
            //## LEVEL 3 - Add Pre sign-up Workflow Trigger
            LambdaLogger.Log("LEVEL 3 - Add Pre sign-up Workflow Trigger");

            // Only allow emails from the whiteList (can be done from UI too)
            string userEmail;
            cognitoPreSignupEvent.Request.UserAttributes.TryGetValue("email", out userEmail);
            if (!MatchEmailDomain(userEmail))
            {
                LambdaLogger.Log("#####EmailDomainNotAllowed");
                var thisError = new PreconditionNotMetException("SignUp incomplete. The email domain is not allowed.");
                throw thisError;
            }
            
            // No username can be admin
            string userName = cognitoPreSignupEvent.UserName;
            if (userName == ("admin"))
            {
                LambdaLogger.Log("#####CannotCreateAdminUser");
                var thisError = new PreconditionNotMetException("The username `admin` is not allowed.");
                throw thisError;
            }
            
            // bypass the verification for sign-ups that have a phone number that starts with 619
            string userPhone;
            cognitoPreSignupEvent.Request.UserAttributes.TryGetValue("phone_number", out userPhone);
            LambdaLogger.Log($"#####userPhone {userPhone}");
            if (userPhone.StartsWith("+1858"))
            {
                LambdaLogger.Log("#####AutoConfirmUser");
                cognitoPreSignupEvent.Response.AutoConfirmUser = true;
                cognitoPreSignupEvent.Response.AutoVerifyPhone = true;
            }
            
            // decline sign-ups that DO NOT have a phone number that starts with 619, 858, or 760
            if (!userPhone.StartsWith("+1858") && !userPhone.StartsWith("+1760") && !userPhone.StartsWith("+1619"))
            {
                LambdaLogger.Log("#####Area Code Error");
                var thisError = new PreconditionNotMetException("Only 619, 858, and 760 area codes are allowed.");
                throw thisError;
            }
            
            return cognitoPreSignupEvent;
        }
        
    }
}