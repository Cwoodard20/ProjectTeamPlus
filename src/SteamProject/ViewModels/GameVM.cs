using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SteamProject.Models;
using SteamProject.DAL.Abstract;
using SteamProject.DAL.Concrete;
using Microsoft.AspNetCore.Mvc.Rendering;
using SteamProject.Models.DTO;
using Newtonsoft.Json.Linq;

namespace SteamProject.ViewModels
{
    public class GameVM
    {
        public int _appId {get; set;}
        public Game _game {get; set;}
        public GameInfoPOCO _poco {get; set;}

        public class Requirements 
        {
            public string? _minimum {get; set;}
            public string? _operatingSystem {get; set;}
            public string? _processor {get; set;}
            public string? _memory {get; set;}
            public string? _graphics {get; set;}
            public string? _storage {get; set;}
        }
        public GameVM()
        {
        }

        public void cleanDescriptions()
        {
            this._poco.response.data.detailed_description = Regex.Replace(this._poco.response.data.detailed_description, @"<[^>]+>|&nbsp;", "").Trim();
            this._poco.response.data.short_description = Regex.Replace(this._poco.response.data.short_description, @"<[^>]+>|&nbsp;", "").Trim();
        }
        public void cleanRequirements()
        {
            //These are the try catch to remove the HTML styling that Steam provides with the requirement descriptions.
            
            //Linux Minimum
            try
            {
                this._poco.response.data.linux_requirements.minimum = Regex.Replace(this._poco.response.data.linux_requirements.minimum, @"<[^>]+>|&nbsp;", " ").Trim();
            }
            catch 
            {
                this._poco.response.data.linux_requirements.minimum = "Steam doesn\'t provide minimum linux requirements for this title.";
            }
            //Mac minimum
            try
            {
                this._poco.response.data.mac_requirements.minimum = Regex.Replace(this._poco.response.data.mac_requirements.minimum, @"<[^>]+>|&nbsp;", " ").Trim();
            }
            catch 
            {
                this._poco.response.data.mac_requirements.minimum = "Steam doesn\'t provide minimum mac requirements for this title.";
            }
            //PC minimum
            try
            {
                this._poco.response.data.pc_requirements.minimum = Regex.Replace(this._poco.response.data.pc_requirements.minimum, @"<[^>]+>|&nbsp;", " ").Trim();
            }
            catch 
            {
                this._poco.response.data.pc_requirements.minimum = "Steam doesn\'t provide minimum pc requirements for this title.";
            }

            //PC recommended
            try
            {
                this._poco.response.data.pc_requirements.recommended = Regex.Replace(this._poco.response.data.pc_requirements.recommended, @"<[^>]+>|&nbsp;", " ").Trim();
            }
            catch 
            {
                this._poco.response.data.pc_requirements.recommended = "Steam doesn\'t provide recommended pc requirements for this title.";
            }
            //Linux recommended
            try
            {
                this._poco.response.data.linux_requirements.recommended = Regex.Replace(this._poco.response.data.linux_requirements.recommended, @"<[^>]+>|&nbsp;", " ").Trim();
            }
            catch 
            {
                this._poco.response.data.linux_requirements.recommended = "Steam doesn\'t provide recommended linux requirements for this title.";
            }
            //Mac recommended
            try
            {
                this._poco.response.data.mac_requirements.recommended = Regex.Replace(this._poco.response.data.mac_requirements.recommended, @"<[^>]+>|&nbsp;", " ").Trim();
            }
            catch 
            {
                this._poco.response.data.mac_requirements.recommended = "Steam doesn\'t provide recommended mac requirements for this title.";
            }
        }

        public void GetRequirementsFromJson(List<string> pcRequirements, List<string> linuxRequirements, List<string> macRequirements)
        {
            string pcMinRequirements = pcRequirements[0];
            string pcRecommendedRequirements = pcRequirements[1];
            string linuxMinRequirements = pcRequirements[0];
            string linuxRecommendedRequirements = pcRequirements[1];
            string macMinRequirements = pcRequirements[0];
            string macRecommendedRequirements = pcRequirements[1];
            
            JObject pcMinRequirementsJSON = JObject.Parse(pcMinRequirements);
            JObject linuxMinRequirementsJSON = JObject.Parse(linuxMinRequirements);
            JObject macMinRequirementsJSON = JObject.Parse(macMinRequirements);
            JObject pcRecommendedRequirementsJSON = JObject.Parse(pcRecommendedRequirements);
            JObject linuxRecommendedRequirementsJSON = JObject.Parse(linuxRecommendedRequirements);
            JObject macRecommendedRequirementsJSON = JObject.Parse(macRecommendedRequirements);
        }
    }

    
}