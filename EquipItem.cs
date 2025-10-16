using UnityEngine;

public class EquipItem : MonoBehaviour
{
    // Slots de mão e Meshes
    public Transform rightHandSlot;
    public Transform leftHandSlot;
    public SkinnedMeshRenderer rightHandMesh;
    public SkinnedMeshRenderer leftHandMesh;

    public float dropRange = 2f;

    // Referências aos objetos instanciados nas mãos
    private GameObject equipadoDireita;
    private GameObject equipadoEsquerda;

    // Referências aos objetos originais (no chão), agora usadas apenas para Drop e Reativação.
    private GameObject originalDireita;
    private GameObject originalEsquerda;

    // NOVO: Referência ao ItemObject original para checar propriedades (ex: duasMaos)
    private ItemObject itemObjectOriginalDireita;
    private ItemObject itemObjectOriginalEsquerda;


    public void Equip(ItemObject itemObj, bool usarMaoDireita = true)
    {
        if (itemObj == null || itemObj.item == null) return;

        Transform slot = usarMaoDireita ? rightHandSlot : leftHandSlot;

        // Referências ao que está atualmente equipado
        ref GameObject antigoEquipado = ref (usarMaoDireita ? ref equipadoDireita : ref equipadoEsquerda);
        ref GameObject originalReferencia = ref (usarMaoDireita ? ref originalDireita : ref originalEsquerda);
        ref ItemObject itemObjReferencia = ref (usarMaoDireita ? ref itemObjectOriginalDireita : ref itemObjectOriginalEsquerda);

        // Destrói o item antigo se houver
        if (antigoEquipado != null) Destroy(antigoEquipado);

        // Instancia o prefab do itemData no slot
        GameObject obj = Instantiate(itemObj.item.itemPrefab, slot);

        // Configura offsets
        Vector3 posOffset = itemObj.positionOffset;
        Vector3 rotOffset = itemObj.rotationOffset;

        if (!usarMaoDireita)
        {
            // Espelhamento de offset para a mão esquerda
            rotOffset.y *= -1f;
            rotOffset.z *= -1f;
        }

        obj.transform.localPosition = posOffset;
        obj.transform.localRotation = Quaternion.Euler(rotOffset);
        obj.transform.localScale = Vector3.one;

        // Atualiza as referências
        antigoEquipado = obj;
        originalReferencia = itemObj.gameObject;
        itemObjReferencia = itemObj; // Armazena a referência do ItemObject para propriedades

        // Desativa o mesh da mão para não colidir com o item
        SkinnedMeshRenderer handMesh = usarMaoDireita ? rightHandMesh : leftHandMesh;
        if (handMesh != null) handMesh.enabled = false;

        // Desativa o objeto do item no mundo (o objeto que foi pego)
        if (originalReferencia != null) originalReferencia.SetActive(false);

        // Desativa o highlight
        itemObj.SetHighlight(false);
    }

    public void DropItem(bool usarMaoDireita = true)
    {
        // Usa referências seguras para evitar repetição de código
        ref GameObject equipado = ref (usarMaoDireita ? ref equipadoDireita : ref equipadoEsquerda);
        ref GameObject original = ref (usarMaoDireita ? ref originalDireita : ref originalEsquerda);
        ref ItemObject itemObjRef = ref (usarMaoDireita ? ref itemObjectOriginalDireita : ref itemObjectOriginalEsquerda);
        SkinnedMeshRenderer handMesh = usarMaoDireita ? rightHandMesh : leftHandMesh;

        if (equipado == null || original == null) return;

        // Checa proximidade: se o item foi dropado muito longe de onde foi pego, ignora.
        if (Vector3.Distance(transform.position, original.transform.position) > dropRange) return;

        Destroy(equipado);

        if (handMesh != null) handMesh.enabled = true;
        original.SetActive(true);

        itemObjRef.SetHighlight(true); // Reativa o highlight

        // Limpa as referências
        equipado = null;
        original = null;
        itemObjRef = null;
    }

    public void SwitchItem()
    {
        // CORREÇÃO: Usamos as referências do ItemObject original armazenadas
        ItemObject ioDireita = itemObjectOriginalDireita;
        ItemObject ioEsquerda = itemObjectOriginalEsquerda;

        if (ioDireita != null && ioDireita.duasMaos) return;
        if (ioEsquerda != null && ioEsquerda.duasMaos) return;

        if (equipadoDireita != null && equipadoEsquerda == null)
            MoverItem(equipadoDireita, true); // Da direita para a esquerda
        else if (equipadoEsquerda != null && equipadoDireita == null)
            MoverItem(equipadoEsquerda, false); // Da esquerda para a direita
    }

    private void MoverItem(GameObject item, bool daDireitaParaEsquerda)
    {
        // Define as referências de origem e destino
        ref GameObject equipadoOrigem = ref (daDireitaParaEsquerda ? ref equipadoDireita : ref equipadoEsquerda);
        ref GameObject equipadoDestino = ref (daDireitaParaEsquerda ? ref equipadoEsquerda : ref equipadoDireita);
        ref GameObject originalOrigem = ref (daDireitaParaEsquerda ? ref originalDireita : ref originalEsquerda);
        ref GameObject originalDestino = ref (daDireitaParaEsquerda ? ref originalEsquerda : ref originalDireita);
        ref ItemObject itemObjOrigem = ref (daDireitaParaEsquerda ? ref itemObjectOriginalDireita : ref itemObjectOriginalEsquerda);
        ref ItemObject itemObjDestino = ref (daDireitaParaEsquerda ? ref itemObjectOriginalEsquerda : ref itemObjectOriginalDireita);

        Transform slotDestino = daDireitaParaEsquerda ? leftHandSlot : rightHandSlot;
        SkinnedMeshRenderer meshAtivar = daDireitaParaEsquerda ? leftHandMesh : rightHandMesh;
        SkinnedMeshRenderer meshDesativar = daDireitaParaEsquerda ? rightHandMesh : leftHandMesh;

        if (item == null || itemObjOrigem == null) return;

        // Pega os offsets do ItemObject original
        Vector3 posOffset = itemObjOrigem.positionOffset;
        Vector3 rotOffset = itemObjOrigem.rotationOffset;

        if (daDireitaParaEsquerda)
        {
            // Aplica espelhamento de rotação
            rotOffset.y *= -1f;
            rotOffset.z *= -1f;
        }

        item.transform.SetParent(slotDestino);
        item.transform.localPosition = posOffset;
        item.transform.localRotation = Quaternion.Euler(rotOffset);

        // Atualiza as referências
        equipadoDestino = item;
        originalDestino = originalOrigem;
        itemObjDestino = itemObjOrigem; // Move a referência do ItemObject

        equipadoOrigem = null;
        originalOrigem = null;
        itemObjOrigem = null; // Limpa a referência de origem

        // Lógica de Mesh
        if (meshAtivar != null) meshAtivar.enabled = false; // Desativa mesh da mão com item
        if (meshDesativar != null) meshDesativar.enabled = true; // Ativa mesh da mão vazia
    }

    // Convertido para Propriedades (Melhor prática)
    public bool IsRightHandOccupied => equipadoDireita != null;
    public bool IsLeftHandOccupied => equipadoEsquerda != null;
}

































