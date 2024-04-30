using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class CM_Engine : MonoBehaviour
{
    public Tabuleiro tabuleiro;

    private int _numBombas = 0;
    private bool _tabuleiroIncializado = false;

    public event EventHandler<PerdeuJogoEventArgs> PerdeuJogo;
    public class PerdeuJogoEventArgs : EventArgs
    {
        public Casa ultimaCasaClicada;
    }

    public event EventHandler GanhouJogo;

    public void CriarMatriz(Vector2Int dimensoes)
    {
        tabuleiro = new(dimensoes);

        for (int i = 0; i < dimensoes.x; i++)
        {
            for (int j = 0; j < dimensoes.y; j++)
            {
                tabuleiro.casas[i, j] = new Casa(true, false, false, new Vector2Int(i, j), 0);
            }
        }
    }

    public void AdicionarBombas(Casa casaDesconsiderada, int numeroDeBombas)
    {
        numeroDeBombas = Mathf.Clamp(numeroDeBombas, 0, tabuleiro.dimensao.x * tabuleiro.dimensao.y);

        List<Vector2Int> coordenadas = tabuleiro.ListarCoordenadas();
        if (numeroDeBombas < tabuleiro.dimensao.x * tabuleiro.dimensao.y) coordenadas.Remove(casaDesconsiderada.coordenadas);

        for (int i = 1; i <= numeroDeBombas; i++)
        {
            int posAleatoria = Random.Range(0, coordenadas.Count);

            tabuleiro.bombas.Add(tabuleiro.EncontrarCasa(coordenadas[posAleatoria]));
            tabuleiro.EncontrarCasa(coordenadas[posAleatoria]).tem_bomba = true;
            coordenadas.RemoveAt(posAleatoria);
        }
    }

    public void AdicionarNumeros()
    {
        foreach (Casa bomba in tabuleiro.bombas)
        {
            foreach (Casa casa in tabuleiro.EncontrarVizinhos(bomba))
            {
                casa.numero++;
            }
        }
    }

    public void AbrirCaminho(Casa casaInicial)
    {
        casaInicial.escondida = false;

        foreach(Casa cs in tabuleiro.EncontrarVizinhos(casaInicial))
        {
            if (cs.numero == 0 && !cs.tem_bomba && cs.escondida)
            {
                cs.escondida = false;
                AbrirCaminho(cs);
                continue;
            }
            
            if (cs.numero != 0 && !cs.tem_bomba && cs.escondida)
            {
                cs.escondida = false;
            }
        }
    }

    public void DescobrirCasa(Casa casaDescoberta)
    {
        if (!_tabuleiroIncializado)
        {
            _tabuleiroIncializado = true;
            AdicionarBombas(casaDescoberta, _numBombas);
            AdicionarNumeros();
        }

        if (casaDescoberta.tem_bandeira) return;

        if (casaDescoberta.tem_bomba)
        {
            PerderJogo(casaDescoberta);
            return;
        }

        if (casaDescoberta.numero == 0) AbrirCaminho(casaDescoberta);

        if (casaDescoberta.numero > 0)
        {
            casaDescoberta.escondida = false;
        }

        if (ChecarVitoria()) GanharJogo();
    }

    public void AlterarBandeira(Casa destino)
    {
        destino.tem_bandeira = !destino.tem_bandeira;
    }

    private void PerderJogo(Casa bombaExplodida)
    {
        PerdeuJogo?.Invoke(this, new PerdeuJogoEventArgs { ultimaCasaClicada = bombaExplodida });
    }

    private void GanharJogo()
    {
        GanhouJogo?.Invoke(this, EventArgs.Empty);
    }

    public bool ChecarVitoria()
    {
        foreach (Casa c in tabuleiro.EncontrarCasas(true))
        {
            if (!c.tem_bomba) return false;
        }

        return true;
    }

    public void GerarTabuleiro(Vector2Int suasDimensoes, int numeroDeBombas)
    {
        CriarMatriz(suasDimensoes);
        _numBombas = numeroDeBombas;
    }
}
