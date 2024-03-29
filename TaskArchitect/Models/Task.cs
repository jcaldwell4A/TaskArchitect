using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace TaskArchitect.Models
    {
        public class Task
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public string Title { get; set; }

            public string Description { get; set; }

            public DateTime? DueDate { get; set; }

            public int Priority { get; set; }

            [Required]
            [ForeignKey("User")]
            public int UserId { get; set; }

            public User User { get; set; }
        }
    }

