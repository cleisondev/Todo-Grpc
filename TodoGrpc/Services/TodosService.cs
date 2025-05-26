using Grpc.Core;
using Microsoft.EntityFrameworkCore;
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

        public override async Task<GetAllResponse> ListToDo(GetAllRequest request,ServerCallContext context)
        {
            try
            {
                var response = new GetAllResponse();
                var todoItens = await _dbContext.ToDoItems.ToListAsync();

                foreach (var item in todoItens)
                {
                    response.ToDo.Add(new ReadToDoResponse
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Description = item.Description,
                        ToDoStatus = item.ToDoStatus
                    });

                }

                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
        {
            try
            {
                if(request.Id < 0)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Id must be grater than 0"));

                var todoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(x => x.Id == request.Id);

                if (todoItem == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Todo item not found"));

                todoItem.Title = request.Title;
                todoItem.Description = request.Description;
                todoItem.ToDoStatus = request.ToDoStatus;

                await _dbContext.SaveChangesAsync();

                return await Task.FromResult(new UpdateToDoResponse
                {
                    Id = todoItem.Id
                });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext contexto)
        {
            try
            {
                if (request.Id < 0)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Id must be grater than 0"));

                var todoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(x => x.Id == request.Id);

                if (todoItem == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Todo item not found"));

                 _dbContext.Remove(todoItem);

                await _dbContext.SaveChangesAsync();

                return await Task.FromResult(new DeleteToDoResponse
                {
                    Id = todoItem.Id
                });
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
