using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{
    [SerializeField] private GameObject[] paineis;

    [Header("Página do Modo Personalizado")]
    [SerializeField] private Slider xSlider;
    [SerializeField] private Slider ySlider;
    [SerializeField] private Slider bombSlider;

    [Header("Página do Placar")]
    [SerializeField] private TextMeshProUGUI modoText;
    [SerializeField] private Transform scoreContentBox;
    [SerializeField] private GameObject scorePrefab;
    [SerializeField] private GameObject avisoPlacar;

    [Header("Página de Configurações")]
    [SerializeField] private TextMeshProUGUI versionText;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioMixer master;
    [SerializeField] private Slider volumeSlider;

    [Header("Salvamento")]
    [SerializeField] private SaveSystem saveSystem;

    private AjustesDeJogo _ajustesDeJogo;

    private readonly AjustesDeJogo _modoFacil = new(AjustesDeJogo.Modo.Facil, new Vector2Int(9, 9), 10);
    private readonly AjustesDeJogo _modoMedio = new(AjustesDeJogo.Modo.Medio, new Vector2Int(16, 16), 40);
    private readonly AjustesDeJogo _modoDificil = new(AjustesDeJogo.Modo.Dificil, new Vector2Int(24, 20), 99);
    private readonly AjustesDeJogo _modoCustomizado = new(AjustesDeJogo.Modo.Customizado, Vector2Int.one * 5, 5);

    private int _placarExibido = 0;
    private Placar _placar = new();

    private void Awake()
    {
        _ajustesDeJogo = _modoFacil;

        AcertarResolucao();

        versionText.text = "Versão " + Application.version.ToString();

        CarregarVolume();

        MostrarPontuacoes(0);
    }
    
    public void AcertarResolucao()
    {
        float width = Screen.currentResolution.width;
        float height = Screen.currentResolution.height;

        if (width/height == 16f/9f)
        {
            Screen.SetResolution(Mathf.FloorToInt(width / 4.8f), Mathf.FloorToInt(height / 1.8f), false);
            return;
        }

        if (width/height == 16f/10f) 
        {
            Screen.SetResolution(Mathf.FloorToInt(width / 4.2f), Mathf.FloorToInt(height / 1.75f), false);
            return;
        }
        
        if (width/height == 4f/3f)
        {
            Screen.SetResolution(Mathf.FloorToInt(width / 3.5f), Mathf.FloorToInt(height / 1.75f), false);
            return;
        }

        Screen.SetResolution(400, 600, false);
    }
    
    public int MenorTempo(Pontuacao x, Pontuacao y)
    {
        if (x.TempoFinal > y.TempoFinal)
        {
            return 1;
        }
        return -1;
    }

    public void SomDeClique()
    {
        audioManager.TocarSom(AudioManager.TipoDeSom.CLIQUE, 3);
    }
    
    public void CarregarVolume()
    {
        float volumeSalvo = saveSystem.Carregar<PreferenciasDoUsuario>(new()).VolumeMestre;
        volumeSlider.SetValueWithoutNotify(volumeSalvo);
        volumeSlider.GetComponent<TextMeshProUGUI>().text = Mathf.Floor(volumeSlider.value * 100).ToString() + "%";
    }

    public void AtualizarVolume()
    {
        master.SetFloat("mastervol", Mathf.Log10(volumeSlider.value) * 20);
        volumeSlider.GetComponent<TextMeshProUGUI>().text = Mathf.Floor(volumeSlider.value*100).ToString() + "%";
        
        PreferenciasDoUsuario preferencias = saveSystem.Carregar<PreferenciasDoUsuario>(new());
        preferencias.VolumeMestre = volumeSlider.value;
        saveSystem.Salvar(preferencias);
    }

    public void MostrarPontuacoes(int modoDesejado)
    {
        _placarExibido += modoDesejado;

        if (_placarExibido < 0) _placarExibido = 3;
        if (_placarExibido > 3) _placarExibido = 0;

        switch (_placarExibido)
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
            Transform caixaDePontuacao = scoreContentBox.GetChild(0).transform;
            caixaDePontuacao.SetParent(null);
            Destroy(caixaDePontuacao.gameObject);
        }

        _placar = saveSystem.Carregar<Placar>(new());

        if (_placar == null || _placar.Pontuacoes.Where(x => x.ModoDoJogo == (AjustesDeJogo.Modo)_placarExibido).Count() == 0)
        {
            avisoPlacar.SetActive(true);
            return;
        }

        avisoPlacar.SetActive(false);
        _placar.Pontuacoes.Sort(MenorTempo);

        int colocacaoAtual = 1;

        foreach (Pontuacao pontuacao in _placar.Pontuacoes.Where(x => x.ModoDoJogo == (AjustesDeJogo.Modo)_placarExibido))
        {
            GameObject caixaDePontuacao = Instantiate(scorePrefab, scoreContentBox);
            caixaDePontuacao.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = colocacaoAtual + ".";
            caixaDePontuacao.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = pontuacao.ModoDoJogo + " (" + pontuacao.DimensoesDoTabuleiro.x + "x" + pontuacao.DimensoesDoTabuleiro.y + "/" + pontuacao.QuantidadeDeBombas + " bombas)\n" + pontuacao.DataDaPontuacao + "\n" + pontuacao.TempoFinal.ToString() + " segundos";

            colocacaoAtual++;
        }
    }

    public void ResetarPlacar()
    {
        saveSystem.Deletar<Placar>(new());
        MostrarPontuacoes(0);
    }

    public void AtualizarSlider(Slider slider)
    {
        bombSlider.maxValue = xSlider.value * ySlider.value;
        slider.GetComponent<TextMeshProUGUI>().text = slider.value.ToString();
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
                _ajustesDeJogo = _modoFacil;
                break;
            case 1:
                _ajustesDeJogo = _modoMedio;
                break;
            case 2:
                _ajustesDeJogo = _modoDificil;
                break;
            case 3:
                _ajustesDeJogo = _modoCustomizado;
                break;
            default: break;
        }
    }

    public void IniciarJogo()
    {
        _modoCustomizado.DimensoesDoTabuleiro = new Vector2Int((int)xSlider.value, (int)ySlider.value);
        _modoCustomizado.QuantidadeDeBombas = (int)bombSlider.value;
        saveSystem.Salvar<AjustesDeJogo>(new(_ajustesDeJogo.ModoAtual, new Vector2Int(_ajustesDeJogo.DimensoesDoTabuleiro.x, _ajustesDeJogo.DimensoesDoTabuleiro.y), _ajustesDeJogo.QuantidadeDeBombas));

        SceneManager.LoadScene(1);
    }
}
