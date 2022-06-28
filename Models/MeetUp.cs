#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Exam.Models;
public class MeetUp
{
    [Key]
    public int MeetUpId {get;set;}
    [Required]
    public string Title {get;set;}
    [Required]
    public DateTime DateAndTime {get;set;}
    [Required]
    public int Duration {get;set;}
    [Required]
    public string Description {get;set;}
    [Required]
    public string Unit {get;set;}
    [Required]
    public int UserId {get;set;}
    public User? Coordinator {get;set;}
    public List<Association> UsersParticipating {get;set;} = new List<Association>();
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
}