using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IVolunteerListGrain : IGrainWithGuidKey
    {
        Task AddNameAsync(string name);

        Task<List<string>> GetNames();
    }
}
