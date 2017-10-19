using System.Collections.Generic;
using Newtonsoft.Json;

namespace UserPoolTriggers.Models
{
     
    public class CustomMessageRequest
    {
        [JsonProperty("userAttributes")]
        public Dictionary<string, string> UserAttributes { get; set; }
        
        [JsonProperty("codeParameter")]
        public string CodeParameter { get; set; }
        
        [JsonProperty("usernameParameter")]
        public string usernameParameter { get; set; }
        
    }

    public class CustomMessageResponse
    {
        [JsonProperty("smsMessage")]
        public string SmsMessage { get; set; }
        
        [JsonProperty("emailMessage")]
        public string EmailMessage { get; set; }
        
        [JsonProperty("emailSubject")]
        public string EmailSubject { get; set; }
        
    }

    public class CustomMessageCallerContext
    {
        [JsonProperty("awsSdkVersion")]
        public string AwsSdkVersion { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }
    }

    public class CustomMessageBase
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("triggerSource")]
        public string TriggerSource { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("userPoolId")]
        public string UserPoolId { get; set; }

        [JsonProperty("callerContext")]
        public CustomMessageCallerContext CallerContext { get; set; }

        [JsonProperty("request")]
        public CustomMessageRequest Request { get; set; }
        
        [JsonProperty("response")]
        public CustomMessageResponse Response { get; set; }

        [JsonProperty("userName", NullValueHandling = NullValueHandling.Ignore)]
        public string UserName { get; set; }
    }

}
