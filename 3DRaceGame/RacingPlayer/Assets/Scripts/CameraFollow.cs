using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Referência para o transform do carro
    public Transform carro;

    // Distância/offset entre a câmera e o carro
    public Vector3 offset = new Vector3(0, 5, -10);

    void LateUpdate()
    {
        // Atualiza a posição da câmera com base na posição do carro + offset
        transform.position = carro.position + offset;
    }
}
