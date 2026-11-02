using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
public class Interactable : MonoBehaviour
{
    LayerMask interactableMask;
    public bool isInteractable = true;
    public float distance = 5f;
    private bool isLookingAt = false;
    public UnityEvent OnInteract;
    public UnityEvent OnEnter;
    public UnityEvent OnExit;
    Renderer[] renderers;
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        interactableMask = LayerMask.GetMask("Interactable");
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (!isInteractable)
        {
            isLookingAt = false;
            DevisualizeInteractable();
            return;
        }
        Collider looksAtCol = LooksAt();
        if (CompareCollider(GetComponent<Collider>(), looksAtCol))
        {
            if (!isLookingAt)
            {
                isLookingAt = true;
                OnEnter.Invoke();
                VisualizeInteractable();
            }
            if (InputManager.Interact())
            {
                OnInteract.Invoke();
            }
        }
        else if (isLookingAt)
        {
            isLookingAt = false;
            OnExit.Invoke();
            DevisualizeInteractable();
        }
    }
    void VisualizeInteractable()
    {
        foreach (var r in renderers)
            r.renderingLayerMask |= SilhouetteOutlineFeature.OutlineLayer;
    }

    void DevisualizeInteractable()
    {
        foreach (var r in renderers)
            r.renderingLayerMask &= ~SilhouetteOutlineFeature.OutlineLayer;
    }

    int GetSubMeshCount(Renderer r)
    {
        if (r is SkinnedMeshRenderer smr && smr.sharedMesh != null) return smr.sharedMesh.subMeshCount;
        if (r.TryGetComponent(out MeshFilter mf) && mf.sharedMesh != null) return mf.sharedMesh.subMeshCount;
        return 1;
    }
    bool CompareCollider(Collider col, Collider other)
    {
        if(other != null && col != null && other == col)
        {
            return true;
        }
        else
        {
            bool found = false;
            foreach (Transform child in col.transform)
            {
                Collider childCol = child.GetComponent<Collider>();
                if (childCol == null)
                {
                    continue;
                }
                if (CompareCollider(childCol, other))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }
    }
    Collider LooksAt()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, distance, interactableMask))
        {
            return hit.collider;
        }
        return null;
    }
}
