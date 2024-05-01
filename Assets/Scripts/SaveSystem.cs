using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public void Salvar<T>(T a)
    {
        string dados = JsonUtility.ToJson(a);
        string diretorio = Application.dataPath + "/" + a.ToString() + ".json";

        System.IO.File.WriteAllText(diretorio, dados);
    }

    public T Carregar<T>(T a)
    {
        string diretorio = Application.dataPath + "/" + a.ToString() + ".json";
        if (!System.IO.File.Exists(diretorio)) return a;
        string dados = System.IO.File.ReadAllText(diretorio);

        T t = JsonUtility.FromJson<T>(dados);

        return t;
    }

    public void Deletar<T>(T a)
    {
        string diretorio = Application.dataPath + "/" + a.ToString() + ".json";
        if (!System.IO.File.Exists(diretorio)) return;
        System.IO.File.Delete(diretorio);
    }
}

[Serializable]
public class Placar
{
    public List<Pontuacao> pontuacoes = new();
}

[Serializable]
public class Pontuacao
{
    public string dataDaPontuacao;
    public float tempoFinal;
    public AjustesDeJogo.Modo modoDeJogo;
    public Vector2Int tamanhoDoTabuleiro;
    public int numeroDeBombas;

    public Pontuacao(string hora, float tempo, AjustesDeJogo.Modo modo, Vector2Int dim, int minas) 
    {
        dataDaPontuacao = hora;
        tempoFinal = tempo;
        modoDeJogo = modo;
        tamanhoDoTabuleiro = dim;
        numeroDeBombas = minas;
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
    public Modo modoAtual;
    public Vector2Int tamanhoDoTabuleiro;
    public int numeroDeBombas;

    public AjustesDeJogo(Modo modo, Vector2Int tamanho, int bombas)
    {
        modoAtual = modo;
        tamanhoDoTabuleiro = tamanho;
        numeroDeBombas = bombas;
    }
}

[Serializable]
public class PreferenciasDoUsuario
{
    public float volumeMestre;

    public PreferenciasDoUsuario(float volume)
    {
        volumeMestre = volume;
    }
}
