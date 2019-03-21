using GrainInterfaces;
using Orleans;
using Orleans.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grains
{
    [StorageProvider(ProviderName = "default")]
    public class VolunteerListGrain : Grain<NameListState>, IVolunteerListGrain
    {
        public VolunteerListGrain()
        {
            
        }

        public override async Task OnActivateAsync()
        {
            this.State = new NameListState { Names = new List<string>() };
            await WriteStateAsync();
        }

        public async Task AddNameAsync(string name)
        {
            this.State.Names.Add(name);
            await WriteStateAsync();
        }

        public async Task<List<string>> GetNames()
        {
            await ReadStateAsync();
            return State.Names;
        }
    }
}
