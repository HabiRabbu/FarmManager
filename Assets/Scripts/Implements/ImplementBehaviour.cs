using UnityEngine;
using Harvey.Farm.Buildings;
using Harvey.Farm.Implements;
using Harvey.Farm.VehicleScripts;

[RequireComponent(typeof(GuidBehaviour))]
public class ImplementBehaviour : MonoBehaviour
{
    [SerializeField] ImplementDefinition definition;
    public ImplementModel Model { get; private set; }
    public ShedBuilding Home { get; private set; }

    public GuidBehaviour guid;
    MeshRenderer mesh;

    public string Id => Model.Id;
    public ImplementType Type => Model.Type;
    public float Durability => Model.Durability;
    public bool IsReserved => Model.IsReserved;
    public Sprite Icon => Model.Icon;
    public string GetGuid() => guid.GetId();

    void Awake()
    {
        guid = GetComponent<GuidBehaviour>();
        mesh = GetComponentInChildren<MeshRenderer>(true);
    }

    // Setup data in Start (If Definition is presetn) or Init (If Model is provided externally)
    public void InitFromModel(ImplementModel model, ShedBuilding parent = null)
    {
        if (model == null) return;
        Debug.Log($"ImplementBehaviour: Initialising from model {model.DisplayName} with Home: {parent}");

        Model = model;
        Home = parent;
        guid.SetId(model.Id);

        ImplementManager.Instance.Register(this);
    }

    void Start()
    {
        if (definition != null)
        {
            Debug.Log($"ImplementBehaviour: Initialising from definition {definition.DisplayName} with Home: {Home}");
            Model = ImplementMappers.FromDefinition(definition, Home, guid.GetId());
            Home = BuildingManager.Instance.GetById<ShedBuilding>(Model.HomeId);
            ImplementManager.Instance.Register(this);
        }
    }

    public int GetAnchorIndex()
    {
        Debug.Log($"Home: {Home}, IsReserved: {IsReserved}");

        if (Home && !IsReserved)
        {
            Transform parentAnchor = this.transform.parent;
            int index = Home.IndexOfAnchor(parentAnchor);
            return index >= 0 ? index : 0;
        }
        return 0;
    }

    public void AttachTo(Transform hitch)
    {
        transform.SetParent(hitch, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        Model.CurrentParentId = hitch.GetComponentInParent<GuidBehaviour>()?.GetId();
        Model.AnchorIndex = 0; // TODO: supply if multiple anchors
    }

    public void Detach()
    {
        transform.SetParent(null, true);
        Model.CurrentParentId = string.Empty;
    }

    public void ApplyWear(float delta)
    {
        Model.Durability = Mathf.Max(0f, Model.Durability - delta);
        if (mesh)
            mesh.transform.localScale = Vector3.one * Mathf.Lerp(0.8f, 1f, Durability);
    }
}
