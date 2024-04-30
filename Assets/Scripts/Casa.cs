using UnityEngine;

public class Casa
{
    public bool escondida, tem_bomba, tem_bandeira;
    public int numero;
    public Vector2Int coordenadas;

    public Casa(bool escondida, bool tem_bomba, bool tem_bandeira, Vector2Int coordenadas, int numero)
    {
        this.escondida = escondida;
        this.tem_bomba = tem_bomba;
        this.tem_bandeira = tem_bandeira;
        this.numero = numero;
        this.coordenadas = coordenadas;
    }
}