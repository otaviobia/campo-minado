using UnityEngine;

public class Casa
{
    public bool Escondida, TemBomba, TemBandeira;
    public int Numero;
    public Vector2Int Coordenadas;

    public Casa(bool escondida, bool tem_bomba, bool tem_bandeira, Vector2Int coordenadas, int numero)
    {
        this.Escondida = escondida;
        this.TemBomba = tem_bomba;
        this.TemBandeira = tem_bandeira;
        this.Numero = numero;
        this.Coordenadas = coordenadas;
    }
}