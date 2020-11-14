using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RequestPercolator.API.Dto
{
    public sealed class PercolationWithFilterDto : PercolationDto
    {
        [Required(AllowEmptyStrings = false)]
        [BindProperty(Name = InternalConstants.FilterKey)]
        public string Filter { get; set; }
    }
}
