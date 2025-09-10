using JsonViewer;
using System;
using System.IO;

namespace JsonViewer
{
    class Program
    {
        private static JsonLoader jsonLoader = new JsonLoader();
        private static JsonQueryEngine queryEngine = new JsonQueryEngine(jsonLoader);

        static void Main(string[] args)
        {
            Console.WriteLine("=== JSON Viewer e Query Tool ===");
            Console.WriteLine("Ferramenta para carregar arquivos JSON e fazer queries por campos específicos\n");

            if (args.Length > 0 && File.Exists(args[0]))
            {
                Console.WriteLine($"Carregando arquivo: {args[0]}");
                jsonLoader.LoadFromFile(args[0]);
            }

            ShowMainMenu();
        }

        static void ShowMainMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== MENU PRINCIPAL ===");
                Console.WriteLine("1. Carregar arquivo JSON");
                Console.WriteLine("2. Carregar JSON de string");
                Console.WriteLine("3. Visualizar estrutura do JSON");
                Console.WriteLine("4. Listar todos os campos");
                Console.WriteLine("5. Fazer query por campo");
                Console.WriteLine("6. Query avançada com filtros");
                Console.WriteLine("0. Sair");
                Console.Write("\nEscolha uma opção: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        LoadJsonFile();
                        break;
                    case "2":
                        LoadJsonString();
                        break;
                    case "3":
                        jsonLoader.DisplayJsonStructure();
                        break;
                    case "4":
                        queryEngine.ListAllFields();
                        break;
                    case "5":
                        ExecuteSimpleQuery();
                        break;
                    case "6":
                        ExecuteAdvancedQuery();
                        break;
                    case "0":
                        Console.WriteLine("Saindo...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }

                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        }

        static void LoadJsonFile()
        {
            Console.WriteLine("Digite o caminho do arquivo JSON:");
            Console.WriteLine("(Dica: Se o caminho contém espaços, você pode usar aspas ou não - o programa tratará automaticamente)");
            Console.Write("Caminho: ");
            string? filePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("Caminho do arquivo não pode estar vazio.");
                return;
            }

            jsonLoader.LoadFromFile(filePath);
        }

        static void LoadJsonString()
        {
            Console.WriteLine("Digite ou cole o JSON (termine com uma linha vazia):");
            string jsonString = "";
            string? line;

            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                jsonString += line + "\n";
            }

            if (string.IsNullOrWhiteSpace(jsonString))
            {
                Console.WriteLine("JSON não pode estar vazio.");
                return;
            }

            jsonLoader.LoadFromString(jsonString);
        }

        static void ExecuteSimpleQuery()
        {
            if (!jsonLoader.IsLoaded())
            {
                Console.WriteLine("Nenhum JSON carregado. Carregue um arquivo primeiro.");
                return;
            }

            Console.Write("Digite o nome do campo para buscar: ");
            string? fieldName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(fieldName))
            {
                Console.WriteLine("Nome do campo não pode estar vazio.");
                return;
            }

            queryEngine.ExecuteQuery(fieldName);
        }

        static void ExecuteAdvancedQuery()
        {
            if (!jsonLoader.IsLoaded())
            {
                Console.WriteLine("Nenhum JSON carregado. Carregue um arquivo primeiro.");
                return;
            }

            Console.Write("Digite o nome do campo para buscar: ");
            string? fieldName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(fieldName))
            {
                Console.WriteLine("Nome do campo não pode estar vazio.");
                return;
            }

            Console.Write("Digite o valor para filtrar (deixe vazio para listar todos): ");
            string? searchValue = Console.ReadLine();

            QueryOperation operation = QueryOperation.Equals;

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                Console.WriteLine("\nEscolha a operação de comparação:");
                Console.WriteLine("1. Igual (padrão)");
                Console.WriteLine("2. Contém");
                Console.WriteLine("3. Começa com");
                Console.WriteLine("4. Termina com");
                Console.WriteLine("5. Expressão regular");
                Console.WriteLine("6. Maior que");
                Console.WriteLine("7. Menor que");
                Console.WriteLine("8. Maior ou igual");
                Console.WriteLine("9. Menor ou igual");
                Console.Write("Opção (1-9): ");

                string? opChoice = Console.ReadLine();
                operation = opChoice switch
                {
                    "2" => QueryOperation.Contains,
                    "3" => QueryOperation.StartsWith,
                    "4" => QueryOperation.EndsWith,
                    "5" => QueryOperation.Regex,
                    "6" => QueryOperation.GreaterThan,
                    "7" => QueryOperation.LessThan,
                    "8" => QueryOperation.GreaterThanOrEqual,
                    "9" => QueryOperation.LessThanOrEqual,
                    _ => QueryOperation.Equals
                };
            }

            queryEngine.ExecuteQuery(fieldName, searchValue, operation);
        }
    }
}
