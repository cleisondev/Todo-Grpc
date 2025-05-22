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

        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
        {
            try
            {
                if (request.Id < 0)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Id must be grater than 0"));

                ToDoItem? todoItem = _dbContext.ToDoItems.FirstOrDefault(x => x.Id == request.Id);

                if (todoItem == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Todo item not found"));

                return await Task.FromResult(new ReadToDoResponse
                {
                    Id = todoItem.Id,
                    Title = todoItem.Title,
                    Description = todoItem.Description
                });
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }
}
