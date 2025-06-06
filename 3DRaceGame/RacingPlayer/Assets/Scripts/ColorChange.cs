using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Troca a cor do objeto, permitindo que o player seja identificado pela cor.
// A cor do carro pode ser trocada.
public class ColorChange : MonoBehaviour
{
    // Referência ao MeshRenderer do objeto
    private MeshRenderer m_MeshRenderer;

    // Array de cores pré-definidas (opcional, para usar uma lista de cores)
    public Color[] m_PredefinedColors;

    void Start()
    {
        // Pega o componente MeshRenderer do objeto
        m_MeshRenderer = GetComponent<MeshRenderer>();

        // Verifica se o MeshRenderer foi encontrado
        if (m_MeshRenderer == null)
        {
            Debug.LogError("MeshRenderer não encontrado no GameObject! Certifique-se de que o carro é 3D e tem um MeshRenderer.");
            return;
        }

        // Chama o método para sortear e aplicar a cor
        RandomColorChange();
    }

    // Sorteio de Cor
    public void RandomColorChange()
    {
        if (m_MeshRenderer == null)
        {
            Debug.LogError("MeshRenderer não inicializado. Chame RandomColorChange() após Start().");
            return;
        }

        Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
        m_MeshRenderer.material.color = randomColor; // Define a cor do material
        Debug.Log("A cor sorteada RGBa foi = " + randomColor);
    }
}