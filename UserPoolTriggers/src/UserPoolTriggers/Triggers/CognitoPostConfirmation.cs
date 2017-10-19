using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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
        private AdminGetUserResponse GetUserInfoFromCognito(string userName, string userPoolId)
        {
            try
            {
                AdminGetUserRequest getUserRequest = new AdminGetUserRequest
                {
                    Username = userName,
                    UserPoolId = userPoolId
                };
                var getUserData = _cognitoIdentityProviderClient.AdminGetUserAsync(getUserRequest).Result;
                LambdaLogger.Log("queriedUserData#####");
                LambdaLogger.Log(JsonConvert.SerializeObject(getUserData));
                return getUserData;
            }
            catch (AggregateException e)
            {
                LambdaLogger.Log($"There was an error getting user information: {e}");   
            }
            return new AdminGetUserResponse();
        }

        private S3Bucket FindBucketData()
        {
            ListBucketsResponse bucketList = _s3Client.ListBucketsAsync().Result;
            var foundBucket = bucketList.Buckets.Find(
                bucket => bucket.BucketName == _usersBucketName
            );
            
            LambdaLogger.Log("bucketValue#####");
            LambdaLogger.Log(JsonConvert.SerializeObject(foundBucket));
            return foundBucket;
        }

        private void CreateBucket()
        {
            PutBucketRequest putBucketRequest = new PutBucketRequest
            {
                BucketName = _usersBucketName,
                UseClientRegion = true
            };
            _s3Client.PutBucketAsync(putBucketRequest).Wait();
            LambdaLogger.Log($"bucketCreated##### {_usersBucketName}");
        }
        
        private void AddBucketPolicy()
        {
            string publicSitePolicy = @"{ 
    ""Statement"":[{ 
    ""Sid"":""PublicReadGetObject"", 
    ""Effect"":""Allow"", 
    ""Principal"": ""*"", 
    ""Action"":[""s3:PutObject"",""s3:GetObject""], 
    ""Resource"":[""arn:aws:s3:::<BUCKET_NAME>/*""] 
}]}";
            LambdaLogger.Log(publicSitePolicy);
            PutBucketPolicyRequest putBucketPolicyRequest = new PutBucketPolicyRequest
            {
                BucketName = _usersBucketName,
                Policy = publicSitePolicy.Replace("<BUCKET_NAME>", _usersBucketName)
            };
            _s3Client.PutBucketPolicyAsync(putBucketPolicyRequest).Wait();
            LambdaLogger.Log($"bucketPolicyAdded##### ");
        }

        private void EnableS3Website()
        {
            WebsiteConfiguration websiteConfiguration = new WebsiteConfiguration
            {
                IndexDocumentSuffix = "index.html",
                ErrorDocument = "error.html"
            };
            var putBucketWebsiteResponse =  _s3Client.PutBucketWebsiteAsync(_usersBucketName, websiteConfiguration).Result;
            LambdaLogger.Log($"s3WebsiteEnabled##### {JsonConvert.SerializeObject(putBucketWebsiteResponse)}");
        }

        private void PopulateBucket(string userName)
        {
            // Loop through the Resources diectory to copy the website files to the s3 bucket
            const string subfolder = "UserPoolTriggers.Resources.";
            var assembly = typeof(Function).GetTypeInfo().Assembly;
            string[] names = assembly.GetManifestResourceNames();
            foreach (var name in names)
            {
                // Skip names outside of your desired subfolder
                if (!name.StartsWith(subfolder))
                {
                    continue;
                }

                using (Stream input = assembly.GetManifestResourceStream(name)) {
                    LambdaLogger.Log($"{name}");
                    // remove the prefix
                    string s3Key = name.Replace(subfolder, "");
                    
                    // replace all dots with `/` except last
                    int lastIndex = s3Key.LastIndexOf('.');
                    if (lastIndex > 0)
                    {
                        s3Key = s3Key.Substring(0, lastIndex).Replace(".", "/")
                                + s3Key.Substring(lastIndex);
                    }

                    PutObjectRequest uploadPartRequest = new PutObjectRequest { };
                    if (name.EndsWith("index.html"))
                    {
                        // replace values in the config
                        byte[] bytes = new byte[input.Length];
                        input.Position = 0;
                        input.Read(bytes, 0, (int)input.Length);
                        var contentBody = Encoding.ASCII.GetString(bytes); // this is your string
                        string customContentBody = contentBody
                            .Replace("<BUCKET_NAME>", _usersBucketName)
                            .Replace("<USER_NAME>", userName);

                        // upload the file
                        uploadPartRequest.BucketName = _usersBucketName;
                        uploadPartRequest.ContentBody = customContentBody;
                        uploadPartRequest.Key = s3Key;
                        uploadPartRequest.CannedACL = S3CannedACL.PublicRead;
                    }
                    else
                    {

                        // upload the file
                        uploadPartRequest.BucketName = _usersBucketName;
                        uploadPartRequest.InputStream = input;
                        uploadPartRequest.Key = s3Key;
                        uploadPartRequest.CannedACL = S3CannedACL.PublicRead;
                    }
                    var result = _s3Client.PutObjectAsync(
                        uploadPartRequest
                    ).Result;
                    LambdaLogger.Log(result.ToString());
                }
            }
        }

        private void UpdateUserRecord(string userName, string userPoolId, string bucketPublicUrl)
        {
            AdminUpdateUserAttributesRequest updateUserS3BucketRequest = new AdminUpdateUserAttributesRequest
            {
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType
                    {
                        Name = "custom:s3_website_url",
                        Value = bucketPublicUrl
                    },
                    new AttributeType
                    {
                        Name = "custom:s3_bucket",
                        Value = _usersBucketName
                    }
                },
                Username = userName,
                UserPoolId = userPoolId
            };
            var updateUserResult = _cognitoIdentityProviderClient.AdminUpdateUserAttributesAsync(updateUserS3BucketRequest).Result;
            LambdaLogger.Log("userUpdated#####");
            LambdaLogger.Log(JsonConvert.SerializeObject(updateUserS3BucketRequest));

        }
        
        public PostConfirmationBase FunctionHandler(PostConfirmationBase cognitoPostConfirmationEvent)
        {
            //## LEVEL 5 - Add Post Confirmation Workflow Trigger
            LambdaLogger.Log("LEVEL 5 - Add Post Confirmation Workflow Trigger");
            string userName = cognitoPostConfirmationEvent.UserName;
            // check that the user status is confirmed
            string userEmail;
            cognitoPostConfirmationEvent.Request.UserAttributes.TryGetValue("email", out userEmail);
            string userPoolId = cognitoPostConfirmationEvent.UserPoolId;

            // 1- get the user info from cognito
            var getUserData = GetUserInfoFromCognito(userName, userPoolId);
            
            // 2- user status is not confirmed throw exception
            var userStatus = getUserData.UserStatus.Value;
            LambdaLogger.Log($"USER STATUS: {userStatus}");   
            if (userStatus != "CONFIRMED")
            {
                var thisError = new PreconditionNotMetException("SignUp process is incomplete. Please verify your account.");
                throw thisError;
            }
            
            // 3- get the timestamp the user was UserCreateDate
            var dateForBucketName = getUserData.UserCreateDate.ToString("yyyy");
           
            // 4- make bucket name
            _usersBucketName = $"{_prefix}-{userName}-{dateForBucketName}";
            LambdaLogger.Log($"Bucket name for {userName}: {_usersBucketName}");

            // 5- check if bucket is found
            if (FindBucketData() == null)
            {
                // 5.1- create if bucket doesn't exits
                CreateBucket();
                
                 //5.2 - add public allow read bucket policy
                AddBucketPolicy();
 
                // 5.3 - enable s3 website
                EnableS3Website();

                // 5.4- populate bucket with html template from the central bucket
                PopulateBucket(userName);
                
                // 5.5- update user record with url to s3 website
                var bucketPublicUrl = $"http://{_usersBucketName}.s3-website-{cognitoPostConfirmationEvent.Region}.amazonaws.com";
                UpdateUserRecord(userName, userPoolId, bucketPublicUrl);
                
            }
            return cognitoPostConfirmationEvent;
        }
    }
}