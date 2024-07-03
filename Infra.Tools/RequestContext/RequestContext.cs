using Domain.Interfaces;

namespace Infra.Tools.RequestContext;

public class RequestContext : IRequestContext
{
    public string UserId { get; set; }
}