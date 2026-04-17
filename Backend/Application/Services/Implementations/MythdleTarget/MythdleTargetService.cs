using Application.Constants.Errors;
using Application.Constants.Successes;
using Application.DTOs.MythdleTarget;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Application.Utils;
using Domain.Models.MythdleTarget;
using Domain.Shared;

namespace Application.Services.Implementations.MythdleTarget;

public class MythdleTargetService : IMythdleTargetService
{
    private readonly IMythdleTargetRepository _repository;

    public MythdleTargetService(IMythdleTargetRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SuccessApiResponse<GetPagedMythdleTargetsResponseDto>>> GetTargetsAsync(int pageNumber, int pageSize)
    {
        var targets = await _repository.GetPagedAsync(pageNumber, pageSize);

        var dto = new GetPagedMythdleTargetsResponseDto
        {
            Targets = targets.Select(t => new MythdleTargetDto
            {
                Name = t.Name,
                Category = t.Category,
                IsFake = t.IsFake,
                Description = t.Description
            })
        };

        return MythdleTargetSuccesses.TargetsFetchedPaged(dto);
    }

    public async Task<Result<SuccessApiResponse<CreateMythdleTargetResponseDto>>> CreateTargetAsync(CreateMythdleTargetDto request)
    {
        var existingTarget = await _repository.GetByNameAsync(request.Name);
        if (existingTarget is not null)
        {
            return Result<SuccessApiResponse<CreateMythdleTargetResponseDto>>.Failure(MythdleTargetErrors.TargetNameAlreadyExists);
        }

        var creationParams = new MythdleTargetCreationParams
        {
            Name = request.Name,
            Category = request.Category,
            IsFake = request.IsFake,
            Description = request.Description
        };

        var target = new Domain.Models.MythdleTarget.MythdleTarget(creationParams);
        
        await _repository.AddAsync(target);
        await _repository.SaveChangesAsync();

        var responseDto = new CreateMythdleTargetResponseDto
        {
            Name = target.Name
        };

        return MythdleTargetSuccesses.TargetCreated(responseDto);
    }

    public async Task<Result<SuccessApiResponse>> DeleteTargetByNameAsync(string name)
    {
        var target = await _repository.GetByNameAsync(name);
        if (target is null)
        {
            return Result<SuccessApiResponse>.Failure(MythdleTargetErrors.TargetNotFound);
        }

        await _repository.DeleteAsync(target);
        await _repository.SaveChangesAsync();

        return MythdleTargetSuccesses.TargetDeleted();
    }
}
