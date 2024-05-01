using System.Collections.Generic;
using UnityEngine;

public class Tabuleiro
{
    public Vector2Int Dimensoes;
    public Casa[,] Casas;
    public List<Casa> Bombas;

    public Casa EncontrarCasa(Vector2Int coordenada)
    {
        foreach (var casa in Casas)
        {
            if (casa.Coordenadas == coordenada)
            {
                return casa;
            }
        }
        return null;
    }

    public List<Casa> EncontrarCasas(bool escondida = false, bool tem_bomba = false, bool tem_bandeira = false)
    {
        List<Casa> lista = new();

        foreach (var casa in Casas)
        {
            if (escondida && casa.Escondida) lista.Add(casa);
            else if (tem_bomba && casa.TemBomba) lista.Add(casa);
            else if (tem_bandeira && casa.TemBandeira) lista.Add(casa);
        }
        return lista;
    }

    public List<Casa> EncontrarVizinhos(Casa casa)
    {
        List<Casa> vizinhos = new();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                Vector2Int coordenadasVizinho = new(casa.Coordenadas.x + i, casa.Coordenadas.y + j);

                if (coordenadasVizinho.x >= 0 && coordenadasVizinho.x < Dimensoes.x && coordenadasVizinho.y >= 0 && coordenadasVizinho.y < Dimensoes.y)
                {
                    Casa casaVizinha = EncontrarCasa(coordenadasVizinho);

                    if (casaVizinha != null)
                    {
                        vizinhos.Add(casaVizinha);
                    }
                }
            }
        }

        return vizinhos;
    }

    public List<Vector2Int> ListarCoordenadas()
    {
        List<Vector2Int> lista = new();

        for (int x = 0; x < Dimensoes.x; x++)
        {
            for (int y = 0; y < Dimensoes.y; y++)
            {
                lista.Add(new Vector2Int(x, y));
            }
        }

        return lista;
    }

    public Tabuleiro(Vector2Int dimensao)
    {
        this.Dimensoes = dimensao;
        this.Casas = new Casa[dimensao.x, dimensao.y];
        this.Bombas = new List<Casa>();
    }
}