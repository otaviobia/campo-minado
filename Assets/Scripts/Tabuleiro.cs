using System.Collections.Generic;
using UnityEngine;

public class Tabuleiro
{
    public Vector2Int dimensao;
    public Casa[,] casas;
    public List<Casa> bombas;

    public Casa EncontrarCasa(Vector2Int coordenada)
    {
        foreach (var casa in casas)
        {
            if (casa.coordenadas == coordenada)
            {
                return casa;
            }
        }
        return null;
    }

    public List<Casa> EncontrarCasas(bool escondida = false, bool tem_bomba = false, bool tem_bandeira = false)
    {
        List<Casa> lista = new();

        foreach (var casa in casas)
        {
            if (escondida && casa.escondida) lista.Add(casa);
            else if (tem_bomba && casa.tem_bomba) lista.Add(casa);
            else if (tem_bandeira && casa.tem_bandeira) lista.Add(casa);
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

                Vector2Int coordenadasVizinho = new(casa.coordenadas.x + i, casa.coordenadas.y + j);

                if (coordenadasVizinho.x >= 0 && coordenadasVizinho.x < dimensao.x && coordenadasVizinho.y >= 0 && coordenadasVizinho.y < dimensao.y)
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

        for (int x = 0; x < dimensao.x; x++)
        {
            for (int y = 0; y < dimensao.y; y++)
            {
                lista.Add(new Vector2Int(x, y));
            }
        }

        return lista;
    }

    public Tabuleiro(Vector2Int dimensao)
    {
        this.dimensao = dimensao;
        this.casas = new Casa[dimensao.x, dimensao.y];
        this.bombas = new List<Casa>();
    }
}