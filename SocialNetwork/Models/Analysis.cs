using Microsoft.AspNetCore.Mvc;
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
        [BindProperty(Name = "a")]
        public string Time { get; set; }

        [BindProperty(Name = "b")]
        public string IsShort { get; set; }

        [BindProperty(Name = "c")]
        public string EnterPrice { get; set; }

        [BindProperty(Name = "d")]
        public string StopGain { get; set; }

        [BindProperty(Name = "e")]
        public string StopLoss { get; set; }

        [BindProperty(Name = "f")]
        public string Drawing { get; set; }

        [BindProperty(Name = "g")]
        public string Template { get; set; }

        public bool ReachedGain { get; set; }

        public bool ReachedLoss { get; set; }

        [Required]
        [Range(0, long.MaxValue)]
        public long ReachedDate { get; set; }
    }

    public class SearchPostWithAnalysisDto : SearchPostDto
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

        public bool ReachedGain { get; set; }

        public bool ReachedLoss { get; set; }

        [Required]
        [Range(0, long.MaxValue)]
        public long ReachedDate { get; set; }
    }

    public class SinglePostWithAnalysisDto : SearchPostWithAnalysisDto
    {
        [JsonProperty("post_author")]
        public SimpleUserDto Author { get; set; }
    }

    public class UpdateAnalysisOrder
    {
        public bool ReachedGain { get; set; }

        public bool ReachedLoss { get; set; }

        [Required]
        [Range(0, long.MaxValue)]
        public long ReachedDate { get; set; }
    }
}
