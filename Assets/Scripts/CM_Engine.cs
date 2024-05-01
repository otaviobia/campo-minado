using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CM_Engine : MonoBehaviour
{
    [HideInInspector] public Tabuleiro Tabuleiro;

    private int _qtdTotalDeBombas = 0;
    private int _qtdDeCliques = 0;
    private bool _tabuleiroIncializado = false;

    public event EventHandler GanhouJogo;
    public event EventHandler CasaComBandeiraFoiAberta;
    public event EventHandler<PerdeuJogoEventArgs> PerdeuJogo;
    public class PerdeuJogoEventArgs : EventArgs
    {
        public Casa UltimaCasaClicada;
    }

    public void CriarMatriz(Vector2Int dimensoes)
    {
        Tabuleiro = new(dimensoes);

        for (int i = 0; i < dimensoes.x; i++)
        {
            for (int j = 0; j < dimensoes.y; j++)
            {
                Tabuleiro.Casas[i, j] = new Casa(true, false, false, new Vector2Int(i, j), 0);
            }
        }
    }

    public void AdicionarBombas(Casa casaDesconsiderada, int numeroDeBombas)
    {
        numeroDeBombas = Mathf.Clamp(numeroDeBombas, 0, Tabuleiro.Dimensoes.x * Tabuleiro.Dimensoes.y);

        List<Vector2Int> coordenadas = Tabuleiro.ListarCoordenadas();

        if (numeroDeBombas < Tabuleiro.Dimensoes.x * Tabuleiro.Dimensoes.y)
        {
            coordenadas.Remove(casaDesconsiderada.Coordenadas);
        }

        if (numeroDeBombas < (Tabuleiro.Dimensoes.x * Tabuleiro.Dimensoes.y)-9)
        {
            foreach(Casa casa in Tabuleiro.EncontrarVizinhos(casaDesconsiderada))
            {
                coordenadas.Remove(casa.Coordenadas);
            }
        }

        for (int i = 1; i <= numeroDeBombas; i++)
        {
            int posAleatoria = Random.Range(0, coordenadas.Count);

            Tabuleiro.Bombas.Add(Tabuleiro.EncontrarCasa(coordenadas[posAleatoria]));
            Tabuleiro.EncontrarCasa(coordenadas[posAleatoria]).TemBomba = true;
            coordenadas.RemoveAt(posAleatoria);
        }
    }

    public void AdicionarNumeros()
    {
        foreach (Casa bomba in Tabuleiro.Bombas)
        {
            foreach (Casa casa in Tabuleiro.EncontrarVizinhos(bomba))
            {
                casa.Numero++;
            }
        }
    }

    public void AbrirCaminho(Casa casaInicial)
    {
        casaInicial.Escondida = false;

        foreach(Casa casaVizinha in Tabuleiro.EncontrarVizinhos(casaInicial))
        {
            if (casaVizinha.Numero == 0 && !casaVizinha.TemBomba && casaVizinha.Escondida)
            {
                if (casaVizinha.TemBandeira && _qtdDeCliques < 2)
                {
                    CasaComBandeiraFoiAberta?.Invoke(this, EventArgs.Empty);
                }

                if (casaVizinha.TemBandeira && _qtdDeCliques > 1) continue;
                
                casaVizinha.Escondida = false;
                AbrirCaminho(casaVizinha);
                continue;
            }
            
            if (casaVizinha.Numero != 0 && !casaVizinha.TemBomba && casaVizinha.Escondida)
            {
                if (casaVizinha.TemBandeira && _qtdDeCliques < 2) CasaComBandeiraFoiAberta?.Invoke(this, EventArgs.Empty);
                if (casaVizinha.TemBandeira && _qtdDeCliques > 1) continue;
                casaVizinha.Escondida = false;
            }
        }
    }

    public bool DescobrirCasa(Casa casaDescoberta)
    {
        _qtdDeCliques++;

        if (!_tabuleiroIncializado)
        {
            _tabuleiroIncializado = true;
            AdicionarBombas(casaDescoberta, _qtdTotalDeBombas);
            AdicionarNumeros();
        }

        if (casaDescoberta.TemBandeira || !casaDescoberta.Escondida) return false;

        if (casaDescoberta.TemBomba)
        {
            PerderJogo(casaDescoberta);
            return false;
        }

        if (casaDescoberta.Numero == 0) AbrirCaminho(casaDescoberta);

        if (casaDescoberta.Numero > 0)
        {
            casaDescoberta.Escondida = false;
        }

        if (ChecarVitoria()) GanharJogo();

        return true;
    }

    public void AlterarBandeira(Casa destino)
    {
        destino.TemBandeira = !destino.TemBandeira;
    }

    private void PerderJogo(Casa bombaExplodida)
    {
        PerdeuJogo?.Invoke(this, new PerdeuJogoEventArgs { UltimaCasaClicada = bombaExplodida });
    }

    private void GanharJogo()
    {
        GanhouJogo?.Invoke(this, EventArgs.Empty);
    }

    public bool ChecarVitoria()
    {
        foreach (Casa casa in Tabuleiro.EncontrarCasas(true))
        {
            if (!casa.TemBomba) return false;
        }

        return true;
    }

    public void GerarTabuleiro(Vector2Int suasDimensoes, int numeroDeBombas)
    {
        CriarMatriz(suasDimensoes);
        _qtdTotalDeBombas = numeroDeBombas;
    }
}
