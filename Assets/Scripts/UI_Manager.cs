using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    private AjustesDeJogo ajustesDeJogo;
    [SerializeField] private CM_Engine engine;
    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private Sprite[] texturas;
    [SerializeField] private GameObject casaPrefab;
    [SerializeField] private Transform objetoTabuleiro;
    [SerializeField] private TextMeshProUGUI bombasTexto, tempoTexto;
    [SerializeField] private Transform menuBotao;
    [SerializeField] private int tamanhoDaCasa;

    private List<GameObject> matrizCasas = new();
    private bool _timerRodando = true;

    private int inicialDeBombas;

    private void Start()
    {
        engine.GanhouJogo += AoGanharJogo;
        engine.PerdeuJogo += AoPerderJogo;

        ajustesDeJogo = saveSystem.LerAjustes();
        inicialDeBombas = ajustesDeJogo.numeroDeBombas;

        engine.GerarTabuleiro(ajustesDeJogo.tamanhoDoTabuleiro, ajustesDeJogo.numeroDeBombas);

        GerarDesignResponsivo();

        bombasTexto.text = ajustesDeJogo.numeroDeBombas.ToString();

        foreach (Vector2Int v in engine.tabuleiro.ListarCoordenadas())
        {
            GameObject w = Instantiate(casaPrefab, new Vector3(v.x, v.y, 0), Quaternion.identity);
            w.transform.parent = objetoTabuleiro;
            matrizCasas.Add(w);
        }

        AjeitarCamera();
    }

    public void GerarDesignResponsivo()
    {
        Screen.SetResolution(ajustesDeJogo.tamanhoDoTabuleiro.x * tamanhoDaCasa, ajustesDeJogo.tamanhoDoTabuleiro.y * tamanhoDaCasa + tamanhoDaCasa + 10, false);

        bombasTexto.transform.parent.position = new Vector3(0.4014f, ajustesDeJogo.tamanhoDoTabuleiro.y + 0.2f, 0);
        tempoTexto.transform.parent.position = new Vector3(ajustesDeJogo.tamanhoDoTabuleiro.x - 1.4014f, ajustesDeJogo.tamanhoDoTabuleiro.y + 0.2f, 0);
        menuBotao.position = new Vector3((float)ajustesDeJogo.tamanhoDoTabuleiro.x / 2 - .5f, ajustesDeJogo.tamanhoDoTabuleiro.y + 0.2f, 0);
    }

    public void AjeitarCamera()
    {
        Vector2Int a = new(engine.tabuleiro.dimensao.x % 2, engine.tabuleiro.dimensao.y % 2);

        if (a.x == 0 && a.y == 0)
        {
            objetoTabuleiro.position = new Vector2(objetoTabuleiro.position.x - (float)(engine.tabuleiro.dimensao.x-1) / 2, objetoTabuleiro.position.y - (float)(engine.tabuleiro.dimensao.y-1) / 2);
        }
        if (a.x == 1 && a.y == 0)
        {
            objetoTabuleiro.position = new Vector2(objetoTabuleiro.position.x - engine.tabuleiro.dimensao.x / 2, objetoTabuleiro.position.y - (float)(engine.tabuleiro.dimensao.y - 1) / 2);
        }
        if (a.x == 0 && a.y == 1)
        {
            objetoTabuleiro.position = new Vector2(objetoTabuleiro.position.x - (float)(engine.tabuleiro.dimensao.x-1) / 2, objetoTabuleiro.position.y - engine.tabuleiro.dimensao.y / 2);
        }
        if (a.x == 1 && a.y == 1)
        {
            objetoTabuleiro.position = new Vector2(objetoTabuleiro.position.x - engine.tabuleiro.dimensao.x / 2, objetoTabuleiro.position.y - engine.tabuleiro.dimensao.y / 2);
        }
               
        Camera.main.orthographicSize = ((float)engine.tabuleiro.dimensao.y / 2) + 1f;
    }

    public void AoGanharJogo(object sender, EventArgs e)
    {
        Pontuacao novaPontuacao = new(DateTime.Now.ToString("G"), (float)Math.Round((double)Time.timeSinceLevelLoad, 2), ajustesDeJogo.modoAtual, ajustesDeJogo.tamanhoDoTabuleiro, inicialDeBombas);
        
        saveSystem.SalvarPontuacao(novaPontuacao);

        foreach (Casa c in engine.tabuleiro.EncontrarCasas(false, true))
        {
            foreach (GameObject g in matrizCasas)
            {
                if (g.transform.localPosition.x == c.coordenadas.x && g.transform.localPosition.y == c.coordenadas.y)
                {
                    g.GetComponent<SpriteRenderer>().sprite = texturas[13];
                }

            }
        }

        _timerRodando = false;
    }

    public void AoPerderJogo(object sender, CM_Engine.PerdeuJogoEventArgs e)
    {
        foreach (Casa c in engine.tabuleiro.EncontrarCasas(false, true))
        {
            foreach (GameObject g in matrizCasas)
            {
                if (g.transform.localPosition.x == c.coordenadas.x && g.transform.localPosition.y == c.coordenadas.y)
                {
                    if (e.ultimaCasaClicada == c)
                        g.GetComponent<SpriteRenderer>().sprite = texturas[12];
                    else
                        g.GetComponent<SpriteRenderer>().sprite = texturas[11];
                }

            }
        }

        _timerRodando = false;
    }

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (_timerRodando) tempoTexto.text = Time.timeSinceLevelLoad.ToString("0");

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (Input.GetKeyDown(KeyCode.Mouse0) && hit.collider != null)
        {
            if (!_timerRodando)
            {
                SceneManager.LoadScene(1);
                return;
            }

            Vector2Int a = new((int)hit.collider.gameObject.transform.localPosition.x, (int)hit.collider.gameObject.transform.localPosition.y);
            engine.DescobrirCasa(engine.tabuleiro.EncontrarCasa(a));

            foreach (GameObject g in matrizCasas)
            {
                Vector2Int gc = new((int)g.transform.localPosition.x, (int)g.transform.localPosition.y);

                if (engine.tabuleiro.EncontrarCasa(gc).escondida == false)
                {
                    g.GetComponent<SpriteRenderer>().sprite = texturas[engine.tabuleiro.EncontrarCasa(gc).numero];
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && hit.collider != null)
        {
            if (!_timerRodando)
            {
                SceneManager.LoadScene(1);
                return;
            }

            Vector2Int a = new((int)hit.collider.gameObject.transform.localPosition.x, (int)hit.collider.gameObject.transform.localPosition.y);
            if (engine.tabuleiro.EncontrarCasa(a).escondida)
            {
                engine.AlterarBandeira(engine.tabuleiro.EncontrarCasa(a));

                foreach (GameObject g in matrizCasas)
                {
                    Vector2Int gc = new((int)g.transform.localPosition.x, (int)g.transform.localPosition.y);

                    if (gc == engine.tabuleiro.EncontrarCasa(a).coordenadas)
                    {
                        if (engine.tabuleiro.EncontrarCasa(gc).tem_bandeira)
                        {
                            g.GetComponent<SpriteRenderer>().sprite = texturas[10];
                            ajustesDeJogo.numeroDeBombas--;
                            bombasTexto.text = ajustesDeJogo.numeroDeBombas.ToString();
                        }
                        else
                        {
                            g.GetComponent<SpriteRenderer>().sprite = texturas[9];
                            ajustesDeJogo.numeroDeBombas++;
                            bombasTexto.text = ajustesDeJogo.numeroDeBombas.ToString();
                        }
                    }
                }
            }            
        }
    }
} 
