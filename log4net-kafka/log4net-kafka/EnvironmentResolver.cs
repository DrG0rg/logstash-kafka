using System;
using System.Text.RegularExpressions;

namespace log4net.kafka
{
    /// <summary>
    ///     Class used to resolve environment variables from strings.
    /// </summary>
    public static class EnvironmentResolver
    {
        /// <summary>
        ///     Resolves any occurrence of an environment variable in the provided <paramref name="input" />.
        /// </summary>
        /// <param name="input"><see cref="string" /> which is searched for occurrences of the "\$\{.*?\}" pattern.</param>
        /// <returns>The <paramref name="input" /> with all replacements.</returns>
        /// <remarks>
        ///     A environment variable (formatted like "${VARNAME}" is resolve by replacing its placeholder with the content
        ///     from <see cref="Environment.GetEnvironmentVariable(string)" />. If no such variable exists no replacement takes
        ///     place. This replacement is done twice to allow environment variables containing environment variables.
        /// </remarks>
        public static string Resolve(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            const string paramPattern = @"\$\{.*?\}";
            Regex paramMatch = new Regex(paramPattern);

            if (!string.IsNullOrEmpty(input))
                input = paramMatch.Replace(input, match =>
                {
                    string key = match.Value.Substring(2, match.Value.Length - 3);
                    string envVar = Environment.GetEnvironmentVariable(key);

                    if (!string.IsNullOrEmpty(envVar))
                        envVar = paramMatch.Replace(envVar, envMatch =>
                        {
                            string innerKey = envMatch.Value.Substring(2, envMatch.Value.Length - 3);
                            string innerEnvVar = Environment.GetEnvironmentVariable(innerKey);
                            return innerEnvVar ?? envMatch.Value;
                        });

                    return envVar ?? match.Value;
                });

            return input;
        }
    }
}