using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    public class Analysis : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Drawing { get; set; }

        [Required]
        public string Template { get; set; }

        [Range(0, long.MaxValue)]
        public long Time { get; set; }

        public bool IsShort { get; set; }

        public double EnterPrice { get; set; }

        public double StopGain { get; set; }

        public double StopLoss { get; set; }

        public bool ReachedGain { get; set; }

        public bool ReachedLoss { get; set; }

        [Range(0, long.MaxValue)]
        public long ReachedDate { get; set; }
    }

    public class PostWithAnalysisCuOrder : PostCuOrder
    {
        [JsonProperty("a")]
        public string Time { get; set; }

        [JsonProperty("b")]
        public string IsShort { get; set; }

        [JsonProperty("c")]
        public string EnterPrice { get; set; }

        [JsonProperty("d")]
        public string StopGain { get; set; }

        [JsonProperty("e")]
        public string StopLoss { get; set; }

        [JsonProperty("f")]
        public string Drawing { get; set; }

        [JsonProperty("g")]
        public string Template { get; set; }
    }

    public class PostWithAnalysisDto : SearchPostDto
    {
        [JsonProperty("a")]
        public long Time { get; set; }

        [JsonProperty("b")]
        public bool IsShort { get; set; }

        [JsonProperty("c")]
        public double EnterPrice { get; set; }

        [JsonProperty("d")]
        public double StopGain { get; set; }

        [JsonProperty("e")]
        public double StopLoss { get; set; }

        [JsonProperty("f")]
        public string Drawing { get; set; }

        [JsonProperty("g")]
        public string Template { get; set; }
    }
}
