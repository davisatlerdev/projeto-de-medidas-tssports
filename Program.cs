using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaModelagemDefinitivo
{
    public class PadraoTamanho
    {
        public string Nome { get; set; }
        public double Torax { get; set; }
        public double Cintura { get; set; }
        public double Quadril { get; set; }
        public double BracoBase { get; set; }
        public double PernaBase { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            bool executar = true;
            while (executar)
            {
                Console.Clear();
                Console.WriteLine("--- TS SPORTS: MODELAGEM ---");
                Console.WriteLine();
                
                Console.Write("Perfil (1-Masculino, 2-Feminino, 3-Infantil): "); int perfil = int.Parse(Console.ReadLine());
                Console.Write("Altura Total (cm): "); double uAltura = double.Parse(Console.ReadLine());
                Console.Write("Tórax (cm): "); double uTorax = double.Parse(Console.ReadLine());
                Console.Write("Cintura (cm): "); double uCintura = double.Parse(Console.ReadLine());
                Console.Write("Quadril (cm): "); double uQuadril = double.Parse(Console.ReadLine());
                Console.Write("Braço (cm): "); double uBraco = double.Parse(Console.ReadLine());
                Console.Write("Entrepernas (cm): "); double uPernas = double.Parse(Console.ReadLine());
                
                Console.Write("Usa Protetor? (S/N): "); bool usaProtetor = Console.ReadLine().ToUpper() == "S";
                Console.Write("Possui Punho? (S/N): "); bool temPunho = Console.ReadLine().ToUpper() == "S";

                // 2. Processamento Técnica
                int nPadrao = DefinirNivel(perfil, uAltura);
                var tabela = GerarTabela(perfil, nPadrao);

                // --- SELEÇÃO BALANCEADA ---
                var pSup = tabela
                    .OrderBy(t => Math.Abs(t.Torax - uTorax) + Math.Abs(t.Cintura - uCintura))
                    .FirstOrDefault() ?? tabela.Last();

                // --- LÓGICA DE CINTURA ---
                double diffRealCintura = uCintura - pSup.Cintura;
                double diffExibicaoCintura = 0;

                // Só sugere aumento se a diferença for maior que a folga natural do molde (2cm)
                if (diffRealCintura > 2.0)
                {
                    diffExibicaoCintura = diffRealCintura - 4.0; 
                }
                // Se for menor, mantém o ajuste negativo para acinturar (até -2cm)
                else if (diffRealCintura < 0)
                {
                    diffExibicaoCintura = Math.Max(diffRealCintura, -4.0);
                }

                var pInf = tabela.Where(t => t.Quadril >= uQuadril).OrderBy(t => t.Quadril).FirstOrDefault() ?? tabela.Last();

                // 3. Ficha Técnica
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("      FICHA TÉCNICA DE PRODUÇÃO         ");
                Console.WriteLine("========================================");
                Console.WriteLine($"RESULTADO: PADRÃO {nPadrao} | SUP. {pSup.Nome} | INF. {pInf.Nome}");
                
                int nAjuste = (Math.Abs(diffExibicaoCintura) > 0.5) ? 2 : 1;
                Console.WriteLine($"MODELAGEM: AJUSTE {nAjuste} {(usaProtetor ? "[C/ PROTETOR]" : "")}");
                Console.WriteLine("----------------------------------------");
                
                Console.WriteLine($"TÓRAX               : PADRÃO");

                // Exibição: se estiver dentro da folga, mostra "PADRÃO"
                string txtCintura = (Math.Abs(diffExibicaoCintura) < 0.3) 
                    ? "PADRÃO" 
                    : $"{(diffExibicaoCintura > 0 ? "+" : "")}{diffExibicaoCintura:F1}cm";
                
                Console.WriteLine($"CINTURA             : {txtCintura}");
                
                double valBraco = ArredondarPreciso(uBraco - pSup.BracoBase);
                string txtBraco = (valBraco >= 0 ? "+" : "") + valBraco.ToString("F1") + "cm";
                if (int.Parse(pSup.Nome) >= 42) txtBraco += " (AFINAR PUNHO -2cm)";
                
                Console.WriteLine($"BRAÇO               : {txtBraco}");
                
                double pFinal = ArredondarPreciso((uPernas - (temPunho ? 5 : 0)) - pInf.PernaBase);
                Console.WriteLine($"PERNA               : {(pFinal >= 0 ? "+" : "")}{pFinal:F1}cm");
                
                Console.WriteLine($"CORPO (GANCHO)      : {CalcularAltCorpo(uAltura, pSup.Nome, nPadrao)}");

                Console.WriteLine("========================================");
                Console.WriteLine("\n[1] Nova Medida | [2] Sair");
                if (Console.ReadLine() == "2") executar = false;
            }
        }

        static double ArredondarPreciso(double valor) => Math.Round(valor * 2, MidpointRounding.AwayFromZero) / 2;

        static int DefinirNivel(int p, double a) 
        {
            if (p == 3) return 0;
            if (p == 2) return (a <= 162 ? 1 : a <= 172 ? 2 : 3);
            return (a <= 176 ? 1 : a <= 188 ? 2 : 3);
        }

        static string CalcularAltCorpo(double alt, string tam, int nivel) 
        {
            int t = int.Parse(tam);
            if (nivel == 1 && alt <= 165 && t >= 44) return "-1,5cm";
            if (alt >= 188) return "+1,5cm";
            return "BOM";
        }

        static List<PadraoTamanho> GerarTabela(int perfil, int nivel) 
        {
            var lista = new List<PadraoTamanho>();
            if (perfil == 1) { 
                double bB = nivel == 1 ? 62 : nivel == 2 ? 65 : 68;
                double pB = nivel == 1 ? 75 : nivel == 2 ? 82 : 90;
                for (int i = 0; i < 15; i++)
                    lista.Add(new PadraoTamanho { Nome = (34+i*2).ToString(), Torax = 86+i*4, Cintura = 86+i*4, Quadril = 86+i*4, BracoBase = bB+i, PernaBase = pB });
            } else if (perfil == 2) { 
                string[] n = {"34","36","38","40","42","44","46","48","50"};
                double[] t = {80, 84, 88, 92, 96, 100, 104, 108, 112};
                double[] c = {68, 72, 76, 80, 84, 88, 92, 96, 100};
                double[] q = {92, 96, 100, 104, 108, 112, 116, 120, 124};
                double bB = nivel == 1 ? 56 : nivel == 2 ? 59 : 62;
                double pB = nivel == 1 ? 78 : nivel == 2 ? 85 : 93;
                for (int i=0; i<n.Length; i++)
                    lista.Add(new PadraoTamanho { Nome=n[i], Torax=t[i], Cintura=c[i], Quadril=q[i], BracoBase=bB+i, PernaBase=pB });
            } else { 
                string[] n = {"6","8","10","12","14","16"};
                double[] t = {62, 66, 70, 74, 78, 85};
                double[] c = {62, 66, 70, 74, 78, 80};
                double[] q = {62, 66, 70, 74, 78, 85};
                double[] br = {44.5, 47, 49.5, 52, 54.5, 57};
                double[] pr = {57, 60, 63, 66, 69, 73};
                for (int i=0; i<n.Length; i++)
                    lista.Add(new PadraoTamanho { Nome=n[i], Torax=t[i], Cintura=c[i], Quadril=q[i], BracoBase=br[i], PernaBase=pr[i] });
            }
            return lista;
        }
    }
}