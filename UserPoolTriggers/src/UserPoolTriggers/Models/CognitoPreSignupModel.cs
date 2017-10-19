using System.Collections.Generic;
using Newtonsoft.Json;

namespace UserPoolTriggers.Models
{
    public class PreSignupRequest
    {
        [JsonProperty("userAttributes")]
        public Dictionary<string, string> UserAttributes { get; set; }
        
        [JsonProperty("validationData")]
        public Dictionary<string,string> ValidationData { get; set; }
    }

    public class PreSignupResponse
    {
        [JsonProperty("autoConfirmUser")]
        public bool AutoConfirmUser { get; set; }

        [JsonProperty("autoVerifyEmail")]
        public bool AutoVerifyEmail { get; set; }

        [JsonProperty("autoVerifyPhone")]
        public bool AutoVerifyPhone { get; set; }
    }

    public class PreSignupCallerContext
    {
        [JsonProperty("awsSdkVersion")]
        public string AwsSdkVersion { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }
    }

    public class PreSignupBase
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
        public PreSignupCallerContext CallerContext { get; set; }

        [JsonProperty("request")]
        public PreSignupRequest Request { get; set; }

        [JsonProperty("response")]
        public PreSignupResponse Response { get; set; }

        [JsonProperty("userName", NullValueHandling = NullValueHandling.Ignore)]
        public string UserName { get; set; }
    }

}