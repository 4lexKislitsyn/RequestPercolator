using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RequestPercolator.API.Dto
{
    public class PercolationDto
    {
        [Url]
        [Required]
        [MinLength(8)]
        [BindProperty(Name = InternalConstants.DestinationKey)]
        public string Destination { get; set; }
    }
}
