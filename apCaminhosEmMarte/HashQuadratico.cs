using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apCaminhosEmMarte
{
    public class HashQuadratico<Tipo> : ITabelaDeHash<Tipo>
         where Tipo : IRegistro<Tipo>
    {

        private int size = 1007;
        Tipo[] dados;

        public HashQuadratico()
        {
            dados = new Tipo[size];
        }

        private int Hash(string chave)
        {
            long tot = 0;
            for (int i = 0; i < chave.Length; i++)
                tot += 37 * tot + (char)chave[i];

            tot = tot % dados.Length;
            if (tot < 0)
                tot += dados.Length;
            return (int)tot;
        }

        void ITabelaDeHash<Tipo>.Inserir(Tipo item)
        {
            int coli = 0;
            int valorHash = Hash(item.Chave.Trim());
            while (true){
                if (valorHash >= dados.Length){
                    valorHash = valorHash - dados.Length;
                }

                if (dados[valorHash] != null){
                    coli++;
                    valorHash = valorHash + coli * coli;
                }else{
                    break;
                }
            }

            dados[valorHash] = item;
        }

        List<Tipo> ITabelaDeHash<Tipo>.Conteudo()
        {
            List<Tipo> saida = new List<Tipo>();
            for (int i = 0; i < dados.Length; i++)
                if (dados[i].Count > 0)
                {
                    string linha = $"{i,5} : ";
                    foreach (Tipo item in dados[i])
                        saida.Add(item);
                }
            return saida;
        }

        public bool Existe(Tipo item, out int onde)
        {
            onde = Hash(item.Chave);
            if (dados[onde].Equals(item)){
                return true;
            }

            return false;
        }

        bool ITabelaDeHash<Tipo>.Remover(Tipo item)
        {
            int onde = 0;
            if (!Existe(item, out onde))
                return false;

            //dados[onde] = default(Tipo);
            return true;
        }
    }
}
