using ERP.DATA.Services.UserService;
using ERP.TRAN.CrossLayers.API.Users.Requests;
using ERP.TRAN.CrossLayers.API.Users.Responses;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Usuarios;

public partial class List : ComponentBase
{
    [Inject] private UserManager UserManager { get; set; } = null!;

    private List<UserDetailDto> _users = [];
    private bool _isLoading = true;
    private string? _errorMessage;
    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalPages = 1;
    private string? _searchQuery;

    protected override async Task OnInitializedAsync()
    {
        await GetUsers();
    }

    private async Task GetUsers()
    {
        _isLoading = true;
        _errorMessage = null;

        var request = new ListUsersRequest(_currentPage, _pageSize)
        {
            Search = _searchQuery
        };

        var result = await UserManager.List(request);

        if (result.IsSuccess)
        {
            _users = result.Value;
            _totalPages = result.Value.TotalPages;
        }
        else
        {
            _errorMessage = result.Error.Message;
        }

        _isLoading = false;
    }

    private async Task HandleSearch(ChangeEventArgs e)
    {
        _searchQuery = e.Value?.ToString();
        _currentPage = 1;
        await GetUsers();
    }

    private async Task PreviousPage()
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            await GetUsers();
        }
    }

    private async Task NextPage()
    {
        if (_currentPage < _totalPages)
        {
            _currentPage++;
            await GetUsers();
        }
    }
}