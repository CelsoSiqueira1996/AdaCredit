using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaCredit.AdaCredit.Transacoes
{
    public class InfoTransacoes
    {
        public static void ProcessarTransacoes()
        {
            Console.Clear();
            Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
            AnsiConsole.Status()
                .Start("Processando transações...", ctx =>
                {
                    var verificador = RepositorioTransacoes.ProcessarTransacoesPendentes();
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));
                    if (!verificador)
                    {
                        AnsiConsole.MarkupLine("Não existem transações pendentes!");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("Transações processadas com sucesso!");
                    }
                });

            Console.WriteLine("\n... operação finalizada ...");
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu anterior.");
            Console.ReadKey();
        }

        public static void RelatorioTransacoes()
        {
            Console.Clear();
            Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
            Table tabela = RepositorioTransacoes.GerarRelatorioTransacoesFalharam();
            if (tabela != null)
            {
                AnsiConsole.Write(tabela);
            }
            Console.WriteLine("\n... operação finalizada ...");
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu anterior.");
            Console.ReadKey();
        }
    }
}
