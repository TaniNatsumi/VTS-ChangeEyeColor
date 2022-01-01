using VTS.Networking.Impl;
using VTS.Models.Impl;
using VTS.Models;
using VTS;
using UnityEngine;
using UnityEngine.UI;
using HSVPicker;
using System.Text.RegularExpressions;
using UnityEditor;

public class ChangeMeshColorWithPicker : VTSPlugin
{
    [SerializeField]
    public Button eyeCheckbtn;
    public ColorPicker pickerR;
    public ColorPicker pickerL;
    public Toggle syncColor;
    public Text infoText;

    private Color32 MeshColor = new Color32();
    private ArtMeshMatcher AmmR = new ArtMeshMatcher();
    private ArtMeshMatcher AmmL = new ArtMeshMatcher();
    private string[] nameArrayEyeR = { "EyeballR" };
    private string[] nameArrayEyeL = { "EyeballL" };
    private string[] nameArrayEyes = { "EyeballR", "EyeballL" };


    // Start is called before the first frame update
    void Start()
    {
        // Everything you need to get started!
        Initialize(
            new WebSocketImpl(),
            new JsonUtilityImpl(),
            new TokenStorageImpl(),
            () => { Debug.Log("Connected!"); },
            () => { Debug.LogWarning("Disconnected!"); },
            () => { Debug.LogError("Error!"); });

        AmmR.tintAll = false;
        AmmL.tintAll = false;
        AmmR.nameContains = nameArrayEyeR;
        AmmL.nameContains = nameArrayEyeL;

        // Listener
        eyeCheckbtn.onClick.AddListener(() => eyeCheck());
        syncColor.onValueChanged.AddListener(setMeshList);
        pickerR.onValueChanged.AddListener(color =>
        {
            MeshColor = color;
            this.TintArtMesh(MeshColor, 1, AmmR, (onSuccess) => { }, (onError) => { });
        });
        pickerL.onValueChanged.AddListener(color =>
        {
            MeshColor = color;
            this.TintArtMesh(MeshColor, 1, AmmL, (onSuccess) => { }, (onError) => { });
        });
    }

    void eyeCheck()
    {
        this.GetArtMeshList(
            (onSuccess) =>
            {
                int i = 0;
                string text = "";
                while (i < onSuccess.data.artMeshNames.Length)
                {
                    if (Regex.IsMatch(onSuccess.data.artMeshNames[i], "^.*Eyeball.*$"))
                    {
                        text = "Eyeball is founded.";
                        break;
                    }
                    i++;
                }

                if(i >= onSuccess.data.artMeshNames.Length) {
                    text = "Eyeball is not founded.";
                }
                infoText.text = text;
            },
            (onError) => {
                infoText.text = "An error occurred.";
            });
    }
    
    void setMeshList(bool isOn)
    {
        if (isOn)
        {
            AmmR.nameContains = nameArrayEyes;
            AmmL.nameContains = nameArrayEyes;
            pickerL.gameObject.SetActive(false);
        }
        else
        {
            AmmR.nameContains = nameArrayEyeR;
            AmmL.nameContains = nameArrayEyeL;
            pickerL.gameObject.SetActive(true);
        }
    }

}
