#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Exam.Models;
public class User
{
    [Key]
    public int UserId {get;set;}
    [Required]
    [MinLength(2)]
    public string Name {get;set;}
    [EmailAddress]
    [Required]
    public string Email {get;set;}
    [Required]
    [DataType(DataType.Password)]
    [MinLength(8)]
    public string Password {get;set;}
    [NotMapped]
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string PassConfirm {get;set;}
    public List<MeetUp> MeetUpsCoordinating {get;set;} = new List<MeetUp>();
    public List<Association> MeetUpsJoined {get;set;} = new List<Association>();
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
}