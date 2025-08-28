using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Domain.TodoTasks
{
    public class TodoTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public User CreatedBy { get; set; } = default!;
        public int CreatedById { get; set; }
        //public User AssignedTo { get; set; } = default!;
        public int AssignedToById { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PriorityStatus PriorityStatus { get; set; } = PriorityStatus.Low;
        public TodoTaskStatus TodoTaskStatus { get; set; } = TodoTaskStatus.Todo;
    }
}
