using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public void Salvar<T>(T objeto)
    {
        string dados = JsonUtility.ToJson(objeto);
        string diretorio = Application.dataPath + "/" + objeto.ToString() + ".json";

        System.IO.File.WriteAllText(diretorio, dados);
    }

    public T Carregar<T>(T objeto)
    {
        string diretorio = Application.dataPath + "/" + objeto.ToString() + ".json";
        if (!System.IO.File.Exists(diretorio)) return objeto;
        string dados = System.IO.File.ReadAllText(diretorio);

        T objetoRetornado = JsonUtility.FromJson<T>(dados);

        return objetoRetornado;
    }

    public void Deletar<T>(T objeto)
    {
        string diretorio = Application.dataPath + "/" + objeto.ToString() + ".json";
        if (!System.IO.File.Exists(diretorio)) return;
        System.IO.File.Delete(diretorio);
    }
}

[Serializable]
public class Placar
{
    public List<Pontuacao> Pontuacoes = new();
}

[Serializable]
public class Pontuacao
{
    public string DataDaPontuacao;
    public float TempoFinal;
    public AjustesDeJogo.Modo ModoDoJogo;
    public Vector2Int DimensoesDoTabuleiro;
    public int QuantidadeDeBombas;

    public Pontuacao(string hora, float tempo, AjustesDeJogo.Modo modo, Vector2Int dim, int minas) 
    {
        DataDaPontuacao = hora;
        TempoFinal = tempo;
        ModoDoJogo = modo;
        DimensoesDoTabuleiro = dim;
        QuantidadeDeBombas = minas;
    }
}

[Serializable]
public class AjustesDeJogo
{
    public enum Modo
    {
        Facil,
        Medio,
        Dificil,
        Customizado
    }
    public Modo ModoAtual;
    public Vector2Int DimensoesDoTabuleiro;
    public int QuantidadeDeBombas;

    public AjustesDeJogo(Modo modo, Vector2Int tamanho, int bombas)
    {
        ModoAtual = modo;
        DimensoesDoTabuleiro = tamanho;
        QuantidadeDeBombas = bombas;
    }
}

[Serializable]
public class PreferenciasDoUsuario
{
    public float VolumeMestre = 100;
}
