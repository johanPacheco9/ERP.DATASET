using ERP.TRAN.CrossLayers.API.Pos.Caja.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Terminals.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Shifts.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.CajaService;

public partial class CajaManager
{
    public async Task<PosTerminalDto> Create(CreateCajaRequest request)
    {
        // 1. Opcional: Validar que el código o prefijo no existan previamente
        bool codeExists = await _context.PosTerminals
            .AnyAsync(t => t.Code == request.Code);

        if (codeExists)
        {
            throw new InvalidOperationException($"Ya existe una terminal registrada con el código '{request.Code}'.");
        }

        // 2. Mapear el Request a la Entidad de Base de Datos (asumiendo que tu entidad se llama PosTerminal)
        var terminalEntity = new PosTerminal
        {
            Name = request.Name,
            Code = request.Code,
            StoreId = request.StoreId,
            WarehouseId = request.WarehouseId,
            Prefix = request.Prefix,
            CurrentConsecutive = request.CurrentConsecutive,
            DianResolutionNumber = request.DianResolutionNumber,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "SYSTEM"
        };

        // 3. Registrar en el contexto y guardar cambios
        _context.PosTerminals.Add(terminalEntity);
        await _context.SaveChangesAsync();

        // 4. Consultar la entidad recién creada con sus relaciones (Store y Warehouse) para construir el DTO de respuesta
        var createdTerminal = await _context.PosTerminals
            .Include(t => t.Store)
            .Include(t => t.Warehouse)
            .Include(t => t.Shifts)
            .ThenInclude(posShift => posShift.Usuarios) // Opcional: por si necesitas evaluar si tiene turnos activos
            .FirstAsync(t => t.Id == terminalEntity.Id);

        // Validar si tiene turno activo en el momento de la creación (por defecto recién creada no debería, pero se evalúa)
        var activeShift = createdTerminal.Shifts?.FirstOrDefault(s => s.Status == PosShiftStatus.Open);

        // 5. Retornar el PosTerminalDto mapeado
        return new PosTerminalDto(
            Id: createdTerminal.Id,
            Name: createdTerminal.Name,
            Code: createdTerminal.Code,
            StoreId: createdTerminal.StoreId,
            StoreName: createdTerminal.Store?.Name ?? "Sin Tienda",
            WarehouseId: createdTerminal.WarehouseId,
            WarehouseName: createdTerminal.Warehouse?.Name ?? "Sin Almacén",
            Prefix: createdTerminal.Prefix,
            CurrentConsecutive: createdTerminal.CurrentConsecutive,
            DianResolutionNumber: createdTerminal.DianResolutionNumber,
            IsActive: createdTerminal.IsActive,
            HasActiveShift: activeShift != null,
            ActiveShiftId: activeShift?.Id,
            ActiveCashierName: activeShift?.Usuarios.UserName
        );
    }
}