using System;
using System.IO;
using System.Collections.Generic;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using UserPoolTriggers.Models;
using Amazon.S3;
using Amazon.S3.Model;

namespace UserPoolTriggers.Triggers
{
    public class CognitoPostConfirmation {

        //--- Fields ---
        private readonly IAmazonS3 _s3Client;
        private readonly IAmazonCognitoIdentityProvider _cognitoIdentityProviderClient;
        private readonly string _prefix;
        private string _usersBucketName;

        //--- Constructors ---
        public CognitoPostConfirmation(string prefix)
        {
            _s3Client = new AmazonS3Client();
            _cognitoIdentityProviderClient = new AmazonCognitoIdentityProviderClient();
            _prefix = prefix;
        }

        //--- Methods ---
        public PostConfirmationBase FunctionHandler(PostConfirmationBase cognitoPostConfirmationEvent)
        {
            //## LEVEL 5 - Add Post Confirmation Workflow Trigger
            LambdaLogger.Log("LEVEL 5 - Add Post Confirmation Workflow Trigger");
            
            return cognitoPostConfirmationEvent;
        }
    }
}