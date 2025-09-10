using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JsonViewer
{
    public class JsonQueryEngine
    {
        private readonly JsonLoader _jsonLoader;

        public JsonQueryEngine(JsonLoader jsonLoader)
        {
            _jsonLoader = jsonLoader;
        }

        public void ExecuteQuery(string fieldPath, string? searchValue = null, QueryOperation operation = QueryOperation.Equals)
        {
            if (!_jsonLoader.IsLoaded())
            {
                Console.WriteLine("Nenhum JSON carregado. Carregue um arquivo primeiro.");
                return;
            }

            var jsonData = _jsonLoader.GetJsonData();
            if (jsonData == null)
            {
                Console.WriteLine("Dados JSON inválidos.");
                return;
            }

            Console.WriteLine($"\n=== Executando Query ===");
            Console.WriteLine($"Campo: {fieldPath}");
            if (!string.IsNullOrEmpty(searchValue))
            {
                Console.WriteLine($"Valor de busca: {searchValue}");
                Console.WriteLine($"Operação: {operation}");
            }
            Console.WriteLine();

            var results = FindFieldValues(jsonData, fieldPath, searchValue, operation);
            DisplayResults(results, fieldPath);
        }

        private List<QueryResult> FindFieldValues(JToken token, string fieldPath, string? searchValue, QueryOperation operation)
        {
            var results = new List<QueryResult>();
            FindFieldValuesRecursive(token, fieldPath, searchValue, operation, results, "");
            return results;
        }

        private void FindFieldValuesRecursive(JToken token, string fieldPath, string? searchValue, QueryOperation operation, List<QueryResult> results, string currentPath)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    var obj = (JObject)token;
                    
                    // Verifica se o campo existe diretamente neste objeto
                    if (obj.ContainsKey(fieldPath))
                    {
                        var value = obj[fieldPath];
                        string fullPath = string.IsNullOrEmpty(currentPath) ? fieldPath : $"{currentPath}.{fieldPath}";
                        
                        if (string.IsNullOrEmpty(searchValue) || MatchesSearchCriteria(value, searchValue, operation))
                        {
                            results.Add(new QueryResult
                            {
                                Path = fullPath,
                                Value = value?.ToString() ?? "null",
                                Type = value?.Type.ToString() ?? "null",
                                ParentObject = obj
                            });
                        }
                    }

                    // Verifica campos aninhados
                    if (fieldPath.Contains("."))
                    {
                        var parts = fieldPath.Split('.', 2);
                        if (obj.ContainsKey(parts[0]))
                        {
                            string newPath = string.IsNullOrEmpty(currentPath) ? parts[0] : $"{currentPath}.{parts[0]}";
                            FindFieldValuesRecursive(obj[parts[0]]!, parts[1], searchValue, operation, results, newPath);
                        }
                    }

                    // Continua a busca recursiva em todos os filhos
                    foreach (var property in obj.Properties())
                    {
                        string newPath = string.IsNullOrEmpty(currentPath) ? property.Name : $"{currentPath}.{property.Name}";
                        FindFieldValuesRecursive(property.Value, fieldPath, searchValue, operation, results, newPath);
                    }
                    break;

                case JTokenType.Array:
                    var array = (JArray)token;
                    for (int i = 0; i < array.Count; i++)
                    {
                        string newPath = string.IsNullOrEmpty(currentPath) ? $"[{i}]" : $"{currentPath}[{i}]";
                        FindFieldValuesRecursive(array[i], fieldPath, searchValue, operation, results, newPath);
                    }
                    break;
            }
        }

        private bool MatchesSearchCriteria(JToken? value, string searchValue, QueryOperation operation)
        {
            if (value == null) return false;

            string valueStr = value.ToString();

            return operation switch
            {
                QueryOperation.Equals => string.Equals(valueStr, searchValue, StringComparison.OrdinalIgnoreCase),
                QueryOperation.Contains => valueStr.Contains(searchValue, StringComparison.OrdinalIgnoreCase),
                QueryOperation.StartsWith => valueStr.StartsWith(searchValue, StringComparison.OrdinalIgnoreCase),
                QueryOperation.EndsWith => valueStr.EndsWith(searchValue, StringComparison.OrdinalIgnoreCase),
                QueryOperation.Regex => Regex.IsMatch(valueStr, searchValue, RegexOptions.IgnoreCase),
                QueryOperation.GreaterThan => CompareNumeric(valueStr, searchValue) > 0,
                QueryOperation.LessThan => CompareNumeric(valueStr, searchValue) < 0,
                QueryOperation.GreaterThanOrEqual => CompareNumeric(valueStr, searchValue) >= 0,
                QueryOperation.LessThanOrEqual => CompareNumeric(valueStr, searchValue) <= 0,
                _ => false
            };
        }

        private int CompareNumeric(string value1, string value2)
        {
            if (double.TryParse(value1, out double num1) && double.TryParse(value2, out double num2))
            {
                return num1.CompareTo(num2);
            }
            return string.Compare(value1, value2, StringComparison.OrdinalIgnoreCase);
        }

        private void DisplayResults(List<QueryResult> results, string fieldPath)
        {
            if (results.Count == 0)
            {
                Console.WriteLine($"Nenhum resultado encontrado para o campo '{fieldPath}'.");
                return;
            }

            Console.WriteLine($"Encontrados {results.Count} resultado(s):\n");

            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                Console.WriteLine($"[{i + 1}] Caminho: {result.Path}");
                Console.WriteLine($"    Tipo: {result.Type}");
                Console.WriteLine($"    Valor: {result.Value}");
                
                if (result.ParentObject != null && result.ParentObject.Count <= 10)
                {
                    Console.WriteLine($"    Contexto: {result.ParentObject.ToString(Newtonsoft.Json.Formatting.None)}");
                }
                Console.WriteLine();
            }
        }

        public void ListAllFields()
        {
            if (!_jsonLoader.IsLoaded())
            {
                Console.WriteLine("Nenhum JSON carregado. Carregue um arquivo primeiro.");
                return;
            }

            var jsonData = _jsonLoader.GetJsonData();
            if (jsonData == null)
            {
                Console.WriteLine("Dados JSON inválidos.");
                return;
            }

            Console.WriteLine("\n=== Todos os Campos Disponíveis ===");
            var fields = new HashSet<string>();
            CollectAllFields(jsonData, "", fields);

            var sortedFields = fields.OrderBy(f => f).ToList();
            for (int i = 0; i < sortedFields.Count; i++)
            {
                Console.WriteLine($"{i + 1:D3}. {sortedFields[i]}");
            }
            Console.WriteLine($"\nTotal: {sortedFields.Count} campos únicos encontrados.");
        }

        private void CollectAllFields(JToken token, string currentPath, HashSet<string> fields)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    var obj = (JObject)token;
                    foreach (var property in obj.Properties())
                    {
                        string fieldPath = string.IsNullOrEmpty(currentPath) ? property.Name : $"{currentPath}.{property.Name}";
                        fields.Add(fieldPath);
                        CollectAllFields(property.Value, fieldPath, fields);
                    }
                    break;

                case JTokenType.Array:
                    var array = (JArray)token;
                    if (array.Count > 0)
                    {
                        CollectAllFields(array[0], currentPath, fields);
                    }
                    break;
            }
        }
    }

    public class QueryResult
    {
        public string Path { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public JObject? ParentObject { get; set; }
    }

    public enum QueryOperation
    {
        Equals,
        Contains,
        StartsWith,
        EndsWith,
        Regex,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual
    }
}