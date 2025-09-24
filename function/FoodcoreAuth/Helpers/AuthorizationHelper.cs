using System.Text.RegularExpressions;
using Foodcore.Auth.Model;

namespace Foodcore.Auth.Helpers
{
    /// <summary>
    /// Classe auxiliar que define as regras de autorização para diferentes endpoints.
    /// </summary>
    public static class AuthorizationHelper
    {
        /// <summary>
        /// Representa o conjunto de regras de segurança para autorização.
        /// </summary>
        public static readonly List<SecurityRules> Rules =
        [
            #region Public Endpoints
            new SecurityRules { HttpMethod = "POST", Pattern = @"^/payments/webhook$", AllowedRoles = null },
            #endregion

            #region Order Endpoints
            new SecurityRules { HttpMethod = "POST", Pattern = @"^/orders$", AllowedRoles = [Role.CUSTOMER.ToString()] },
            new SecurityRules { HttpMethod = "PATCH", Pattern = @"^/orders/.*$", AllowedRoles = [Role.ADMIN.ToString()] },
            new SecurityRules { HttpMethod = "GET", Pattern = @"^/orders/.*$", AllowedRoles = [Role.ADMIN.ToString()] },
            new SecurityRules { HttpMethod = "GET", Pattern = @"^/orders/active$", AllowedRoles = [Role.ADMIN.ToString()] },
            #endregion

            #region Catalog Endpoints
            new SecurityRules { HttpMethod = "GET", Pattern = @"^/catalogs/.*$", AllowedRoles = [Role.ADMIN.ToString(), Role.CUSTOMER.ToString()] },
            new SecurityRules { HttpMethod = "POST", Pattern = @"^/catalogs/.*$", AllowedRoles = [Role.ADMIN.ToString()] },
            new SecurityRules { HttpMethod = "PATCH", Pattern = @"^/catalogs/.*$", AllowedRoles = [Role.ADMIN.ToString()] },
            new SecurityRules { HttpMethod = "PUT", Pattern = @"^/catalogs/.*$", AllowedRoles = [Role.ADMIN.ToString()] },
            new SecurityRules { HttpMethod = "DELETE", Pattern = @"^/catalogs/.*$", AllowedRoles = [Role.ADMIN.ToString()] },
            #endregion

            #region Payment Endpoints
            new SecurityRules { HttpMethod = "GET", Pattern = @"^/payments/[^/]+/qrCode$", AllowedRoles = [Role.ADMIN.ToString(), Role.CUSTOMER.ToString()] },
            new SecurityRules { HttpMethod = "GET", Pattern = @"^/payments/[^/]+/status$", AllowedRoles = [Role.ADMIN.ToString(), Role.CUSTOMER.ToString()] },
            new SecurityRules { HttpMethod = "GET", Pattern = @"^/payments/merchant_orders/[^/]+$", AllowedRoles = [Role.ADMIN.ToString()] },
            #endregion
        ];
    }
}
