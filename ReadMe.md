# λ# - Have an app with an identity crisis? Sign Up with Cognito User Pools! 

This challenge is about using Cognito User Pools and setting up workflow triggers to change the user registration workflow.

## Pre-requisites

The following tools and accounts are required to complete these instructions.

* [Complete Step 1 of the AWS Lambda Getting Started Guide](http://docs.aws.amazon.com/lambda/latest/dg/setup.html)
  * Setup an AWS account
  * Setup the AWS CLI
* Create a CLI profile
  * Use `aws configure --profile lambdasharp` to create a profile to use with the example scripts.
  * Provide an AWS Access Key ID and the corresponding Secret Key
  * `us-west-2` is a good choice for region
  * Leave default output format as `None` it doesn't matter.
* [Install .NET Core 1.0.4](https://github.com/dotnet/core/blob/master/release-notes/download-archives/1.0.4-download.md)

## Level 1 - Create a Cognito User Pool

### Cognito User Pool Creation
Setup a Cognito user pool with the user pool name `lambdasharp_pool`. The sign up form should ask a minimum of a phone number and username. The user should login with their username and require MFA to login. Create an app client named `lambdasharp-client` that does not generate a client secret.

<details>
  <summary>Steps - Cognito User Pool Creation</summary>

* User pool name: `lambdasharp_pool`
    * `Step through settings`
* Attributes
    * How do you want your end users to sign in? `Username`
    * Which standard attributes do you want to require?
        * Email
        * Name
        * Phone number
    * >NOTE: You cannot change these values after creation, but you can add custom attributes at any time.
* Policy
    * What password strength do you want to require?
        * Minimum password length must be greater than 6.
        * >NOTE: You can make the password policy any kind you desire, but for testing, you might not want to make it complicated.
    * Do you want to allow users to sign themselves up? `Allow users to sign themselves up`
    * Leave the rest as default
* Verifications
    * Do you want to enable Multi-Factor Authentication (MFA)? `Required`
    * Do you want to require verification of emails or phone numbers? `Phone`
    * You must provide a role to allow Amazon Cognito to send SMS messages
        * Enter role name: `lambdasharp_cognito_sms_role`
        * Press: `Create Role`
        * >Note: You must create this role during setup
* Message Customizations: On this page, you can customize the message that is sent to the user, leave as default or change it as desired.
* Tags: Leave blank or add tags as desired
* Devices: Leave as default
* App Clients
    * Add an app client
        * App client name: `lambdasharp-client`
        * Uncheck `Generate client secret`
        * Leave the rest as the defaults
* Triggers: Leave as default for now
* Create pool
</details>

### App Client Settings
After the user pool is created configure the app client settings. Enable the `Cognito User Pool` as an identity provider and allow `Authorization code grant` and `Openid` as the flow and scope. Enter `https://www.google.com` for the callback and sign out URLs.
Add a domain name prefix to use the sign-up and sign-in pages hosted by Cognito.

<details>
    <summary>Steps - App Client Settings</summary>
    
* App client settings
    * Enabled Identity Providers
        * Cognito User Pool
* Callback URL(s):
    * Enter `https://www.google.com`
    * > NOTE: callback urls only allow https, Ideally you would have a secure app that you would callback.
* Sign out URL(s): Enter the same url for Callback URL(s)
* Allowed OAuth Flows  
    * Authorization code grant
* Allowed OAuth Scopes
    * Openid
* Save app client settings
* Domain name
    * Can enter any domain name as long as it’s unique
</details>

### Navigate to Sign Up/In Page and Try Adding Users
Access the sign-up and sign-in pages hosted by Cognito, by replacing the `<DOMAIN_PREFIX>`, `<REGION>`, and `<CLIENT_ID>` values in the URL below and navigating to it in the browser.

```
https://<DOMAIN_PREFIX>.auth.<REGION>.amazoncognito.com/login?response_type=code&client_id=<CLIENT_ID>&redirect_uri=https://www.google.com
```

<details>
    <summary>Steps - Navigate to Sign Up/In Page and Try Adding Users</summary>

* Copy the domain name prefix set in the domain settings and replace `<DOMAIN_PREFIX>`. 
* Replace `<REGION>` with the region you are working in (most likely `us-west-2`.)
* Copy the client id from the `App clients` or `App client settings` section in the User Pool configuration page and replace `<CLIENT_ID>`.
</details>

### LEVEL 1 RESULT
From the sign-up and sign-in page you can successfully:

* create a user
* verify the user
* login as the user
* after login, redirected to google.com

## LEVEL 2 - Setup the Workflow Triggers and Configure Lambda Tests
Deploy the UserPoolTriggers Lambda function and hook it up to the triggers in the user pool. View CloudWatch logs and setup the Lambda function for easier development.

### Add User Pool ARN to Policy
Edit the file `UserPoolTriggers/aws/lambda-cognito-user-pool-triggers-role-policy.json` and replace `<USER_POOL_ARN>` with your user pool arn.

### Create the Lambda role
Create the role`lambdasharp_cognito_user_pool_triggers_role` that will be used in a Lambda function.

<details>
    <summary>Steps - Create the Lambda role</summary>
    
```Shell
cd UserPoolTriggers/aws

aws iam create-role --role-name lambdasharp_cognito_user_pool_triggers_role --assume-role-policy-document file://lambda-cognito-user-pool-triggers-role-trust.json --profile lambdasharp

aws iam put-role-policy --role-name lambdasharp_cognito_user_pool_triggers_role --policy-name lambdasharp_cognito_user_pool_triggers_policy --policy-document file://lambda-cognito-user-pool-triggers-role-policy.json --profile lambdasharp
```
</details>

### Deploy the Lambda function
From the directory `UserPoolTriggers/src/UserPoolTriggers`, deploy the Lambda function and name it`lambdasharp_user_pool_triggers`.

<details>
    <summary>Steps - Deploy the Lambda function</summary>
    
```Shell
cd UserPoolTriggers/src/UserPoolTriggers

dotnet restore

dotnet lambda deploy-function lambdasharp_user_pool_triggers
```

>NOTE: repeat deploy-function command for subsequent deployments

</details>

### Hook up the User Pool Triggers

Set the Lambda function `lambdasharp_user_pool_triggers` as the triggers in the user pool. At minimum `Pre sign-up`, `Custom message` and `Post confirmation`.

Go through the process of signing up a user again.

<details>
	<summary>Steps - Hook up the User Pool Triggers</summary>
	
* Enable the Lambda function in the `Triggers > Pre sign-up` section of the user pool.
* Enable the Lambda function in the `Triggers > Custom message` section of the user pool.
* Enable the Lambda function in the `Triggers > Post confirmation` section of the user pool.
* Using the login URL from earlier, sign up a new user.
</details>

### Add Test Payloads to the Lambda function

After signing up a user, find the events from the signup in CloudWatch. Use Lambda's **Save Test Events** feature to create 3 test events using the payloads found in CloudWatch. Use these saved test events in the next levels.

<details>
	<summary>Steps - Add Test Payloads to the Lambda function</summary>

* Go to the Lambda Console for `lambdasharp_user_pool_triggers`. Click the `monitoring` tab, then `View logs in CloudWatch`.
* Look at the CloudWatch logs and find the payloads for `PreSignUp_SignUp`, `CustomMessage_SignUp`, `PostConfirmation_ConfirmSignUp`.
* Back at the Lambda Console for `lambdasharp_user_pool_triggers`, next to the `Test` button, choose `Configure test events` from the drop down.
* Add `PreSignUpSignUp`, `CustomMessageSignUp`, and `PostConfirmationConfirmSignUp` test events using their respective payloads from CloudWatch.
* This will help speed up development and testing without having to fill out the sign up form every time.

</details>

### LEVEL 2 RESULT

* You can successfully trigger the Lambda function with each of the test events 

## LEVEL 3 - Add Pre sign-up Workflow Trigger
Pre sign-up triggers will run just after the basic form validations and before the user data is saved to the user pool. If the user does not pass the pre signup, their information is not saved.
 
Edit `UserPoolTriggers/Triggers/CognitoPreSignup.cs` to:

* decline sign-ups that DO NOT have a phone number that starts with `619`, `858`, or `760`
* bypass the verification for sign-ups that have a phone number that starts with `858`
* no username can be admin
* only allow emails from a whiteList (can be done from UI in lieu of code as well)

Use the `PreSignUpSignUp` test event for debugging.

### LEVEL 3 RESULT
* Only users that pass all the validations above have an identity in the user pool

## LEVEL 4 - Add Custom Message Signup Workflow Trigger
Custom message signup triggers after successful signup and before the verification code is sent.

Edit `UserPoolTriggers/Triggers/CognitoCustomMessage.cs` to:
* use custom messages in the custom verification message and code to your user

Use the `CustomMessageSignUp` test event for debugging.

### LEVEL 4 RESULT
* Receive an SMS or email with a custom message and a verification code.

## LEVEL 5 - Add Post Confirmation Workflow Trigger
Post Confirmation triggers just after their verification code is confirmed.

In the User Pool attributes:

* Add custom attributes to the user pool:
    * string - `s3_website_url`
    * string - `s3_bucket`
    * >NOTE: custom attributes do not display on built-in sign up page.
    * >NOTE: custom attribute names allow max 20 of characters

Edit `UserPoolTriggers/Triggers/CognitoCustomMessage.cs` to:

* create an s3 Website Bucket for the verified user.
* populate the user s3 Website with the files from `UserPoolTriggers/Resources` 
* populate the custom attributes with the s3 bucket created for that user.

Use the `PostConfirmationConfirmSignUp` test event for debugging.

</details>

### LEVEL 5 RESULT
* There is an S3 Website bucket for the user.
* The custom attributes for the user are populated with the correct values.
* The URL from the user attributes loads the index.html Webpage.
