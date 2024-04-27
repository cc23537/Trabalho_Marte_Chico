using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apCaminhosEmMarte
{
    public class HashQuadratico<Tipo> : ITabelaDeHash<Tipo>
         where Tipo : IRegistro<Tipo>
    {

        private int size = 1007;
        ArrayList[] dados;

        public HashQuadratico()
        {
            dados = new ArrayList[size];
            for (int i = 0; i < size; i++)
            {
                // coloca em cada posição do vetor, um arrayList vazio
                dados[i] = new ArrayList(1);
            }
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

                if (dados[valorHash].Count > 0){
                    coli++;
                    valorHash = valorHash + coli * coli;
                }else{
                    break;
                }
            }

            dados[valorHash].Add(item);
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
            return dados[onde].Contains(item);
        }

        bool ITabelaDeHash<Tipo>.Remover(Tipo item)
        {
            int onde = 0;
            if (!Existe(item, out onde))
                return false;

            dados[onde].Remove(item);
            return true;
        }
    }
}
