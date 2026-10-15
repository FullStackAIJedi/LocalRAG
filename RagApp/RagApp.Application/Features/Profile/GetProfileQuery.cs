namespace RagApp.Application.Features.Profile
{ 
    using MediatR;
    using RagApp.Application.Dtos;
     
    public record GetProfileQuery(Guid UserId) : IRequest<UserProfileDto>;
}
