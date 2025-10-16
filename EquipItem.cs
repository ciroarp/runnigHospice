using UnityEngine;

public class EquipItem : MonoBehaviour
{
    // Slots de m�o e Meshes
    public Transform rightHandSlot;
    public Transform leftHandSlot;
    public SkinnedMeshRenderer rightHandMesh;
    public SkinnedMeshRenderer leftHandMesh;

    public float dropRange = 2f;

    // Refer�ncias aos objetos instanciados nas m�os
    private GameObject equipadoDireita;
    private GameObject equipadoEsquerda;

    // Refer�ncias aos objetos originais (no ch�o), agora usadas apenas para Drop e Reativa��o.
    private GameObject originalDireita;
    private GameObject originalEsquerda;

    // NOVO: Refer�ncia ao ItemObject original para checar propriedades (ex: duasMaos)
    private ItemObject itemObjectOriginalDireita;
    private ItemObject itemObjectOriginalEsquerda;


    public void Equip(ItemObject itemObj, bool usarMaoDireita = true)
    {
        if (itemObj == null || itemObj.item == null) return;

        Transform slot = usarMaoDireita ? rightHandSlot : leftHandSlot;

        // Refer�ncias ao que est� atualmente equipado
        ref GameObject antigoEquipado = ref (usarMaoDireita ? ref equipadoDireita : ref equipadoEsquerda);
        ref GameObject originalReferencia = ref (usarMaoDireita ? ref originalDireita : ref originalEsquerda);
        ref ItemObject itemObjReferencia = ref (usarMaoDireita ? ref itemObjectOriginalDireita : ref itemObjectOriginalEsquerda);

        // Destr�i o item antigo se houver
        if (antigoEquipado != null) Destroy(antigoEquipado);

        // Instancia o prefab do itemData no slot
        GameObject obj = Instantiate(itemObj.item.itemPrefab, slot);

        // Configura offsets
        Vector3 posOffset = itemObj.positionOffset;
        Vector3 rotOffset = itemObj.rotationOffset;

        if (!usarMaoDireita)
        {
            // Espelhamento de offset para a m�o esquerda
            rotOffset.y *= -1f;
            rotOffset.z *= -1f;
        }

        obj.transform.localPosition = posOffset;
        obj.transform.localRotation = Quaternion.Euler(rotOffset);
        obj.transform.localScale = Vector3.one;

        // Atualiza as refer�ncias
        antigoEquipado = obj;
        originalReferencia = itemObj.gameObject;
        itemObjReferencia = itemObj; // Armazena a refer�ncia do ItemObject para propriedades

        // Desativa o mesh da m�o para n�o colidir com o item
        SkinnedMeshRenderer handMesh = usarMaoDireita ? rightHandMesh : leftHandMesh;
        if (handMesh != null) handMesh.enabled = false;

        // Desativa o objeto do item no mundo (o objeto que foi pego)
        if (originalReferencia != null) originalReferencia.SetActive(false);

        // Desativa o highlight
        itemObj.SetHighlight(false);
    }

    public void DropItem(bool usarMaoDireita = true)
    {
        // Usa refer�ncias seguras para evitar repeti��o de c�digo
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

        // Limpa as refer�ncias
        equipado = null;
        original = null;
        itemObjRef = null;
    }

    public void SwitchItem()
    {
        // CORRE��O: Usamos as refer�ncias do ItemObject original armazenadas
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
        // Define as refer�ncias de origem e destino
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
            // Aplica espelhamento de rota��o
            rotOffset.y *= -1f;
            rotOffset.z *= -1f;
        }

        item.transform.SetParent(slotDestino);
        item.transform.localPosition = posOffset;
        item.transform.localRotation = Quaternion.Euler(rotOffset);

        // Atualiza as refer�ncias
        equipadoDestino = item;
        originalDestino = originalOrigem;
        itemObjDestino = itemObjOrigem; // Move a refer�ncia do ItemObject

        equipadoOrigem = null;
        originalOrigem = null;
        itemObjOrigem = null; // Limpa a refer�ncia de origem

        // L�gica de Mesh
        if (meshAtivar != null) meshAtivar.enabled = false; // Desativa mesh da m�o com item
        if (meshDesativar != null) meshDesativar.enabled = true; // Ativa mesh da m�o vazia
    }

    // Convertido para Propriedades (Melhor pr�tica)
    public bool IsRightHandOccupied => equipadoDireita != null;
    public bool IsLeftHandOccupied => equipadoEsquerda != null;
}

































