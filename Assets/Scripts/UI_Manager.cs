using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private CM_Engine engine;
    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private AudioManager audioManager;

    [Header("Tabuleiro")]
    [SerializeField] private Transform objetoTabuleiro;
    [SerializeField] private GameObject casaPrefab;
    [SerializeField] private Sprite[] texturas;

    [Header("Interface de Usuário")]
    [SerializeField] private TextMeshProUGUI bombasTexto;
    [SerializeField] private TextMeshProUGUI tempoTexto;
    [SerializeField] private Transform menuBotao;


    private int _tamanhoDaCasa;
    public AjustesDeJogo _ajustesDeJogo;
    private int _qtdInicialDeBombas;
    private readonly List<GameObject> _matrizDeCasas = new();
    private bool _timerRodando = true;
    private Collider2D _ultimaCasaSelecionada = null;
    private RaycastHit2D casaSelecionada;

    private void Start()
    {
        _tamanhoDaCasa = Screen.currentResolution.width / 64;

        engine.GanhouJogo += AoGanharJogo;
        engine.PerdeuJogo += AoPerderJogo;
        engine.CasaComBandeiraFoiAberta += AumentarNumeroDeBombas;

        _ajustesDeJogo = saveSystem.Carregar<AjustesDeJogo>(new(AjustesDeJogo.Modo.Facil, Vector2Int.one * 9, 10));
        _qtdInicialDeBombas = _ajustesDeJogo.QuantidadeDeBombas;

        engine.GerarTabuleiro(_ajustesDeJogo.DimensoesDoTabuleiro, _ajustesDeJogo.QuantidadeDeBombas);

        GerarDesignResponsivo();

        bombasTexto.text = _ajustesDeJogo.QuantidadeDeBombas.ToString();

        foreach (Vector2Int coordenada in engine.Tabuleiro.ListarCoordenadas())
        {
            GameObject casa = Instantiate(casaPrefab, new Vector3(coordenada.x, coordenada.y, 0), Quaternion.identity);
            casa.transform.parent = objetoTabuleiro;
            _matrizDeCasas.Add(casa);
        }

        AjeitarCamera();
    }

    private void AumentarNumeroDeBombas(object sender, EventArgs e)
    {
        _ajustesDeJogo.QuantidadeDeBombas++;
        bombasTexto.text = _ajustesDeJogo.QuantidadeDeBombas.ToString();
    }

    public void GerarDesignResponsivo()
    {
        Screen.SetResolution(_ajustesDeJogo.DimensoesDoTabuleiro.x * _tamanhoDaCasa, _ajustesDeJogo.DimensoesDoTabuleiro.y * _tamanhoDaCasa + _tamanhoDaCasa + 10, false);

        bombasTexto.transform.parent.position = new Vector3(0.4014f, _ajustesDeJogo.DimensoesDoTabuleiro.y + 0.2f, 0);
        tempoTexto.transform.parent.position = new Vector3(_ajustesDeJogo.DimensoesDoTabuleiro.x - 1.4014f, _ajustesDeJogo.DimensoesDoTabuleiro.y + 0.2f, 0);
        menuBotao.position = new Vector3((float)_ajustesDeJogo.DimensoesDoTabuleiro.x / 2 - .5f, _ajustesDeJogo.DimensoesDoTabuleiro.y + 0.2f, 0);
    }

    public void AjeitarCamera()
    {
        Vector2Int dimensaoDivisivelPorDois = new(engine.Tabuleiro.Dimensoes.x % 2, engine.Tabuleiro.Dimensoes.y % 2);

        if (dimensaoDivisivelPorDois.x == 0 && dimensaoDivisivelPorDois.y == 0)
        {
            objetoTabuleiro.position = new Vector2(objetoTabuleiro.position.x - (float)(engine.Tabuleiro.Dimensoes.x-1) / 2, objetoTabuleiro.position.y - (float)(engine.Tabuleiro.Dimensoes.y-1) / 2);
        }
        if (dimensaoDivisivelPorDois.x == 1 && dimensaoDivisivelPorDois.y == 0)
        {
            objetoTabuleiro.position = new Vector2(objetoTabuleiro.position.x - engine.Tabuleiro.Dimensoes.x / 2, objetoTabuleiro.position.y - (float)(engine.Tabuleiro.Dimensoes.y - 1) / 2);
        }
        if (dimensaoDivisivelPorDois.x == 0 && dimensaoDivisivelPorDois.y == 1)
        {
            objetoTabuleiro.position = new Vector2(objetoTabuleiro.position.x - (float)(engine.Tabuleiro.Dimensoes.x-1) / 2, objetoTabuleiro.position.y - engine.Tabuleiro.Dimensoes.y / 2);
        }
        if (dimensaoDivisivelPorDois.x == 1 && dimensaoDivisivelPorDois.y == 1)
        {
            objetoTabuleiro.position = new Vector2(objetoTabuleiro.position.x - engine.Tabuleiro.Dimensoes.x / 2, objetoTabuleiro.position.y - engine.Tabuleiro.Dimensoes.y / 2);
        }
               
        Camera.main.orthographicSize = ((float)engine.Tabuleiro.Dimensoes.y / 2) + 1f;
    }

    public void AoGanharJogo(object sender, EventArgs e)
    {
        audioManager.TocarSom(AudioManager.TipoDeSom.VENCER, 1);

        Pontuacao novaPontuacao = new(DateTime.Now.ToString("G"), (float)Math.Round((double)Time.timeSinceLevelLoad, 2), _ajustesDeJogo.ModoAtual, _ajustesDeJogo.DimensoesDoTabuleiro, _qtdInicialDeBombas);
        Placar placarAtualizado = saveSystem.Carregar<Placar>(new());
        placarAtualizado.Pontuacoes.Add(novaPontuacao);
        saveSystem.Salvar(placarAtualizado);

        foreach (Casa casaComBomba in engine.Tabuleiro.EncontrarCasas(false, true))
        {
            foreach (GameObject objetoCasa in _matrizDeCasas)
            {
                if (objetoCasa.transform.localPosition.x == casaComBomba.Coordenadas.x && objetoCasa.transform.localPosition.y == casaComBomba.Coordenadas.y)
                {
                    objetoCasa.GetComponent<SpriteRenderer>().sprite = texturas[13];
                }

            }
        }

        _timerRodando = false;
    }

    public void AoPerderJogo(object sender, CM_Engine.PerdeuJogoEventArgs e)
    {
        audioManager.TocarSom(AudioManager.TipoDeSom.EXPLOSAO, 1);

        foreach (Casa casaComBomba in engine.Tabuleiro.EncontrarCasas(false, true))
        {
            foreach (GameObject objetoCasa in _matrizDeCasas)
            {
                if (objetoCasa.transform.localPosition.x == casaComBomba.Coordenadas.x && objetoCasa.transform.localPosition.y == casaComBomba.Coordenadas.y)
                {
                    if (e.UltimaCasaClicada == casaComBomba)
                        objetoCasa.GetComponent<SpriteRenderer>().sprite = texturas[12];
                    else
                        objetoCasa.GetComponent<SpriteRenderer>().sprite = texturas[11];
                }

            }
        }

        _timerRodando = false;
    }

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void RessaltarCasasAoMouse()
    {
        if (_ultimaCasaSelecionada != casaSelecionada.collider)
        {
            if (_ultimaCasaSelecionada != null)
            {
                _ultimaCasaSelecionada.transform.localScale = Vector3.one * 1.0f;
                _ultimaCasaSelecionada.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }

            if (casaSelecionada.collider != null)
            {
                Casa casaRessaltada = engine.Tabuleiro.EncontrarCasa(new Vector2Int((int)casaSelecionada.transform.localPosition.x, (int)casaSelecionada.transform.localPosition.y));

                if (casaRessaltada.Escondida)
                {
                    casaSelecionada.transform.localScale = Vector3.one * 1.1f;
                    casaSelecionada.collider.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
                }
            }

            _ultimaCasaSelecionada = casaSelecionada.collider;
        }
    }

    private void DetectarCliqueEsquerdo()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && casaSelecionada.collider != null)
        {
            if (!_timerRodando)
            {
                SceneManager.LoadScene(1);
                return;
            }

            if (_ultimaCasaSelecionada != null)
            {
                _ultimaCasaSelecionada.transform.localScale = Vector3.one * 1.0f;
                _ultimaCasaSelecionada.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }

            Vector2Int coordenadaSelecionada = new((int)casaSelecionada.collider.gameObject.transform.localPosition.x, (int)casaSelecionada.collider.gameObject.transform.localPosition.y);
            if (engine.DescobrirCasa(engine.Tabuleiro.EncontrarCasa(coordenadaSelecionada))) audioManager.TocarSom(AudioManager.TipoDeSom.CLIQUE, 1);

            foreach (GameObject objetoCasa in _matrizDeCasas)
            {
                Vector2Int coordenadaDaCasa = new((int)objetoCasa.transform.localPosition.x, (int)objetoCasa.transform.localPosition.y);

                if (engine.Tabuleiro.EncontrarCasa(coordenadaDaCasa).Escondida == false)
                {
                    objetoCasa.GetComponent<SpriteRenderer>().sprite = texturas[engine.Tabuleiro.EncontrarCasa(coordenadaDaCasa).Numero];
                }
            }
        }
    }

    private void DetectarCliqueDireito()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && casaSelecionada.collider != null)
        {
            Vector2Int coordenadaSelecionada = new((int)casaSelecionada.collider.gameObject.transform.localPosition.x, (int)casaSelecionada.collider.gameObject.transform.localPosition.y);
            if (engine.Tabuleiro.EncontrarCasa(coordenadaSelecionada).Escondida)
            {
                engine.AlterarBandeira(engine.Tabuleiro.EncontrarCasa(coordenadaSelecionada));

                foreach (GameObject objetoCasa in _matrizDeCasas)
                {
                    Vector2Int coordenadaDaCasa = new((int)objetoCasa.transform.localPosition.x, (int)objetoCasa.transform.localPosition.y);

                    if (coordenadaDaCasa == engine.Tabuleiro.EncontrarCasa(coordenadaSelecionada).Coordenadas)
                    {
                        if (engine.Tabuleiro.EncontrarCasa(coordenadaDaCasa).TemBandeira)
                        {
                            audioManager.TocarSom(AudioManager.TipoDeSom.BOTAR_BANDEIRA, 1);

                            objetoCasa.GetComponent<SpriteRenderer>().sprite = texturas[10];
                            _ajustesDeJogo.QuantidadeDeBombas--;
                            bombasTexto.text = _ajustesDeJogo.QuantidadeDeBombas.ToString();
                        }
                        else
                        {
                            audioManager.TocarSom(AudioManager.TipoDeSom.TIRAR_BANDEIRA, 1);

                            objetoCasa.GetComponent<SpriteRenderer>().sprite = texturas[9];
                            _ajustesDeJogo.QuantidadeDeBombas++;
                            bombasTexto.text = _ajustesDeJogo.QuantidadeDeBombas.ToString();
                        }
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        casaSelecionada = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
    }

    private void Update()
    {
        if (_timerRodando)
        {
            tempoTexto.text = Time.timeSinceLevelLoad.ToString("0");
            RessaltarCasasAoMouse();
            DetectarCliqueDireito();
        }

        DetectarCliqueEsquerdo();
    }
} 
