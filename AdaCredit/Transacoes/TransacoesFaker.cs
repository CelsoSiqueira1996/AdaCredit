using AdaCredit.AdaCredit.Clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaCredit.AdaCredit.Transacoes
{
    public static class TransacoesFaker
    {
        public static void GerarArquivosFaker()
        {
            Transacoes.CriarPastasTransacoes();
            List<string[]> transacoesPendentes = RepositorioClientes.GerarTransacoesFaker();
            var nomeArquivoAtual = "";
            string nomeArquivo;
            string dadosLinhaArquivo;
            FileInfo arquivo = null;
            foreach (string[] transacao in transacoesPendentes.OrderBy(x => x[1]))
            {
                dadosLinhaArquivo = transacao[0];
                nomeArquivo = Path.Combine(Transacoes._caminhoTransacoesPendentes, transacao[1]);
                if (nomeArquivoAtual != nomeArquivo)
                {
                    arquivo = new FileInfo(nomeArquivo);
                    if (!arquivo.Exists)
                    {
                        using (arquivo.Create()) { }
                    }
                    nomeArquivoAtual = nomeArquivo;
                }
                using (StreamWriter sw = arquivo.AppendText())
                {
                    sw.WriteLine(dadosLinhaArquivo);
                }
            }
        }
    }
}
