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
            new SecurityRules { HttpMethod = "POST", Pattern = @"^/payment/webhook$", AllowedRoles = null },
            #endregion

            #region Order Endpoints
            new SecurityRules { HttpMethod = "POST", Pattern = @"^/order$", AllowedRoles = [Role.CUSTOMER.ToString()] },
            new SecurityRules { HttpMethod = "PATCH", Pattern = @"^/order/.*$", AllowedRoles = [Role.ADMIN.ToString()] },
            new SecurityRules { HttpMethod = "GET", Pattern = @"^/order/[^/]+$", AllowedRoles = [Role.ADMIN.ToString()] },
            new SecurityRules { HttpMethod = "GET", Pattern = @"^/order/active$", AllowedRoles = [Role.ADMIN.ToString()] },
            #endregion

            #region Catalog Endpoints
            new SecurityRules { HttpMethod = "GET", Pattern = @"^/catalog.*$", AllowedRoles = [Role.ADMIN.ToString(), Role.CUSTOMER.ToString()] },
            new SecurityRules { HttpMethod = "POST", Pattern = @"^/catalog.*$", AllowedRoles = [Role.ADMIN.ToString()] },
            new SecurityRules { HttpMethod = "PATCH", Pattern = @"^/catalog.*$", AllowedRoles = [Role.ADMIN.ToString()] },
            new SecurityRules { HttpMethod = "PUT", Pattern = @"^/catalog.*$", AllowedRoles = [Role.ADMIN.ToString()] },
            new SecurityRules { HttpMethod = "DELETE", Pattern = @"^/catalog.*$", AllowedRoles = [Role.ADMIN.ToString()] },
            #endregion

            #region Payment Endpoints
            new SecurityRules { HttpMethod = "GET", Pattern = @"^/payment/[^/]+/qrCode$", AllowedRoles = [Role.ADMIN.ToString(), Role.CUSTOMER.ToString()] },
            new SecurityRules { HttpMethod = "GET", Pattern = @"^/payment/[^/]+/status$", AllowedRoles = [Role.ADMIN.ToString(), Role.CUSTOMER.ToString()] },
            new SecurityRules { HttpMethod = "GET", Pattern = @"^/payment/[^/]+/latest$", AllowedRoles = [Role.ADMIN.ToString()] },
            new SecurityRules { HttpMethod = "GET", Pattern = @"^/payment/merchant_orders/[^/]+$", AllowedRoles = [Role.ADMIN.ToString()] },
            #endregion
        ];
    }
}
