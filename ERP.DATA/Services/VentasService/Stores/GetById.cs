using ERP.TRAN.CrossLayers.API.Pos.Stores.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Stores.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Terminals.Responses;
using ERP.TRAN.CrossLayers.API.Stores.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.Stores;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.Stores;

public partial class StoresManager
{
    /// <summary>
    /// Obtiene una tienda específica por su Id, incluyendo sus cajas y bodegas asociadas para el panel de edición.
    /// </summary>
    public async Task<StoreSummaryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var store = await _context.Store
            .AsNoTracking()
            .Include(s => s.Bodegas)
            .Include(s => s.Cajas)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (store == null) return null;

        return new StoreSummaryDto(
            store.Id,
            store.Name,
            store.Description,
            store.IsMainStore,
            store.IsActive,
            store.Bodegas.Count,
            store.Cajas.Count
        );
    }

    /// <summary>
    /// Crea una nueva sucursal/tienda en el sistema.
    /// </summary>
    public async Task<int> CreateAsync(CreateStoreRequest request, CancellationToken cancellationToken = default)
    {
        // Opcional: Si se marca como tienda principal, podríamos validar o apagar las demás, 
        // pero por ahora realizamos la inserción directa.
        var storeEntity = new Store
        {
            Name = request.Name,
            Description = request.Description,
            IsMainStore = request.IsMainStore,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Store.Add(storeEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return storeEntity.Id;
    }

    /// <summary>
    /// Actualiza la información de una tienda existente.
    /// </summary>
    public async Task UpdateAsync(int id, UpdateStoreRequest request, CancellationToken cancellationToken = default)
    {
        var storeEntity = await _context.Store
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (storeEntity == null)
        {
            throw new InvalidOperationException($"La tienda con ID {id} no fue encontrada.");
        }

        storeEntity.Name = request.Name;
        storeEntity.Description = request.Description;
        storeEntity.IsMainStore = request.IsMainStore;
        storeEntity.IsActive = request.IsActive;
        storeEntity.UpdatedAt = DateTime.UtcNow;

        _context.Store.Update(storeEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}