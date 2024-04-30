using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.VisualScripting;

public class Launcher : MonoBehaviour
{
    [SerializeField] private Slider xSlider, ySlider, bombSlider;
    [SerializeField] private TextMeshProUGUI xText, yText, bombText, versionText, modoText;
    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private GameObject[] paineis;
    [SerializeField] private GameObject scorePrefab;
    [SerializeField] private GameObject avisoPlacar;
    [SerializeField] private Transform scoreContentBox;

    private AjustesDeJogo modoEscolhido;

    private readonly AjustesDeJogo facil = new(AjustesDeJogo.Modo.Facil, new Vector2Int(9, 9), 10);
    private readonly AjustesDeJogo medio = new(AjustesDeJogo.Modo.Medio, new Vector2Int(16, 16), 40);
    private readonly AjustesDeJogo dificil = new(AjustesDeJogo.Modo.Dificil, new Vector2Int(20, 24), 99);
    private readonly AjustesDeJogo customizado = new(AjustesDeJogo.Modo.Customizado, Vector2Int.one * 5, 5);

    private int placarAtual = 0;
    private Placar p = new();

    public int MenorTempo(Pontuacao x, Pontuacao y)
    {
        if (x.tempoFinal > y.tempoFinal)
        {
            return 1;
        }
        return -1;
    }

    private void Awake()
    {
        Screen.SetResolution(400, 600, false);

        versionText.text = "Versão " + Application.version.ToString();

        MostrarPontuacoes(0);
    }

    public void MostrarPontuacoes(int modoDesejado)
    {
        placarAtual += modoDesejado;

        if (placarAtual < 0) placarAtual = 3;
        if (placarAtual > 3) placarAtual = 0;

        switch (placarAtual)
        {
            case 0:
                modoText.text = "Fácil";
                break;
            case 1:
                modoText.text = "Médio";
                break;
            case 2:
                modoText.text = "Difícil";
                break;
            case 3:
                modoText.text = "Customizado";
                break;
            default:
                modoText.text = "Erro";
                break;
        }

        // Limpar lista de pontuações atual se existir
        while (scoreContentBox.childCount > 0)
        {
            Transform g = scoreContentBox.GetChild(0).transform;
            g.SetParent(null);
            Destroy(g.gameObject);
        }

        p = saveSystem.CarregarPlacar();

        if (p == null || p.pontuacoes.Where(x => x.modoDeJogo == (AjustesDeJogo.Modo)placarAtual).Count() == 0)
        {
            avisoPlacar.SetActive(true);
            return;
        }

        avisoPlacar.SetActive(false);
        p.pontuacoes.Sort(MenorTempo);

        int i = 1;

        foreach (Pontuacao pont in p.pontuacoes.Where(x => x.modoDeJogo == (AjustesDeJogo.Modo)placarAtual))
        {
            GameObject o = Instantiate(scorePrefab, scoreContentBox);
            o.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = i + ".";
            o.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = pont.modoDeJogo + " (" + pont.tamanhoDoTabuleiro.x + "x" + pont.tamanhoDoTabuleiro.y + "/" + pont.numeroDeBombas + " bombas)\n" + pont.dataDaPontuacao + "\n" + pont.tempoFinal.ToString() + " segundos";

            i++;
        }
    }

    public void ResetarPlacar()
    {
        saveSystem.LimparPlacar();
        MostrarPontuacoes(0);
    }

    private void Start()
    {
        xSlider.minValue = 5; xSlider.maxValue = 32;
        ySlider.minValue = 5; ySlider.maxValue = 32;

        bombSlider.minValue = 0;
    }

    private void Update()
    {
        xText.text = xSlider.value.ToString();
        yText.text = ySlider.value.ToString();
        bombText.text = bombSlider.value.ToString();

        bombSlider.maxValue = xSlider.value * ySlider.value;
    }

    public void TrocarPainel(int painel)
    {
        foreach (GameObject p in paineis)
        {
            if (paineis[painel-1] == p) p.SetActive(true);
            else p.SetActive(false);
        }
    }

    public void EscolherModo(int modo)
    {
        switch (modo)
        {
            case 0:
                modoEscolhido = facil;
                break;
            case 1:
                modoEscolhido = medio;
                break;
            case 2:
                modoEscolhido = dificil;
                break;
            case 3:
                modoEscolhido = customizado;
                break;
            default: break;
        }
    }

    public void IniciarJogo()
    {
        customizado.tamanhoDoTabuleiro = new Vector2Int((int)xSlider.value, (int)ySlider.value);
        customizado.numeroDeBombas = (int)bombSlider.value;
        saveSystem.SalvarAjustes(modoEscolhido.modoAtual, modoEscolhido.tamanhoDoTabuleiro.x, modoEscolhido.tamanhoDoTabuleiro.y, modoEscolhido.numeroDeBombas);

        SceneManager.LoadScene(1);
    }
}
