using System.Collections.Generic;
using CommandAPI.Models;
using Microsoft.AspNetCore.Mvc;
using CommandAPI.Data;
using AutoMapper;
using CommandAPI.Dtos;
using Microsoft.AspNetCore.JsonPatch;
namespace CommandAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandAPIRepo _repository;
        private readonly IMapper _mapper;
        public CommandsController(ICommandAPIRepo repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // [HttpGet]
        // public ActionResult<IEnumerable<Command>> GetAllCommands()
        // {
        //     var commandItems = _repository.GetAllCommands();
        //     return Ok(commandItems);
        // }


        // [HttpGet("{id}")]
        // public ActionResult<Command> GetCommandById(int id)
        // {
        //     var commandItem = _repository.GetCommandById(id);
        //     if (commandItem == null)
        //     {
        //         return NotFound();
        //     }
        //     return Ok(commandItem);

        // }

        [HttpPost]
        public ActionResult <CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto)
        {
            var commandModel = _mapper.Map<Command>(commandCreateDto);
            _repository.CreateCommand(commandModel);
            _repository.SaveChanges();
            var commandReadDto = _mapper.Map<CommandReadDto>(commandModel);
            return CreatedAtRoute(nameof(GetCommandById), new {Id = commandReadDto.Id}, commandReadDto);
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands()
        {
            var commandItems = _repository.GetAllCommands();
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
        }

        [HttpGet("{id}",Name="GetCommandById")]
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var commandItem = _repository.GetCommandById(id);
            if (commandItem == null){
                return NotFound();
            }
            return Ok(_mapper.Map<CommandReadDto>(commandItem));
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);
            if (commandModelFromRepo == null)
            {
            return NotFound();
            }
            _mapper.Map(commandUpdateDto, commandModelFromRepo);
            _repository.UpdateCommand(commandModelFromRepo);
            _repository.SaveChanges();
            return NoContent();
       }

       [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id,JsonPatchDocument<CommandUpdateDto> patchDoc)
        {
        var commandModelFromRepo = _repository.GetCommandById(id);
        if(commandModelFromRepo == null)
        {
        return NotFound();
        }
        var commandToPatch = _mapper.Map<CommandUpdateDto>(commandModelFromRepo);
        patchDoc.ApplyTo(commandToPatch, ModelState);
        if(!TryValidateModel(commandToPatch))
        {
        return ValidationProblem(ModelState);
        }
        _mapper.Map(commandToPatch, commandModelFromRepo);
        _repository.UpdateCommand(commandModelFromRepo);
        _repository.SaveChanges();
        return NoContent();
        }

    }
}