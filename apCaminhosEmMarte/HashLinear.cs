using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apCaminhosEmMarte
{
    public class HashLinear<Tipo> : ITabelaDeHash<Tipo>
        where Tipo : IRegistro<Tipo>

       
    {
        int b = 0;
        private const int SIZE = 131;
        ArrayList[] dados;

        public HashLinear()
        {
            dados = new ArrayList[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                // coloca em cada posição do vetor, um arrayList vazio
                dados[i] = new ArrayList(1);
            }


        }

        public int Hash(int a)
        {
            long tot = 0;
            for (int i = 0; i <= a; i++)
                tot = tot + 1;


            return (int)tot;
        }

        public List<Tipo> Conteudo()
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

        public bool Existe(Tipo item, out int posicao)
        {
            posicao = Hash(b);
            return dados[posicao].Contains(item);
            
        }

        public void Inserir(Tipo item)
        {
            int valorDeHash = Hash(b);
            if (!dados[valorDeHash].Contains(item))
                dados[valorDeHash].Add(item);
            b = b + 1;
        }

        public bool Remover(Tipo item)
        {
            int onde = 0;
            if (!Existe(item, out onde))
                return false;

            dados[onde].Remove(item);
            return true;
        }
    }
}
