using System.ComponentModel.DataAnnotations;

namespace TodoListApi.Models
{
    public class TodoItemDto
    {
        public Guid? Id { get; set; }

        [Required, MinLength(1), MaxLength(200)]
        public required string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;

    }
}
