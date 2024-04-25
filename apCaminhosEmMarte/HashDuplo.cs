using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apCaminhosEmMarte
{
    public class HashDuplo<Tipo> : ITabelaDeHash<Tipo>

        where Tipo : IRegistro<Tipo>
    {
        List<Tipo> ITabelaDeHash<Tipo>.Conteudo()
        {
            throw new NotImplementedException();
        }

        bool ITabelaDeHash<Tipo>.Existe(Tipo item, out int onde)
        {
            throw new NotImplementedException();
        }

        void ITabelaDeHash<Tipo>.Inserir(Tipo item)
        {
            throw new NotImplementedException();
        }

        bool ITabelaDeHash<Tipo>.Remover(Tipo item)
        {
            throw new NotImplementedException();
        }
    }
}
