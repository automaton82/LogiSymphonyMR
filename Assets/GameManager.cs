using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;
using WebView;
using LazyFollow = UnityEngine.XR.Interaction.Toolkit.UI.LazyFollow;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    ARPlaneManager m_ARPlaneManager;

    [SerializeField]
    FadeMaterial m_FadeMaterial;
    
    [SerializeField]
    GameObject m_SpatialPlayer;

    [SerializeField]
    GameObject m_CoachingUIParent;

    [SerializeField]
    WebViewControllerClient m_WebViewControllerClient;

    [SerializeField]
    TMP_Dropdown m_LeftVisualDropdown;

    [SerializeField]
    TMP_Dropdown m_RightVisualDropdown;
    
    [SerializeField]
    TMP_Dropdown m_FilterVisualDropdown;

    [SerializeField]
    Toggle m_Toggle;

    Vector3 m_TargetOffset = new Vector3(0f, -1.0f, 1.25f);

    // Start is called before the first frame update
    void Start()
    {
        if (m_FadeMaterial != null)
        {
            m_FadeMaterial.FadeSkybox(true);
        }
        if (m_SpatialPlayer != null)
        {
            m_SpatialPlayer.SetActive(false);
        }

        StartCoroutine(TurnOnPlanes());

        if (m_LeftVisualDropdown != null)
        {
            m_LeftVisualDropdown.onValueChanged.AddListener(OnLeftVisualSelectorDropdownValueChanged);
        }
        if (m_RightVisualDropdown != null)
        {
            m_RightVisualDropdown.onValueChanged.AddListener(OnRightVisualSelectorDropdownValueChanged);
        }
        if (m_FilterVisualDropdown != null)
        {
            m_FilterVisualDropdown.onValueChanged.AddListener(OnFilterSelectorDropdownValueChanged);
        }
        if (m_Toggle != null)
        {
            m_Toggle.onValueChanged.AddListener(ToggleValueChanged);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator TurnOnPlanes()
    {
        yield return new WaitForSeconds(1f);
        m_ARPlaneManager.enabled = true;
    }

    void OnLeftVisualSelectorDropdownValueChanged(int value)
    {
        handleSelectorChange("leftVisual", value);
    }

    void OnRightVisualSelectorDropdownValueChanged(int value)
    {
        handleSelectorChange("rightVisual", value);
    }

    void OnFilterSelectorDropdownValueChanged(int value)
    {
        if (m_WebViewControllerClient != null)
        {
            switch (value)
            {
                case 0:
                case 1:
                    m_WebViewControllerClient.EvaluateJavascript("window.setFilter('Accessories');");
                    break;
                case 2:
                    m_WebViewControllerClient.EvaluateJavascript("window.setFilter('Bikes');");
                    break;
                case 3:
                    m_WebViewControllerClient.EvaluateJavascript("window.setFilter('Clothing');");
                    break;
                case 4:
                    m_WebViewControllerClient.EvaluateJavascript("window.setFilter('Components');");
                    break;
            }
        }
    }

    private void ShowDashboard1()
    {
        ShowDashboard();
        StartCoroutine(LoadUrl("http://dashboard1"));
    }

    private void ShowDashboard2()
    {
        ShowDashboard();
        StartCoroutine(LoadUrl("http://dashboard2"));
    }

    private void ShowDashboard3()
    {
        ShowDashboard();
        StartCoroutine(LoadUrl("http://dashboard3"));
    }

    private void ShowDashboard4()
    {
        ShowDashboard();
        StartCoroutine(LoadUrl("http://dashboard4"));
    }


    private void ShowDashboard()
    {
        if (m_CoachingUIParent != null)
        {
            m_CoachingUIParent.SetActive(false);
        }

        // Place the spartial player in front of the user.
        var follow = m_SpatialPlayer.GetComponent<LazyFollow>();
        if (follow != null)
            follow.rotationFollowMode = LazyFollow.RotationFollowMode.None;

        m_SpatialPlayer.SetActive(false);
        var target = Camera.main.transform;
        var targetRotation = target.rotation;
        var newTransform = target;
        var targetEuler = targetRotation.eulerAngles;
        targetRotation = Quaternion.Euler
        (
            0f,
            targetEuler.y,
            targetEuler.z
        );

        newTransform.rotation = targetRotation;
        var targetPosition = target.position + newTransform.TransformVector(m_TargetOffset);
        m_SpatialPlayer.transform.position = targetPosition;


        var forward = target.position - m_SpatialPlayer.transform.position;
        var targetPlayerRotation = forward.sqrMagnitude > float.Epsilon ? Quaternion.LookRotation(forward, Vector3.up) : Quaternion.identity;
        targetPlayerRotation *= Quaternion.Euler(new Vector3(0f, 180f, 0f));
        var targetPlayerEuler = targetPlayerRotation.eulerAngles;
        var currentEuler = m_SpatialPlayer.transform.rotation.eulerAngles;
        targetPlayerRotation = Quaternion.Euler
        (
            currentEuler.x,
            targetPlayerEuler.y,
            currentEuler.z
        );

        m_SpatialPlayer.transform.rotation = targetPlayerRotation;
        m_SpatialPlayer.SetActive(true);
        if (follow != null)
            follow.rotationFollowMode = LazyFollow.RotationFollowMode.LookAtWithWorldUp;
    }

    public IEnumerator LoadUrl(string url)
    {
        yield return new WaitForSeconds(1f);
        m_WebViewControllerClient.LoadUrl(url);
    }

    private void Reset()
    {
        m_SpatialPlayer.SetActive(false);
        m_CoachingUIParent.SetActive(true);
    }

    private void handleSelectorChange(string name, int value)
    {
        if (m_WebViewControllerClient != null)
        {
            switch (value)
            {
                case 0:
                    m_WebViewControllerClient.EvaluateJavascript("window.revisToTable('" + name + "');");
                    break;
                case 1:
                    m_WebViewControllerClient.EvaluateJavascript("window.revisToChart('" + name + "', 'Line');");
                    break;
                case 2:
                    m_WebViewControllerClient.EvaluateJavascript("window.revisToChart('" + name + "', 'Bar');");
                    break;
                case 3:
                    m_WebViewControllerClient.EvaluateJavascript("window.revisToChart('" + name + "', 'Point');");
                    break;
                case 4:
                    m_WebViewControllerClient.EvaluateJavascript("window.revisToChart('" + name + "', 'Stacked Bar');");
                    break;
            }
        }
    }

    void ToggleValueChanged(bool value)
    {
        if (value)
        {
            m_WebViewControllerClient.EvaluateJavascript("window.dundas.context.getService('ThemeService').getThemeById('72cd17a0-b63e-4456-98e9-cb7152659bf6').done((theme) => { window.dundas.context.baseViewService.currentView.applyTheme(theme); });");
        }
        else
        {
            m_WebViewControllerClient.Reload();
        }
    }
}
