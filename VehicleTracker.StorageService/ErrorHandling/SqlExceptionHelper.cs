using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace VehicleTracker.StorageService.ErrorHandling
{
    public static class SqlExceptionHelper
    {
        public static bool IsDuplicateError(this DbUpdateException ex)
        {
            var sqlEx = ex.InnerException as SqlException;
            return sqlEx?.Number == 2601 || sqlEx?.Number == 2627;
        }
    }
}
