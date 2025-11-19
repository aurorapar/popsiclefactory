using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class MetaDataDto
    {
        public DateTime DateCreated;
        public DateTime DateModified { get; set; }

        [Required]
        public readonly string Creator;

        [Required]
        public string Modifier { get; set; }

        public bool Enabled { get; set; }

        public MetaDataDto(DateTime dateCreated, DateTime? dateModified, string creator, string modifier, bool? enabled = null)
        {
            DateCreated = dateCreated;
            DateModified = dateModified is null ? dateCreated : (DateTime)dateModified;
            Creator = creator;
            Modifier = string.IsNullOrEmpty(modifier) ? creator : modifier;
            Enabled = enabled ?? true;
        }
    }
}
