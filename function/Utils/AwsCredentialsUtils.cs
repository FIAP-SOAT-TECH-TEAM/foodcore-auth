namespace Foodcore.Auth.Utils
{
    /// <summary>
    /// Utilitários para converter o conteúdo de credenciais da AWS em um dicionário.
    /// </summary>
    public static class AwsCredentialsUtils
    {
        /// <summary>
        /// Analisa uma string no formato do arquivo de credenciais da AWS e retorna os pares chave/valor.
        /// </summary>
        /// <param name="rawCreds">
        /// Conteúdo bruto do arquivo de credenciais (por exemplo, linhas como "aws_access_key_id=..." e "aws_secret_access_key=...").
        /// Cabeçalhos de seção como "[default]" ou "[profile xyz]" são ignorados.
        /// </param>
        /// <returns>
        /// Um dicionário com comparação de chave case-insensitive contendo as chaves e valores encontrados.
        /// Retorna vazio se a entrada for nula ou vazia.
        /// </returns>
        /// <remarks>
        /// - Linhas vazias e cabeçalhos de seção são ignorados.
        /// - Cada linha é dividida no primeiro '=' em duas partes (chave e valor), ambas com Trim().
        /// - As chaves são comparadas usando StringComparer.OrdinalIgnoreCase.
        /// </remarks>
        public static Dictionary<string, string> GetAwsCredentialsDict(string rawCreds)
        {
            var credsDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrEmpty(rawCreds))
            {
                foreach (var line in rawCreds.Split('\n'))
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("[")) continue;

                    var parts = trimmed.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        credsDict[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }

            if (credsDict.Count < 3)
            {
                throw new InvalidOperationException("AWS_CREDENTIALS is missing required keys.");
            }

            return credsDict;
        }
    }
}