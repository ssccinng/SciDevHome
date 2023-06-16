using SciDevHome.Client.WinUI.Core.Models;

namespace SciDevHome.Client.WinUI.Core.Contracts.Services;

// Remove this class once your pages/features are using your data.
public interface ISampleDataService
{
    Task<IEnumerable<SampleOrder>> GetContentGridDataAsync();
}
