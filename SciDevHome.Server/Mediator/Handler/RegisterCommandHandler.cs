using MediatR;
using SciDevHome.Data;
using SciDevHome.Server.Mediator.Command;

namespace SciDevHome.Server.Mediator.Handler
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, HomeUser>
    {
        public Task<HomeUser> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
