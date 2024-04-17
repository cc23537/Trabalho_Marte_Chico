using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apCaminhosEmMarte
{
  internal class Cidade : IRegistro<Cidade>
  {
        const int
                tamNome = 15,
                tamX = 7,
                tamY = 7,
                inicioNome = 0,
                inicioX = inicioNome + tamNome,
                inicioY = inicioX + tamX;
        
    string nomeCidade;
    double x, y;

    public string Chave => NomeCidade;

        public string NomeCidade { 
            get => nomeCidade;
            set {
                nomeCidade = value.PadRight(tamNome, ' ').Substring(0,tamNome); 
            } 
        }
        public double X {
            get => x;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new Exception("X fora do intervalo de 0 a 1");
                }
            }
        }
        public double Y {
            get => y;
            set { 
                if (value <0 || value > 1)
                {
                    throw new Exception("Y fora do intervalo de 0 a 1");
                } 
            } 
        }

        public void GravarDados(StreamWriter arquivo)
    {
      if (arquivo != null) // esta abert para escrita
            {
                arquivo.WriteLine($"{NomeCidade}{X:7.5f}{Y}");
            }
    }

    public void LerRegistro(StreamReader arquivo)
    {
            if (arquivo != null) //arquivo foi aberto
            {
                if (! arquivo.EndOfStream){
                    string linhaLida = arquivo.ReadLine(); //le a prox linha do arquivo
                    NomeCidade = linhaLida.Substring(inicioNome, tamNome);
                    string strx = linhaLida.Substring(inicioX, tamX);
                    X = double.Parse(strx);
                    Y = double.Parse(linhaLida.Substring(inicioY, tamY));
                    
                }
            }
    }
    public override string ToString()
        {
            return NomeCidade + "  " + X + "  " + "   " + Y;
        }
  }
}
