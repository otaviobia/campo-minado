using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    // O modo customizado permite o usuário escolher as dimensões do tabuleiro e o número de bombas
    // Um arquivo .json é utilizado para transmitir a escolha do usuário entre as cenas do programa  
    public void SalvarAjustes(AjustesDeJogo.Modo modo, int tamX, int tamY, int bombas)
    {
        AjustesDeJogo ajustes = new(modo, new Vector2Int(tamX, tamY), bombas);

        string dadosAjustes = JsonUtility.ToJson(ajustes);
        string diretorio = Application.dataPath + "/ajustes_de_jogo.json";

        System.IO.File.WriteAllText(diretorio, dadosAjustes);
    }

    public AjustesDeJogo LerAjustes()
    {
        string diretorio = Application.dataPath + "/ajustes_de_jogo.json";
        string dadosAjustes = System.IO.File.ReadAllText(diretorio);

        AjustesDeJogo modoDeJogo = JsonUtility.FromJson<AjustesDeJogo>(dadosAjustes);

        return modoDeJogo;
    }

    // Ao vencer o jogo, o programa adiciona ao placar uma nova pontuação que contém o tempo final
    // de jogo, a data e hora real, o modo de jogo e as definições do tabuleiro (caso customizado)
    public void SalvarPontuacao(Pontuacao novaPontuacao)
    {
        Placar placar = CarregarPlacar();
        placar.pontuacoes.Add(novaPontuacao);

        string dadosPlacar = JsonUtility.ToJson(placar);
        string diretorio = Application.dataPath + "/placar.json";

        System.IO.File.WriteAllText(diretorio, dadosPlacar);
    }

    public Placar CarregarPlacar()
    {
        string diretorio = Application.dataPath + "/placar.json";
        if (!System.IO.File.Exists(diretorio)) return new Placar();
        string dadosPlacar = System.IO.File.ReadAllText(diretorio);

        Placar placar = JsonUtility.FromJson<Placar>(dadosPlacar);

        return placar;
    }

    public void LimparPlacar()
    {
        string diretorio = Application.dataPath + "/placar.json";
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
