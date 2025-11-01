using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.TRAN.CrossLayers.Core.Utilities.Base.Requests;
public abstract class GetByIdRequest
{
    public virtual Guid Id { get; init; }
}
