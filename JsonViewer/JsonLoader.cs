using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace JsonViewer
{
    public class JsonLoader
    {
        private JToken? _jsonData;
        private string? _filePath;

        public bool LoadFromFile(string filePath)
        {
            try
            {
                // Normaliza o caminho do arquivo
                string normalizedPath = NormalizePath(filePath);
                
                if (!File.Exists(normalizedPath))
                {
                    Console.WriteLine($"Erro: Arquivo não encontrado: {normalizedPath}");
                    Console.WriteLine($"Caminho original: {filePath}");
                    return false;
                }

                string jsonContent = File.ReadAllText(normalizedPath);
                _jsonData = JToken.Parse(jsonContent);
                _filePath = normalizedPath;
                
                Console.WriteLine($"Arquivo JSON carregado com sucesso: {normalizedPath}");
                return true;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Erro ao parsear JSON: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar arquivo: {ex.Message}");
                return false;
            }
        }

        public bool LoadFromString(string jsonString)
        {
            try
            {
                _jsonData = JToken.Parse(jsonString);
                _filePath = null;
                
                Console.WriteLine("JSON carregado com sucesso da string.");
                return true;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Erro ao parsear JSON: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar JSON: {ex.Message}");
                return false;
            }
        }

        public JToken? GetJsonData()
        {
            return _jsonData;
        }

        public string? GetFilePath()
        {
            return _filePath;
        }

        public bool IsLoaded()
        {
            return _jsonData != null;
        }

        private string NormalizePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return filePath;

            // Remove aspas do início e fim se existirem
            string normalizedPath = filePath.Trim();
            if ((normalizedPath.StartsWith('"') && normalizedPath.EndsWith('"')) ||
                (normalizedPath.StartsWith('\'') && normalizedPath.EndsWith('\'')))
            {
                normalizedPath = normalizedPath.Substring(1, normalizedPath.Length - 2);
            }

            // Expande o caminho completo
            try
            {
                normalizedPath = Path.GetFullPath(normalizedPath);
            }
            catch (Exception)
            {
                // Se não conseguir expandir, usa o caminho original
            }

            return normalizedPath;
        }

        public void DisplayJsonStructure()
        {
            if (_jsonData == null)
            {
                Console.WriteLine("Nenhum JSON carregado.");
                return;
            }

            Console.WriteLine("\n=== Estrutura do JSON ===");
            DisplayTokenStructure(_jsonData, "", 0);
        }

        private void DisplayTokenStructure(JToken token, string path, int depth)
        {
            if (depth > 5) // Limita a profundidade para evitar saída muito longa
            {
                Console.WriteLine($"{new string(' ', depth * 2)}{path}: [Estrutura muito profunda...]");
                return;
            }

            string indent = new string(' ', depth * 2);

            switch (token.Type)
            {
                case JTokenType.Object:
                    var obj = (JObject)token;
                    Console.WriteLine($"{indent}{path}: Object ({obj.Count} propriedades)");
                    foreach (var property in obj.Properties())
                    {
                        string newPath = string.IsNullOrEmpty(path) ? property.Name : $"{path}.{property.Name}";
                        DisplayTokenStructure(property.Value, newPath, depth + 1);
                    }
                    break;

                case JTokenType.Array:
                    var array = (JArray)token;
                    Console.WriteLine($"{indent}{path}: Array ({array.Count} elementos)");
                    if (array.Count > 0)
                    {
                        DisplayTokenStructure(array[0], $"{path}[0]", depth + 1);
                        if (array.Count > 1)
                        {
                            Console.WriteLine($"{indent}  ... e mais {array.Count - 1} elemento(s)");
                        }
                    }
                    break;

                default:
                    Console.WriteLine($"{indent}{path}: {token.Type} = {token.ToString().Substring(0, Math.Min(50, token.ToString().Length))}{(token.ToString().Length > 50 ? "..." : "")}");
                    break;
            }
        }
    }
}