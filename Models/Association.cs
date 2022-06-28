#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Exam.Models;
public class Association
{
    [Key]
    public int AssociationId {get;set;}
    public int UserId {get;set;}
    public User? Participant {get;set;}
    public int MeetUpId {get;set;}
    public MeetUp? MeetUp {get;set;}
}