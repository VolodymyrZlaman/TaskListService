using System.Linq.Expressions;
using AutoMapper;
using NSubstitute;
using TaskListService.Application.Contracts.Persistence;
using TaskListService.Application.Exceptions;
using TaskListService.Application.Services;
using TaskListService.Application.Services.Commands;
using TaskListService.Application.Services.Vm;
using TaskService.Domain.Entities;
using Xunit;

namespace TaskListService.Application.Tests;

public class TaskListServiceTests
{
    private readonly Services.TaskListService _service;
    private readonly ITaskListRepository _repository = Substitute.For<ITaskListRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();

    public TaskListServiceTests()
    {
        _service = new Services.TaskListService(_repository, _mapper);
    }

    [Fact]
    public async Task CreateTaskListAsync_ShouldCreateTaskList()
    {
        // Arrange
        var command = new CreateTaskListCommand { OwnerId = "user1", TaskListId = "task1", Name = "Test TaskList" };
        var taskList = new TaskList { Id = "task1", OwnerId = "user1", Name = "Test TaskList", CreationDate = DateTime.Now };
        
        _mapper.Map<TaskList>(command).Returns(taskList);
        _repository.CreateAsync(taskList).Returns(taskList);
        _mapper.Map<TaskListItemVm>(taskList).Returns(new TaskListItemVm { Id = "task1", Name = "Test TaskList", OwnerId = "user1" });
        
        // Act
        var result = await _service.CreateTaskListAsync(command);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("task1", result.Id);
    }

    [Fact]
    public async Task GetTaskListItemAsync_ShouldReturnTaskList_WhenExists()
    {
        // Arrange
        var taskList = new TaskList { Id = "task1", OwnerId = "user1", Name = "Test TaskList" };
        _repository.GetOneByFilterAsync(Arg.Any<Expression<Func<TaskList, bool>>>(), CancellationToken.None).Returns(taskList);
        _mapper.Map<TaskListItemVm>(taskList).Returns(new TaskListItemVm { Id = "task1", Name = "Test TaskList", OwnerId = "user1" });
        
        // Act
        var result = await _service.GetTaskListItemAsync("task1", "user1");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("task1", result.Id);
    }

    [Fact]
    public async Task DeleteTaskListAsync_ShouldCallRepository_WhenTaskExists()
    {
        // Act
        await _service.DeleteTaskListAsync("task1", "user1");
        
        // Assert
        await _repository.Received(1).DeleteAsync(Arg.Any<Expression<Func<TaskList, bool>>>());
    }

    [Fact]
    public async Task ShareTaskListAsync_ShouldCallRepository()
    {
        // Act
        await _service.ShareTaskListAsync("task1", "targetUser", "user1");
        
        // Assert
        await _repository.Received(1).ShareTaskListAsync("task1", "targetUser", "user1");
    }

    [Fact]
    public async Task UnshareTaskListAsync_ShouldCallRepository()
    {
        // Act
        await _service.UnshareTaskListAsync("task1", "targetUser", "user1");
        
        // Assert
        await _repository.Received(1).UnshareTaskListAsync("task1", "targetUser", "user1");
    }
}
