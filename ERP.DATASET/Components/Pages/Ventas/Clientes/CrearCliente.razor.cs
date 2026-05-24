using ERP.DATA.Services.VentasService.ClientService;
using ERP.TRAN.CrossLayers.API.Pos.Clients.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Clients.Requests;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Ventas.Clientes;

public partial class CrearCliente
{
    [Inject] private ClientService ClientService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private readonly ClientForm _form = new();
    private bool _saving;
    private string? _error;

    private async Task Guardar()
    {
        _saving = true;
        _error = null;
        try
        {
            var request = new CreateClientRequest
            {
                Name = _form.Name,
                DniType = _form.DniType,
                IdentificationNumber = _form.IdentificationNumber,
                PhoneNumber = _form.PhoneNumber,
                Address = _form.Address,
                City = _form.City,
                _CreatorAuth0Id = "system"
            };

            await ClientService.CreateAsync(request, CancellationToken.None);
            Navigation.NavigateTo("/clientes");
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _saving = false;
        }
    }

    private sealed class ClientForm
    {
        public string Name { get; set; } = "";
        public DniType DniType { get; set; } = DniType.cc;
        public string IdentificationNumber { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
    }
}
