using Grpc.Core;
using TodoGrpc;
using TodoGrpc.Data;
using TodoGrpc.Models;

namespace TodoGrpc.Services
{
    public class TodosService : Todoit.TodoitBase
    {
        private readonly AppDbContext _dbContext;

        public TodosService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<CreateTodoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
        {
            if(request.Title == string.Empty || request.Description == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Title and Description cannot be empty"));


            var todoItem = new ToDoItem
            {
                Title = request.Title,
                Description = request.Description
            };

            _dbContext.AddAsync(todoItem);
            _dbContext.SaveChangesAsync();

            return Task.FromResult(new CreateTodoResponse
            {
                Id = todoItem.Id
            });
        }
    }
}
