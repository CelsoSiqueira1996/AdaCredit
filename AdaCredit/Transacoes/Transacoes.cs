using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaCredit.AdaCredit.Transacoes
{
    public static class Transacoes
    {
        public static readonly string _caminhoDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static readonly string _caminhoTransacoes = Path.Combine(_caminhoDesktop, "Transactions");
        public static readonly string _caminhoTransacoesPendentes = Path.Combine(_caminhoTransacoes, "Pending");
        public static readonly string _caminhoTransacoesRealizadas = Path.Combine(_caminhoTransacoes, "Completed");
        public static readonly string _caminhoTransacoesFalharam = Path.Combine(_caminhoTransacoes, "Failed");
        public static void CriarPastasTransacoes()
        {
            var pastaTransacoes = new DirectoryInfo(_caminhoTransacoes);
            var pastaTransacoesPendentes = new DirectoryInfo(_caminhoTransacoesPendentes);
            var pastaTransacoesRealizadas = new DirectoryInfo(_caminhoTransacoesRealizadas);
            var pastaTransacoesFalharam = new DirectoryInfo(_caminhoTransacoesFalharam);
            if (!pastaTransacoes.Exists)
            {
                pastaTransacoes.Create();
                pastaTransacoesPendentes.Create();
                pastaTransacoesRealizadas.Create();
                pastaTransacoesFalharam.Create();
                return;
            }
            if (!pastaTransacoesPendentes.Exists)
            {
                pastaTransacoesPendentes.Create();
            }
            if (!pastaTransacoesRealizadas.Exists)
            {
                pastaTransacoesRealizadas.Create();
            }
            if (!pastaTransacoesFalharam.Exists)
            {
                pastaTransacoesFalharam.Create();
            }
            return;
        }

        public static void CriarPastasTransacoesProcessadas(string nomeArquivo)
        {
            var arquivoTransacoesRealizadas = new FileInfo(Path.Combine(_caminhoTransacoesRealizadas, nomeArquivo+"-completed.csv"));
            if (!arquivoTransacoesRealizadas.Exists)
            {
                using (arquivoTransacoesRealizadas.Create()) { }
            }
            var arquivoTransacoesFalharam = new FileInfo(Path.Combine(_caminhoTransacoesFalharam, nomeArquivo+"-failed.csv"));
            if (!arquivoTransacoesRealizadas.Exists)
            {
                using (arquivoTransacoesFalharam.Create()) { }
            }
            return;
        }
    }
}
